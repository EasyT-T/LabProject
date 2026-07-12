namespace LabProject.Audio;

using System;
using System.Buffers;
using System.Threading;
using Cysharp.Threading.Tasks;
using LabProject.Audio.Abstractions;
using VoiceChat;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;

public class AudioPlayer(ReferenceHub referenceHub) : IDisposable
{
    public bool Playing { get; private set; }

    private bool _paused = false;

    private IAudioResource? _currentResource = null;

    private readonly OpusEncoder _encoder = new OpusEncoder(OpusApplicationType.Audio);

    private readonly float[] _pcmBuffer = ArrayPool<float>.Shared.Rent(VoiceChatSettings.PacketSizePerChannel * VoiceChatSettings.Channels);
    private readonly byte[] _encodeBuffer = ArrayPool<byte>.Shared.Rent(VoiceChatSettings.MaxEncodedSize);

    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public void Play(IAudioResource resource)
    {
        this._currentResource = resource;

        this.Playing = true;

        this.Update(this._cancellationTokenSource.Token).Forget();
    }

    public void Pause()
    {
        this._paused = true;
    }

    public void Resume()
    {
        this._paused = false;
    }

    public void Stop()
    {
        this._cancellationTokenSource.Cancel();

        this.Playing = false;

        this._currentResource?.Dispose();
        this._currentResource = null;
    }

    private async UniTask Update(CancellationToken ct)
    {
        var pcmBuffer = new Memory<float>(this._pcmBuffer);
        var encodeBuffer = new Memory<byte>(this._encodeBuffer);

        while (true)
        {
            if (ct.IsCancellationRequested || this._currentResource is null)
            {
                break;
            }

            if (this._paused)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, ct);

                continue;
            }

            var cnt = await this._currentResource.ReadAsync(pcmBuffer, ct);

            cnt = this._encoder.Encode(this._pcmBuffer, this._encodeBuffer, cnt);
        }
    }

    public void Dispose()
    {
        this._currentResource?.Dispose();
        this._encoder.Dispose();

        ArrayPool<byte>.Shared.Return(this._encodeBuffer);
    }
}
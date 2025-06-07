namespace LabProject.Model;

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using global::LabProject.Extension;
using global::LabProject.Logging;
using global::LabProject.Pooling;
using MEC;
using UnityEngine;
using VoiceChat;
using VoiceChat.Codec;
using VoiceChat.Networking;

public class AudioPlayer : ILoggerProvider, IDisposable
{
    public Bot Bot { get; }
    public bool LeaveOpen { get; }

    public bool Playing { get; private set; }
    public bool Stopped { get; private set; } = true;
    public bool Disposed { get; private set; }
    public byte Volume { get; set; } = 100;
    public bool Paused { get; set; }
    public bool Looping { get; set; }

    internal float VolumeFactor => Mathf.Clamp(this.Volume, 0, 100) / 100.0f;

    private readonly Func<PlaybackContext, string>? nicknameFunc;
    private readonly Queue<PlaybackResource> playbackQueue = [];
    private readonly ConcurrentQueue<Action> actionQueue = [];
    private readonly PlaybackBuffer playbackBuffer = PlaybackBufferPool.Shared.Rent();

    private readonly OpusEncoder opusEncoder = OpusEncoderPool.Shared.Rent();

    private readonly float[] sendBuffer = new float[VoiceChatSettings.PacketSizePerChannel];

    private volatile float allowedSamples;
    private CancellationTokenSource? cts;
    private CoroutineHandle? updateHandle;
    private Task? playbackTask;

    public AudioPlayer(Bot bot, bool leaveOpen = false)
    {
        this.Bot = bot;
        this.LeaveOpen = leaveOpen;
    }

    public AudioPlayer(Bot bot, Func<PlaybackContext, string> nicknameFunc, bool leaveOpen)
    {
        this.nicknameFunc = nicknameFunc;
        this.Bot = bot;

        this.LeaveOpen = leaveOpen;
    }

    ~AudioPlayer()
    {
        this.Dispose(true);

        this.GetInternalLogger().Debug("Audio player has been destroyed automatically.");
    }

    public void EnqueueAudio(CustomAudio audio)
    {
        this.playbackQueue.Enqueue(new PlaybackResource(audio));
    }

    public void Play()
    {
        if (!this.Stopped)
        {
            return;
        }

        this.Stopped = false;

        this.cts = new CancellationTokenSource();

        this.updateHandle = Timing.RunCoroutine(this.Update(), Segment.Update);
        this.playbackTask = this.Playback(this.cts.Token);
    }

    public void Stop()
    {
        this.Stopped = true;

        this.cts?.Cancel();

        if (this.updateHandle != null)
        {
            Timing.KillCoroutines(this.updateHandle.Value);
        }

        this.playbackTask?.Wait();

        this.allowedSamples = 0.0f;
    }

    public void Dispose()
    {
        this.Dispose(false);
    }

    protected internal void Dispose(bool disposing)
    {
        this.Disposed = true;

        this.Stop();

        this.cts?.Dispose();

        PlaybackBufferPool.Shared.Return(this.playbackBuffer);
        OpusEncoderPool.Shared.Return(this.opusEncoder);

        if (this.LeaveOpen)
        {
            this.Bot.Dispose();
        }

        if (disposing)
        {
            GC.SuppressFinalize(this);
        }
    }

    private IEnumerator<float> Update()
    {
        while (!this.Disposed && !this.Stopped)
        {
            Interlocked.Exchange(ref this.allowedSamples,
                this.allowedSamples + Time.deltaTime * VoiceChatSettings.SampleRate);

            try
            {
                for (var i = 0; i < this.actionQueue.Count; i++)
                {
                    if (!this.actionQueue.TryDequeue(out var action))
                    {
                        break;
                    }

                    action();
                }
            }
            catch (Exception e)
            {
                this.GetInternalLogger().Error($"Encountered an exception while updating audio player: {e}");
                this.updateHandle = null;

                this.Stop();
                yield break;
            }

            yield return Timing.WaitForOneFrame;
        }
    }

    private async Task Playback(CancellationToken token = default)
    {
        PlaybackResource? resource = null;

        try
        {
            while (!this.Disposed && !this.Stopped)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (this.Paused)
                {
                    await Task.Delay(50, token).ConfigureAwait(false);
                    continue;
                }

                if (resource == null)
                {
                    if (!this.playbackQueue.TryDequeue(out resource))
                    {
                        this.Playing = false;
                        await Task.Delay(50, token).ConfigureAwait(false);
                        continue;
                    }

                    this.Playing = true;
                }

                if (this.playbackBuffer.Length < this.sendBuffer.Length)
                {
                    var hasMoreSamples = await this.ReadResourceSamples(resource, token).ConfigureAwait(false);

                    if (!hasMoreSamples)
                    {
                        resource = null;
                        continue;
                    }
                }

                while (this.allowedSamples > this.sendBuffer.Length &&
                       this.playbackBuffer.Length >= this.sendBuffer.Length)
                {
                    await this.SendAudioMessage(resource, token).ConfigureAwait(false);
                }

                await Task.Delay((int)TimeSpan.FromSeconds(Time.deltaTime).TotalMilliseconds, token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            this.GetInternalLogger().Error($"Encountered an exception while playing audio {resource?.PlaybackName ?? "(null)"}: {e}");
        }
        finally
        {
            resource?.Dispose();
            this.playbackTask = null;

            this.Stop();
        }
    }

    private Task<bool> ReadResourceSamples(PlaybackResource resource, CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
        {
            token.ThrowIfCancellationRequested();
        }

        var cnt = resource.SampleProvider.Read(this.playbackBuffer.Buffer, 0,
            this.playbackBuffer.Buffer.Length - this.playbackBuffer.Length);

        if (cnt == 0)
        {
            if (this.Looping)
            {
                resource.WaveStream.Seek(0, SeekOrigin.Begin);
                this.playbackBuffer.Clear();
            }
            else
            {
                resource.Dispose();
                return Task.FromResult(false);
            }
        }

        this.playbackBuffer.ReadHead = this.playbackBuffer.HeadToIndex(this.playbackBuffer.ReadHead);
        this.playbackBuffer.WriteHead = cnt + this.playbackBuffer.ReadHead;

        return Task.FromResult(true);
    }

    private Task SendAudioMessage(PlaybackResource resource, CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
        {
            token.ThrowIfCancellationRequested();
        }

        this.playbackBuffer.ReadTo(this.sendBuffer, this.sendBuffer.Length);

        if (this.Volume != 100)
        {
            Parallel.For(0, this.playbackBuffer.Buffer.Length,
                i => this.playbackBuffer.Buffer[i] *= this.VolumeFactor);
        }

        var encodedBuffer = ArrayPool<byte>.Shared.Rent(VoiceChatSettings.MaxEncodedSize);
        var encodedLen = this.opusEncoder.Encode(this.sendBuffer, encodedBuffer);
        var playbackContext = new PlaybackContext(resource.PlaybackName,
            resource.WaveStream.CurrentTime, resource.WaveStream.TotalTime);

        this.actionQueue.Enqueue(() =>
            {
                if (this.nicknameFunc != null)
                {
                    var name = this.nicknameFunc?.Invoke(playbackContext);
                    this.Bot.ReferenceHub.nicknameSync.MyNick = name;
                }

                this.Bot.Observer?.Connection.Send(new VoiceMessage(this.Bot.ReferenceHub,
                    VoiceChatChannel.Proximity, encodedBuffer, encodedLen, false));
                ArrayPool<byte>.Shared.Return(encodedBuffer);
            }
        );

        Interlocked.Exchange(ref this.allowedSamples, this.allowedSamples - this.sendBuffer.Length);

        return Task.CompletedTask;
    }
}
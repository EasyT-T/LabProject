namespace LabProject.Audio.IO;

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LabProject.Audio.Abstractions;
using LabProject.Extensions;

internal class StreamAudioResource(int sampleRate, int channels, Stream stream) : IAudioResource
{
    private bool _disposed;

    public int SampleRate { get; } = sampleRate;

    public int Channels { get; } = channels;

    public int Position => (int)stream.Position / sizeof(float);

    public int SampleLength { get; } = (int)stream.Length / sizeof(float);

    public TimeSpan Duration { get; } = TimeSpan.FromSeconds((float)stream.Length / sizeof(float) / channels / sampleRate);

    public float Read()
    {
        this.CheckDisposed();

        return stream.Read<float>();
    }

    public ValueTask<float> ReadAsync(CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(this.Read());
    }

    public int Read(Span<float> buffer)
    {
        return stream.Read(buffer);
    }

    public ValueTask<int> ReadAsync(Memory<float> buffer, CancellationToken cancellationToken = default)
    {
        return stream.ReadAsync(buffer, cancellationToken).ToValueTask();
    }

    public float[] Read(int length)
    {
        this.CheckDisposed();

        var buffer = new float[length];

        stream.Read(buffer.AsSpan());

        return buffer;
    }

    public async ValueTask<float[]> ReadAsync(int length, CancellationToken cancellationToken = default)
    {
        this.CheckDisposed();

        var buffer = new float[length];

        await stream.ReadAsync(buffer.AsMemory(), cancellationToken);

        return buffer;
    }

    public float[] Read(TimeSpan interval)
    {
        var length = (int)interval.TotalSeconds * this.SampleRate * this.Channels;

        return this.Read(length);
    }

    public ValueTask<float[]> ReadAsync(TimeSpan interval, CancellationToken cancellationToken = default)
    {
        var length = (int)interval.TotalSeconds * this.SampleRate * this.Channels;

        return this.ReadAsync(length, cancellationToken);
    }

    public int Seek(int offset, SeekOrigin origin)
    {
        this.CheckDisposed();

        return (int)stream.Seek(offset * sizeof(float), origin);
    }

    public void Dispose()
    {
        stream.Dispose();

        this._disposed = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckDisposed()
    {
        if (!this._disposed)
        {
            return;
        }

        throw new ObjectDisposedException(nameof(StreamAudioResource));
    }
}
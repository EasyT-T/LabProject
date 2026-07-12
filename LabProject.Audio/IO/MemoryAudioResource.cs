namespace LabProject.Audio.IO;

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LabProject.Audio.Abstractions;
using LabProject.Extensions;

internal class MemoryAudioResource(int sampleRate, int channels, byte[] buffer) : IAudioResource
{
    private bool _disposed;

    private readonly ReadOnlyMemory<float> _samples = MemoryMarshal.Cast<byte, float>(new ReadOnlyMemory<byte>(buffer));

    public int SampleRate { get; } = sampleRate;

    public int Channels { get; } = channels;

    public int Position { get; private set; }

    public int SampleLength { get; } = buffer.Length / sizeof(float);

    public TimeSpan Duration { get; } = TimeSpan.FromSeconds((float)buffer.Length / sizeof(float) / channels / sampleRate);

    public float Read()
    {
        this.CheckDisposed();

        if (this.Position >= this.SampleLength)
        {
            return 0.0f;
        }

        return this._samples.Span[this.Position++];
    }

    public ValueTask<float> ReadAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<float>(cancellationToken);
        }

        return ValueTask.FromResult(this.Read());
    }

    public int Read(Span<float> buffer)
    {
        var length = Math.Min(buffer.Length, this.SampleLength - this.Position);

        var result = this._samples.Slice(this.Position, length);

        this.Position += length;

        result.Span.CopyTo(buffer);

        return length;
    }

    public ValueTask<int> ReadAsync(Memory<float> buffer, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<int>(cancellationToken);
        }

        return ValueTask.FromResult(this.Read(buffer.Span));
    }

    public float[] Read(int length)
    {
        this.CheckDisposed();

        var result = this._samples.Slice(this.Position, length).ToArray();

        this.Position += length;

        return result;
    }

    public ValueTask<float[]> ReadAsync(int length, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<float[]>(cancellationToken);
        }

        return ValueTask.FromResult(this.Read(length));
    }

    public float[] Read(TimeSpan interval)
    {
        var length = (int)(interval.TotalSeconds * this.SampleRate * this.Channels);

        return this.Read(length);
    }

    public ValueTask<float[]> ReadAsync(TimeSpan interval, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<float[]>(cancellationToken);
        }

        return ValueTask.FromResult(this.Read(interval));
    }

    public int Seek(int offset, SeekOrigin origin)
    {
        this.CheckDisposed();

        return this.Position = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => this.Position + offset,
            SeekOrigin.End => this.SampleLength - offset,
            _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null),
        };
    }

    public void Dispose()
    {
        this._disposed = true;

        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckDisposed()
    {
        if (!this._disposed)
        {
            return;
        }

        throw new ObjectDisposedException(nameof(MemoryAudioResource));
    }
}
namespace LabProject.Audio.Abstractions;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IAudioResource : IDisposable
{
    int SampleRate { get; }

    int Channels { get; }

    int Position { get; }

    int SampleLength { get; }

    TimeSpan Duration { get; }

    float Read();

    ValueTask<float> ReadAsync(CancellationToken cancellationToken = default);

    int Read(Span<float> buffer);

    ValueTask<int> ReadAsync(Memory<float> buffer, CancellationToken cancellationToken = default);

    float[] Read(int length);

    ValueTask<float[]> ReadAsync(int length, CancellationToken cancellationToken = default);

    float[] Read(TimeSpan interval);

    ValueTask<float[]> ReadAsync(TimeSpan interval, CancellationToken cancellationToken = default);

    int Seek(int offset, SeekOrigin origin);
}
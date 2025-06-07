namespace LabProject.Pooling;

using System;
using System.Collections.Concurrent;
using VoiceChat.Networking;

public class PlaybackBufferPool : IDisposable
{
    public const int MaxSize = 32;

    public static PlaybackBufferPool Shared { get; } = new PlaybackBufferPool();

    private readonly ConcurrentBag<PlaybackBuffer> pool = [];

    public PlaybackBuffer Rent()
    {
        return this.pool.TryTake(out var buffer) ? buffer : new PlaybackBuffer();
    }

    public void Return(PlaybackBuffer buffer)
    {
        if (this.pool.Count >= MaxSize)
        {
            buffer.Dispose();
            return;
        }

        this.pool.Add(buffer);
    }

    public void Dispose()
    {
        foreach (var playbackBuffer in this.pool)
        {
            playbackBuffer.Dispose();
        }
    }
}
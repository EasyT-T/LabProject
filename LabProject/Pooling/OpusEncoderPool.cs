namespace LabProject.Pooling;

using System;
using System.Collections.Concurrent;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;

public class OpusEncoderPool : IDisposable
{
    public const int MaxSize = 32;

    public static OpusEncoderPool Shared { get; } = new OpusEncoderPool();

    private readonly ConcurrentBag<OpusEncoder> pool = [];

    public OpusEncoder Rent()
    {
        return this.pool.TryTake(out var encoder) ? encoder : new OpusEncoder(OpusApplicationType.Voip);
    }

    public void Return(OpusEncoder encoder)
    {
        if (this.pool.Count >= MaxSize)
        {
            return;
        }

        this.pool.Add(encoder);
    }

    public void Dispose()
    {
        foreach (var encoder in this.pool)
        {
            encoder.Dispose();
        }
    }
}
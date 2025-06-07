namespace LabProject.Model;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using global::LabProject.Wave;
using NAudio.Vorbis;
using NAudio.Wave;

public class CustomAudio
{
    public delegate WaveStream WaveStreamCreator(string filename);

    private static readonly Dictionary<string, WaveStreamCreator> ExtendStreamCreators =
        new Dictionary<string, WaveStreamCreator>
        {
            { ".ogg", filename => new VorbisWaveReader(filename) },
            { ".wav", filename => new WaveFileReader(filename) },
            { ".mp3", filename => new LayerMp3FileReader(filename) },
            { ".aiff", filename => new AiffFileReader(filename) },
            { ".aif", filename => new AiffFileReader(filename) },
        };

    public string FileName { get; }

    private readonly ConcurrentBag<WaveStream> pool = [];

    private readonly Func<WaveStream> createNewWave;

    public CustomAudio(string filename)
    {
        var fileExt = Path.GetExtension(filename);

        if (!ExtendStreamCreators.TryGetValue(fileExt, out var creator))
        {
            throw new NotSupportedException("Audio file extension not supported.");
        }

        this.createNewWave = () => creator(filename);
        this.FileName = filename;
    }

    public WaveStream CreateWaveStream()
    {
        return this.pool.TryTake(out var waveStream) ? waveStream : this.createNewWave();
    }

    public void ReturnWaveStream(WaveStream waveStream)
    {
        waveStream.Seek(0, SeekOrigin.Begin);
        this.pool.Add(waveStream);
    }
}
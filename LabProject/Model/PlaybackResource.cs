namespace LabProject.Model;

using System;
using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using VoiceChat;

public class PlaybackResource : IDisposable
{
    public string PlaybackName { get; }

    public WaveStream WaveStream { get; }
    public ISampleProvider SampleProvider { get; }

    private readonly CustomAudio audio;

    public PlaybackResource(CustomAudio audio)
    {
        var stream = audio.CreateWaveStream();
        var provider = stream.ToSampleProvider();

        if (stream.WaveFormat.Channels != VoiceChatSettings.Channels)
        {
            provider = new StereoToMonoSampleProvider(provider);
        }

        if (stream.WaveFormat.SampleRate != VoiceChatSettings.SampleRate)
        {
            provider = new WdlResamplingSampleProvider(provider, VoiceChatSettings.SampleRate);
        }

        this.audio = audio;

        this.PlaybackName = Path.GetFileNameWithoutExtension(audio.FileName);
        this.WaveStream = stream;
        this.SampleProvider = provider;
    }

    public void Dispose()
    {
        this.audio.ReturnWaveStream(this.WaveStream);
    }
}
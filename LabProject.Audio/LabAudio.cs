namespace LabProject.Audio;

using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using LabProject.Audio.Abstractions;
using LabProject.Audio.IO;

public static class LabAudio
{
    private static readonly ContainerFormat PCM32Format = FFMpeg.GetContainerFormat("s32le");

    public static IAudioResource LoadFromFile(string filename, bool streaming = true)
    {
        var input = FFMpegArguments.FromFileInput(filename);

        return streaming ? LoadToStream(input) : LoadToMemory(input); // TODO(260613) Real streaming
    }

    public static Task<IAudioResource> LoadFromFileAsync(string filename, bool streaming = true)
    {
        var input = FFMpegArguments.FromFileInput(filename);

        return streaming ? LoadToStreamAsync(input) : LoadToMemoryAsync(input);
    }

    private static IAudioResource LoadToMemory(FFMpegArguments arguments)
    {
        var outputStream = new MemoryStream();

        var result = CreateOutputToStream(arguments, outputStream).ProcessSynchronously();

        Guard.IsTrue(result);

        return MemoryAudioResource(outputStream.GetBuffer());
    }

    private static async Task<IAudioResource> LoadToMemoryAsync(FFMpegArguments arguments)
    {
        var outputStream = new MemoryStream();

        var result = await CreateOutputToStream(arguments, outputStream).ProcessAsynchronously();

        Guard.IsTrue(result);

        return MemoryAudioResource(outputStream.GetBuffer());
    }

    private static IAudioResource LoadToStream(FFMpegArguments arguments)
    {
        var tempFile = Path.GetTempFileName();

        var result = CreateOutputToFile(arguments, tempFile).ProcessSynchronously();

        Guard.IsTrue(result);

        return StreamAudioResource(new TempFileStream(tempFile, FileMode.Open));
    }

    private static async Task<IAudioResource> LoadToStreamAsync(FFMpegArguments arguments)
    {
        var tempFile = Path.GetTempFileName();

        var result = await CreateOutputToFile(arguments, tempFile).ProcessAsynchronously();

        Guard.IsTrue(result);

        return StreamAudioResource(new TempFileStream(tempFile, FileMode.Open));
    }

    private static MemoryAudioResource MemoryAudioResource(byte[] buffer)
    {
        return new MemoryAudioResource(AudioFacts.SampleRate, AudioFacts.Channels, buffer);
    }

    private static StreamAudioResource StreamAudioResource(Stream stream)
    {
        return new StreamAudioResource(AudioFacts.SampleRate, AudioFacts.Channels, stream);
    }

    private static void CreateOptions(FFMpegArgumentOptions options)
    {
        options
            .ForceFormat(PCM32Format)
            .WithAudioSamplingRate()
            .WithCustomArgument($"-ac {AudioFacts.Channels}");
    }

    private static FFMpegArgumentProcessor CreateOutputToStream(FFMpegArguments arguments, MemoryStream memory)
    {
        var outputPipe = new StreamPipeSink(memory);

        return arguments.OutputToPipe(
            outputPipe,
            CreateOptions);
    }

    private static FFMpegArgumentProcessor CreateOutputToFile(FFMpegArguments arguments, string filePath)
    {
        return arguments.OutputToFile(filePath, true, CreateOptions);
    }
}
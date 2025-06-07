namespace LabProject.Example;

using System.Collections.Generic;
using LabApi.Features.Wrappers;
using LabProject.Model;
using LabProject.Pooling;

public class AudioManager
{
    public static AudioManager Singleton { get; } = new AudioManager();

    private HashSet<AudioPlayer> audioPlayers = [];

    private AudioManager()
    {
    }

    public AudioPlayer Play(Player player, string filename)
    {
        var bot = BotPool.Shared.Rent(player, "Audio Player");

        var audioPlayer = new AudioPlayer(bot, true);

        audioPlayer.EnqueueAudio(new CustomAudio(filename));
        audioPlayer.Play();

        this.audioPlayers.Add(audioPlayer);

        return audioPlayer;
    }

    public void Stop(AudioPlayer audioPlayer)
    {
        this.audioPlayers.Remove(audioPlayer);

        audioPlayer.Dispose();

        BotPool.Shared.Return(audioPlayer.Bot);
    }
}
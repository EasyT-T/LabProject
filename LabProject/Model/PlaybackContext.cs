namespace LabProject.Model;

using System;

public readonly struct PlaybackContext(string name, TimeSpan currentTime, TimeSpan totalTime)
{
    public string AudioName { get; } = name;
    public TimeSpan CurrentTime { get; } = currentTime;
    public TimeSpan TotalTime { get; } = totalTime;
}
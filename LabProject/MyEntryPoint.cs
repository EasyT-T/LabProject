namespace LabProject;

using LabProject.Attributes;
using LabProject.DI;
using Microsoft.Extensions.Logging;

[Injectable]
public class MyEntryPoint(ILogger<MyEntryPoint> logger) : ILoad
{
    public void Load()
    {
        logger.LogInformation("Example loaded!");
    }
}
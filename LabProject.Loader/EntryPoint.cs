namespace LabProject.Loader;

using System;
using CommunityToolkit.Diagnostics;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using LabProject.Loader.Extensions;
using LabProject.Loader.Injectables;
using LabProject.Loader.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

internal class EntryPoint : Plugin
{
    public override string Name => "LabProject";

    public override string Description => "Ciallo～(∠・ω< )⌒★";

    public override string Author => "EasyT_T";

    public override Version Version { get; } = new Version(0, 1, 0);

    public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;

    private ServiceManager? _serviceManager;

    public override void Enable()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddSingleton<Plugin, EntryPoint>(_ => this)
            .AddSingleton<IEnumerable<ServiceDescriptor>>(_ => serviceCollection)
            .AddLogging(builder => builder.Services.AddSingleton<ILoggerProvider, LabLoggerProvider>())
            .AddAllServices();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        this._serviceManager = serviceProvider.GetRequiredService<ServiceManager>();

        this._serviceManager.Load();
    }

    public override void Disable()
    {
        Guard.IsNotNull(this._serviceManager);

        this._serviceManager.Unload();
    }
}
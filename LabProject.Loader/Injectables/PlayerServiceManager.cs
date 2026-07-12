namespace LabProject.Loader.Injectables;

using System.Linq.Expressions;
using System.Reflection;
using LabApi.Features.Wrappers;
using LabProject.Attributes;
using LabProject.DI;
using LabProject.Loader.Logging;
using LabProject.Loader.PlayerInjectables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

[Injectable]
public class PlayerServiceManager(IEnumerable<IService> services, IEnumerable<ServiceDescriptor> descriptors) : ILoad, IUnload
{
    private readonly ServiceCollection _serviceCollection = [];

    private readonly List<Type> _serviceTypes = [];

    private static Delegate CastFuncDynamic<T, TResult>(Func<T, TResult> sourceFunc, Type targetReturnType)
    {
        var paramType = typeof(T);

        var parameter = Expression.Parameter(paramType, "param");

        var invokeExpression = Expression.Invoke(Expression.Constant(sourceFunc), parameter);

        var castExpression = Expression.Convert(invokeExpression, targetReturnType);

        var delegateType = typeof(Func<,>).MakeGenericType(paramType, targetReturnType);

        var lambda = Expression.Lambda(delegateType, castExpression, parameter);

        return lambda.Compile();
    }

    void ILoad.Load()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            this.HandleAssembly(assembly);
        }

        var group = descriptors.Join(
            services,
            descriptor => descriptor.ImplementationType,
            service => service.GetType(),
            (descriptor, service) => new { Descriptor = descriptor, Service = service });

        foreach (var groupItem in group)
        {
            this._serviceCollection.Add(new ServiceDescriptor(groupItem.Descriptor.ServiceType, groupItem.Service));
        }

        this._serviceCollection.AddScoped<PlayerContext>();
        this._serviceCollection.AddScoped<IPlayerContext, PlayerContext>();
        this._serviceCollection.AddLogging(builder =>
            builder.Services.AddScoped<ILoggerProvider, PlayerLoggerProvider>());

        foreach (var service in this._serviceTypes)
        {
            var setServiceFactoryMethod = service.GetMethod("SetServiceFactory", BindingFlags.Static | BindingFlags.Public);
            var registerMethod = service.GetMethod("RegisterService", BindingFlags.Static | BindingFlags.Public);

            setServiceFactoryMethod?.Invoke(null, [CastFuncDynamic<Player, PlayerService>(Factory, service)]);
            registerMethod?.Invoke(null, null);

            continue;

            PlayerService Factory(Player player) => this.CreateService(player, service);
        }
    }

    void IUnload.Unload()
    {
        foreach (var unregisterMethod in this._serviceTypes.Select(service =>
                     service.GetMethod("UnregisterService", BindingFlags.Static | BindingFlags.Public)))
        {
            unregisterMethod?.Invoke(null, null);
        }

        this._serviceCollection.Clear();
        this._serviceTypes.Clear();
    }

    private PlayerService CreateService(Player player, Type serviceType)
    {
        var provider = this._serviceCollection.BuildServiceProvider();

        var context = provider.GetRequiredService<PlayerContext>();

        context.Player = player;

        return (PlayerService)provider.GetRequiredService(serviceType);
    }

    private static IEnumerable<Type> GetPlayerServicesFromAssembly(Assembly assembly)
    {
        return assembly.GetTypes().Where(type => typeof(PlayerService).IsAssignableFrom(type) && !type.IsAbstract);
    }

    private void HandleAssembly(Assembly assembly)
    {
        foreach (var type in GetPlayerServicesFromAssembly(assembly))
        {
            this._serviceTypes.Add(type);

            this._serviceCollection.AddScoped(type);

            var interfaces = type.GetInterfaces();

            if (interfaces.Length <= 0)
            {
                continue;
            }

            foreach (var serviceType in interfaces)
            {
                this._serviceCollection.AddScoped(serviceType, type);
            }
        }
    }
}
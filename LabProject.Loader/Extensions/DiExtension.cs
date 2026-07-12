namespace LabProject.Loader.Extensions;

using System;
using System.Collections.Concurrent;
using System.Reflection;
using LabProject.Attributes;
using LabProject.DI;
using Microsoft.Extensions.DependencyInjection;

public static class DiExtension
{
    private struct Injectable(InjectableAttribute attribute, Type injectionType)
    {
        public readonly InjectableAttribute Attribute = attribute;

        public readonly Type InjectionType = injectionType;
    }

    private static void AddInjectableFromAssembly(Assembly assembly, ConcurrentBag<Injectable> injectableBag)
    {
        var types = assembly.GetTypes();

        Parallel.ForEach(
            types,
            type =>
            {
                var attribute = type.GetCustomAttribute<InjectableAttribute>();

                if (attribute is null)
                {
                    return;
                }

                injectableBag.Add(new Injectable(attribute, type));
            });
    }

    extension(IServiceCollection serviceCollection)
    {
        internal IServiceCollection AddAllServices()
        {
            var injectableBag = new ConcurrentBag<Injectable>();

            Parallel.ForEach(AppDomain.CurrentDomain.GetAssemblies(), assembly => AddInjectableFromAssembly(assembly, injectableBag));

            var ordered = injectableBag.OrderBy(injectable => injectable.Attribute.Priority);

            foreach (var injectable in ordered)
            {
                var attribute = injectable.Attribute;
                var type = injectable.InjectionType;

                var singleFactory = attribute.InjectionType.GetInjectFactorySingle(serviceCollection);

                singleFactory(type);

                var interfaces = type.GetInterfaces();

                if (interfaces.Length == 0)
                {
                    continue;
                }

                var factory = attribute.InjectionType.GetInjectFactoryWithImplementations(serviceCollection);

                foreach (var @interface in interfaces)
                {
                    factory(@interface, type);
                }
            }

            return serviceCollection;
        }
    }

    extension(InjectionType injectionType)
    {
        public Func<Type, IServiceCollection> GetInjectFactorySingle(IServiceCollection serviceCollection)
        {
            return injectionType switch
            {
                InjectionType.Transient => serviceCollection.AddTransient,
                InjectionType.Scoped => serviceCollection.AddScoped,
                InjectionType.Singleton => serviceCollection.AddSingleton,
                _ => throw new ArgumentOutOfRangeException(nameof(injectionType), injectionType, null),
            };
        }

        public Func<Type, Type, IServiceCollection> GetInjectFactoryWithImplementations(IServiceCollection serviceCollection)
        {
            return injectionType switch
            {
                InjectionType.Transient => serviceCollection.AddTransient,
                InjectionType.Scoped => serviceCollection.AddScoped,
                InjectionType.Singleton => serviceCollection.AddSingleton,
                _ => throw new ArgumentOutOfRangeException(nameof(injectionType), injectionType, null),
            };
        }
    }
}
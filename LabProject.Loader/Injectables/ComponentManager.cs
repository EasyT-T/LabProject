namespace LabProject.Loader.Injectables;

using System;
using System.Collections.Generic;
using System.Reflection;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabProject.Attributes;
using LabProject.DI;
using UnityEngine;

[Injectable]
public class ComponentManager : IService, ILoad, IUnload
{
    private readonly List<Type> _components = [];

    void ILoad.Load()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            this.AddComponentsFromAssembly(assembly);
        }

        PlayerEvents.Joined += this.OnPlayerJoined;
    }

    void IUnload.Unload()
    {
        PlayerEvents.Joined -= this.OnPlayerJoined;

        this._components.Clear();
    }

    private void OnPlayerJoined(PlayerJoinedEventArgs ev)
    {
        if (ev.Player.GameObject == null)
        {
            return;
        }

        foreach (var component in this._components)
        {
            ev.Player.GameObject.AddComponent(component);
        }
    }

    private void AddComponentsFromAssembly(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!Attribute.IsDefined(type, typeof(PlayerComponentAttribute)))
            {
                continue;
            }

            if (!typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                continue;
            }

            this._components.Add(type);
        }
    }
}
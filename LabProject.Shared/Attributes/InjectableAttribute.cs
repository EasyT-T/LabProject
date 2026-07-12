namespace LabProject.Attributes;

using System;
using JetBrains.Annotations;
using LabProject.DI;

[AttributeUsage(validOn: AttributeTargets.Class, Inherited = false)]
[MeansImplicitUse]
public class InjectableAttribute(InjectionType injectionType = InjectionType.Singleton, int priority = 10000) : Attribute
{
    public InjectionType InjectionType { get; } = injectionType;

    public int Priority { get; } = priority;
}
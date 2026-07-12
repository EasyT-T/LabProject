namespace LabProject.Attributes;

using System;
using JetBrains.Annotations;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[MeansImplicitUse]
public class PlayerComponentAttribute : Attribute;
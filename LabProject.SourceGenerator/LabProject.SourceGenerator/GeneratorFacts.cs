namespace LabProject.SourceGenerator;

using Microsoft.CodeAnalysis;

public static class GeneratorFacts
{
    public static readonly DiagnosticDescriptor ClassMustBePartial = new DiagnosticDescriptor(
        id: "PS001",
        title: "Target class must be declared as partial",
        messageFormat: "The class '{0}' must have the 'partial' modifier to allow source generation",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The source generator needs to emit a partial class companion file. Without the 'partial' modifier, the generated code cannot be merged into the existing class."
    );

    public static readonly DiagnosticDescriptor ClassMustBeAbstract = new DiagnosticDescriptor(
        id: "PS002",
        title: "Target class must be declared as abstract",
        messageFormat: "The class '{0}' must have the 'abstract' modifier to allow source generation",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor InvalidTypeArgument = new DiagnosticDescriptor(
        id: "PS003",
        title: "Invalid type specified in typeof expression",
        messageFormat: "The type '{0}' is not a valid target. The 'typeof' argument must be a concrete named type (a class, struct, or interface) available in the source code.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The source generator cannot inspect or generate code for arrays, pointers, open generics, or types located in external compiled assemblies."
    );

    public static readonly DiagnosticDescriptor MustInheritFromBaseClass = new DiagnosticDescriptor(
        id: "PS004",
        title: "Class does not inherit from any base class",
        messageFormat: "The class '{0}' does not inherit from any base class. It must inherit from a class decorated with [PlayerServiceBaseAttribute].",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The targeted class must have an explicit base class that is decorated with [PlayerServiceBaseAttribute]."
    );

    public static readonly DiagnosticDescriptor MissingRequiredBaseAttribute = new DiagnosticDescriptor(
        id: "PS005",
        title: "Base class is missing PlayerServiceBaseAttribute",
        messageFormat: "The class '{0}' inherits from '{1}', but '{1}' does not have the required [PlayerServiceBaseAttribute] attribute",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Classes targeted by this attribute must inherit from a base class that is decorated with [PlayerServiceBaseAttribute]."
    );

    public const string GlobalAutoGenPrefix = "AutoGen_";

    public const string LocalAutoGenPrefix = "ag_";

    public const string Namespace = "LabProject.SourceGenerator";

    public const string PlayerServiceAttributeName = "PlayerServiceAttribute";

    public const string PlayerServiceBaseAttributeName = "PlayerServiceBaseAttribute";

    public const string ServiceDictionaryName = GlobalAutoGenPrefix + "Services";

    public const string GlobalHandlerPrefix = GlobalAutoGenPrefix + "GlobalOnPlayer";

    public const string LocalHandlerPrefix = "OnPlayer";

    public const string GetServiceMethodName = "GetService";

    public const string ServiceFactoryName = GlobalAutoGenPrefix + "ServiceFactory";

    public const string SetServiceFactoryMethodName = "SetServiceFactory";

    public const string CreateServiceMethodName = "CreateService";

    public const string DestroyServiceMethodName = "DestroyService";

    public const string RegisterServiceMethodName = "RegisterService";

    public const string UnregisterServiceMethodName = "UnregisterService";

    public const string PlayerJoinedEvent = "Joined";

    public const string PlayerJoinedEventArgs = "LabApi.Events.Arguments.PlayerEvents.PlayerJoinedEventArgs";

    public const string PlayerLeftEvent = "Left";

    public const string PlayerLeftEventArgs = "LabApi.Events.Arguments.PlayerEvents.PlayerLeftEventArgs";
}
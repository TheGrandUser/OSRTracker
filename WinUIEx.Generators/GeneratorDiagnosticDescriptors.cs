using Microsoft.CodeAnalysis;

namespace WinUIEx.Generators;

public class GeneratorDiagnosticDescriptors
{
   public static readonly DiagnosticDescriptor TopLevelError = new(id: "WINUIEXGEN001",
                                                                                 title: "Class must be top level",
                                                                                 messageFormat: "Class '{0}' using WinUIExGenerator must be top level",
                                                                                 category: "WinUIExGenerator",
                                                                                 DiagnosticSeverity.Error,
                                                                                 isEnabledByDefault: true);

   public static readonly DiagnosticDescriptor WrongBaseType = new(id: "WINUIEXGEN002",
                                                                                 title: "Class must inherit from RowWrapperBase",
                                                                                 messageFormat: "Class '{0}' does not inherit from RowWrapperBase",
                                                                                 category: "WinUIExGenerator",
                                                                                 DiagnosticSeverity.Error,
                                                                                 isEnabledByDefault: true);

   public static readonly DiagnosticDescriptor ObjectIsRowWrapperBaseType = new(id: "WINUIEXGEN004",
                                                                                 title: "Object is not a valid type parameter",
                                                                                 messageFormat: "Defined conversions to or from a base type are not allowed for class '{0}'",
                                                                                 category: "WinUIExGenerator",
                                                                                 DiagnosticSeverity.Error,
                                                                                 isEnabledByDefault: true);

   public static readonly DiagnosticDescriptor UserDefinedConversionsToOrFromAnInterfaceAreNotAllowed = new(id: "WINUIEXGEN005",
                                                                                 title: "user-defined conversions to or from an interface are not allowed",
                                                                                 messageFormat: "user-defined conversions to or from an interface are not allowed",
                                                                                 category: "WinUIExGenerator",
                                                                                 DiagnosticSeverity.Error,
                                                                                 isEnabledByDefault: true);
}
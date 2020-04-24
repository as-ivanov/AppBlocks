using System.Collections.Immutable;
using AppBlocks.CodeGeneration.Roslyn.Common;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn
{
  public class MetricsCollectorDescriptor: IInterfaceImplementationDescriptor
  {
	  private readonly string _className;
	  private readonly string[] _inheritedInterfaceTypes;
	  private readonly TypeDeclarationSyntax _typeDeclarationSyntax;
	  private readonly bool _isAbstract;
	  private readonly string _baseClassName;
    private readonly string _contextName;
    private readonly ImmutableArray<MetricsCollectorMethod> _methods;

    public MetricsCollectorDescriptor(TypeDeclarationSyntax typeDeclarationSyntax, bool isAbstract, string contextName, string className, string baseClassName, string[] inheritedInterfaceTypes, ImmutableArray<MetricsCollectorMethod> methods)
    {
      _typeDeclarationSyntax = typeDeclarationSyntax;
      _isAbstract = isAbstract;
      _contextName = contextName;
      _className = className;
      _baseClassName = baseClassName;
      _inheritedInterfaceTypes = inheritedInterfaceTypes;
      _methods = methods;
    }

    public string ClassName => _className;

    public string BaseClassName => _baseClassName;

    public string[] InheritedInterfaceTypes => _inheritedInterfaceTypes;

    public TypeDeclarationSyntax DeclarationSyntax => _typeDeclarationSyntax;

    public string ContextName => _contextName;

    public ImmutableArray<MetricsCollectorMethod> Methods => _methods;

    public bool IsAbstract => _isAbstract;
  }
}
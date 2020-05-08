using System.Collections.Immutable;
using AppBlocks.CodeGeneration.Roslyn.Common;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn
{
	public class MetricsCollectorDescriptor : IInterfaceImplementationDescriptor
	{
		private readonly string _className;
		private readonly string _contextName;
		private readonly string[] _inheritedInterfaceTypes;
		private readonly ImmutableArray<MetricsCollectorMethod> _methods;
		private readonly TypeDeclarationSyntax _typeDeclarationSyntax;

		public MetricsCollectorDescriptor(TypeDeclarationSyntax typeDeclarationSyntax, string contextName, string className, string[] inheritedInterfaceTypes, ImmutableArray<MetricsCollectorMethod> methods)
		{
			_typeDeclarationSyntax = typeDeclarationSyntax;
			_contextName = contextName;
			_className = className;
			_inheritedInterfaceTypes = inheritedInterfaceTypes;
			_methods = methods;
		}

		public string ContextName => _contextName;

		public ImmutableArray<MetricsCollectorMethod> Methods => _methods;

		public string ClassName => _className;

		public string[] InheritedInterfaceTypes => _inheritedInterfaceTypes;

		public TypeDeclarationSyntax DeclarationSyntax => _typeDeclarationSyntax;
	}
}
using System.Collections.Immutable;
using AppBlocks.CodeGeneration.Roslyn.Common;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.Logging.CodeGeneration.Roslyn
{
	public class LoggerDescriptor : IInterfaceImplementationDescriptor
	{
		private readonly string _className;
		private readonly string[] _inheritedInterfaceTypes;
		private readonly ImmutableArray<LoggerMethod> _methods;
		private readonly TypeDeclarationSyntax _typeDeclarationSyntax;

		public LoggerDescriptor(TypeDeclarationSyntax typeDeclarationSyntax, string className, string[] inheritedInterfaceTypes, ImmutableArray<LoggerMethod> methods)
		{
			_className = className;
			_inheritedInterfaceTypes = inheritedInterfaceTypes;
			_typeDeclarationSyntax = typeDeclarationSyntax;
			_methods = methods;
		}

		public ImmutableArray<LoggerMethod> Methods => _methods;
		public string[] InheritedInterfaceTypes => _inheritedInterfaceTypes;
		public string ClassName => _className;
		public TypeDeclarationSyntax DeclarationSyntax => _typeDeclarationSyntax;
	}
}
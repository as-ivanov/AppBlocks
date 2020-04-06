using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneration.Roslyn.Logger
{
	internal class LoggerDescriptor
	{
		private readonly string _className;
		private readonly string[] _inheritedInterfaceTypes;
		private readonly TypeDeclarationSyntax _typeDeclarationSyntax;
		private readonly string _baseClass;
		private readonly ImmutableArray<LoggerMethod> _methods;

		public LoggerDescriptor(TypeDeclarationSyntax typeDeclarationSyntax, string className, string baseClass, string[] inheritedInterfaceTypes, ImmutableArray<LoggerMethod> methods)
		{
			_className = className;
			_inheritedInterfaceTypes = inheritedInterfaceTypes;
			_typeDeclarationSyntax = typeDeclarationSyntax;
			_baseClass = baseClass;
			_methods = methods;
		}

		public string BaseClass => _baseClass;
		public ImmutableArray<LoggerMethod> Methods => _methods;

		public string[] InheritedInterfaceTypes => _inheritedInterfaceTypes;

		public string ClassName => _className;

		public TypeDeclarationSyntax DeclarationSyntax => _typeDeclarationSyntax;
	}
}
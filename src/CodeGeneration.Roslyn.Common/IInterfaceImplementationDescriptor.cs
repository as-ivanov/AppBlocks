using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneration.Roslyn.Common
{
	public interface IInterfaceImplementationDescriptor
	{
		string ClassName { get; }

		string BaseClassName { get; }
		string[] InheritedInterfaceTypes { get; }

		TypeDeclarationSyntax DeclarationSyntax { get; }
	}
}
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public interface IInterfaceImplementationDescriptor
	{
		string ClassName { get; }
		public bool IsAbstract { get; }
		string BaseClassName { get; }
		string[] InheritedInterfaceTypes { get; }
		TypeDeclarationSyntax DeclarationSyntax { get; }
	}
}
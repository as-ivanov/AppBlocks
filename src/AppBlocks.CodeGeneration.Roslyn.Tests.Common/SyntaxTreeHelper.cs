using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common
{
	public static class SyntaxTreeHelper
	{
		public static SyntaxTree GetEmptyInterfaceSyntax(string namespaceName, string interfaceName)
		{
			return CSharpSyntaxTree.ParseText($"namespace {namespaceName} {{ public interface {interfaceName} {{ }} }}");
		}
	}
}

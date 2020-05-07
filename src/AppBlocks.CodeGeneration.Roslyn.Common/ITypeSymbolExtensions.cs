using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	internal static class ITypeSymbolExtensions
	{
		public static bool IsNullable(this ITypeSymbol symbol)
			=> symbol?.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

		public static TypeSyntax GenerateTypeSyntax(
			this INamespaceOrTypeSymbol symbol)
		{
			return symbol.ToTypeSyntax();
		}
	}
}
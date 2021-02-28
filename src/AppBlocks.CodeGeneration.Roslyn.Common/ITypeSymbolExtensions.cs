using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	internal static class ITypeSymbolExtensions
	{
		public static bool IsNullable(this ITypeSymbol symbol)
		{
			return symbol?.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
		}

		public static TypeSyntax GenerateTypeSyntax(
			this INamespaceOrTypeSymbol symbol)
		{
			return symbol.ToTypeSyntax();
		}

		public static NameSyntax ToGlobalAliasQualifiedName(this ITypeSymbol symbol)
		{
			return IdentifierName(symbol.GetFullTypeName());
		}
	}
}

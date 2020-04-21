using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public static class TypeExtensions
	{
		public static TypeSyntax GetGlobalTypeSyntax(this Type type)
		{
			return AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName(type.FullName));
		}
	}
}

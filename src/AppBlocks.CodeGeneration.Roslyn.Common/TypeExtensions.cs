using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public static class TypeExtensions
	{
		public static TypeSyntax GetTypeSyntax(this Type type)
		{
			return AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName(type.FullName));
		}

		public static TypeSyntax GetTypeSyntax1(this Type type)
		{
			return AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName(type.FullName));
		}

	}
}

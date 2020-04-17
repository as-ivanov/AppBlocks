using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneration.Roslyn.Common
{
	public static class SyntaxExtensions
	{
		public static string ToCamelCase(this SyntaxToken syntaxToken)
		{
			var id = (string) syntaxToken.Value;
			return id.ToCamelCase();
		}

		public static string GetClassNameFromInterfaceDeclaration(this TypeDeclarationSyntax typeDeclarationSyntax, bool fullName = true)
		{
			return typeDeclarationSyntax.Identifier.WithoutTrivia().Text.GetClassNameFromInterfaceName(fullName);
		}

		public static string GetBaseClassName(this TypeDeclarationSyntax typeDeclarationSyntax)
		{
			var baseList = typeDeclarationSyntax.BaseList?.Types;
			if (baseList.HasValue
			    && baseList.Value.Any())
			{
				var firstOne = baseList.Value.First();
				var fullName = firstOne.Type.ToString();
				return fullName.GetClassNameFromInterfaceName();
			}

			return null;
		}

		public static SeparatedSyntaxList<T> ToSeparatedList<T>(this IEnumerable<T> nodes, SyntaxKind separator = SyntaxKind.CommaToken)
			where T : SyntaxNode
		{
			var nodesArray = nodes == null ? new T[0] : nodes.ToArray();
			return SyntaxFactory.SeparatedList(nodesArray, Enumerable.Repeat(SyntaxFactory.Token(separator), Math.Max(nodesArray.Length - 1, 0)));
		}

		public static string GetFullTypeName(this INamedTypeSymbol symbol)
		{
			var nameBuilder = new StringBuilder();
			ISymbol symbolOrParent = symbol;
			while (symbolOrParent != null && !string.IsNullOrEmpty(symbolOrParent.Name))
			{
				if (nameBuilder.Length > 0)
				{
					nameBuilder.Insert(0, ".");
				}

				nameBuilder.Insert(0, symbolOrParent.Name);
				symbolOrParent = symbolOrParent.ContainingSymbol;
			}

			return nameBuilder.ToString();
		}
	}
}
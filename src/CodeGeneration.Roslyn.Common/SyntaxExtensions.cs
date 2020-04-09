using Microsoft.CodeAnalysis;
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

		public static string GetClassName(this TypeDeclarationSyntax typeDeclarationSyntax)
		{
			return typeDeclarationSyntax.Identifier.WithoutTrivia().Text.GetClassName();
		}

		public static string GetBaseClassName(this TypeDeclarationSyntax typeDeclarationSyntax)
		{
			var baseList = typeDeclarationSyntax.BaseList?.Types;
			if (baseList.HasValue
			    && baseList.Value.Any())
			{
				var firstOne = baseList.Value.First();
				var fullName = firstOne.Type.ToString();
				return fullName.GetClassName();
			}

			return null;
		}
	}
}
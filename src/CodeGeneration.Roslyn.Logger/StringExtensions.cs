using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneration.Roslyn.Logger
{
	internal static class StringExtensions
	{
		public static string ToPascalCase(this string word)
		{
			return string.Join(" ", word.Split(' ')
			  .Select(w => w.Trim())
			  .Where(w => w.Length > 0)
			  .Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1)));
		}

		public static string ToCamelCase(this string input)
		{
			return char.ToLowerInvariant(input[0]) + input.Substring(1);
		}

		public static string GetClassName(this string word)
		{
			if (word.StartsWith('I'))
			{
				word = word.Substring(1);
			}
			return word;
		}

		public static LiteralExpressionSyntax GetLiteralExpression(this string str)
		{
			var text = str == null ? "\"\"" : "@\"" + str.Replace("\"", "\"\"") + "\"";
			var syntaxToken = SyntaxFactory.Literal(
				SyntaxFactory.TriviaList(),
				text,
				str,
				SyntaxFactory.TriviaList());
			return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, syntaxToken);
		}
	}
}
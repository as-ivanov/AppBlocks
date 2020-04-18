using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneration.Roslyn.Common
{
	public static class StringExtensions
	{
		public static string ToPascalCase(this string input)
		{
			return string.Join(" ", input.Split(' ')
				.Select(w => w.Trim())
				.Where(w => w.Length > 0)
				.Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1)));
		}

		public static string ToCamelCase(this string input)
		{
			return char.ToLowerInvariant(input[0]) + input.Substring(1);
		}

		public static string GetTypeNameWithoutNamespaces(this string input)
		{
			var index = input.LastIndexOf(".", StringComparison.Ordinal);
			if (index == -1)
			{
				return input;
			}
			return input.Substring(index + 1);
		}

		public static string GetNamespacesWithoutTypeName(this string input)
		{
			var index = input.LastIndexOf(".", StringComparison.Ordinal);
			if (index == -1)
			{
				return string.Empty;
			}
			return input.Substring(0, index);
		}

		public static string GetClassNameFromInterfaceName(this string input, bool fullName = true)
		{
			var typeName = input.GetTypeNameWithoutNamespaces();
			if (typeName.StartsWith("I"))
			{
				typeName = typeName.Substring(1);
			}
			if (!fullName)
			{
				return typeName;
			}
			var namespaces = input.GetNamespacesWithoutTypeName();
			return string.IsNullOrEmpty(namespaces) ? typeName : namespaces + "." + typeName;
		}

		public static LiteralExpressionSyntax GetLiteralExpression(this string input)
		{
			var text = input == null ? "\"\"" : "@\"" + input.Replace("\"", "\"\"") + "\"";
			var syntaxToken = SyntaxFactory.Literal(
				SyntaxFactory.TriviaList(),
				text,
				input,
				SyntaxFactory.TriviaList());
			return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, syntaxToken);
		}
	}
}
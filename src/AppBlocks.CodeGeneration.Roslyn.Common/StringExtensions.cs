using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;

namespace AppBlocks.CodeGeneration.Roslyn.Common
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
			return index == -1 ? input : input.Substring(index + 1);
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
			if (input == null)
			{
				return SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
			}

			var text = "@\"" + input.Replace("\"", "\"\"") + "\"";
			var syntaxToken = SyntaxFactory.Literal(
				SyntaxFactory.TriviaList(),
				text,
				input,
				SyntaxFactory.TriviaList());
			return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, syntaxToken);
		}

		public static SyntaxToken ToIdentifierToken(
			this string identifier,
			bool isQueryContext = false)
		{
			var escaped = identifier.EscapeIdentifier(isQueryContext);

			if (escaped.Length == 0 || escaped[0] != '@')
			{
				return SyntaxFactory.Identifier(escaped);
			}

			var unescaped = identifier.StartsWith("@", StringComparison.Ordinal)
				? identifier.Substring(1)
				: identifier;

			var token = SyntaxFactory.Identifier(
				default, SyntaxKind.None, "@" + unescaped, unescaped, default);

			if (!identifier.StartsWith("@", StringComparison.Ordinal))
			{
				token = token.WithAdditionalAnnotations(Simplifier.Annotation);
			}

			return token;
		}

		public static string EscapeIdentifier(
			this string identifier,
			bool isQueryContext = false)
		{
			var nullIndex = identifier.IndexOf('\0');
			if (nullIndex >= 0)
			{
				identifier = identifier.Substring(0, nullIndex);
			}

			var needsEscaping = SyntaxFacts.GetKeywordKind(identifier) != SyntaxKind.None;

			// Check if we need to escape this contextual keyword
			needsEscaping = needsEscaping || isQueryContext && SyntaxFacts.IsQueryContextualKeyword(SyntaxFacts.GetContextualKeywordKind(identifier));

			return needsEscaping ? "@" + identifier : identifier;
		}

		public static IdentifierNameSyntax ToIdentifierName(this string identifier)
		{
			return SyntaxFactory.IdentifierName(identifier.ToIdentifierToken());
		}

		public static bool IsPointerType(this ISymbol symbol)
		{
			return symbol is IPointerTypeSymbol;
		}
	}
}
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public static class SyntaxNodeExtensions
	{
		public static T WithPrependedLeadingTrivia<T>(
			this T node,
			params SyntaxTrivia[] trivia)
			where T : SyntaxNode
		{
			if (trivia.Length == 0)
			{
				return node;
			}

			return node.WithPrependedLeadingTrivia((IEnumerable<SyntaxTrivia>)trivia);
		}

		public static T WithPrependedLeadingTrivia<T>(
			this T node,
			IEnumerable<SyntaxTrivia> trivia)
			where T : SyntaxNode
		{
			var list = default(SyntaxTriviaList);
			list = list.AddRange(trivia);

			return node.WithPrependedLeadingTrivia(list);
		}

		public static T WithAppendedTrailingTrivia<T>(
			this T node,
			params SyntaxTrivia[] trivia)
			where T : SyntaxNode
		{
			if (trivia.Length == 0)
			{
				return node;
			}

			return node.WithAppendedTrailingTrivia((IEnumerable<SyntaxTrivia>)trivia);
		}

		public static T WithAppendedTrailingTrivia<T>(
			this T node,
			IEnumerable<SyntaxTrivia> trivia)
			where T : SyntaxNode
		{
			var list = default(SyntaxTriviaList);
			list = list.AddRange(trivia);

			return node.WithAppendedTrailingTrivia(list);
		}
	}
}

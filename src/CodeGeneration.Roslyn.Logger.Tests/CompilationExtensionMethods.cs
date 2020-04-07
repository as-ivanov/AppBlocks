using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CodeGeneration.Roslyn.Logger.Tests
{
	internal static class CompilationExtensionMethods
	{
		public static ImmutableArray<AttributeData> GetAttributeData<TCompilation>(
			this TCompilation compilation, SemanticModel document, SyntaxNode syntaxNode)
			where TCompilation : Compilation
		{
			IEnumerable<AttributeData> MineForAttributeData()
			{
				foreach (var y in syntaxNode.DescendantNodesAndSelf().SelectMany(
					x => document.GetDeclaredSymbol(x)?.GetAttributes()
					     ?? ImmutableArray<AttributeData>.Empty))
				{
					yield return y;
				}
			}

			return MineForAttributeData().ToImmutableArray();
		}
	}
}
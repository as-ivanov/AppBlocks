using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	internal class MemberDeclarationByModifierSorter : IComparer<MemberDeclarationSyntax>
	{
		private readonly SyntaxKind _modifierSyntax;

		internal MemberDeclarationByModifierSorter(SyntaxKind modifierSyntax)
		{
			_modifierSyntax = modifierSyntax;
		}

		public int Compare(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
		{
			static bool ContainsToken(MemberDeclarationSyntax list, SyntaxKind kind)
			{
				return list.Modifiers.Any(modifier => modifier.Kind() == kind);
			}

			var xHasModifier = ContainsToken(x, _modifierSyntax);
			var yHasModifier = ContainsToken(y, _modifierSyntax);
			if (xHasModifier == yHasModifier)
			{
				return 0;
			}

			return xHasModifier ? -1 : 1;
		}
	}
}
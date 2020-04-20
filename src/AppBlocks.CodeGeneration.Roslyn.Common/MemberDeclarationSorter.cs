using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public class MemberDeclarationSorter : IComparer<MemberDeclarationSyntax>
	{
		private readonly MemberDeclarationByModifierSorter[] _sorters = {
			new MemberDeclarationByModifierSorter(SyntaxKind.ConstKeyword),
			new MemberDeclarationByModifierSorter(SyntaxKind.StaticKeyword),
			new MemberDeclarationByModifierSorter(SyntaxKind.ReadOnlyKeyword),
		};

		public int Compare(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
		{
			foreach (var sorter in _sorters)
			{
				var result = sorter.Compare(x, y);
				if (result != 0)
				{
					return result;
				}
			}
			return 0;
		}
	}
}

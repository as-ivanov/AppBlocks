using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public class EmptyStatementRemoval : CSharpSyntaxRewriter
	{
		public override SyntaxNode VisitEmptyStatement(EmptyStatementSyntax node)
		{
			//Simply remove all Empty Statements
			return null;
		}
	}
}

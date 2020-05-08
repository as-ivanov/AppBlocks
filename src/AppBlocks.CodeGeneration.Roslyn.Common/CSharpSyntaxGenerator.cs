using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	internal static class CSharpSyntaxGenerator
	{
		public static SyntaxNode MemberAccessExpression(SyntaxNode expression, SyntaxNode memberName)
		{
			return SyntaxFactory.MemberAccessExpression(
				SyntaxKind.SimpleMemberAccessExpression,
				ParenthesizeLeft((ExpressionSyntax) expression),
				(SimpleNameSyntax) memberName);
		}

		internal static ExpressionSyntax ParenthesizeLeft(ExpressionSyntax expression)
		{
			if (expression is TypeSyntax ||
			    expression.IsKind(SyntaxKind.ThisExpression) ||
			    expression.IsKind(SyntaxKind.BaseExpression) ||
			    expression.IsKind(SyntaxKind.ParenthesizedExpression) ||
			    expression.IsKind(SyntaxKind.SimpleMemberAccessExpression) ||
			    expression.IsKind(SyntaxKind.InvocationExpression) ||
			    expression.IsKind(SyntaxKind.ElementAccessExpression) ||
			    expression.IsKind(SyntaxKind.MemberBindingExpression))
			{
				return expression;
			}

			return Parenthesize(expression);
		}

		private static ExpressionSyntax Parenthesize(SyntaxNode expression)
		{
			return ((ExpressionSyntax) expression).Parenthesize();
		}

		public static SyntaxNode DefaultExpression(ITypeSymbol type)
		{
			// If it's just a reference type, then "null" is the default expression for it.  Note:
			// this counts for actual reference type, or a type parameter with a 'class' constraint.
			// Also, if it's a nullable type, then we can use "null".
			if (type.IsReferenceType ||
			    type.IsPointerType() ||
			    type.IsNullable())
			{
				return SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
			}

			switch (type.SpecialType)
			{
				case SpecialType.System_Boolean:
					return SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
				case SpecialType.System_SByte:
				case SpecialType.System_Byte:
				case SpecialType.System_Int16:
				case SpecialType.System_UInt16:
				case SpecialType.System_Int32:
				case SpecialType.System_UInt32:
				case SpecialType.System_Int64:
				case SpecialType.System_UInt64:
				case SpecialType.System_Decimal:
				case SpecialType.System_Single:
				case SpecialType.System_Double:
					return SyntaxFactory.LiteralExpression(
						SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal("0", 0));
			}

			// Default to a "default(<typename>)" expression.
			return SyntaxFactory.DefaultExpression(type.GenerateTypeSyntax());
		}

		public static SyntaxNode BitwiseOrExpression(SyntaxNode left, SyntaxNode right)
		{
			return CreateBinaryExpression(SyntaxKind.BitwiseOrExpression, left, right);
		}

		private static SyntaxNode CreateBinaryExpression(SyntaxKind syntaxKind, SyntaxNode left, SyntaxNode right)
		{
			return SyntaxFactory.BinaryExpression(syntaxKind, Parenthesize(left), Parenthesize(right));
		}
	}
}
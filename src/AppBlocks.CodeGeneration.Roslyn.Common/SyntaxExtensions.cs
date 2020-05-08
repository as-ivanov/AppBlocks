using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public static class SyntaxExtensions
	{
		public static string ToCamelCase(this SyntaxToken syntaxToken)
		{
			var id = (string) syntaxToken.Value;
			return id.ToCamelCase();
		}

		public static string GetClassNameFromInterfaceDeclaration(this TypeDeclarationSyntax typeDeclarationSyntax,
			bool fullName = true)
		{
			return typeDeclarationSyntax.Identifier.WithoutTrivia().Text.GetClassNameFromInterfaceName(fullName);
		}

		public static string GetBaseClassName(this TypeDeclarationSyntax typeDeclarationSyntax)
		{
			var baseList = typeDeclarationSyntax.BaseList?.Types;
			if (baseList.HasValue
			    && baseList.Value.Any())
			{
				var firstOne = baseList.Value.First();
				var fullName = firstOne.Type.ToString();
				return fullName.GetClassNameFromInterfaceName();
			}

			return null;
		}

		public static SyntaxList<T> ToSyntaxList<T>(params T[] nodes)
			where T : SyntaxNode
		{
			return new SyntaxList<T>(nodes);
		}

		public static SeparatedSyntaxList<T> ToSeparatedList<T>(this IEnumerable<T> nodes,
			SyntaxKind separator = SyntaxKind.CommaToken)
			where T : SyntaxNode
		{
			var nodesArray = nodes == null ? new T[0] : nodes.ToArray();
			return SyntaxFactory.SeparatedList(nodesArray,
				Enumerable.Repeat(SyntaxFactory.Token(separator), Math.Max(nodesArray.Length - 1, 0)));
		}

		public static string GetFullTypeName(this TypeDeclarationSyntax source)
		{
			var namespaces = new LinkedList<NamespaceDeclarationSyntax>();
			var types = new LinkedList<TypeDeclarationSyntax>();
			for (var parent = source.Parent; parent is object; parent = parent.Parent)
			{
				if (parent is NamespaceDeclarationSyntax @namespace)
				{
					namespaces.AddFirst(@namespace);
				}
				else if (parent is TypeDeclarationSyntax type)
				{
					types.AddFirst(type);
				}
			}

			var result = new StringBuilder();
			for (var item = namespaces.First; item is object; item = item.Next)
			{
				result.Append(item.Value.Name).Append(CSharpConst.NamespaceClassDelimiter);
			}

			static void AppendName(StringBuilder builder, TypeDeclarationSyntax type)
			{
				builder.Append(type.Identifier.Text);
				var typeArguments = type.TypeParameterList?.ChildNodes()
					.Count(node => node is TypeParameterSyntax) ?? 0;
				if (typeArguments != 0)
					builder.Append(CSharpConst.TypeParameterClassDelimiter).Append(typeArguments);
			}

			for (var item = types.First; item is object; item = item.Next)
			{
				var type = item.Value;
				AppendName(result, type);
				result.Append(CSharpConst.NestedClassDelimiter);
			}

			AppendName(result, source);

			return result.ToString();
		}

		public static MemberDeclarationSyntax[] SortMembers(this IEnumerable<MemberDeclarationSyntax> input)
		{
			return input.OrderBy(_ => _, new MemberDeclarationSorter()).ToArray();
		}

		public static SyntaxList<TypeParameterConstraintClauseSyntax> GetAllowedImplicitImplementationConstraintClause(
			this SyntaxList<TypeParameterConstraintClauseSyntax> typeParameterConstraintClauseSyntaxList)
		{
			var methodConstraintClauses = new List<TypeParameterConstraintClauseSyntax>();
			foreach (var typeConstraintClauses in typeParameterConstraintClauseSyntaxList)
			{
				var classOrStructConstraints = typeConstraintClauses.Constraints.OfType<ClassOrStructConstraintSyntax>();
				var typeClassOrStructConstraintsClauses = typeConstraintClauses.WithConstraints(classOrStructConstraints.ToSeparatedList<TypeParameterConstraintSyntax>());
				methodConstraintClauses.Add(typeClassOrStructConstraintsClauses);
			}

			return methodConstraintClauses.ToSyntaxList();
		}

		public static ExpressionSyntax GetToStringExpression(this SyntaxToken identifier)
		{
			return SyntaxFactory.InvocationExpression(
				SyntaxFactory.MemberAccessExpression(
					SyntaxKind.SimpleMemberAccessExpression,
					SyntaxFactory.IdentifierName(identifier.WithoutTrivia().ToFullString()),
					SyntaxFactory.IdentifierName(nameof(ToString))));
		}
	}
}
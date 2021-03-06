using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	internal static class AttributeGenerator
	{
		public static SyntaxList<AttributeListSyntax> GenerateAttributeLists(ImmutableArray<AttributeData> attributes,
			SyntaxToken? target = null)
		{
			var attributeDeclarations = attributes.OrderBy(a => a.AttributeClass.Name)
				.Select(a => GenerateAttributeDeclaration(a, target)).Where(_ => _ != null).ToList();
			return attributeDeclarations.Count == 0
				? default
				: SyntaxFactory.List(attributeDeclarations);
		}

		private static AttributeListSyntax GenerateAttributeDeclaration(
			AttributeData attribute, SyntaxToken? target)
		{
			var attributeSyntax = GenerateAttribute(attribute);
			return attributeSyntax == null
				? null
				: SyntaxFactory.AttributeList(
					target.HasValue ? SyntaxFactory.AttributeTargetSpecifier(target.Value) : null,
					SyntaxFactory.SingletonSeparatedList(attributeSyntax));
		}

		private static AttributeSyntax GenerateAttribute(AttributeData attribute)
		{
			var attributeArguments = GenerateAttributeArgumentList(attribute);
			return !(attribute.AttributeClass.GenerateTypeSyntax() is NameSyntax nameSyntax)
				? null
				: SyntaxFactory.Attribute(nameSyntax, attributeArguments);
		}

		private static AttributeArgumentListSyntax GenerateAttributeArgumentList(AttributeData attribute)
		{
			if (attribute.ConstructorArguments.Length == 0 && attribute.NamedArguments.Length == 0)
			{
				return null;
			}

			var arguments = new List<AttributeArgumentSyntax>();
			arguments.AddRange(attribute.ConstructorArguments.Select(c =>
				SyntaxFactory.AttributeArgument(ExpressionGenerator.GenerateExpression(c))));

			arguments.AddRange(attribute.NamedArguments.Select(kvp =>
				SyntaxFactory.AttributeArgument(
					SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(kvp.Key)), null,
					ExpressionGenerator.GenerateExpression(kvp.Value))));

			return SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(arguments));
		}
	}
}

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public abstract class InterfaceImplementationGenerator<TImplementationDescriptor> : IRichCodeGenerator
		where TImplementationDescriptor : IInterfaceImplementationDescriptor
	{
		private readonly AttributeData _attributeData;
		private readonly Version _version;

		protected InterfaceImplementationGenerator(AttributeData attributeData, Version version)
		{
			_attributeData = attributeData ?? throw new ArgumentNullException(nameof(attributeData));
			_version = version;
		}

		protected AttributeData Data => _attributeData;

		public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context,
			IProgress<Diagnostic> progress, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<RichGenerationResult> GenerateRichAsync(TransformationContext context, IProgress<Diagnostic> progress,
			CancellationToken cancellationToken)
		{
			if (!(context.ProcessingNode is TypeDeclarationSyntax tds) || !tds.IsKind(SyntaxKind.InterfaceDeclaration))
			{
				throw new Exception("Generator attribute must be declared on interface.");
			}
			return GenerateAsync(tds, context, Data);
		}

		private Task<RichGenerationResult> GenerateAsync(TypeDeclarationSyntax typeDeclaration,
			TransformationContext context, AttributeData attributeData)
		{
			if (!(context.ProcessingNode.Parent is NamespaceDeclarationSyntax namespaceDeclarationSyntax))
			{
				throw new Exception($"Failed to determine namespace for type:'{context.ProcessingNode.Parent}'.");
			}

			var descriptor = GetImplementationDescriptor(typeDeclaration, context, attributeData);

			var implementationMemberDeclaration = GetImplementation(descriptor);

			var @namespace = NamespaceDeclaration(namespaceDeclarationSyntax.Name)
				.AddMembers(implementationMemberDeclaration);

			var generatedMembers = new List<MemberDeclarationSyntax> {@namespace};
			var result = new RichGenerationResult {Members = new SyntaxList<MemberDeclarationSyntax>(generatedMembers)};

			return Task.FromResult(result);
		}

		protected abstract TImplementationDescriptor GetImplementationDescriptor(TypeDeclarationSyntax typeDeclaration,
			TransformationContext context, AttributeData attributeData);

		private MemberDeclarationSyntax GetImplementation(TImplementationDescriptor implementationDescriptor)
		{
			var baseTypes = GetClassBaseList(implementationDescriptor, implementationDescriptor.InheritedInterfaceTypes);

			var classModifiers = new List<SyntaxToken> {Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.SealedKeyword), Token(SyntaxKind.PartialKeyword)};

			var classDeclaration = ClassDeclaration(implementationDescriptor.ClassName)
				.WithModifiers(TokenList(classModifiers))
				.WithTypeParameterList(implementationDescriptor.DeclarationSyntax.TypeParameterList)
				.WithConstraintClauses(implementationDescriptor.DeclarationSyntax.ConstraintClauses)
				.AddAttributeLists(GetAttributeList())
				.AddBaseListTypes(baseTypes.ToArray())
				.AddMembers(GetFields(implementationDescriptor).SortMembers())
				.AddMembers(GetConstructors(implementationDescriptor).SortMembers())
				.AddMembers(GetMethods(implementationDescriptor).SortMembers());

			return classDeclaration;
		}

		protected abstract IEnumerable<MemberDeclarationSyntax> GetFields(TImplementationDescriptor descriptor);

		protected abstract IEnumerable<ConstructorDeclarationSyntax> GetConstructors(TImplementationDescriptor descriptor);

		protected abstract IEnumerable<MemberDeclarationSyntax> GetMethods(TImplementationDescriptor descriptor);

		private static IEnumerable<SimpleBaseTypeSyntax> GetClassBaseList(TImplementationDescriptor descriptor,
			IEnumerable<string> inheritedInterfaceTypes)
		{
			yield return SimpleBaseType(AliasQualifiedName(
				IdentifierName(Token(SyntaxKind.GlobalKeyword)),
				IdentifierName(descriptor.DeclarationSyntax.GetFullTypeName()))
			);
			foreach (var inheritedInterfaceType in inheritedInterfaceTypes)
			{
				yield return SimpleBaseType(AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName(inheritedInterfaceType)));
			}
		}

		private AttributeListSyntax GetAttributeList()
		{
			return AttributeList(SingletonSeparatedList(GetGeneratedCodeAttributeSyntax()));
		}

		private AttributeSyntax GetGeneratedCodeAttributeSyntax()
		{
			return Attribute(typeof(GeneratedCodeAttribute).GetGlobalTypeSyntax())
				.AddArgumentListArguments(AttributeArgument(GetType().Name.GetLiteralExpression()),
					AttributeArgument(_version.ToString().GetLiteralExpression()));
		}
	}
}

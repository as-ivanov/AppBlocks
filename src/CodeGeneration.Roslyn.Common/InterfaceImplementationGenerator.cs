using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeGeneration.Roslyn.Common
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
				throw new Exception($"Generator attribute must be declared on interface.");
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

			var usingDirectives = GetUsingDirectives();

			TImplementationDescriptor descriptor = GetImplementationDescriptor(typeDeclaration, context, attributeData);

			var loggerClass = GetImplementation(descriptor);

			var @namespace = NamespaceDeclaration(namespaceDeclarationSyntax.Name)
				.AddUsings(usingDirectives)
				.AddMembers(loggerClass);

			var generatedMembers = new List<MemberDeclarationSyntax> {@namespace};
			var result = new RichGenerationResult {Members = new SyntaxList<MemberDeclarationSyntax>(generatedMembers)};

			return Task.FromResult(result);
		}

		protected abstract TImplementationDescriptor GetImplementationDescriptor(TypeDeclarationSyntax typeDeclaration,
			TransformationContext context, AttributeData attributeData);

		protected abstract string[] GetNamespaces();

		private UsingDirectiveSyntax[] GetUsingDirectives()
		{
			return List(GetNamespaces().Select(_ => UsingDirective(ParseName(_))).ToArray()).ToArray();
		}

		protected MemberDeclarationSyntax GetImplementation(TImplementationDescriptor implementationDescriptor)
		{
			var baseTypes = GetLoggerBaseList(implementationDescriptor, implementationDescriptor.InheritedInterfaceTypes);
			var classDeclaration = ClassDeclaration(implementationDescriptor.ClassName)
				.WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword)))
				.AddAttributeLists(GetAttributeList())
				.AddBaseListTypes(baseTypes)
				.AddMembers(GetFields(implementationDescriptor))
				.AddMembers(GetConstructors(implementationDescriptor))
				.AddMembers(GetMethods(implementationDescriptor));

			return classDeclaration;
		}

		protected abstract MemberDeclarationSyntax[] GetFields(TImplementationDescriptor loggerDescriptor);

		protected abstract ConstructorDeclarationSyntax[] GetConstructors(TImplementationDescriptor loggerDescriptor);

		protected abstract MemberDeclarationSyntax[] GetMethods(TImplementationDescriptor loggerDescriptor);

		private static BaseTypeSyntax[] GetLoggerBaseList(TImplementationDescriptor loggerDescriptor,
			string[] inheritedInterfaceTypes)
		{
			var baseTypeList = new List<BaseTypeSyntax>();

			if (loggerDescriptor.BaseClassName != null)
			{
				baseTypeList.Add(SimpleBaseType(IdentifierName(loggerDescriptor.BaseClassName)));
			}

			baseTypeList.Add(SimpleBaseType(IdentifierName(loggerDescriptor.DeclarationSyntax.Identifier)));
			if (inheritedInterfaceTypes != null)
			{
				foreach (var inheritedInterfaceType in inheritedInterfaceTypes)
				{
					baseTypeList.Add(SimpleBaseType(IdentifierName(inheritedInterfaceType)));
				}
			}

			return baseTypeList.ToArray();
		}

		private AttributeListSyntax GetAttributeList()
		{
			return AttributeList(SingletonSeparatedList(GetGeneratedCodeAttributeSyntax()));
		}

		private AttributeSyntax GetGeneratedCodeAttributeSyntax()
		{
			return Attribute(ParseName(typeof(GeneratedCodeAttribute).FullName))
				.AddArgumentListArguments(AttributeArgument(GetType().Name.GetLiteralExpression()),
					AttributeArgument(_version.ToString().GetLiteralExpression()));
		}
	}
}
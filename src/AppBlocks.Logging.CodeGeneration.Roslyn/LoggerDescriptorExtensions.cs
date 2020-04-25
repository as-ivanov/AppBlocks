using System;
using System.Collections.Immutable;
using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Common;
using CodeGeneration.Roslyn;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AppBlocks.Logging.CodeGeneration.Roslyn
{
	internal static class LoggerDescriptorExtensions
	{
		public static LoggerDescriptor ToLoggerDescriptor(this TypeDeclarationSyntax typeDeclarationSyntax,
			TransformationContext context, AttributeData attributeData)
		{
			var baseClass = typeDeclarationSyntax.GetBaseClassName();

			var compilation = context.Compilation;

			var inheritedInterfaceTypes = attributeData.GetInheritedInterfaceTypes();

			var className = typeDeclarationSyntax.GetClassNameFromInterfaceDeclaration();

			var exceptionType = compilation.GetTypeByMetadataName(typeof(Exception).FullName);

			return new LoggerDescriptor(
				typeDeclarationSyntax,
				className,
				baseClass,
				inheritedInterfaceTypes,
				typeDeclarationSyntax.GetLoggerMethods(context, exceptionType));
		}

		private static ImmutableArray<LoggerMethod> GetLoggerMethods(this TypeDeclarationSyntax typeDeclarationSyntax,
			TransformationContext context, INamedTypeSymbol exceptionType)
		{
			return typeDeclarationSyntax.Members.OfType<MethodDeclarationSyntax>()
				.Select(p => p.ToLoggerMethod(context, exceptionType))
				.Where(_ => _ != null)
				.ToImmutableArray();
		}

		private static LoggerMethod ToLoggerMethod(this MethodDeclarationSyntax methodDeclarationSyntax,
			TransformationContext context, INamedTypeSymbol exceptionType)
		{
			var attributeData = GetAttributeData(context, methodDeclarationSyntax);
			var loggerMethodStubAttributeData =
				attributeData.FirstOrDefault(_ => _.AttributeClass.Name == nameof(Attributes.LoggerMethodStubAttribute));

			var message = loggerMethodStubAttributeData.GetNamedArgumentValue(nameof(Attributes.LoggerMethodStubAttribute.Message), methodDeclarationSyntax.Identifier.WithoutTrivia().ToFullString().Humanize());
			var level = loggerMethodStubAttributeData.GetNamedArgumentValue(nameof(Attributes.LoggerMethodStubAttribute.Level), LogLevel.Information);

			var parameters = methodDeclarationSyntax.ParameterList.Parameters
				.Select(p => p.ToLoggerMethodParameter(context, exceptionType)).ToImmutableArray();

			return new LoggerMethod(
				methodDeclarationSyntax,
				level,
				message,
				parameters);
		}

		private static LoggerMethodParameter ToLoggerMethodParameter(this ParameterSyntax parameterSyntax,
			TransformationContext context, INamedTypeSymbol exceptionType)
		{
			var typeInfo = context.SemanticModel.GetTypeInfo(parameterSyntax.Type);
			var conversionInfo = context.Compilation.ClassifyCommonConversion(typeInfo.Type, exceptionType);
			return new LoggerMethodParameter(parameterSyntax, typeInfo, conversionInfo.Exists && conversionInfo.IsImplicit);
		}

		private static ImmutableArray<AttributeData> GetAttributeData(TransformationContext context, SyntaxNode syntaxNode)
		{
			switch (syntaxNode)
			{
				case CompilationUnitSyntax syntax:
					return context.Compilation.Assembly.GetAttributes()
						.Where(x => x.ApplicationSyntaxReference.SyntaxTree == syntax.SyntaxTree).ToImmutableArray();
				default:
					return context.SemanticModel.GetDeclaredSymbol(syntaxNode)?.GetAttributes() ??
					       ImmutableArray<AttributeData>.Empty;
			}
		}
	}
}
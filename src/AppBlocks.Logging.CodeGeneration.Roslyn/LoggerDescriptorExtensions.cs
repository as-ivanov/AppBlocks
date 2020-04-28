using System;
using System.Collections.Generic;
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
			var fieldNameCounter = new Dictionary<string, int>(); //Consider that methods may have the same name
			return typeDeclarationSyntax.Members.OfType<MethodDeclarationSyntax>()
				.Select(p => p.ToLoggerMethod(context, fieldNameCounter, exceptionType))
				.ToImmutableArray();
		}

		private static LoggerMethod ToLoggerMethod(this MethodDeclarationSyntax methodDeclarationSyntax,
			TransformationContext context, Dictionary<string, int> fieldNameCounter, INamedTypeSymbol exceptionType)
		{
			var attributeData = GetAttributeData(context, methodDeclarationSyntax);
			var logOptionsAttributeAttributeData =
				attributeData.FirstOrDefault(_ => _.AttributeClass.Name == nameof(Attributes.LogOptionsAttribute));

			var message = logOptionsAttributeAttributeData.GetNamedArgumentValue(nameof(Attributes.LogOptionsAttribute.Message), methodDeclarationSyntax.Identifier.WithoutTrivia().ToFullString().Humanize());
			var level = logOptionsAttributeAttributeData.GetNamedArgumentValue(nameof(Attributes.LogOptionsAttribute.Level), LogLevel.Information);

			var parameters = methodDeclarationSyntax.ParameterList.Parameters
				.Select(p => p.ToLoggerMethodParameter(context, exceptionType)).ToImmutableArray();

			var methodNameCamelCase = methodDeclarationSyntax.Identifier.WithoutTrivia().Text.ToCamelCase();
			string delegateFieldName;
			if (!fieldNameCounter.TryGetValue(methodNameCamelCase, out var currentFiledCounter))
			{
				fieldNameCounter[methodNameCamelCase] = 0;
				delegateFieldName = $"_{methodNameCamelCase}Delegate";
			}
			else
			{
				fieldNameCounter[methodNameCamelCase] = currentFiledCounter + 1;
				delegateFieldName = $"_{methodNameCamelCase}Delegate{currentFiledCounter}";
			}

			return new LoggerMethod(
				methodDeclarationSyntax,
				level,
				message,
				delegateFieldName,
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
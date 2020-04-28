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
			var className = typeDeclarationSyntax.GetClassNameFromInterfaceDeclaration();

			var typeFullName = typeDeclarationSyntax.GetFullTypeName();
			var typeSymbol = context.Compilation.GetTypeByMetadataName(typeFullName);
			if (typeSymbol == null)
			{
				throw new Exception($"{typeFullName} not found in assembly.");
			}

			var inheritedInterfaceTypes = attributeData.GetInheritedInterfaceTypes();
			var inheritedInterfaceSymbols = typeSymbol.GetInheritedInterfaceSymbolsWithMeRecursive();

			var exceptionType = context.Compilation.GetTypeByMetadataName(typeof(Exception).FullName);

			var methods = inheritedInterfaceSymbols.GetLoggerMethods(context, exceptionType);

			return new LoggerDescriptor(
				typeDeclarationSyntax,
				className,
				inheritedInterfaceTypes,
				methods);
		}

		private static ImmutableArray<LoggerMethod> GetLoggerMethods(this IEnumerable<INamedTypeSymbol> inheritedInterfaceSymbols,
			TransformationContext context, INamedTypeSymbol exceptionType)
		{
			var fieldNameCounter = new Dictionary<string, int>(); //Consider that methods may have the same name
			return inheritedInterfaceSymbols.GetAllMethodDeclarations().Select(entry => entry.MethodDeclaration.ToLoggerMethod(context, entry.TypeDeclaration, fieldNameCounter, exceptionType)).ToImmutableArray();;
		}

		private static LoggerMethod ToLoggerMethod(this MethodDeclarationSyntax methodDeclarationSyntax,
			TransformationContext context, TypeDeclarationSyntax typeDeclaration, Dictionary<string, int> fieldNameCounter, INamedTypeSymbol exceptionType)
		{
			var methodSemanticModel = context.Compilation.GetSemanticModel(methodDeclarationSyntax.SyntaxTree);
			var attributeData = methodSemanticModel.GetDeclaredSymbol(methodDeclarationSyntax).GetAttributes();
			var logOptionsAttributeAttributeData =
				attributeData.FirstOrDefault(_ => _.AttributeClass.Name == nameof(Attributes.LogOptionsAttribute));

			var message = logOptionsAttributeAttributeData.GetNamedArgumentValue(
				nameof(Attributes.LogOptionsAttribute.Message),
				methodDeclarationSyntax.Identifier.WithoutTrivia().ToFullString().Humanize());
			var level = logOptionsAttributeAttributeData.GetNamedArgumentValue(nameof(Attributes.LogOptionsAttribute.Level),
				LogLevel.Information);

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
				typeDeclaration,
				level,
				message,
				delegateFieldName,
				parameters);
		}

		private static LoggerMethodParameter ToLoggerMethodParameter(this ParameterSyntax parameterSyntax,
			TransformationContext context, INamedTypeSymbol exceptionType)
		{
			var semanticModel = context.Compilation.GetSemanticModel(parameterSyntax.SyntaxTree);
			var typeInfo = semanticModel.GetTypeInfo(parameterSyntax.Type);
			var conversionInfo = context.Compilation.ClassifyCommonConversion(typeInfo.Type, exceptionType);
			return new LoggerMethodParameter(parameterSyntax, typeInfo, conversionInfo.Exists && conversionInfo.IsImplicit);
		}
	}
}
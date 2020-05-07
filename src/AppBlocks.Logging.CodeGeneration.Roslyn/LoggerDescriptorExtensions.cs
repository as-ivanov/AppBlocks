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

			var methodDeclarations = typeDeclarationSyntax.GetAllMethodDeclarations(context);

			var exceptionType = context.Compilation.GetTypeByMetadataName(typeof(Exception).FullName);
			var methods = methodDeclarations.GetLoggerMethods(context, exceptionType);

			var inheritedInterfaceTypes = attributeData.GetInheritedInterfaceTypes();

			return new LoggerDescriptor(
				typeDeclarationSyntax,
				className,
				inheritedInterfaceTypes,
				methods);
		}

		private static ImmutableArray<LoggerMethod> GetLoggerMethods(this IEnumerable<(MethodDeclarationSyntax MethodDeclaration, TypeDeclarationSyntax DeclaredInterface, INamedTypeSymbol DeclaredInterfaceSymbol)> methodDeclarations,
			TransformationContext context, INamedTypeSymbol exceptionType)
		{
			var fieldNameCounter = new Dictionary<string, int>(); //Consider that methods may have the same name
			return methodDeclarations.Select(entry => entry.MethodDeclaration.ToLoggerMethod(context, entry.DeclaredInterfaceSymbol, fieldNameCounter, exceptionType)).ToImmutableArray();;
		}

		private static LoggerMethod ToLoggerMethod(this MethodDeclarationSyntax methodDeclarationSyntax,
			TransformationContext context, INamedTypeSymbol declaredInterfaceSymbol, Dictionary<string, int> fieldNameCounter, INamedTypeSymbol exceptionType)
		{
			var methodSymbol = declaredInterfaceSymbol.GetMembers().OfType<IMethodSymbol>()
				.FirstOrDefault(_ => _.Name == methodDeclarationSyntax.Identifier.WithoutTrivia().ToFullString());
			var attributeData = methodSymbol.GetAttributes();
			var logOptionsAttributeAttributeData =
				attributeData.FirstOrDefault(_ => _.AttributeClass.Name == nameof(Attributes.LogOptionsAttribute));

			var message = logOptionsAttributeAttributeData.GetNamedArgumentValue(
				nameof(Attributes.LogOptionsAttribute.Message),
				methodDeclarationSyntax.Identifier.WithoutTrivia().ToFullString().Humanize());
			var level = logOptionsAttributeAttributeData.GetNamedArgumentValue(nameof(Attributes.LogOptionsAttribute.Level),
				LogLevel.Information);

			var parameters = methodDeclarationSyntax.ParameterList.Parameters
				.Select(p => p.ToLoggerMethodParameter(context, methodSymbol, exceptionType)).ToImmutableArray();

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
				declaredInterfaceSymbol,
				level,
				message,
				delegateFieldName,
				parameters);
		}

		private static LoggerMethodParameter ToLoggerMethodParameter(this ParameterSyntax parameterSyntax,
			TransformationContext context, IMethodSymbol methodSymbol, ITypeSymbol exceptionType)
		{
			var parameterSymbol = methodSymbol.Parameters.FirstOrDefault(_ => _.Name == parameterSyntax.Identifier.WithoutTrivia().ToFullString());
			var conversionInfo = context.Compilation.ClassifyCommonConversion(parameterSymbol.Type, exceptionType);
			return new LoggerMethodParameter(parameterSyntax, parameterSymbol, conversionInfo.Exists && conversionInfo.IsImplicit);
		}
	}
}
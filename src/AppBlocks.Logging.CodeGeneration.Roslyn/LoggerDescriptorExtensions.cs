using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Common;
using AppBlocks.Logging.CodeGeneration.Attributes;
using CodeGeneration.Roslyn;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
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

			var innerLoggerTypeSymbol = context.Compilation.GetTypeByMetadataName(typeof(ILogger).FullName);

			var filtered = methodDeclarations.Where(_ =>
			{
				var conversion = context.Compilation.ClassifyCommonConversion(_.DeclaredInterfaceSymbol, innerLoggerTypeSymbol);
				return conversion.Exists && conversion.IsImplicit;
			});

			//var exposeInnerLogger = filtered.Count() < methodDeclarations.Count();

			var exceptionTypeSymbol = context.Compilation.GetTypeByMetadataName(typeof(Exception).FullName);
			var objectTypeSymbol = context.Compilation.GetTypeByMetadataName(typeof(object).FullName);
			var methods = methodDeclarations.GetLoggerMethods(context, innerLoggerTypeSymbol, exceptionTypeSymbol);

			var inheritedInterfaceTypes = attributeData.GetInheritedInterfaceTypes();

			return new LoggerDescriptor(
				typeDeclarationSyntax,
				className,
				inheritedInterfaceTypes,
				methods,
				objectTypeSymbol);
		}

		private static ImmutableArray<LoggerMethod> GetLoggerMethods(this IEnumerable<(MethodDeclarationSyntax MethodDeclaration, IMethodSymbol methodSymbol, TypeDeclarationSyntax DeclaredInterface, INamedTypeSymbol DeclaredInterfaceSymbol)> methodDeclarations,
			TransformationContext context, INamedTypeSymbol innerLoggerTypeSymbol, INamedTypeSymbol exceptionType)
		{
			var fieldNameCounter = new Dictionary<string, int>(); //Consider that methods may have the same name
			return methodDeclarations
				.Select((entry) =>
					entry.MethodDeclaration.ToLoggerMethod(entry.methodSymbol, context, entry.DeclaredInterfaceSymbol,
						fieldNameCounter,
						innerLoggerTypeSymbol,
						exceptionType))
				.ToImmutableArray();
			;
		}

		private static LoggerMethod ToLoggerMethod(this MethodDeclarationSyntax methodDeclarationSyntax,
			IMethodSymbol methodSymbol,
			TransformationContext context,
			INamedTypeSymbol declaredInterfaceSymbol,
			Dictionary<string, int> fieldNameCounter,
			INamedTypeSymbol innerLoggerTypeSymbol,
			INamedTypeSymbol exceptionType)
		{

			var innerLoggerMethod = declaredInterfaceSymbol.Equals(innerLoggerTypeSymbol);
			if (innerLoggerMethod)
			{
				return new LoggerMethod(methodDeclarationSyntax, declaredInterfaceSymbol);
			}

			var attributeData = methodSymbol.GetAttributes();
			var logOptionsAttributeAttributeData =
				attributeData.FirstOrDefault(_ => _.AttributeClass.Name == nameof(LogOptionsAttribute));

			var message = logOptionsAttributeAttributeData.GetNamedArgumentValueOrDefault(
				nameof(LogOptionsAttribute.Message),
				methodDeclarationSyntax.Identifier.WithoutTrivia().ToFullString().Humanize());
			var level = logOptionsAttributeAttributeData.GetNamedArgumentValueOrDefault(nameof(LogOptionsAttribute.Level),
				LogLevel.Information);

			var parameters = methodDeclarationSyntax.ParameterList.Parameters
				.Select(p => p.ToLoggerMethodParameter(context, methodSymbol, exceptionType, level)).ToImmutableArray();

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

			var subLevels = parameters.Where(_ => _.MinLogLevel.HasValue).Select(_ => _.MinLogLevel.Value).Distinct().ToArray();

			return new LoggerMethod(
				methodDeclarationSyntax,
				declaredInterfaceSymbol,
				level,
				message,
				delegateFieldName,
				parameters,
				subLevels);
		}

		private static LoggerMethodParameter ToLoggerMethodParameter(this ParameterSyntax parameterSyntax,
			TransformationContext context, IMethodSymbol methodSymbol, ITypeSymbol exceptionType, LogLevel methodLogLevel)
		{
			try
			{
				var parameterSymbol =
					methodSymbol.Parameters.FirstOrDefault(_ =>
						_.Name == parameterSyntax.Identifier.WithoutTrivia().ToFullString().TrimStart('@'));
				var conversionInfo = context.Compilation.ClassifyCommonConversion(parameterSymbol.Type, exceptionType);

				var attributeData = parameterSymbol.GetAttributes();
				var logConditionAttributeData =
					attributeData.FirstOrDefault(_ => _.AttributeClass.Name == nameof(LogConditionAttribute));
				LogLevel? minLogLevel = null;
				if (logConditionAttributeData != null)
				{
					minLogLevel = logConditionAttributeData.GetConstructorArgumentValue<LogLevel>();
					if (minLogLevel >= methodLogLevel) // no sense to check if LogLevel.Critical enabled if we are inside LogLevel.Information enabled condition
					{
						minLogLevel = null;
					}
				}

				return new LoggerMethodParameter(parameterSyntax, parameterSymbol, conversionInfo.Exists && conversionInfo.IsImplicit, minLogLevel);
			}
			catch (Exception e)
			{
				//Debugger.Launch();
				throw;
			}
		}
	}
}

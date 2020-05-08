using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Common;
using AppBlocks.Monitoring.CodeGeneration.Attributes;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn
{
	internal static class MetricsCollectorDescriptorExtensions
	{
		public static MetricsCollectorDescriptor ToMetricsCollectorDescriptor(
			this TypeDeclarationSyntax typeDeclarationSyntax, TransformationContext context, AttributeData attributeData)
		{
			var className = typeDeclarationSyntax.GetClassNameFromInterfaceDeclaration(false);

			var methodDeclarations = typeDeclarationSyntax.GetAllMethodDeclarations(context);

			var inheritedInterfaceTypes = attributeData.GetInheritedInterfaceTypes();

			var metricsCollectorMethods = methodDeclarations.GetMetricsCollectorMethods(context);

			var contextName =
				attributeData.GetNamedArgumentValueOrDefault(nameof(GenerateMetricsCollectorAttribute.ContextName), className);
			return new MetricsCollectorDescriptor(typeDeclarationSyntax, contextName, className,
				inheritedInterfaceTypes, metricsCollectorMethods);
		}

		private static ImmutableArray<MetricsCollectorMethod> GetMetricsCollectorMethods(
			this IEnumerable<(MethodDeclarationSyntax MethodDeclaration, TypeDeclarationSyntax DeclaredInterface, INamedTypeSymbol DeclaredInterfaceSymbol)> methodDeclarations,
			TransformationContext context)
		{
			var fieldNameCounter = new Dictionary<string, int>(); //Consider that methods may have the same name
			return methodDeclarations.Select(entry => entry.MethodDeclaration.ToMetricsCollectorMethod(context, entry.DeclaredInterfaceSymbol, fieldNameCounter)).ToImmutableArray();
			;
		}

		private static MetricsCollectorMethod ToMetricsCollectorMethod(this MethodDeclarationSyntax methodDeclaration,
			TransformationContext context, INamedTypeSymbol declaredInterfaceSymbol, Dictionary<string, int> fieldNameCounter)
		{
			var metricsCollectorType = GetMetricsCollectorType(methodDeclaration.ReturnType);
			var (metricName, unitName) = GetMetricOptions(declaredInterfaceSymbol, methodDeclaration, context);

			var methodNameCamelCase = methodDeclaration.Identifier.WithoutTrivia().Text.ToCamelCase();
			string methodKeysFieldName;
			if (!fieldNameCounter.TryGetValue(methodNameCamelCase, out var currentFiledCounter))
			{
				fieldNameCounter[methodNameCamelCase] = 0;
				methodKeysFieldName = $"_{methodNameCamelCase}Keys";
			}
			else
			{
				fieldNameCounter[methodNameCamelCase] = currentFiledCounter + 1;
				methodKeysFieldName = $"_{methodNameCamelCase}Keys{currentFiledCounter}";
			}

			return new MetricsCollectorMethod(methodDeclaration, declaredInterfaceSymbol, metricName, methodKeysFieldName, unitName,
				metricsCollectorType);
		}

		private static (string MetricName, string UnitName) GetMetricOptions(INamedTypeSymbol declaredInterfaceSymbol, MethodDeclarationSyntax methodDeclaration,
			TransformationContext context)
		{
			var methodSymbol = declaredInterfaceSymbol.GetMembers()
				.FirstOrDefault(_ => _.Name == methodDeclaration.Identifier.WithoutTrivia().ToFullString());
			var attributeData = methodSymbol.GetAttributes()
				.FirstOrDefault(_ => _.AttributeClass.Name == nameof(MetricOptionsAttribute));

			var metricName = attributeData.GetNamedArgumentValueOrDefault(nameof(MetricOptionsAttribute.MetricName),
				methodDeclaration.Identifier.WithoutTrivia().Text);

			var unitName = attributeData.GetNamedArgumentValueOrDefault(nameof(MetricOptionsAttribute.MeasurementUnitName));
			return (metricName, unitName);
		}

		private static MetricsCollectorIndicatorType GetMetricsCollectorType(TypeSyntax returnType)
		{
			var returnTypeText = returnType.WithoutTrivia().GetText().ToString();
			returnTypeText = returnTypeText.GetTypeNameWithoutNamespaces();
			if (!Enum.TryParse<MetricsCollectorIndicatorType>(returnTypeText.TrimStart('I'), out var type))
			{
				throw new Exception($"Illegal return type:'{returnTypeText}'");
			}

			return type;
		}
	}
}
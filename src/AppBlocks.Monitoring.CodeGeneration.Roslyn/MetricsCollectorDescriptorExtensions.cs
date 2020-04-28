using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Common;
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

			var typeFullName = typeDeclarationSyntax.GetFullTypeName();
			var typeSymbol = context.Compilation.Assembly.GetTypeByMetadataName(typeFullName);
			if (typeSymbol == null)
			{
				throw new Exception($"{typeFullName} not found in assembly.");
			}
			var inheritedInterfaceTypes = attributeData.GetInheritedInterfaceTypes();
			var inheritedInterfaceSymbols = typeSymbol.GetInheritedInterfaceSymbolsWithMeRecursive();
			inheritedInterfaceTypes = inheritedInterfaceSymbols.Select(_ => _.OriginalDefinition.ToDisplayString()).Union(inheritedInterfaceTypes).ToArray();

			var metricsCollectorMethods = inheritedInterfaceSymbols.GetMetricsCollectorMethods(context);

			var contextName =
				attributeData.GetNamedArgumentValue(nameof(Attributes.MetricsCollectorStubAttribute.ContextName), className);
			return new MetricsCollectorDescriptor(typeDeclarationSyntax, contextName, className,
				inheritedInterfaceTypes, metricsCollectorMethods);
		}

		private static ImmutableArray<MetricsCollectorMethod> GetMetricsCollectorMethods(
			this IEnumerable<INamedTypeSymbol> inheritedInterfaceSymbols,
			TransformationContext context)
		{
			var fieldNameCounter = new Dictionary<string, int>(); //Consider that methods may have the same name
			return inheritedInterfaceSymbols.GetAllMethodDeclarations().Select(entry => entry.MethodDeclaration.ToMetricsCollectorMethod(context, entry.TypeDeclaration, fieldNameCounter)).ToImmutableArray();;
		}

		private static MetricsCollectorMethod ToMetricsCollectorMethod(this MethodDeclarationSyntax methodDeclaration, TransformationContext context, TypeDeclarationSyntax typeDeclaration, Dictionary<string, int> fieldNameCounter)
		{
			var metricsCollectorType = GetMetricsCollectorType(methodDeclaration.ReturnType);
			var (metricName, unitName) = GetMetricOptions(methodDeclaration, context);

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

			return new MetricsCollectorMethod(methodDeclaration, typeDeclaration, metricName, methodKeysFieldName, unitName,
				metricsCollectorType);
		}

		private static (string MetricName, string UnitName) GetMetricOptions(MethodDeclarationSyntax methodDeclaration,
			TransformationContext context)
		{
			var methodSemanticModel = context.Compilation.GetSemanticModel(methodDeclaration.SyntaxTree);
			var attributeData = methodSemanticModel.GetDeclaredSymbol(methodDeclaration).GetAttributes()
				.FirstOrDefault(_ => _.AttributeClass.Name == nameof(Attributes.MetricOptionsAttribute));

			var metricName = attributeData.GetNamedArgumentValue(nameof(Attributes.MetricOptionsAttribute.MetricName),
				methodDeclaration.Identifier.WithoutTrivia().Text);

			var unitName = attributeData.GetNamedArgumentValue(nameof(Attributes.MetricOptionsAttribute.MeasurementUnitName));
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
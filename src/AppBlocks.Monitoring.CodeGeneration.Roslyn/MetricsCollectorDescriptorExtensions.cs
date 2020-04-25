using System;
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
			var inheritedInterfaceTypes = attributeData.GetInheritedInterfaceTypes();
			var className = typeDeclarationSyntax.GetClassNameFromInterfaceDeclaration(false);
			var baseClassName = typeDeclarationSyntax.GetBaseClassName();
			if (attributeData.AttributeClass.Name == nameof(Attributes.AbstractMetricsCollectorStubAttribute))
			{
				return new MetricsCollectorDescriptor(typeDeclarationSyntax, true, null, className, baseClassName,
					inheritedInterfaceTypes, typeDeclarationSyntax.GetLoggerMethods(context));
			}

			var contextName = attributeData.GetNamedArgumentValue(nameof(Attributes.MetricsCollectorStubAttribute.ContextName), className);
			return new MetricsCollectorDescriptor(typeDeclarationSyntax, false, contextName, className, baseClassName,
				inheritedInterfaceTypes, typeDeclarationSyntax.GetLoggerMethods(context));
		}

		private static ImmutableArray<MetricsCollectorMethod> GetLoggerMethods(this TypeDeclarationSyntax typeDeclaration,
			TransformationContext context)
		{
			return typeDeclaration.Members.OfType<MethodDeclarationSyntax>()
				.Select(p => p.ToMetricsCollectorMethod(context))
				.Where(_ => _ != null)
				.ToImmutableArray();
		}

		private static MetricsCollectorMethod ToMetricsCollectorMethod(this MethodDeclarationSyntax methodDeclaration,
			TransformationContext context)
		{
			var metricsCollectorType = GetMetricsCollectorType(methodDeclaration.ReturnType);
			var (metricName, unitName) = GetMetricOptions(methodDeclaration, context);
			return new MetricsCollectorMethod(methodDeclaration, metricName, unitName, metricsCollectorType);
		}

		private static (string MetricName, string UnitName) GetMetricOptions(MethodDeclarationSyntax methodDeclaration,
			TransformationContext context)
		{
			var methodSemanticModel = context.Compilation.GetSemanticModel(methodDeclaration.SyntaxTree);
			var attributeData = methodSemanticModel.GetDeclaredSymbol(methodDeclaration).GetAttributes()
				.FirstOrDefault(_ => _.AttributeClass.Name == nameof(Attributes.MetricsCollectorMethodStubAttribute));

			var metricName = attributeData.GetNamedArgumentValue(nameof(Attributes.MetricsCollectorMethodStubAttribute.MetricName),
				methodDeclaration.Identifier.WithoutTrivia().Text);

			var unitName = attributeData.GetNamedArgumentValue(nameof(Attributes.MetricsCollectorMethodStubAttribute.MeasurementUnitName));
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
using System;
using System.Collections.Immutable;
using System.Linq;
using CodeGeneration.Roslyn.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneration.Roslyn.MetricsCollector
{
	internal static class MetricsCollectorDescriptorExtensions
	{
		public static MetricsCollectorDescriptor ToMetricsCollectorDescriptor(
			this TypeDeclarationSyntax typeDeclarationSyntax, TransformationContext context, AttributeData attributeData)
		{
			if (attributeData.ConstructorArguments.Length != 2)
			{
				throw new Exception($"Expected 2 parameters in AttributeData");
			}
			var contextNameParameter = attributeData.ConstructorArguments.FirstOrDefault();
			var contextName = contextNameParameter.Value as string;
			var inheritedInterfaceTypes = attributeData.GetInheritedInterfaceTypes();
			var className = typeDeclarationSyntax.GetClassName();
			var baseClassName = typeDeclarationSyntax.GetBaseClassName();
			return new MetricsCollectorDescriptor(typeDeclarationSyntax, contextName, className, baseClassName,
				inheritedInterfaceTypes, typeDeclarationSyntax.GetLoggerMethods(context));
		}

		private static ImmutableArray<MetricsCollectorMethod> GetLoggerMethods(this TypeDeclarationSyntax typeDeclaration, TransformationContext context)
		{
			return typeDeclaration.Members.OfType<MethodDeclarationSyntax>()
				.Select(p => p.ToMetricsCollectorMethod(context))
				.Where(_ => _ != null)
				.ToImmutableArray();
		}

		private static MetricsCollectorMethod ToMetricsCollectorMethod(this MethodDeclarationSyntax methodDeclaration, TransformationContext context)
		{
			var metricsCollectorType = GetMetricsCollectorType(methodDeclaration.ReturnType);
			var (metricName, unitName) = GetMetricOptions(methodDeclaration, context);
			return new MetricsCollectorMethod(methodDeclaration, metricName, unitName, metricsCollectorType);
		}

		private static (string MetricName, string UnitName) GetMetricOptions(MethodDeclarationSyntax methodDeclaration, TransformationContext context)
		{
			var methodSemanticModel = context.Compilation.GetSemanticModel(methodDeclaration.SyntaxTree);
			var attributeData = methodSemanticModel.GetDeclaredSymbol(methodDeclaration).GetAttributes().FirstOrDefault(_=>_.AttributeClass.Name == nameof(Attributes.MetricsCollectorMethodStubAttribute));
			if (attributeData == null)
			{
				return (null, null);
			}
			return (attributeData.ConstructorArguments[0].Value as string, attributeData.ConstructorArguments[1].Value as string);
		}

		// public static string GetMetricsCounterName(MethodDeclarationSyntax methodDeclaration)
		// {
		// 	return $"{metricsContextName.ToLower()}__{methodDeclaration.Identifier.WithoutTrivia().Text.ToLower()}";
		// }

		private static MetricsCollectorType GetMetricsCollectorType(TypeSyntax returnType)
		{
			if (!Enum.TryParse<MetricsCollectorType>(returnType.WithoutTrivia().GetText().ToString().TrimStart('I'), out var type))
			{
				throw new Exception($"Illegal return type:'{returnType.GetText()}'");
			}

			return type;
		}
	}
}
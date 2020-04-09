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
			var contextName = attributeData.ConstructorArguments.First().Value as string;
			var inheritedInterfaceTypes = attributeData.GetInheritedInterfaceTypes();
			var className = typeDeclarationSyntax.GetClassName();
			var baseClassName = typeDeclarationSyntax.GetBaseClassName();
			return new MetricsCollectorDescriptor(typeDeclarationSyntax, contextName, className, baseClassName,
				inheritedInterfaceTypes, typeDeclarationSyntax.GetLoggerMethods());
		}

		private static ImmutableArray<MetricsCollectorMethod> GetLoggerMethods(this TypeDeclarationSyntax typeDeclaration)
		{
			return typeDeclaration.Members.OfType<MethodDeclarationSyntax>()
				.Select(p => p.ToMetricsCollectorMethod())
				.Where(_ => _ != null)
				.ToImmutableArray();
		}

		private static MetricsCollectorMethod ToMetricsCollectorMethod(this MethodDeclarationSyntax methodDeclaration)
		{
			var metricsCollectorType = GetMetricsCollectorType(methodDeclaration.ReturnType);
			return new MetricsCollectorMethod(
				methodDeclaration,
				metricsCollectorType);
		}

		private static MetricsCollectorType GetMetricsCollectorType(TypeSyntax returnType)
		{
			if (!Enum.TryParse<MetricsCollectorType>(returnType.GetText().ToString().TrimStart('I'), out var type))
			{
				throw new Exception($"Illegal return type:'{returnType.GetText()}'");
			}

			return type;
		}
	}
}
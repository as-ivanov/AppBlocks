using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn.Attributes.Common;
using CodeGeneration.Roslyn.Tests.Common;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using MetricsCollector.Abstractions;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using Xunit;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class MetricsCollectorClassGeneratorIntegrationTests
	{
		public static IEnumerable<object[]> Generate()
		{
			var options = new MetricsCollectorStubGeneratorOptions();
			var compilationUnitDataBuilder = new CompilationUnitDataBuilder(options);
			var combinations = compilationUnitDataBuilder.Build();
			return combinations.Select(_ => new object[] { _ });
		}

		[Theory]
		[MemberData(nameof(Generate))]
		public Task PositiveReportingMetricsEnabledTest(ITestGenerationContext generationContext)
		{
			return MetricsCollectorMethodGenerationTest(generationContext, true);
		}

		[Theory]
		[MemberData(nameof(Generate))]
		public Task NegativeReportingMetricsDisabledTest(ITestGenerationContext generationContext)
		{
			return MetricsCollectorMethodGenerationTest(generationContext, false);
		}

		private static async Task MetricsCollectorMethodGenerationTest(ITestGenerationContext generationContext, bool metricEnabled)
		{
			//var interfaces = generationContext.Entries.SelectMany(_ => _.Namespaces).SelectMany(_ => _.Members).OfType<InterfaceData>();

			//var inheritedInterfaces = interfaces.SelectMany(_ => _.InheritedInterfaces);
			//var interfacesFromAttributes = interfaces.SelectMany(_ => _.AttributeDataList).OfType<MetricsCollectorInterfaceAttributeData>().SelectMany(_ => _.InheritedInterfaces);

			//var extraEntries = inheritedInterfaces.Concat(interfacesFromAttributes).Select(_ => new NamespaceData(_.Namespace, _)).Select(_ => new CompilationEntryData(Array.Empty<string>(), _));

			//var entries = generationContext.Entries.Concat(extraEntries);

			var syntaxTrees = generationContext.Entries.Select(entry => CSharpSyntaxTree.ParseText(entry.ToString())).ToArray();

			var extraTypes = new[]
			{
				typeof(GeneratedCodeAttribute),
				typeof(Attributes.MetricsCollectorStubAttribute),
				typeof(ImplementInterfaceAttribute),
				typeof(IMetricsProvider)
			};

			var assembly =
				await syntaxTrees.ProcessTransformationAndCompile(extraTypes, CancellationToken.None);

			var metricsCollectorInterfaceMembers = generationContext.Entries.SelectMany(_ => _.Namespaces).SelectMany(_ => _.Members).Where(_ => _.IsSut);

			foreach (var metricsCollectorInterface in metricsCollectorInterfaceMembers)
			{
				var metricsCollectorInterfaceType =
					assembly.GetType(metricsCollectorInterface.Namespace + "." + metricsCollectorInterface.Name, true);
				var metricsCollectorType = assembly.GetTypes()
					.SingleOrDefault(_ => metricsCollectorInterfaceType.IsAssignableFrom(_) && !_.IsAbstract);
				if (metricsCollectorType == null)
				{
					throw new Exception(
						$"MetricsCollector implementation for '{metricsCollectorInterface}' not found in emitted assembly");
				}
			}


			// var parameters = methodParameters.Select(p => p.Value).ToArray();
			//
			// var metricsProvider = GetMetricsProvider();
			//
			// var metricsCollector = Activator.CreateInstance(metricsCollectorType, metricsProvider.Object);
			// var metricsCollectorMethod = metricsCollectorType.GetTypeInfo().GetDeclaredMethod(methodName);
			// if (metricsCollectorMethod == null)
			// {
			// 	throw new Exception($"Metrics collector method not found in emitted assembly");
			// }
			//
			// metricsCollectorMethod.Invoke(metricsCollector, parameters);
			//
			//
			// metricsProvider.Verify();
		}

		private Mock<IMetricsProvider> GetMetricsProvider(string contextName,
			string indicatorName, string measurementUnit, Tags tags,
			MetricsCollectorIndicatorType metricsCollectorIndicatorType)
		{
			var metricsProvider = new Mock<IMetricsProvider>(MockBehavior.Strict);
			Action verify;
			switch (metricsCollectorIndicatorType)
			{
				case MetricsCollectorIndicatorType.Counter:
					var counter = new Mock<ICounter>();
					metricsProvider.Setup(_ => _.CreateCounter(contextName, indicatorName, measurementUnit, tags))
						.Returns(counter.Object).Verifiable();
					break;
				case MetricsCollectorIndicatorType.Gauge:
					break;
				case MetricsCollectorIndicatorType.HitPercentageGauge:
					break;
				case MetricsCollectorIndicatorType.Timer:
					break;
				case MetricsCollectorIndicatorType.Meter:
					break;
				case MetricsCollectorIndicatorType.Histogram:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(metricsCollectorIndicatorType), metricsCollectorIndicatorType,
						null);
			}

			return metricsProvider;
		}
	}
}
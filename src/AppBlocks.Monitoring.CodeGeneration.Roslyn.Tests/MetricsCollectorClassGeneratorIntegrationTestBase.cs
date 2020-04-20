using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AppBlocks.CodeGeneration.Attributes.Common;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using AppBlocks.Monitoring.Abstractions;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using Xunit.Abstractions;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn.Tests
{
	public class MetricsCollectorClassGeneratorIntegrationTestBase
	{
		private readonly ITestOutputHelper _output;

		protected MetricsCollectorClassGeneratorIntegrationTestBase(ITestOutputHelper output)
		{
			_output = output;
		}

		public static IEnumerable<object[]> Generate()
		{
			var options = new MetricsCollectorInterfaceGeneratorOptions();
			var compilationUnitDataBuilder = new CompilationUnitDataBuilder(options);
			var combinations = compilationUnitDataBuilder.Build();
			return combinations.Select(_ => new object[] {_});
		}

		protected async Task MetricsCollectorMethodGenerationTest(ITestContext context, bool metricEnabled)
		{
			var syntaxTrees = context.CompilationEntries.Select(entry => CSharpSyntaxTree.ParseText(entry.ToString()))
				.ToArray();

			var extraTypes = new[]
			{
				typeof(GeneratedCodeAttribute),
				typeof(global::AppBlocks.Monitoring.CodeGeneration.Attributes.MetricsCollectorStubAttribute),
				typeof(ImplementInterfaceAttribute),
				typeof(IMetricsProvider)
			};

			Assembly assembly = null;
			try
			{
				assembly = await syntaxTrees.ProcessTransformationAndCompile(extraTypes, _output, CancellationToken.None);
			}
			catch (Exception)
			{
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
				else
				{
					throw;
				}
			}

			var sutMembers = context.CompilationEntries.SelectMany(_ => _.Namespaces).SelectMany(_ => _.Members)
				.Where(_ => _.IsSut).OfType<InterfaceData>();

			foreach (var sutMember in sutMembers)
			{
				var metricsCollectorType =
					assembly.GetType(sutMember.Namespace + "." + sutMember.Name.TrimStart('I'), true);
				if (metricsCollectorType == null)
				{
					throw new Exception(
						$"MetricsCollector implementation for '{sutMember}' not found in emitted assembly");
				}

				BuildAndVerify(sutMember, metricsCollectorType, metricEnabled);
			}
		}

		private void BuildAndVerify(InterfaceData metricsCollectorTypeData, Type metricsCollectorType, bool metricEnabled)
		{
			var (metricsProvider, metricsPolicy) = GetMetricsProvider(metricsCollectorTypeData, metricEnabled);

			var metricsCollector = Activator.CreateInstance(metricsCollectorType, metricsProvider.Object, metricsPolicy.Object);
			foreach (var interfaceMethodData in metricsCollectorTypeData.Methods)
			{
				var metricsCollectorMethod = metricsCollectorType.GetTypeInfo().GetDeclaredMethod(interfaceMethodData.Name);
				if (metricsCollectorMethod == null)
				{
					throw new Exception($"Metrics collector method not found in emitted assembly");
				}

				var parameters = interfaceMethodData.Parameters.Select(p => p.Value).ToArray();
				_output.WriteLine(
					$"Invoke method:'{interfaceMethodData}' with parameters:{string.Join('|', interfaceMethodData.Parameters.Select(_ => $"{_.Name}:{_.Value}"))}");
				metricsCollectorMethod.Invoke(metricsCollector, parameters);
			}

			try
			{
				metricsProvider.Verify();
				metricsPolicy.Verify();
			}
			catch (Exception)
			{
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
				else
				{
					throw;
				}
			}
		}

		private (Mock<IMetricsProvider>, Mock<IMetricsPolicy>) GetMetricsProvider(InterfaceData metricsCollectorTypeData, bool metricEnabled)
		{
			var metricsProviderMock = new Mock<IMetricsProvider>(MockBehavior.Strict);
			var metricsPolicyMock = new Mock<IMetricsPolicy>(MockBehavior.Strict);
			var metricsCollectorInterfaceAttributeData = metricsCollectorTypeData.AttributeDataList
				.OfType<MetricsCollectorInterfaceAttributeData>().FirstOrDefault();
			if (metricsCollectorInterfaceAttributeData == null)
			{
				throw new Exception($"{typeof(MetricsCollectorInterfaceAttributeData)} not found");
			}

			var contextName = metricsCollectorInterfaceAttributeData.ContextName;
			foreach (var metricsCollectorMethodData in metricsCollectorTypeData.Methods)
			{
				SetupMetricsProviderMember(metricsProviderMock, metricsPolicyMock, contextName, metricsCollectorMethodData, metricEnabled);
			}

			return (metricsProviderMock, metricsPolicyMock);
		}


		private void SetupMetricsProviderMember(Mock<IMetricsProvider> metricsProvider, Mock<IMetricsPolicy> metricsPolicy, string contextName,
			InterfaceMethodData interfaceMethodData, bool metricEnabled)
		{
			var (metricsCollectorIndicatorType, metricName, measurementUnitName, tags) =
				GetMetricsCollectorIndicatorInfo(interfaceMethodData);

			_output.WriteLine(
				$"Got metrics collector indicator info. ContextName:'{contextName}'. IndicatorType:{metricsCollectorIndicatorType}. MetricName:{metricName}. MeasurementUnitName:{measurementUnitName}. Tags:{tags}. Original method:{interfaceMethodData}");

			metricsPolicy.Setup(_ => _.IsEnabled(contextName, metricName))
				.Returns(metricEnabled).Verifiable();

			if (!metricEnabled)
			{
				return;
			}

			switch (metricsCollectorIndicatorType)
			{
				case MetricsCollectorIndicatorType.Counter:
					var counter = new Mock<ICounter>();
					metricsProvider.Setup(_ => _.CreateCounter(contextName, metricName, measurementUnitName, tags))
						.Returns(counter.Object).Verifiable();
					break;
				case MetricsCollectorIndicatorType.Gauge:
					var gauge = new Mock<IGauge>();
					metricsProvider.Setup(_ => _.CreateGauge(contextName, metricName, measurementUnitName, tags))
						.Returns(gauge.Object).Verifiable();
					break;
				case MetricsCollectorIndicatorType.HitPercentageGauge:
					var hitPercentageGauge = new Mock<IHitPercentageGauge>();
					metricsProvider.Setup(_ => _.CreateHitPercentageGauge(contextName, metricName, measurementUnitName, tags))
						.Returns(hitPercentageGauge.Object).Verifiable();
					break;
				case MetricsCollectorIndicatorType.Timer:
					var timer = new Mock<ITimer>();
					metricsProvider.Setup(_ => _.CreateTimer(contextName, metricName, measurementUnitName, tags))
						.Returns(timer.Object).Verifiable();
					break;
				case MetricsCollectorIndicatorType.Meter:
					var meter = new Mock<IMeter>();
					metricsProvider.Setup(_ => _.CreateMeter(contextName, metricName, measurementUnitName, tags))
						.Returns(meter.Object).Verifiable();
					break;
				case MetricsCollectorIndicatorType.Histogram:
					var histogram = new Mock<IHistogram>();
					metricsProvider.Setup(_ => _.CreateHistogram(contextName, metricName, measurementUnitName, tags))
						.Returns(histogram.Object).Verifiable();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(metricsCollectorIndicatorType), metricsCollectorIndicatorType,
						null);
			}
		}

		private static (MetricsCollectorIndicatorType metricsCollectorIndicatorType, string metricName, string
			measurementUnitName, Tags tags) GetMetricsCollectorIndicatorInfo(InterfaceMethodData interfaceMethodData)
		{
			var metricsCollectorIndicatorTypeString = interfaceMethodData.ReturnType.Name.TrimStart('I');
			if (!Enum.TryParse<MetricsCollectorIndicatorType>(metricsCollectorIndicatorTypeString, true,
				out var metricsCollectorIndicatorType))
			{
				throw new Exception($"Illegal return type:'{interfaceMethodData.ReturnType}'");
			}

			string metricName;
			string measurementUnitName = null;
			var metricsCollectorMethodAttributeData = interfaceMethodData.AttributeDataList
				.OfType<MetricsCollectorMethodAttributeData>().FirstOrDefault();
			if (metricsCollectorMethodAttributeData == null)
			{
				metricName = interfaceMethodData.Name;
			}
			else
			{
				metricName = metricsCollectorMethodAttributeData.MetricName;
				measurementUnitName = metricsCollectorMethodAttributeData.MeasurementUnitName;
			}

			Tags tags;
			if (!interfaceMethodData.Parameters.Any())
			{
				tags = Tags.Empty;
			}
			else if (interfaceMethodData.Parameters.Length == 1)
			{
				tags = new Tags(interfaceMethodData.Parameters[0].Name, interfaceMethodData.Parameters[0].Value.ToString());
			}
			else
			{
				tags = new Tags(interfaceMethodData.Parameters.Select(_ => _.Name).ToArray(),
					interfaceMethodData.Parameters.Select(_ => _.Value?.ToString()).ToArray());
			}

			return (metricsCollectorIndicatorType, metricName, measurementUnitName, tags);
		}
	}
}
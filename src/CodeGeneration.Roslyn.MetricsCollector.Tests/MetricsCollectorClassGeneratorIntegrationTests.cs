using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGeneration.Roslyn.Attributes.Common;
using CodeGeneration.Roslyn.Tests.Common;
using MetricsCollector.Abstractions;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class MetricsCollectorClassGeneratorIntegrationTests
	{
		[Theory]
		[MemberData(nameof(MethodSignatureGenerator))]
		public Task PositiveReportingMetricsEnabledTest(string[] baseInterfaceList, string methodSignature, string methodName, string metricsContext,
			MetricsCollectorType metricsCollectorType, MethodParameter[] methodParameters)
		{
			return MetricsCollectorMethodGenerationTest(baseInterfaceList, methodSignature, methodName,
				metricsContext, metricsCollectorType, methodParameters, true);
		}

		[Theory]
		[MemberData(nameof(MethodSignatureGenerator))]
		public Task NegativeReportingMetricsDisabledTest(string[] baseInterfaceList, string methodSignature, string methodName, string metricsContext,
			MetricsCollectorType metricsCollectorType, MethodParameter[] methodParameters)
		{
			return MetricsCollectorMethodGenerationTest(baseInterfaceList, methodSignature, methodName, metricsContext,
				metricsCollectorType, methodParameters, false);
		}


		private async Task MetricsCollectorMethodGenerationTest(string[] baseInterfaceList, string methodSignature, string methodName, string metricsContext,
			MetricsCollectorType metricsCollectorType, MethodParameter[] methodParameters, bool metricEnabled)
		{
			const string metricsCollectorTypeName = "ITestMetricsCollector";
			const string metricsCollectorTypeNamespace = "TestNamespace";

			var baseInterfaceListLeadingComma = baseInterfaceList.Any() ? "," : string.Empty;
			var interfaceSyntaxTree = CSharpSyntaxTree.ParseText(
				$"using {typeof(Action).Namespace};{Environment.NewLine}" +
				$"using {typeof(Attributes.MetricsCollectorStubAttribute).Namespace};{Environment.NewLine}" +
				$"using {typeof(ICounter).Namespace};{Environment.NewLine}" +
				$"namespace {metricsCollectorTypeNamespace}{{ {Environment.NewLine}" +
				$"[{nameof(Attributes.MetricsCollectorStubAttribute)}(\"{metricsContext}\" {baseInterfaceListLeadingComma} {string.Join(',', baseInterfaceList.Select(_ => $"\"{metricsCollectorTypeNamespace}.{_}\""))})]{Environment.NewLine}" +
				$"public interface {metricsCollectorTypeName} {Environment.NewLine}{{{Environment.NewLine} {string.Join(Environment.NewLine, methodSignature)} {Environment.NewLine}}} {Environment.NewLine}}}");

			var extraInterfaces =
				baseInterfaceList.Select(_ => SyntaxTreeHelper.GetEmptyInterfaceSyntax(metricsCollectorTypeNamespace, _));

			var extraTypes = new[]
			{
				typeof(GeneratedCodeAttribute),
				typeof(Attributes.MetricsCollectorStubAttribute),
				typeof(ImplementInterfaceAttribute),
				typeof(IMetricsProvider),
				typeof(IGauge),
				typeof(IHistogram),
				typeof(IHitPercentageGauge),
				typeof(IMeter),
				typeof(ITimer)
			};

			var assembly =
				await interfaceSyntaxTree.ProcessTransformationAndCompile<MetricsCollectorClassGenerator>(extraInterfaces,
					extraTypes);
		}

		public static IEnumerable<object[]> MethodSignatureGenerator
		{
			get
			{
				var metricsCollectorTypes = Enum.GetValues(typeof(MetricsCollectorType))
					.Cast<MetricsCollectorType>();

				foreach (var metricsCollectorType in metricsCollectorTypes)
				{
					var metricsContext = "MetricsContext" + Guid.NewGuid();
					yield return new object[]
					{
						new[] {"ITestInterface"}, $"{Environment.NewLine} I{metricsCollectorType} MethodWithoutAttribute();",
						"MethodWithoutAttribute", metricsContext, metricsCollectorType,
						Array.Empty<MethodParameter>()
					};
				}

				foreach (var metricsCollectorType in metricsCollectorTypes)
				{
					var metricsContext = "MetricsContext" + Guid.NewGuid();
					yield return new object[]
					{
						Array.Empty<string>(), $"{Environment.NewLine} I{metricsCollectorType} MethodWithoutAttribute();",
						"MethodWithoutAttribute",  metricsContext, metricsCollectorType, Array.Empty<MethodParameter>()
					};
				}


				var combinations = from metricsCollectorType in metricsCollectorTypes
					from paramsCount in Enumerable.Range(0, 2)
					from type in TypeHelper.Types
					from baseTypeCount in Enumerable.Range(0, 2)
					from addException in Enumerable.Range(0, 1)
					select new {metricsCollectorType, paramsCount, type, addException, baseTypeCount};

				var index = 0;
				foreach (var combination in combinations)
				{
					var metricsContext = "MetricsContext" + Guid.NewGuid();
					var parameters = Enumerable.Range(0, combination.paramsCount)
						.Select(_ => new MethodParameter($"param{_}", combination.type));
					var parametersString = string.Join(",", parameters.Select(_ => $"{_.Type.FullName} {_.Name}"));
					var methodName = $"Method{index}";
					index++;
					yield return new object[]
					{
						Enumerable.Range(0, combination.baseTypeCount).Select(_ => $"ITestInterface{_}").ToArray(),
						$"[{nameof(Attributes.MetricsCollectorMethodStubAttribute)}] {Environment.NewLine} I{combination.metricsCollectorType} {methodName}({parametersString});",
						methodName, metricsContext, combination.metricsCollectorType, parameters
					};
				}
			}
		}
	}
}
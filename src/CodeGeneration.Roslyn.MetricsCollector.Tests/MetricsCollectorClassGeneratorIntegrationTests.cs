using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn.Attributes.Common;
using CodeGeneration.Roslyn.Tests.Common;
using MetricsCollector.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class MetricsCollectorClassGeneratorIntegrationTests
	{
		[Theory]
		[MemberData(nameof(MethodSignatureGenerator))]
		public Task PositiveLoggingLogEnabledTest(string[] baseInterfaceList, string methodSignature, string methodName,
			MethodParameter[] methodParameters)
		{
			return LoggerMethodGenerationTest(baseInterfaceList, methodSignature, methodName,
				methodParameters, true);
		}

		[Theory]
		[MemberData(nameof(MethodSignatureGenerator))]
		public Task NegativeLoggingLogDisabledTest(string[] baseInterfaceList, string methodSignature, string methodName,
			MethodParameter[] methodParameters)
		{
			return LoggerMethodGenerationTest(baseInterfaceList, methodSignature, methodName,
				methodParameters, false);
		}


		private async Task LoggerMethodGenerationTest(string[] baseInterfaceList, string methodSignature, string methodName,
			MethodParameter[] methodParameters, bool logEnabled)
		{
			const string loggerTypeName = "ITestLogger";
			const string loggerTypeNamespace = "TestNamespace";

			var interfaceSyntaxTree = CSharpSyntaxTree.ParseText(
				$"using {typeof(Action).Namespace};{Environment.NewLine}" +
				$"using {typeof(Attributes.MetricsCollectorStubAttribute).Namespace};{Environment.NewLine}" +
				$"namespace {loggerTypeNamespace}{{ {Environment.NewLine}" +
				$"[{nameof(Attributes.MetricsCollectorStubAttribute)}({string.Join(',', baseInterfaceList.Select(_ => $"\"{loggerTypeNamespace}.{_}\""))})]{Environment.NewLine}" +
				$"public interface {loggerTypeName} {Environment.NewLine}{{{Environment.NewLine} {string.Join(Environment.NewLine, methodSignature)} {Environment.NewLine}}} {Environment.NewLine}}}");

			var extraInterfaces =
				baseInterfaceList.Select(_ => SyntaxTreeHelper.GetEmptyInterfaceSyntax(loggerTypeNamespace, _));

			var extraTypes = new[]
			{
				typeof(GeneratedCodeAttribute), typeof(Attributes.MetricsCollectorStubAttribute),
				typeof(ImplementInterfaceAttribute), typeof(IMetricsProvider)
			};

			var assembly =
				await interfaceSyntaxTree.ProcessTransformationAndCompile<MetricsCollectorClassGenerator>(extraInterfaces,
					extraTypes);


			// var loggerInterfaceType = assembly.GetType(loggerTypeNamespace + "." + loggerTypeName, true);
			// var loggerType = assembly.GetTypes()
			// 	.SingleOrDefault(_ => loggerInterfaceType.IsAssignableFrom(_) && !_.IsAbstract);
			// if (loggerType == null)
			// {
			// 	throw new Exception($"Logger type not found in emitted assembly");
			// }
			//
			// var internalLogger = new TestLogger(new EventId(1, methodName), methodSignature, methodName, message, logLevel,
			// 	methodParameters, logEnabled);
			// var loggerFactory = new TestLoggerFactory(internalLogger);
			// var logger = Activator.CreateInstance(loggerType, loggerFactory);
			// var loggerMethod = loggerType.GetTypeInfo().GetDeclaredMethod(methodName);
			// if (loggerMethod == null)
			// {
			// 	throw new Exception($"Logger method not found in emitted assembly");
			// }
			//
			// var parameters = methodParameters.Select(p => p.Value).ToArray();
			// loggerMethod.Invoke(logger, parameters);
			// internalLogger.Verify();
		}

		public static IEnumerable<object[]> MethodSignatureGenerator
		{
			get
			{
				yield return new object[]
				{
					new[] {"ITestInterface"}, $"{Environment.NewLine} void MethodWithoutAttribute();",
					"MethodWithoutAttribute", "MethodWithoutAttribute", Microsoft.Extensions.Logging.LogLevel.Information,
					Array.Empty<MethodParameter>()
				};

				yield return new object[]
				{
					Array.Empty<string>(), $"{Environment.NewLine} void MethodWithoutAttribute();", "MethodWithoutAttribute",
					"MethodWithoutAttribute", Microsoft.Extensions.Logging.LogLevel.Information, Array.Empty<MethodParameter>()
				};


				var logLevels = Enum.GetValues(typeof(Microsoft.Extensions.Logging.LogLevel))
					.Cast<Microsoft.Extensions.Logging.LogLevel>();

				var combinations = from logLevel in logLevels
					from paramsCount in Enumerable.Range(0, 2)
					from type in TypeHelper.Types
					from baseTypeCount in Enumerable.Range(0, 2)
					from addException in Enumerable.Range(0, 1)
					select new {logLevel, paramsCount, type, addException, baseTypeCount};

				var index = 0;
				foreach (var combination in combinations)
				{
					var message = "Message " + Guid.NewGuid();
					var parameters = Enumerable.Range(0, combination.paramsCount)
						.Select(_ => new MethodParameter($"param{_}", combination.type));
					var parametersString = string.Join(",", parameters.Select(_ => $"{_.Type.FullName} {_.Name}"));
					var methodName = $"Method{index}";
					index++;
					yield return new object[]
					{
						Enumerable.Range(0, combination.baseTypeCount).Select(_ => $"ITestInterface{_}").ToArray(),
						$"[{nameof(Attributes.MetricsCollectorMethodStubAttribute)}({typeof(Microsoft.Extensions.Logging.LogLevel).FullName}.{combination.logLevel}, \"{message}\")] {Environment.NewLine} void {methodName}({parametersString});",
						methodName, message, combination.logLevel, parameters
					};
				}
			}
		}
	}
}
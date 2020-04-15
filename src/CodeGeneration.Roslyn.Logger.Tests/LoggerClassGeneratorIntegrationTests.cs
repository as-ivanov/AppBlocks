using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn.Attributes.Common;
using CodeGeneration.Roslyn.Tests.Common;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CodeGeneration.Roslyn.Logger.Tests
{
	public class LoggerClassGeneratorIntegrationTests
	{
		public static IEnumerable<object[]> Generate()
		{
			var options = new LoggerInterfaceGeneratorOptions();
			var compilationUnitDataBuilder = new CompilationUnitDataBuilder(options);
			var combinations = compilationUnitDataBuilder.Build();
			return combinations.Select(_ => new object[] {_});
		}

		[Theory]
		[MemberData(nameof(Generate))]
		public Task PositiveLoggingLogEnabledTest(ITestGenerationContext generationContext)
		{
			return LoggerMethodGenerationTest(generationContext, true);
		}

		[Theory]
		[MemberData(nameof(Generate))]
		public Task NegativeLoggingLogDisabledTest(ITestGenerationContext generationContext)
		{
			return LoggerMethodGenerationTest(generationContext, false);
		}


		private static async Task LoggerMethodGenerationTest(ITestGenerationContext generationContext, bool logEnabled)
		{
			var syntaxTrees = generationContext.Entries.Select(entry => CSharpSyntaxTree.ParseText(entry.ToString()))
				.ToArray();

			var extraTypes = new[]
			{
				typeof(GeneratedCodeAttribute),
				typeof(Attributes.LoggerStubAttribute),
				typeof(ImplementInterfaceAttribute),
				typeof(ILogger)
			};

			var assembly = await syntaxTrees.ProcessTransformationAndCompile(extraTypes, CancellationToken.None);

			var sutMembers = generationContext.Entries.SelectMany(_ => _.Namespaces).SelectMany(_ => _.Members)
				.Where(_ => _.IsSut).OfType<InterfaceData>();

			foreach (var sutMember in sutMembers)
			{
				var loggerType = assembly.GetType(sutMember.Namespace + "." + sutMember.Name.TrimStart('I'), true);
				if (loggerType == null)
				{
					throw new Exception($"Logger type not found in emitted assembly");
				}

				foreach (var interfaceMethod in sutMember.Methods)
				{
					var methodName = interfaceMethod.Name;
					var loggerInterfaceMethodAttributeData = interfaceMethod.AttributeDataList
						.OfType<LoggerInterfaceMethodAttributeData>().FirstOrDefault();
					string message;
					Microsoft.Extensions.Logging.LogLevel logLevel;
					if (loggerInterfaceMethodAttributeData == null)
					{
						message = methodName;
						logLevel = Microsoft.Extensions.Logging.LogLevel.Information;
					}
					else
					{
						message = loggerInterfaceMethodAttributeData.Message;
						logLevel = loggerInterfaceMethodAttributeData.Level;
					}

					var internalLogger = new TestLogger(new EventId(1, methodName), methodName, message, logLevel,
						interfaceMethod.Parameters, logEnabled);
					var loggerFactory = new TestLoggerFactory(internalLogger);
					var logger = Activator.CreateInstance(loggerType, loggerFactory);
					var loggerMethod = loggerType.GetTypeInfo().GetDeclaredMethod(methodName);
					if (loggerMethod == null)
					{
						throw new Exception($"Logger method not found in emitted assembly");
					}

					var parameters = interfaceMethod.Parameters.Select(p => p.Value).ToArray();
					loggerMethod.Invoke(logger, parameters);
					internalLogger.Verify();
				}
			}
		}
	}
}
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn.Attributes.Common;
using CodeGeneration.Roslyn.Tests.Common;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Microsoft.Extensions.Logging;

namespace CodeGeneration.Roslyn.Logger.Tests
{
	public class LoggerClassGeneratorIntegrationTestBase
	{
		public static IEnumerable<object[]> Generate()
		{
			var options = new LoggerInterfaceGeneratorOptions();
			var compilationUnitDataBuilder = new CompilationUnitDataBuilder(options);
			var combinations = compilationUnitDataBuilder.Build();
			return combinations.Select(_ => new object[] {_});
		}

		protected static async Task LoggerMethodGenerationTest(ITestContext context, bool logEnabled)
		{
			var syntaxTrees = context.CompilationEntries.Select(entry => CSharpSyntaxTree.ParseText(entry.ToString()))
				.ToArray();

			var extraTypes = new[]
			{
				typeof(GeneratedCodeAttribute),
				typeof(Attributes.LoggerStubAttribute),
				typeof(ImplementInterfaceAttribute),
				typeof(ILogger)
			};

			Assembly assembly = null;
			try
			{
				assembly = await syntaxTrees.ProcessTransformationAndCompile(extraTypes, CancellationToken.None);
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
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CodeGeneration.Roslyn.Logger.Tests
{
	public class LoggerClassGeneratorIntegrationTests
	{
		private static readonly Type[] _types =
		{
			typeof(string),
			typeof(char),
			typeof(byte),
			typeof(sbyte),
			typeof(ushort),
			typeof(short),
			typeof(uint),
			typeof(int),
			typeof(ulong),
			typeof(long),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(DateTime),
			typeof(object),
		};

		[Theory]
		[MemberData(nameof(MethodSignatureGenerator))]
		public Task PositiveLoggingLogEnabledTest(string methodSignature, string methodName, string message,
			Microsoft.Extensions.Logging.LogLevel logLevel,
			MethodParameter[] methodParameters)
		{
			return LoggerMethodGenerationTest(methodSignature, methodName, message,
				logLevel, methodParameters, true);
		}

		[Theory]
		[MemberData(nameof(MethodSignatureGenerator))]
		public Task NegativeLoggingLogDisabledTest(string methodSignature, string methodName, string message,
			Microsoft.Extensions.Logging.LogLevel logLevel,
			MethodParameter[] methodParameters)
		{
			return LoggerMethodGenerationTest(methodSignature, methodName, message,
				logLevel, methodParameters, false);
		}


		private async Task LoggerMethodGenerationTest(string methodSignature, string methodName, string message, Microsoft.Extensions.Logging.LogLevel logLevel,
			MethodParameter[] methodParameters, bool logEnabled)
		{
			const string loggerTypeName = "ITestLogger";
			const string loggerTypeNamespace = "TestNamespace";

			var interfaceSyntaxTree = CSharpSyntaxTree.ParseText(
				$"using {typeof(Action).Namespace};{Environment.NewLine}" +
				$"using {typeof(ILogger).Namespace};{Environment.NewLine}" +
				$"using {typeof(Attributes.LoggerStubAttribute).Namespace};{Environment.NewLine}" +
				$"namespace {loggerTypeNamespace}{{ {Environment.NewLine}" +
				$"[{nameof(Attributes.LoggerStubAttribute)}]{Environment.NewLine}" +
				$"public interface {loggerTypeName} {Environment.NewLine}{{{Environment.NewLine} {string.Join(Environment.NewLine, methodSignature)} {Environment.NewLine}}} {Environment.NewLine}}}");
			var compilation = CreateCompilation(interfaceSyntaxTree);
			var inputSemanticModel = compilation.GetSemanticModel(interfaceSyntaxTree);
			var attributeData = compilation.GetAttributeData(inputSemanticModel, interfaceSyntaxTree.GetRoot()).First();

			var generator = new LoggerClassGenerator(attributeData);

			var transformationContext = CreateTransformationContext(interfaceSyntaxTree, inputSemanticModel, compilation);
			var progress = new NoopProgress();

			var emitted = await generator.GenerateRichAsync(transformationContext, progress, CancellationToken.None);

			var usings = GetUsingDirectives();
			var compilationUnit =
				SyntaxFactory.CompilationUnit(
						default, usings, default,
						SyntaxFactory.List(emitted.Members))
					.NormalizeWhitespace();
			var loggerSyntaxTree = CSharpSyntaxTree.ParseText(compilationUnit.SyntaxTree.GetText()); //TODO fixes compilation error if using compilation.AddSyntaxTrees(compilationUnit.SyntaxTree)
			compilation = compilation.AddSyntaxTrees(loggerSyntaxTree);

			var assembly = CompileAndLoadAssembly(compilation);


			var loggerInterfaceType = assembly.GetType(loggerTypeNamespace + "." + loggerTypeName, true);
			var loggerType = assembly.GetTypes()
				.SingleOrDefault(_ => loggerInterfaceType.IsAssignableFrom(_) && !_.IsAbstract);
			if (loggerType == null)
			{
				throw new Exception($"Logger type not found in emitted assembly");
			}
			var internalLogger = new TestLogger(new EventId(1, methodName), methodSignature, methodName, message,  logLevel, methodParameters, logEnabled);
			var loggerFactory = new TestLoggerFactory(internalLogger);
			var logger = Activator.CreateInstance(loggerType, loggerFactory);
			var loggerMethod = loggerType.GetTypeInfo().GetDeclaredMethod(methodName);
			if (loggerMethod == null)
			{
				throw new Exception($"Logger method not found in emitted assembly");
			}
			var parameters = methodParameters.Select(p => p.Value).ToArray();
			loggerMethod.Invoke(logger, parameters);
			internalLogger.Verify();
		}

		private static SyntaxList<UsingDirectiveSyntax> GetUsingDirectives()
		{
			return
				SyntaxFactory.List(
					new[]
					{
						SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(typeof(Action).Namespace)),
						SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(typeof(Attributes.LoggerStubAttribute).Namespace)),
						SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(typeof(ILogger).Namespace))
					});
		}


		private static TransformationContext CreateTransformationContext(SyntaxTree root, SemanticModel inputSemanticModel,
			CSharpCompilation compilation)
		{
			var inputCompilationUnit = root.GetCompilationUnitRoot();

			var emittedExterns = inputCompilationUnit
				.Externs
				.Select(x => x.WithoutTrivia())
				.ToImmutableArray();

			var emittedUsings = inputCompilationUnit
				.Usings
				.Select(x => x.WithoutTrivia())
				.ToImmutableArray();

			var interfaceNode = root.GetRoot()
				.DescendantNodesAndSelf()
				.OfType<InterfaceDeclarationSyntax>().FirstOrDefault();

			var context = new TransformationContext(
				interfaceNode,
				inputSemanticModel,
				compilation,
				string.Empty,
				emittedUsings,
				emittedExterns);

			return context;
		}

		public static IEnumerable<object[]> MethodSignatureGenerator
		{
			get
			{
				var logLevels = Enum.GetValues(typeof(Microsoft.Extensions.Logging.LogLevel))
					.Cast<Microsoft.Extensions.Logging.LogLevel>();

				var combinations = from logLevel in logLevels
					from paramsCount in Enumerable.Range(0, 2)
					from type in _types
					from addException in Enumerable.Range(0, 1)
					select new {logLevel, paramsCount, type, addException};

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
						$"[{nameof(Attributes.LoggerMethodStubAttribute)}({typeof(Microsoft.Extensions.Logging.LogLevel).FullName}.{combination.logLevel}, \"{message}\")] {Environment.NewLine} void {methodName}({parametersString});",
						methodName, message, combination.logLevel, parameters
					};
				}
			}
		}

		private static CSharpCompilation CreateCompilation(params SyntaxTree[] syntaxTrees)
		{
			var assemblyName = Guid.NewGuid().ToString();
			var references = GetAssemblyReferences();
			var compilation = CSharpCompilation.Create(
				assemblyName,
				syntaxTrees,
				references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
					assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
			return compilation;
		}

		private static Assembly CompileAndLoadAssembly(CSharpCompilation compilation)
		{
			using var ms = new MemoryStream();
			var result = compilation.Emit(ms);
			ThrowExceptionIfCompilationFailure(result);
			ms.Seek(0, SeekOrigin.Begin);
			return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(ms);
		}

		private static void ThrowExceptionIfCompilationFailure(EmitResult result)
		{
			if (!result.Success)
			{
				var compilationErrors = result.Diagnostics.Where(diagnostic =>
						diagnostic.IsWarningAsError ||
						diagnostic.Severity == DiagnosticSeverity.Error)
					.ToList();
				if (compilationErrors.Any())
				{
					var firstError = compilationErrors.First();
					var errorNumber = firstError.Id;
					var errorDescription = firstError.GetMessage();
					var firstErrorMessage = $"{errorNumber}: {errorDescription};";
					throw new Exception($"Compilation failed, first error is: {firstErrorMessage}");
				}
			}
		}

		private static IEnumerable<MetadataReference> GetAssemblyReferences()
		{
			var coreAssemblyNames = new[]
			{
				"mscorlib.dll",
				"netstandard.dll",
				"System.dll",
				"System.Core.dll",
				"System.Private.CoreLib.dll",
				"System.Runtime.dll",
			};

			var coreAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
			var coreMetaReferences =
				coreAssemblyNames.Select(x => MetadataReference.CreateFromFile(Path.Combine(coreAssemblyPath, x)));
			var extraReferences = new MetadataReference[]
			{
				MetadataReference.CreateFromFile(typeof(Microsoft.Extensions.Logging.LogLevel).GetTypeInfo().Assembly.Location),
				MetadataReference.CreateFromFile(typeof(Attributes.LoggerStubAttribute).GetTypeInfo().Assembly.Location),
				MetadataReference.CreateFromFile(typeof(GeneratedCodeAttribute).GetTypeInfo().Assembly.Location)
			};
			return coreMetaReferences.Concat(extraReferences);
		}
	}
}
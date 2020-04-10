using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;

namespace CodeGeneration.Roslyn.Tests.Common
{
	public static class TransformationHelper
	{
		public static async Task<Assembly> ProcessTransformationAndCompile<TGenerator>(this SyntaxTree root,
			IEnumerable<SyntaxTree> extraNodes, IEnumerable<Type> extraTypes
		)
			where TGenerator : IRichCodeGenerator //TODO Get generator type from attribute
		{
			var references = GetAssemblyReferences(extraTypes);
			var compilation = TransformationHelper.CreateCompilation(root, references);

			var interfaceNode = root.GetRoot()
				.DescendantNodesAndSelf()
				.OfType<InterfaceDeclarationSyntax>().First();

			var inputSemanticModel = compilation.GetSemanticModel(interfaceNode.SyntaxTree);

			var attributeData = inputSemanticModel.GetDeclaredSymbol(interfaceNode).GetAttributes().First();

			var generator = (TGenerator) Activator.CreateInstance(typeof(TGenerator), attributeData);

			var transformationContext = CreateTransformationContext(root, inputSemanticModel, compilation);
			var progress = new NoopProgress();

			var emitted = await generator.GenerateRichAsync(transformationContext, progress, CancellationToken.None);

			var usings = GetUsingDirectives(extraTypes);

			var compilationUnit =
				SyntaxFactory.CompilationUnit(
						default, usings, default,
						SyntaxFactory.List<MemberDeclarationSyntax>(emitted.Members))
					.NormalizeWhitespace();
			var loggerSyntaxTree =
				CSharpSyntaxTree.ParseText(compilationUnit.SyntaxTree
					.GetText()); //TODO fixes compilation error if using compilation.AddSyntaxTrees(compilationUnit.SyntaxTree)
			compilation = compilation.AddSyntaxTrees(loggerSyntaxTree);

			foreach (var extraNode in extraNodes)
			{
				compilation = compilation.AddSyntaxTrees(extraNode);
			}

			return compilation.CompileAndLoadAssembly();
		}

		private static IEnumerable<MetadataReference> GetAssemblyReferences(IEnumerable<Type> extraTypes)
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
			var extraReferences = extraTypes.Select(_ => MetadataReference.CreateFromFile(_.GetTypeInfo().Assembly.Location));
			return coreMetaReferences.Concat(extraReferences);
		}

		private static SyntaxList<UsingDirectiveSyntax> GetUsingDirectives(IEnumerable<Type> extraTypes)
		{
			var distinctNamespaces = extraTypes.Select(_ => _.Namespace).Distinct();
			var directives = distinctNamespaces.Select(_ => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(_)));
			return SyntaxFactory.List(directives);
		}

		private static TransformationContext CreateTransformationContext(this SyntaxTree root,
			SemanticModel inputSemanticModel,
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

		private static CSharpCompilation CreateCompilation(SyntaxTree syntaxTrees,
			IEnumerable<MetadataReference> references)
		{
			var assemblyName = Guid.NewGuid().ToString();
			var compilation = CSharpCompilation.Create(
				assemblyName,
				new[] {syntaxTrees},
				references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
					assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
			return compilation;
		}

		private static Assembly CompileAndLoadAssembly(this CSharpCompilation compilation)
		{
			static void ThrowExceptionIfCompilationFailure(EmitResult result)
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

			using var ms = new MemoryStream();
			var result = compilation.Emit(ms);
			ThrowExceptionIfCompilationFailure(result);
			ms.Seek(0, SeekOrigin.Begin);
			return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(ms);
		}
	}
}
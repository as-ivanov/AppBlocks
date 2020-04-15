using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;

namespace CodeGeneration.Roslyn.Tests.Common
{
	public static class TransformationHelper
	{
		public static async Task<Assembly> ProcessTransformationAndCompile(this SyntaxTree[] syntaxTrees, IEnumerable<Type> extraTypes, CancellationToken cancellationToken)
		{
			var references = GetAssemblyReferences(extraTypes);
			var compilation = CreateCompilation(syntaxTrees, references);


			foreach (var syntaxTree in compilation.SyntaxTrees)
			{

				var nodes = (await syntaxTree.GetRootAsync(cancellationToken))
					.DescendantNodesAndSelf().OfType<CSharpSyntaxNode>();

				foreach (var node in nodes)
				{
					var inputSemanticModel = compilation.GetSemanticModel(node.SyntaxTree);

					var attributeData = GetAttributeData(compilation, inputSemanticModel, node);
					var generators = FindCodeGenerators(attributeData);
					foreach (var generator in generators)
					{
						var transformationContext = CreateTransformationContext(syntaxTree, node, inputSemanticModel, compilation);
						var progress = new NoopProgress();

						var emitted = await generator.GenerateRichAsync(transformationContext, progress, CancellationToken.None);

						var usings = GetUsingDirectives(extraTypes);

						var compilationUnit =
							SyntaxFactory.CompilationUnit(
									default, usings, default,
									SyntaxFactory.List<MemberDeclarationSyntax>(emitted.Members))
								.NormalizeWhitespace();

						var syntaxTreeText = await compilationUnit.SyntaxTree.GetTextAsync(cancellationToken);

						var emittedSyntaxTree =
							CSharpSyntaxTree.ParseText(syntaxTreeText); //TODO fixes compilation error if using compilation.AddSyntaxTrees(compilationUnit.SyntaxTree)
						compilation = compilation.AddSyntaxTrees(emittedSyntaxTree);
					}
				}
			}

			return compilation.CompileAndLoadAssembly();
		}

		private static ImmutableArray<AttributeData> GetAttributeData(Compilation compilation, SemanticModel document, SyntaxNode syntaxNode)
		{
			switch (syntaxNode)
			{
				case CompilationUnitSyntax syntax:
					return compilation.Assembly.GetAttributes().Where(x => x.ApplicationSyntaxReference.SyntaxTree == syntax.SyntaxTree).ToImmutableArray();
				default:
					return document.GetDeclaredSymbol(syntaxNode)?.GetAttributes() ?? ImmutableArray<AttributeData>.Empty;
			}
		}

		private static IEnumerable<IRichCodeGenerator> FindCodeGenerators(ImmutableArray<AttributeData> nodeAttributes)
		{
			foreach (var attributeData in nodeAttributes)
			{
				var generatorType = GetCodeGeneratorTypeForAttribute(attributeData.AttributeClass);
				if (generatorType != null)
				{
					IRichCodeGenerator generator;
					try
					{
						generator = (IRichCodeGenerator)Activator.CreateInstance(generatorType, attributeData);
					}
					catch (MissingMethodException)
					{
						throw new InvalidOperationException(
							$"Failed to instantiate {generatorType}. ICodeGenerator implementations must have" +
							$" a constructor accepting Microsoft.CodeAnalysis.AttributeData argument.");
					}
					yield return generator;
				}
			}
		}

		private static Type GetCodeGeneratorTypeForAttribute(INamedTypeSymbol attributeType)
		{
			if (attributeType == null)
			{
				return null;
			}

			foreach (var generatorCandidateAttribute in attributeType.GetAttributes())
			{
				if (generatorCandidateAttribute.AttributeClass.Name != typeof(CodeGenerationAttributeAttribute).Name)
				{
					continue;
				}

				string assemblyName = null;
				string fullTypeName = null;
				var firstArg = generatorCandidateAttribute.ConstructorArguments.Single();
				if (firstArg.Value is string typeName)
				{
					// This string is the full name of the type, which MAY be assembly-qualified.
					int commaIndex = typeName.IndexOf(',');
					bool isAssemblyQualified = commaIndex >= 0;
					if (isAssemblyQualified)
					{
						fullTypeName = typeName.Substring(0, commaIndex);
						assemblyName = typeName.Substring(commaIndex + 1).Trim();
					}
					else
					{
						fullTypeName = typeName;
						assemblyName = generatorCandidateAttribute.AttributeClass.ContainingAssembly.Name;
					}
				}
				else if (firstArg.Value is INamedTypeSymbol typeOfValue)
				{
					// This was a typeof(T) expression
					fullTypeName = typeOfValue.GetFullTypeName();
					assemblyName = typeOfValue.ContainingAssembly.Name;
				}

				if (assemblyName != null)
				{
					var assembly = Assembly.Load(new AssemblyName(assemblyName));
					if (assembly != null)
					{
						return assembly.GetType(fullTypeName);
					}
				}
			}

			return null;
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

		private static TransformationContext CreateTransformationContext(this SyntaxTree root, CSharpSyntaxNode processingNode,
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


			var context = new TransformationContext(
				processingNode,
				inputSemanticModel,
				compilation,
				string.Empty,
				emittedUsings,
				emittedExterns);

			return context;
		}

		private static CSharpCompilation CreateCompilation(SyntaxTree[] syntaxTrees,
			IEnumerable<MetadataReference> references)
		{
			var assemblyName = Guid.NewGuid().ToString();
			var compilation = CSharpCompilation.Create(
				assemblyName,
				syntaxTrees,
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
						var text = firstError.Location.SourceTree.GetText();
						var firstErrorMessage = $"{errorNumber}: {errorDescription}; {Environment.NewLine} {text}";
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
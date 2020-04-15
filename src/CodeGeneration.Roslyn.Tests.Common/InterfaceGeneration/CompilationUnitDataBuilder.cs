using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public class CompilationUnitDataBuilder
	{
		private readonly ITestInterfaceGenerationOptions _options;

		public CompilationUnitDataBuilder(ITestInterfaceGenerationOptions options)
		{
			_options = options;
		}

		public IEnumerable<ITestGenerationContext> Build()
		{
			foreach (var interfaceNumber in _options.InterfaceNumbers)
			{
				var interfaceCombinations = InterfaceData.GetAllCombinations(_options).ToList().Combinations(interfaceNumber);
				foreach (var interfaceCombination in interfaceCombinations)
				{
					var generationContext = new TestGenerationContext(_options);
					var namespaceData = new NamespaceData(_options.InterfaceNamespace, interfaceCombination.Select(_ => _.Invoke(generationContext)).ToArray());
					var compilationEntryData = new CompilationEntryData(_options.UsingNamespaces, namespaceData);
					generationContext.AddEntry(compilationEntryData);
					yield return generationContext;
				}
			}
		}
	}
}
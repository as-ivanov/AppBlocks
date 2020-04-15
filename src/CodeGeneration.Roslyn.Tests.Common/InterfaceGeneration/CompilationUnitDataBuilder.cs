using System;
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
			var interfaceVariations = InterfaceData.GetPossibleVariations(_options).ToList();
			foreach (var interfaceNumber in _options.InterfaceNumbers)
			{
				if (interfaceNumber == 1)
				{
					var interfaceCombinations = interfaceVariations.Combinations(interfaceNumber);
					foreach (var interfaceCombination in interfaceCombinations)
					{
						var generationContext = GetInterfaceCombinationData(interfaceCombination);
						yield return generationContext;
					}
				}
				else
				{
					var interfaceCombination = interfaceVariations.Take(interfaceNumber);
					var generationContext = GetInterfaceCombinationData(interfaceCombination);
					yield return generationContext;
				}
			}
		}

		private ITestGenerationContext GetInterfaceCombinationData(IEnumerable<Func<ITestGenerationContext, InterfaceData>> interfaceCombination)
		{
			var generationContext = new TestGenerationContext(_options);
			var namespaceData = new NamespaceData(_options.InterfaceNamespace, interfaceCombination.Select(_ => _.Invoke(generationContext)).ToArray());
			var compilationEntryData = new CompilationEntryData(_options.UsingNamespaces, namespaceData);
			generationContext.AddEntry(compilationEntryData);
			return generationContext;
		}
	}
}
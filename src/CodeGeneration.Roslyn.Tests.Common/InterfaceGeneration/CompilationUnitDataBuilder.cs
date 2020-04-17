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

		public IEnumerable<ITestContext> Build()
		{
			var interfaceVariations = InterfaceData.GetPossibleVariations(_options).ToList();
			foreach (var interfaceCount in _options.InterfaceCounts)
			{
				if (interfaceCount == 1)
				{
					var interfaceCombinations = interfaceVariations.GetPossibleCombinations(interfaceCount);
					foreach (var interfaceCombination in interfaceCombinations)
					{
						var generationContext = GetInterfaceCombinationData(interfaceCombination);
						yield return generationContext;
					}
				}
				else
				{
					var interfaceCombination = interfaceVariations.Take(interfaceCount);
					var generationContext = GetInterfaceCombinationData(interfaceCombination);
					yield return generationContext;
				}
			}
		}

		private ITestContext GetInterfaceCombinationData(IEnumerable<Func<ITestContext, InterfaceData>> interfaceCombination)
		{
			var generationContext = new TestContext(_options);
			var namespaceData = new NamespaceData(_options.InterfaceNamespace, interfaceCombination.Select(_ => _.Invoke(generationContext)).ToArray());
			var compilationEntryData = new CompilationEntryData(_options.UsingNamespaces, namespaceData);
			generationContext.AddCompilationEntry(compilationEntryData);
			return generationContext;
		}
	}
}
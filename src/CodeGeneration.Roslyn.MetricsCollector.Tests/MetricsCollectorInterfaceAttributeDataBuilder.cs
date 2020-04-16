using System;
using System.Collections.Generic;
using System.Linq;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{

	public class MetricsCollectorInterfaceAttributeDataBuilder : IAttributeDataBuilder
	{
		public IEnumerable<Func<ITestGenerationContext, IEnumerable<AttributeData>>> GetPossibleCombinations(ITestInterfaceGenerationOptions options)
		{
			var attributeToInheritInterfacesNumbers = Enumerable.Range(0, 2);
			foreach (var attributeToInheritInterfacesNumber in attributeToInheritInterfacesNumbers)
			{
				yield return context =>
				{
					var interfacesToInherit = Enumerable.Range(context.NextId(), attributeToInheritInterfacesNumber)
						.Select(GetBaseTypeInterfaceData).ToArray();
					var contextName = "Context_" + context.NextId();

					foreach (var interfaceToInherit in interfacesToInherit)
					{
						var namespaceData = new NamespaceData(interfaceToInherit.Namespace, interfaceToInherit);
						var compilationEntryData = new CompilationEntryData(options.UsingNamespaces, namespaceData);
						context.AddEntry(compilationEntryData);
					}
					return new AttributeData[] { new MetricsCollectorInterfaceAttributeData(contextName, interfacesToInherit) };
				};
			}
		}

		private static InterfaceData GetBaseTypeInterfaceData(int index)
		{
			return new InterfaceData("ITestInterfaceToInherit" + index, "TestNamespaceForITestInterfaceToInherit" + index);
		}
	}
}
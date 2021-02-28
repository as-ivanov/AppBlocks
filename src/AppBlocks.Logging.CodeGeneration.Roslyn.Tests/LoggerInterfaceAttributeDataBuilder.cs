using System;
using System.Collections.Generic;
using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerInterfaceAttributeDataBuilder : IAttributeDataBuilder
	{
		public IEnumerable<Func<ITestContext, IEnumerable<AttributeData>>> GetPossibleCombinations(ITestInterfaceGenerationOptions options)
		{
			var attributeToInheritInterfacesNumbers = Enumerable.Range(0, 2);
			foreach (var attributeToInheritInterfacesNumber in attributeToInheritInterfacesNumbers)
			{
				yield return context =>
				{
					var interfacesToInherit = Enumerable.Range(context.NextId(), attributeToInheritInterfacesNumber)
						.Select(GetBaseTypeInterfaceData).ToArray();

					foreach (var interfaceToInherit in interfacesToInherit)
					{
						var namespaceData = new NamespaceData(interfaceToInherit.Namespace, interfaceToInherit);
						var compilationEntryData = new CompilationEntryData(options.UsingNamespaces, namespaceData);
						context.AddCompilationEntry(compilationEntryData);
					}

					return new AttributeData[] {new LoggerInterfaceAttributeData(interfacesToInherit)};
				};
			}
		}

		private static InterfaceData GetBaseTypeInterfaceData(int index)
		{
			return new InterfaceData("ITestInterfaceToInherit" + index + "<TInterfaceParam1, TInterfaceParam2>", "TestNamespaceForITestInterfaceToInherit" + index);
		}
	}
}

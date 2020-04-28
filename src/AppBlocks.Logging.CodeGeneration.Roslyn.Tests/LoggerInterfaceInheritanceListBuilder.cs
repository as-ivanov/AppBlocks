using System;
using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerInterfaceInheritanceListBuilder : IInheritanceListBuilder
	{
		public Func<ITestContext, InterfaceData[]> GetInheritedInterfaces(ITestInterfaceGenerationOptions options, int count)
		{
			return (context) =>
			{
				var interfaces = Enumerable.Range(context.NextId(), count).Select(_ => GetInheritedInterfaceData(_, context)).ToArray();
				foreach (var @interface in interfaces)
				{
					var namespaceData = new NamespaceData(@interface.Namespace, @interface);
					var compilationEntryData = new CompilationEntryData(options.UsingNamespaces, namespaceData);
					context.AddCompilationEntry(compilationEntryData);
				}
				return interfaces;
			};
		}

		private static InterfaceData GetInheritedInterfaceData(int index, ITestContext context)
		{
			var methods = InterfaceMethodData.GetPossibleVariations(context.Options).ToList()
				.GetPossibleCombinations(context.Options.InterfaceMethodsCounts.Max()).First().Select(_ => _.Invoke(context))
				.ToArray();
			return new InterfaceData("ITestInheritedInterface" + index, "TestNamespaceForITestInheritedInterface" + index, Array.Empty<AttributeData>(), methods, Array.Empty<InterfaceData>(), false);
		}
	}
}
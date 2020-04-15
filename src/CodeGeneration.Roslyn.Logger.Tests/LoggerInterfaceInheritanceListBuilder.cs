using System;
using System.Linq;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace CodeGeneration.Roslyn.Logger.Tests
{
	public class LoggerInterfaceInheritanceListBuilder : IInheritanceListBuilder
	{
		public Func<ITestGenerationContext, InterfaceData[]> GetInheritedInterfaces(ITestInterfaceGenerationOptions options, int count)
		{
			return (context) =>
			{
				var interfaces = Enumerable.Range(context.NextId(), count).Select(GetInheritedInterfaceData).ToArray();
				foreach (var @interface in interfaces)
				{
					var namespaceData = new NamespaceData(@interface.Namespace, @interface);
					var compilationEntryData = new CompilationEntryData(options.UsingNamespaces, namespaceData);
					context.AddEntry(compilationEntryData);
				}
				return interfaces;
			};
		}

		private static InterfaceData GetInheritedInterfaceData(int index)
		{
			var loggerInterfaceAttributeData = new LoggerInterfaceAttributeData(Array.Empty<InterfaceData>());
			return new InterfaceData("ITestInheritedInterface" + index, "TestNamespaceForITestInheritedInterface" + index, new AttributeData[] { loggerInterfaceAttributeData }, Array.Empty<InterfaceMethodData>(), Array.Empty<InterfaceData>(), true);
		}
	}
}
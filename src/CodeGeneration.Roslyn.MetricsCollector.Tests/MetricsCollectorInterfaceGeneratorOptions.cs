using System;
using System.Linq;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using MetricsCollector.Abstractions;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class MetricsCollectorInterfaceGeneratorOptions : ITestInterfaceGenerationOptions
	{
		private readonly string[] _usingNamespaces =
		{
			typeof(Action).Namespace,
			typeof(Attributes.MetricsCollectorStubAttribute).Namespace,
			typeof(ICounter).Namespace
		};

		private readonly Type[] _interfaceMethodReturnTypes =
		{
			typeof(ICounter),
			// typeof(IHistogram),
			// typeof(IHitPercentageGauge),
			// typeof(IMeter),
			// typeof(IGauge),
			// typeof(ITimer)
		};

		private readonly int[] _interfaceNumbers = Enumerable.Range(1, 2).ToArray();
		private readonly int[] _inheritedInterfaceNumbers = Enumerable.Range(0, 2).ToArray();
		private readonly int[] _interfaceMethodsNumbers = Enumerable.Range(0, 2).ToArray();
		private readonly int[] _methodParameterNumbers = Enumerable.Range(0, 3).ToArray();

		private const string _interfaceNamespace = "TestNamespace";

		private readonly IAttributeDataBuilder _interfaceAttributeDataBuilder =
			new MetricsCollectorInterfaceAttributeDataBuilder();

		private readonly IAttributeDataBuilder _methodAttributeDataBuilder =
			new MetricsCollectorInterfaceMethodAttributeDataDataBuilder();

		private readonly Type[] _methodParameterTypes = {
			typeof(string),
			typeof(char),
			// typeof(byte),
			//typeof(sbyte),
			//typeof(ushort),
			//typeof(short),
			//typeof(uint),
			//typeof(int),
			//typeof(ulong),
			//typeof(long),
			//typeof(float),
			// typeof(double),
			// typeof(decimal),
			//typeof(DateTime),
			//typeof(object),
			//typeof(Exception),
		};

		private readonly IInheritanceListBuilder _inheritanceListBuilder = new MetricsCollectorInterfaceInheritanceListBuilder();

		public string[] UsingNamespaces => _usingNamespaces;

		public IAttributeDataBuilder InterfaceAttributeDataBuilder => _interfaceAttributeDataBuilder;

		public IAttributeDataBuilder MethodAttributeDataBuilder => _methodAttributeDataBuilder;

		public Type[] InterfaceMethodReturnTypes => _interfaceMethodReturnTypes;

		public int[] InterfaceCounts => _interfaceNumbers;

		public int[] InheritedInterfaceCounts => _inheritedInterfaceNumbers;

		public int[] InterfaceMethodsCounts => _interfaceMethodsNumbers;

		public int[] MethodParameterCounts => _methodParameterNumbers;

		public Type[] MethodParameterTypes => _methodParameterTypes;

		public string InterfaceNamespace => _interfaceNamespace;

		public IInheritanceListBuilder InheritanceListBuilder => _inheritanceListBuilder;
	}
}
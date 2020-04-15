using System;
using System.Linq;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using MetricsCollector.Abstractions;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class MetricsCollectorStubGeneratorOptions : ITestInterfaceGenerationOptions
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
			//typeof(IHistogram),
			//typeof(IHitPercentageGauge),
			//typeof(IMeter),
			//typeof(IGauge),
			//typeof(ITimer)
		};

		private readonly int[] _interfaceNumbers = Enumerable.Range(1, 2).ToArray();
		private readonly int[] _inheritedInterfaceNumbers = Enumerable.Range(0, 2).ToArray();
		private readonly int[] _interfaceMethodsNumbers = Enumerable.Range(0, 2).ToArray();
		private readonly int[] _methodParameterNumbers = Enumerable.Range(0, 2).ToArray();

		private const string _namespace = "TestNamespace";

		private readonly IAttributeDataBuilder _interfaceAttributeDataBuilder =
			new MetricsCollectorInterfaceAttributeDataBuilder();

		private readonly IAttributeDataBuilder _methodAttributeDataBuilder =
			new MetricsCollectorMethodAttributeDataDataBuilder();

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

		private readonly IInheritanceListBuilder _inheritanceListBuilder = new MetricsCollectorStubInheritanceListBuilder();

		public string[] UsingNamespaces => _usingNamespaces;

		public IAttributeDataBuilder InterfaceAttributeDataBuilder => _interfaceAttributeDataBuilder;

		public IAttributeDataBuilder MethodAttributeDataBuilder => _methodAttributeDataBuilder;

		public Type[] InterfaceMethodReturnTypes => _interfaceMethodReturnTypes;

		public int[] InterfaceNumbers => _interfaceNumbers;

		public int[] InheritedInterfaceNumbers => _inheritedInterfaceNumbers;

		public int[] InterfaceMethodsNumbers => _interfaceMethodsNumbers;

		public int[] MethodParameterNumbers => _methodParameterNumbers;

		public Type[] MethodParameterTypes => _methodParameterTypes;

		public string InterfaceNamespace => _namespace;

		public IInheritanceListBuilder InheritanceListBuilder => _inheritanceListBuilder;
	}
}
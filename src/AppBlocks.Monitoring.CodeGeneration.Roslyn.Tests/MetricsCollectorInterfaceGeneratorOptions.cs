using System;
using System.Linq;
using System.Threading.Tasks;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using AppBlocks.Monitoring.Abstractions;
using AppBlocks.Monitoring.CodeGeneration.Attributes;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn.Tests
{
	public class MetricsCollectorInterfaceGeneratorOptions : ITestInterfaceGenerationOptions
	{
		private const string _interfaceNamespace = "TestNamespace";

		private readonly IInheritanceListBuilder _inheritanceListBuilder =
			new MetricsCollectorInterfaceInheritanceListBuilder();

		private readonly int[] _inheritedInterfaceCounts = Enumerable.Range(0, 3).ToArray();

		private readonly IAttributeDataBuilder _interfaceAttributeDataBuilder =
			new MetricsCollectorInterfaceAttributeDataBuilder();

		private readonly int[] _interfaceCounts = Enumerable.Range(1, 2).ToArray();

		private readonly Type[] _interfaceMethodReturnTypes =
		{
			typeof(ICounter),
			typeof(IHistogram),
			typeof(IHitPercentageGauge),
			typeof(IMeter),
			typeof(IGauge),
			typeof(ITimer)
		};

		private readonly int[] _interfaceMethodsCounts = Enumerable.Range(0, 3).ToArray();

		private readonly IAttributeDataBuilder _methodAttributeDataBuilder =
			new MetricsCollectorInterfaceMethodAttributeDataBuilder();

		private readonly int[] _methodParameterCounts = Enumerable.Range(0, 3).ToArray();

		private static readonly Type[] _methodParameterTypes =
		{
			typeof(string),
			typeof(char),
			typeof(byte),
			typeof(sbyte),
			//typeof(ushort),
			typeof(short),
			typeof(uint),
			//typeof(int),
			//typeof(ulong),
			//typeof(long),
			typeof(float),
			// typeof(double),
			// typeof(decimal),
			typeof(DateTime),
			typeof(object),
			typeof(Exception),
			typeof(Task)
		};

		private readonly string[] _usingNamespaces = _methodParameterTypes.Select(_=>_.Namespace).Union(new[]
		{
			typeof(Action).Namespace,
			typeof(GenerateMetricsCollectorAttribute).Namespace,
			typeof(ICounter).Namespace
		}).Distinct().ToArray();

		private readonly IParameterValuesBuilder _parameterValuesBuilder = new MetricsCollectorInterfaceParameterValuesBuilder();

		public string[] UsingNamespaces => _usingNamespaces;

		public IAttributeDataBuilder InterfaceAttributeDataBuilder => _interfaceAttributeDataBuilder;

		public IAttributeDataBuilder MethodAttributeDataBuilder => _methodAttributeDataBuilder;

		public Type[] InterfaceMethodReturnTypes => _interfaceMethodReturnTypes;

		public int[] InterfaceCounts => _interfaceCounts;

		public int[] InheritedInterfaceCounts => _inheritedInterfaceCounts;

		public int[] InterfaceMethodsCounts => _interfaceMethodsCounts;

		public int[] MethodParameterCounts => _methodParameterCounts;

		public Type[] MethodParameterTypes => _methodParameterTypes;

		public string InterfaceNamespace => _interfaceNamespace;

		public IInheritanceListBuilder InheritanceListBuilder => _inheritanceListBuilder;

		public IParameterValuesBuilder ParameterValuesBuilder => _parameterValuesBuilder;
	}
}
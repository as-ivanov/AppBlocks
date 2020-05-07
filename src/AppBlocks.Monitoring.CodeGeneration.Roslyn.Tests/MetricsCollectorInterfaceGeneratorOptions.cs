using System;
using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using AppBlocks.Monitoring.Abstractions;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn.Tests
{
	public class MetricsCollectorInterfaceGeneratorOptions : ITestInterfaceGenerationOptions
	{
		private readonly string[] _usingNamespaces =
		{
			typeof(Action).Namespace,
			typeof(AppBlocks.Monitoring.CodeGeneration.Attributes.GenerateMetricsCollectorAttribute).Namespace,
			typeof(ICounter).Namespace
		};

		private readonly Type[] _interfaceMethodReturnTypes =
		{
			typeof(ICounter),
			typeof(IHistogram),
			typeof(IHitPercentageGauge),
			typeof(IMeter),
			typeof(IGauge),
			typeof(ITimer)
		};

		private readonly int[] _interfaceCounts = Enumerable.Range(1, 2).ToArray();
		private readonly int[] _inheritedInterfaceCounts = Enumerable.Range(0, 3).ToArray();
		private readonly int[] _interfaceMethodsCounts = Enumerable.Range(0, 3).ToArray();
		private readonly int[] _methodParameterCounts = Enumerable.Range(0, 3).ToArray();

		private const string _interfaceNamespace = "TestNamespace";

		private readonly IAttributeDataBuilder _interfaceAttributeDataBuilder =
			new MetricsCollectorInterfaceAttributeDataBuilder();

		private readonly IAttributeDataBuilder _methodAttributeDataBuilder =
			new MetricsCollectorInterfaceMethodAttributeDataBuilder();

		private readonly Type[] _methodParameterTypes =
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
			typeof(Exception)
		};

		private readonly IInheritanceListBuilder _inheritanceListBuilder =
			new MetricsCollectorInterfaceInheritanceListBuilder();

		private IParameterValuesBuilder _parameterValuesBuilder = new MetricsCollectorInterfaceParameterValuesBuilder();

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
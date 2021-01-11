using System;
using System.Linq;
using System.Threading.Tasks;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using AppBlocks.Logging.CodeGeneration.Attributes;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerInterfaceGeneratorOptions : ITestInterfaceGenerationOptions
	{
		private const string _interfaceNamespace = "TestNamespace";

		private readonly IInheritanceListBuilder _inheritanceListBuilder = new LoggerInterfaceInheritanceListBuilder();
		private readonly int[] _inheritedInterfaceCounts = Enumerable.Range(0, 3).ToArray();

		private readonly IAttributeDataBuilder _interfaceAttributeDataBuilder = new LoggerInterfaceAttributeDataBuilder();
		private readonly int[] _interfaceCounts = Enumerable.Range(1, 2).ToArray();
		private readonly Type[] _interfaceMethodReturnTypes = {typeof(void)};
		private readonly int[] _interfaceMethodsCounts = Enumerable.Range(0, 3).ToArray();
		private readonly IAttributeDataBuilder _methodAttributeDataBuilder = new LoggerInterfaceMethodAttributeDataBuilder();
		private readonly int[] _methodParameterCounts = Enumerable.Range(0, 3).ToArray();


		private static readonly Type[] _methodParameterTypes =
		{
			typeof(string),
			typeof(char),
			typeof(byte),
			typeof(byte?),
			typeof(short),
			typeof(long),
			typeof(float),
			typeof(decimal),
			typeof(DateTime),
			typeof(object),
			typeof(Exception),
			typeof(Task)
		};

		private readonly IParameterValuesBuilder _parameterValuesBuilder = new LoggerInterfaceParameterValuesBuilder();

		private readonly string[] _usingNamespaces = _methodParameterTypes.Select(_ => _.Namespace).Union(
			new[]
			{
				typeof(Action).Namespace,
				typeof(GenerateLoggerAttribute).Namespace,
				typeof(ILogger).Namespace
			}).Distinct().ToArray();

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
using System;
using System.Linq;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Microsoft.Extensions.Logging;

namespace CodeGeneration.Roslyn.Logger.Tests
{
	public class LoggerInterfaceGeneratorOptions : ITestInterfaceGenerationOptions
	{
		private readonly string[] _usingNamespaces =
		{
			typeof(Action).Namespace,
			typeof(Attributes.LoggerStubAttribute).Namespace,
			typeof(ILogger).Namespace
		};

		private readonly IAttributeDataBuilder _interfaceAttributeDataBuilder = new LoggerInterfaceAttributeDataBuilder();
		private readonly IAttributeDataBuilder _methodAttributeDataBuilder = new LoggerInterfaceMethodAttributeDataBuilder();
		private readonly Type[] _interfaceMethodReturnTypes = new Type[] { typeof(void) };
		private readonly int[] _interfaceCounts = Enumerable.Range(1, 2).ToArray();
		private readonly int[] _inheritedInterfaceCounts = Enumerable.Range(0, 2).ToArray();
		private readonly int[] _interfaceMethodsCounts = Enumerable.Range(0, 3).ToArray();
		private readonly int[] _methodParameterCounts = Enumerable.Range(0, 3).ToArray();


		private readonly Type[] _methodParameterTypes =
		{
			typeof(string),
			typeof(char),
			typeof(byte),
			typeof(short),
			typeof(long),
			typeof(float),
			typeof(decimal),
			typeof(DateTime),
			typeof(object),
			typeof(Exception)
		};

		private const string _interfaceNamespace = "TestNamespace";

		private readonly IInheritanceListBuilder _inheritanceListBuilder = new LoggerInterfaceInheritanceListBuilder();
		private readonly IParameterValuesBuilder _parameterValuesBuilder = new LoggerInterfaceParameterValuesBuilder();

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
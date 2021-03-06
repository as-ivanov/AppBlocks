using System;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public interface ITestInterfaceGenerationOptions
	{
		string[] UsingNamespaces { get; }
		IAttributeDataBuilder InterfaceAttributeDataBuilder { get; }
		IAttributeDataBuilder MethodAttributeDataBuilder { get; }
		IAttributeDataBuilder ParameterAttributeDataBuilder { get; }
		Type[] InterfaceMethodReturnTypes { get; }
		int[] InterfaceCounts { get; }
		int[] InheritedInterfaceCounts { get; }
		int[] InterfaceMethodsCounts { get; }
		int[] MethodParameterCounts { get; }
		Type[] MethodParameterTypes { get; }
		string InterfaceNamespace { get; }
		IInheritanceListBuilder InheritanceListBuilder { get; }
		IParameterValuesBuilder ParameterValuesBuilder { get; }
	}
}

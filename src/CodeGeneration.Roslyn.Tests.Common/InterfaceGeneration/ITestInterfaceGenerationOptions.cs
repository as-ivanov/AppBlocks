using System;

namespace CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public interface ITestInterfaceGenerationOptions
	{
		string[] UsingNamespaces { get; }
		IAttributeDataBuilder InterfaceAttributeDataBuilder { get; }
		IAttributeDataBuilder MethodAttributeDataBuilder { get; }
		Type[] InterfaceMethodReturnTypes { get; }
		int[] InterfaceNumbers { get; }
		int[] InheritedInterfaceNumbers { get; }
		int[] InterfaceMethodsNumbers { get; }
		int[] MethodParameterNumbers { get; }
		Type[] MethodParameterTypes { get; }
		string InterfaceNamespace { get; }
		IInheritanceListBuilder InheritanceListBuilder { get; }
	}
}
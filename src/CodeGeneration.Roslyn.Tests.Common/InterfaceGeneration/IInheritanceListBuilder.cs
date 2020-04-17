using System;

namespace CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public interface IInheritanceListBuilder
	{
		Func<ITestContext, InterfaceData[]> GetInheritedInterfaces(ITestInterfaceGenerationOptions options, int count);
	}
}
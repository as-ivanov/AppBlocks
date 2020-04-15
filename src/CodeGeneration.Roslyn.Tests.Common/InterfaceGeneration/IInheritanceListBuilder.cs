using System;

namespace CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public interface IInheritanceListBuilder
	{
		Func<ITestGenerationContext, InterfaceData[]> GetInheritedInterfaces(ITestInterfaceGenerationOptions options, int count);
	}
}
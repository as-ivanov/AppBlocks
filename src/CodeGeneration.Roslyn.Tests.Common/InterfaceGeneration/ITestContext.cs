using System.Collections.Generic;

namespace CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public interface ITestContext
	{
		void AddCompilationEntry(CompilationEntryData compilationEntryData);
		ITestInterfaceGenerationOptions Options { get; }
		IReadOnlyList<CompilationEntryData> CompilationEntries { get; }
		int NextId();
	}
}
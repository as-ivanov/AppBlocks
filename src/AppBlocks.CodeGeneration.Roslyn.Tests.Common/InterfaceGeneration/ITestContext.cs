using System.Collections.Generic;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public interface ITestContext
	{
		ITestInterfaceGenerationOptions Options { get; }
		IReadOnlyList<CompilationEntryData> CompilationEntries { get; }
		void AddCompilationEntry(CompilationEntryData compilationEntryData);
		int NextId();
	}
}

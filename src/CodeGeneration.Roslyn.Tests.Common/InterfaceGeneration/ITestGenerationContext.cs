using System.Collections.Generic;

namespace CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public interface ITestGenerationContext
	{
		void AddEntry(CompilationEntryData compilationEntryData);
		ITestInterfaceGenerationOptions Options { get; }
		IReadOnlyList<CompilationEntryData> Entries { get; }
	}
}
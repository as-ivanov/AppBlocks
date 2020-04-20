using System;
using System.Collections.Generic;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public class TestContext : ITestContext
	{
		private readonly ITestInterfaceGenerationOptions _options;
		private readonly List<CompilationEntryData> _entries = new List<CompilationEntryData>();
		private int _currentId = 1;

		public TestContext(ITestInterfaceGenerationOptions options)
		{
			_options = options;
		}

		public void AddCompilationEntry(CompilationEntryData compilationEntryData)
		{
			_entries.Add(compilationEntryData);
		}

		public ITestInterfaceGenerationOptions Options => _options;

		public IReadOnlyList<CompilationEntryData> CompilationEntries => _entries;
		public int NextId()
		{
			return _currentId++;
		}

		public override string ToString()
		{
			return string.Join(Environment.NewLine, CompilationEntries);
		}
	}
}
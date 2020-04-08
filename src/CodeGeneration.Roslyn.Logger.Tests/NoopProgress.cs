using System;
using Microsoft.CodeAnalysis;

namespace CodeGeneration.Roslyn.Logger.Tests
{
	public class NoopProgress : IProgress<Diagnostic>
	{
		public void Report(Diagnostic value)
		{
		}
	}
}
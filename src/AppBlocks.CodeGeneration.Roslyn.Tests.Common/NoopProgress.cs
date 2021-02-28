using System;
using Microsoft.CodeAnalysis;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common
{
	public class NoopProgress : IProgress<Diagnostic>
	{
		public void Report(Diagnostic value)
		{
		}
	}
}

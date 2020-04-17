using System;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CodeGeneration.Roslyn.Tests.Common
{
	public class NoopProgress : IProgress<Diagnostic>
	{
		public void Report(Diagnostic value)
		{
		}
	}
}
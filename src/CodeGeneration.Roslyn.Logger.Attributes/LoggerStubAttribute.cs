using System;
using System.Diagnostics;

namespace CodeGeneration.Roslyn.Logger.Attributes
{
	[AttributeUsage(AttributeTargets.Interface)]
	[Conditional("CodeGeneration")]
	[CodeGenerationAttribute("CodeGeneration.Roslyn.Logger.LoggerClassGenerator, CodeGeneration.Roslyn.Logger")]
	public class LoggerStubAttribute : Attribute
	{
		public LoggerStubAttribute(params string[] inheritedInterfaceTypes)
		{
		}
	}
}
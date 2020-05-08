using System;
using System.Diagnostics;
using AppBlocks.CodeGeneration.Attributes.Common;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	[Conditional(CodeGenerationAttributesConsts.CodeGenerationConditionName)]
	public class LogOptionsAttribute : Attribute
	{
		public LogLevel Level { get; set; }

		public string Message { get; set; }
	}
}
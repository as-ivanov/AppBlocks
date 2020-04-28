using System;
using System.Diagnostics;
using AppBlocks.CodeGeneration.Attributes.Common;
using AppBlocks.Logging.CodeGeneration.Attributes;

namespace AppBlocks.Logging.CodeGeneration.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	[Conditional(CodeGenerationAttributesConsts.CodeGenerationConditionName)]
	public class LogOptionsAttribute : Attribute
	{
		public Microsoft.Extensions.Logging.LogLevel Level { get; set; }

		public string Message { get; set; }
	}
}

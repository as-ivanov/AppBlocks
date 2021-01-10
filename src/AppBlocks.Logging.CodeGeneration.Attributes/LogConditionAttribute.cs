using System;
using System.Diagnostics;
using AppBlocks.CodeGeneration.Attributes.Common;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter)]
	[Conditional(CodeGenerationAttributesConsts.CodeGenerationConditionName)]
	public class LogConditionAttribute : Attribute
	{
		public LogLevel MinLevel { get; set; }
	}
}
using System;
using System.Diagnostics;
using AppBlocks.CodeGeneration.Attributes.Common;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	[Conditional(CodeGenerationAttributesConsts.CodeGenerationConditionName)]
	public class LogConditionAttribute : Attribute
	{
		internal static readonly string DefaultPlaceholder = "{...}";
		public LogLevel MinLevel { get; set; }
		public string Placeholder { get; set; } = DefaultPlaceholder;
	}
}
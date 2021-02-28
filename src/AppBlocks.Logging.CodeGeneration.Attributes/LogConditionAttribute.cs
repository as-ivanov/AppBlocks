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
		private readonly LogLevel _minLevel;

		public LogConditionAttribute(LogLevel minLevel)
		{
			_minLevel = minLevel;
		}

		public LogLevel MinLevel => _minLevel;
	}
}

using System;
using System.Diagnostics;
using AppBlocks.CodeGeneration.Attributes.Common;

namespace AppBlocks.Logging.CodeGeneration.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	[Conditional(CodeGenerationAttributesConsts.CodeGenerationConditionName)]
	public class LoggerMethodStubAttribute : Attribute
	{
		private readonly Microsoft.Extensions.Logging.LogLevel _logLevel;
		private readonly string _message;

		public LoggerMethodStubAttribute(Microsoft.Extensions.Logging.LogLevel logLevel) : this(logLevel, null)
		{
		}

		public LoggerMethodStubAttribute(Microsoft.Extensions.Logging.LogLevel logLevel, string message)
		{
			_logLevel = logLevel;
			_message = message;
		}

		public Microsoft.Extensions.Logging.LogLevel Level => _logLevel;

		public string Message => _message;
	}
}
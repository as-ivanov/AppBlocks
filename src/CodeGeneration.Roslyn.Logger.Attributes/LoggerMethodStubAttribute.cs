using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CodeGeneration.Roslyn.Logger.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	[Conditional("CodeGeneration")]
	public class LoggerMethodStubAttribute : Attribute
	{
		private readonly LogLevel _logLevel;
		private readonly string _message;

		public LoggerMethodStubAttribute(LogLevel logLevel, string message = null)
		{
			_logLevel = logLevel;
			_message = message ?? string.Empty;
		}

		public LogLevel Level => _logLevel;

		public string Message => _message;
	}
}
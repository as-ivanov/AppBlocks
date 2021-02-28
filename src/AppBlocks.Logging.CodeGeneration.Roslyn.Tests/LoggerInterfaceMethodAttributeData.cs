using AppBlocks.CodeGeneration.Roslyn.Tests.Common;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using AppBlocks.Logging.CodeGeneration.Attributes;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerInterfaceMethodAttributeData : AttributeData
	{
		private readonly LogLevel _logLevel;
		private readonly bool _logLevelIsDefined;
		private readonly string _message;
		private readonly bool _messageIsDefined;

		private LoggerInterfaceMethodAttributeData(LogLevel logLevel, bool logLevelIsDefined,
			string message, bool messageIsDefined) :
			base(
				nameof(LogOptionsAttribute))
		{
			_logLevel = logLevel;
			_logLevelIsDefined = logLevelIsDefined;
			_message = message;
			_messageIsDefined = messageIsDefined;
		}

		public LogLevel Level => _logLevel;

		public string Message => _message;

		public bool MessageIsDefined => _messageIsDefined;

		public bool LogLevelIsDefined => _logLevelIsDefined;

		public static LoggerInterfaceMethodAttributeData Create(LogLevel logLevel,
			string message)
		{
			return new LoggerInterfaceMethodAttributeData(logLevel, true, message, true);
		}

		public static LoggerInterfaceMethodAttributeData Create(LogLevel logLevel)
		{
			return new LoggerInterfaceMethodAttributeData(logLevel, true, null, false);
		}

		public static LoggerInterfaceMethodAttributeData Create(string message)
		{
			return new LoggerInterfaceMethodAttributeData(LogLevel.Information, false, message, true);
		}

		public override string ToString()
		{
			var paramSb = new AttributeNamedParameterStringBuilder();
			if (LogLevelIsDefined)
			{
				paramSb.Append(nameof(LogOptionsAttribute.Level), $"LogLevel.{Level}");
			}

			if (MessageIsDefined)
			{
				paramSb.Append(nameof(LogOptionsAttribute.Message), Message == null ? "null" : $"\"{Message}\"");
			}

			return $"[{Name}({paramSb})]";
		}
	}
}

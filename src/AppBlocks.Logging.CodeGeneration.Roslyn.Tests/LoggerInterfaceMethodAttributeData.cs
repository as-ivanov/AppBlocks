using System.Text;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerInterfaceMethodAttributeData : AttributeData
	{
		private readonly Microsoft.Extensions.Logging.LogLevel _logLevel;
		private readonly bool _logLevelIsDefined;
		private readonly string _message;
		private readonly bool _messageIsDefined;
		public static LoggerInterfaceMethodAttributeData Create(Microsoft.Extensions.Logging.LogLevel logLevel,
			string message)
		{
			return new LoggerInterfaceMethodAttributeData(logLevel, true, message, true);
		}

		public static LoggerInterfaceMethodAttributeData Create(Microsoft.Extensions.Logging.LogLevel logLevel)
		{
			return new LoggerInterfaceMethodAttributeData(logLevel, true, null, false);
		}

		public static LoggerInterfaceMethodAttributeData Create(string message)
		{
			return new LoggerInterfaceMethodAttributeData(Microsoft.Extensions.Logging.LogLevel.Information, false, message, true);
		}

		private LoggerInterfaceMethodAttributeData(Microsoft.Extensions.Logging.LogLevel logLevel, bool logLevelIsDefined,
			string message, bool messageIsDefined) :
			base(
				nameof(Attributes.LogOptionsAttribute))
		{
			_logLevel = logLevel;
			_logLevelIsDefined = logLevelIsDefined;
			_message = message;
			_messageIsDefined = messageIsDefined;
		}

		public Microsoft.Extensions.Logging.LogLevel Level => _logLevel;

		public string Message => _message;

		public bool MessageIsDefined => _messageIsDefined;

		public bool LogLevelIsDefined => _logLevelIsDefined;

		public override string ToString()
		{
			var paramSb = new AttributeNamedParameterStringBuilder();
			if (LogLevelIsDefined)
			{
				paramSb.Append(nameof(Attributes.LogOptionsAttribute.Level),  $"LogLevel.{Level}");
			}
			if (MessageIsDefined)
			{
				paramSb.Append(nameof(Attributes.LogOptionsAttribute.Message),  Message == null ? "null" : $"\"{Message}\"");
			}
			return $"[{Name}({paramSb})]";
		}
	}
}
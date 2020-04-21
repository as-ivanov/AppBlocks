using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerInterfaceMethodAttributeData : AttributeData
	{
		private readonly Microsoft.Extensions.Logging.LogLevel _logLevel;
		private readonly string _message;
		private readonly bool _messageIsDefined;

		public LoggerInterfaceMethodAttributeData(Microsoft.Extensions.Logging.LogLevel logLevel) : this(logLevel, null, false)
		{
		}

		public LoggerInterfaceMethodAttributeData(Microsoft.Extensions.Logging.LogLevel logLevel, string message) : this(logLevel, message, true)
		{
		}

		private LoggerInterfaceMethodAttributeData(Microsoft.Extensions.Logging.LogLevel logLevel, string message, bool messageIsDefined) :
			base(
				nameof(Attributes.LoggerMethodStubAttribute))
		{
			_logLevel = logLevel;
			_message = message;
			_messageIsDefined = messageIsDefined;
		}

		public Microsoft.Extensions.Logging.LogLevel Level => _logLevel;

		public string Message => _message;

		public bool MessageIsDefined => _messageIsDefined;

		public override string ToString()
		{
			var messageParameter = Message == null ? "" : $",\"{Message}\"";
			return $"[{Name}(LogLevel.{Level}{messageParameter})]";
		}
	}
}
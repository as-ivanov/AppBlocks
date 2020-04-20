using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerInterfaceMethodAttributeData : AttributeData
	{
		private readonly Microsoft.Extensions.Logging.LogLevel _logLevel;
		private readonly string _message;

		public LoggerInterfaceMethodAttributeData(Microsoft.Extensions.Logging.LogLevel logLevel, string message = null) :
			base(
				nameof(Attributes.LoggerMethodStubAttribute))
		{
			_logLevel = logLevel;
			_message = message;
		}

		public Microsoft.Extensions.Logging.LogLevel Level => _logLevel;

		public string Message => _message;

		public override string ToString()
		{
			var messageParameter = Message == null ? "" : $",\"{Message}\"";
			return $"[{Name}(LogLevel.{Level}{messageParameter})]";
		}
	}
}
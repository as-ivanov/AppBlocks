using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using AppBlocks.Logging.CodeGeneration.Attributes;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LogConditionAttributeData : AttributeData
	{
		private readonly LogLevel _minLogLevel;

		private LogConditionAttributeData(LogLevel minLogLevel) : base(nameof(LogConditionAttribute))
		{
			_minLogLevel = minLogLevel;
		}
		public LogLevel MinLogLevel => _minLogLevel;

		public static AttributeData Create(LogLevel minLogLevel)
		{
			return new LogConditionAttributeData(minLogLevel);
		}

		public override string ToString()
		{
			return $"[{Name}(LogLevel.{MinLogLevel})]";
		}
	}
}
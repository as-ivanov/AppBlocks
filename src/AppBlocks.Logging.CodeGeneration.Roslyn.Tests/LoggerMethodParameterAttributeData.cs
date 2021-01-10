using AppBlocks.CodeGeneration.Roslyn.Tests.Common;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using AppBlocks.Logging.CodeGeneration.Attributes;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerMethodParameterAttributeData : AttributeData
	{
		private readonly LogLevel _minLogLevel;

		private LoggerMethodParameterAttributeData(LogLevel minLogLevel) : base(nameof(LogConditionAttribute))
		{
			_minLogLevel = minLogLevel;
		}

		public static AttributeData Create(LogLevel minLogLevel)
		{
			return new LoggerMethodParameterAttributeData(minLogLevel);
		}

		public override string ToString()
		{
			var paramSb = new AttributeNamedParameterStringBuilder();
			paramSb.Append(nameof(LogConditionAttribute.MinLevel), $"LogLevel.{_minLogLevel}");
			return $"[{Name}({paramSb})]";
		}
	}
}
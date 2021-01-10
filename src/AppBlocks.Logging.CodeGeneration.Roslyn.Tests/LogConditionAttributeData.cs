using System;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using AppBlocks.Logging.CodeGeneration.Attributes;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LogConditionAttributeData : AttributeData
	{
		private readonly LogLevel _minLogLevel;
		private readonly string _placeholder;
		private readonly bool _hasPlaceholder;

		private LogConditionAttributeData(LogLevel minLogLevel, string placeholder = null) : base(nameof(LogConditionAttribute))
		{
			_minLogLevel = minLogLevel;
			if (placeholder == null)
			{
				_hasPlaceholder = false;
				_placeholder = LogConditionAttribute.DefaultPlaceholder;
			}
			else
			{
				_placeholder = placeholder;
				_hasPlaceholder = true;
			}
		}

		public LogLevel MinLogLevel => _minLogLevel;

		public string Placeholder => _placeholder;

		public bool HasPlaceholder { get => _hasPlaceholder;}

		public static AttributeData Create(LogLevel minLogLevel)
		{
			return new LogConditionAttributeData(minLogLevel);
		}

		public static AttributeData Create(LogLevel minLogLevel, string placeholder)
		{
			return new LogConditionAttributeData(minLogLevel, placeholder);
		}

		public override string ToString()
		{
			var paramSb = new AttributeNamedParameterStringBuilder();
			paramSb.Append(nameof(LogConditionAttribute.MinLevel), $"LogLevel.{MinLogLevel}");
			if (HasPlaceholder)
			{
				paramSb.Append(nameof(LogConditionAttribute.Placeholder), $"{Placeholder}");
			}
			return $"[{Name}({paramSb})]";
		}
	}
}
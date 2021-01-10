using System;
using System.Globalization;
using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public static class MethodParameterDataExtension
	{
		public static string GetFormattedValue(this MethodParameterData methodParameterData, LogLevel loggerLevel)
		{
			if (methodParameterData.Value == null)
			{
				return string.Empty;
			}

			var logConditionAttributeData = methodParameterData.AttributeDataList.OfType<LogConditionAttributeData>().FirstOrDefault();
			if (logConditionAttributeData != null && logConditionAttributeData.MinLogLevel <= loggerLevel)
			{
				return logConditionAttributeData.Placeholder;
			}

			if (methodParameterData.Value is DateTime dateTime)
			{
				return dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
			}

			return methodParameterData.Value.ToString();
		}
	}
}
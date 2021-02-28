using System;
using System.Globalization;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public static class MethodParameterDataExtension
	{
		public static string GetFormattedValue(this MethodParameterData methodParameterData)
		{
			if (methodParameterData.Value == null)
			{
				return string.Empty;
			}

			if (methodParameterData.Value is DateTime dateTime)
			{
				return dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
			}

			return methodParameterData.Value.ToString();
		}
	}
}

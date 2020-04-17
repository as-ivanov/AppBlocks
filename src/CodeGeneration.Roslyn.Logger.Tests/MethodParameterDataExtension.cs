using System;
using System.Globalization;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace CodeGeneration.Roslyn.Logger.Tests
{
	public static class MethodParameterDataExtension
	{
		public static string GetFormattedValue(this MethodParameterData methodParameterData)
		{
				if (methodParameterData.Value == null)
				{
					return "(null)";
				}
				if (methodParameterData.Value is DateTime dateTime)
				{
					return dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
				}
				return methodParameterData.Value.ToString();
		}
	}
}
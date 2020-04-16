using System;
using System.Collections.Generic;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace CodeGeneration.Roslyn.Logger.Tests
{
	public class LoggerInterfaceMethodAttributeDataBuilder : IAttributeDataBuilder
	{
		public IEnumerable<Func<ITestGenerationContext, IEnumerable<AttributeData>>> GetPossibleCombinations(
			ITestInterfaceGenerationOptions options)
		{
			yield return c => { return new AttributeData[0]; };
			yield return c =>
			{
				var message = Guid.NewGuid().ToString();
				return new AttributeData[]
					{new LoggerInterfaceMethodAttributeData(Microsoft.Extensions.Logging.LogLevel.Warning, message)};
			};
			yield return c =>
			{
				return new AttributeData[]
					{new LoggerInterfaceMethodAttributeData(Microsoft.Extensions.Logging.LogLevel.Warning)};
			};
		}
	}
}
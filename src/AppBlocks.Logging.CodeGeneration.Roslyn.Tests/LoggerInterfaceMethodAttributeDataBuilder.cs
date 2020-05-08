using System;
using System.Collections.Generic;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerInterfaceMethodAttributeDataBuilder : IAttributeDataBuilder
	{
		public IEnumerable<Func<ITestContext, IEnumerable<AttributeData>>> GetPossibleCombinations(
			ITestInterfaceGenerationOptions options)
		{
			yield return c => { return new AttributeData[0]; };
			yield return c =>
			{
				var message = Guid.NewGuid().ToString();
				return new AttributeData[]
					{LoggerInterfaceMethodAttributeData.Create(LogLevel.Warning, message)};
			};
			yield return c =>
			{
				return new AttributeData[]
					{LoggerInterfaceMethodAttributeData.Create(LogLevel.Warning)};
			};
			yield return c =>
			{
				var message = Guid.NewGuid().ToString();
				return new AttributeData[]
					{LoggerInterfaceMethodAttributeData.Create(message)};
			};
		}
	}
}
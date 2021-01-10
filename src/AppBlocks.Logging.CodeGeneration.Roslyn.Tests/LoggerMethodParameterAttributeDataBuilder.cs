using System;
using System.Collections.Generic;
using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerMethodParameterAttributeDataBuilder : IAttributeDataBuilder
	{
		public IEnumerable<Func<ITestContext, IEnumerable<AttributeData>>> GetPossibleCombinations(
			ITestInterfaceGenerationOptions options)
		{
			yield return c => { return new AttributeData[0]; };
			var logLevels = Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>().ToArray();
			foreach (var logLevel in logLevels)
			{
				yield return c => { return new[]
					{LoggerMethodParameterAttributeData.Create(logLevel)}; };
			}
		}
	}
}
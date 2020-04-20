using System;
using System.Collections.Generic;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn.Tests
{
	public class MetricsCollectorInterfaceMethodAttributeDataBuilder : IAttributeDataBuilder
	{
		public IEnumerable<Func<ITestContext, IEnumerable<AttributeData>>> GetPossibleCombinations(ITestInterfaceGenerationOptions options)
		{
			yield return c => { return new AttributeData[0]; };
			yield return c =>
			{
				var metricName = "Metric" + Guid.NewGuid();
				var unitName = "Unit" + Guid.NewGuid();
				return new AttributeData[] {new MetricsCollectorMethodAttributeData(metricName, unitName)};
			};
			yield return c =>
			{
				var metricName = "Metric" + Guid.NewGuid();
				return new AttributeData[] {new MetricsCollectorMethodAttributeData(metricName, null)};
			};

			yield return c =>
			{
				var unitName = "Unit" + Guid.NewGuid();
				return new AttributeData[] {new MetricsCollectorMethodAttributeData(null, unitName)};
			};
		}
	}
}
using System;
using System.Collections.Generic;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class MetricsCollectorInterfaceMethodAttributeDataDataBuilder : IAttributeDataBuilder
	{
		public IEnumerable<Func<ITestGenerationContext, IEnumerable<AttributeData>>> GetCombinations(ITestInterfaceGenerationOptions options)
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
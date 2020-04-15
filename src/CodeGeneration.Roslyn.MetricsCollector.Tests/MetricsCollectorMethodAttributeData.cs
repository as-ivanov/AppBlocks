using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class MetricsCollectorMethodAttributeData : AttributeData
	{
		private readonly string _metricName;
		private readonly string _unit;

		public MetricsCollectorMethodAttributeData(string metricName = null, string unit = null) : base(
			nameof(Attributes.MetricsCollectorMethodStubAttribute))
		{
			_metricName = metricName;
			_unit = unit;
		}

		public override string ToString()
		{
			var metricNameParameter = "metricName = " + (_metricName == null ? "null" : $"\"{_metricName}\"");
			var unitNameParameter = "unitName = " + (_unit == null ? "null" : $"\"{_unit}\"");
			return $"[{Name}({metricNameParameter}, {unitNameParameter})]";
		}
	}
}
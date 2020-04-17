using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class MetricsCollectorMethodAttributeData : AttributeData
	{
		private readonly string _metricName;
		private readonly string _measurementUnitName;

		public MetricsCollectorMethodAttributeData(string metricName = null, string measurementUnitName = null) : base(
			nameof(Attributes.MetricsCollectorMethodStubAttribute))
		{
			_metricName = metricName;
			_measurementUnitName = measurementUnitName;
		}

		public string MetricName => _metricName;

		public string MeasurementUnitName => _measurementUnitName;

		public override string ToString()
		{
			var metricNameParameter = "metricName: " + (MetricName == null ? "null" : $"\"{MetricName}\"");
			var unitNameParameter = "measurementUnitName: " + (MeasurementUnitName == null ? "null" : $"\"{MeasurementUnitName}\"");
			return $"[{Name}({metricNameParameter}, {unitNameParameter})]";
		}
	}
}
using System.Text;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn.Tests
{
	public class MetricsCollectorMethodAttributeData : AttributeData
	{
		private readonly string _metricName;
		private readonly bool _isMetricNameSet;
		private readonly string _measurementUnitName;
		private readonly bool _isMeasurementUnitNameSet;

		public static MetricsCollectorMethodAttributeData Create(string metricName, string measurementUnitName)
		{
			return new MetricsCollectorMethodAttributeData(metricName, true, measurementUnitName, true);
		}

		public static MetricsCollectorMethodAttributeData CreateWithMetricNameOnly(string metricName)
		{
			return new MetricsCollectorMethodAttributeData(metricName, true, null, false);
		}

		public static MetricsCollectorMethodAttributeData CreateWithMeasurementUnitNameOnly(string measurementUnitName)
		{
			return new MetricsCollectorMethodAttributeData(null, false, measurementUnitName, true);
		}

		private MetricsCollectorMethodAttributeData(string metricName, bool isMetricNameSet, string measurementUnitName,bool isMeasurementUnitNameSet) : base(
			nameof(global::AppBlocks.Monitoring.CodeGeneration.Attributes.MetricsCollectorMethodStubAttribute))
		{
			_metricName = metricName;
			_isMetricNameSet = isMetricNameSet;
			_measurementUnitName = measurementUnitName;
			_isMeasurementUnitNameSet = isMeasurementUnitNameSet;
		}

		public string MetricName => _metricName;

		public string MeasurementUnitName => _measurementUnitName;

		public bool IsMetricNameSet => _isMetricNameSet;

		public bool IsMeasurementUnitNameSet => _isMeasurementUnitNameSet;

		public override string ToString()
		{
			var metricNameParameter = $"{nameof(Attributes.MetricsCollectorMethodStubAttribute.MetricName)} = " + (MetricName == null ? "null" : $"\"{MetricName}\"");
			var unitNameParameter = $"{nameof(Attributes.MetricsCollectorMethodStubAttribute.MeasurementUnitName)} = " + (MeasurementUnitName == null ? "null" : $"\"{MeasurementUnitName}\"");
			var paramSb = new StringBuilder();
			if (IsMetricNameSet)
			{
				paramSb.Append(metricNameParameter);
			}
			if (IsMeasurementUnitNameSet)
			{
				if (paramSb.Length > 0)
				{
					paramSb.Append(", ");
				}
				paramSb.Append(unitNameParameter);
			}
			return $"[{Name}({paramSb})]";
		}
	}
}
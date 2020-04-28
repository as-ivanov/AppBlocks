using System.Text;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn.Tests
{
	public class MetricsCollectorMethodAttributeData : AttributeData
	{
		private readonly string _metricName;
		private readonly bool _isMetricNameDefined;
		private readonly string _measurementUnitName;
		private readonly bool _isMeasurementUnitNameDefined;

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

		private MetricsCollectorMethodAttributeData(string metricName, bool isMetricNameDefined, string measurementUnitName,bool isMeasurementUnitNameDefined) : base(
			nameof(Attributes.MetricOptionsAttribute))
		{
			_metricName = metricName;
			_isMetricNameDefined = isMetricNameDefined;
			_measurementUnitName = measurementUnitName;
			_isMeasurementUnitNameDefined = isMeasurementUnitNameDefined;
		}

		public string MetricName => _metricName;

		public string MeasurementUnitName => _measurementUnitName;

		public bool IsMetricNameDefined => _isMetricNameDefined;

		public bool IsMeasurementUnitNameDefined => _isMeasurementUnitNameDefined;

		public override string ToString()
		{
			var paramSb = new AttributeNamedParameterStringBuilder();
			if (IsMetricNameDefined)
			{
				paramSb.Append(nameof(Attributes.MetricOptionsAttribute.MetricName), MetricName == null ? "null" : $"\"{MetricName}\"");
			}
			if (IsMeasurementUnitNameDefined)
			{
				paramSb.Append(nameof(Attributes.MetricOptionsAttribute.MeasurementUnitName), MeasurementUnitName == null ? "null" : $"\"{MeasurementUnitName}\"");
			}
			return $"[{Name}({paramSb})]";
		}
	}
}
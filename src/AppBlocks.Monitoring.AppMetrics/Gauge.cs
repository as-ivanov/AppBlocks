using App.Metrics;
using App.Metrics.Gauge;
using IGauge = AppBlocks.Monitoring.Abstractions.IGauge;

namespace AppBlocks.Monitoring.AppMetrics
{
	public class Gauge : IGauge
	{
		private readonly GaugeOptions _gaugeOptions;
		private readonly IMetrics _metrics;
		private readonly MetricTags _metricTags;

		public Gauge(IMetrics metrics, GaugeOptions gaugeOptions, in MetricTags metricTags)
		{
			_metrics = metrics;
			_gaugeOptions = gaugeOptions;
			_metricTags = metricTags;
		}

		public void SetValue(double value)
		{
			_metrics.Measure.Gauge.SetValue(_gaugeOptions, _metricTags, value);
		}
	}
}
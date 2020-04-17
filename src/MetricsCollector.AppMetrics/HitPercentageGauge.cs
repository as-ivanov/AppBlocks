using App.Metrics;
using App.Metrics.Gauge;
using App.Metrics.Meter;
using MetricsCollector.Abstractions;
using IMeter = MetricsCollector.Abstractions.IMeter;

namespace MetricsCollector.AppMetrics
{
	public class HitPercentageGauge : IHitPercentageGauge
	{
		private readonly GaugeOptions _gaugeOptions;
		private readonly IMetrics _metrics;
		private readonly MetricTags _metricTags;

		public HitPercentageGauge(IMetrics metrics, GaugeOptions gaugeOptions, in MetricTags metricTags)
		{
			_metrics = metrics;
			_gaugeOptions = gaugeOptions;
			_metricTags = metricTags;
		}


		public void Ratio(IMeter numerator, IMeter denominator)
		{
			var internalNumerator = _metrics.Provider.Meter.Instance((numerator as Meter)?.Options, (numerator as Meter).Tags);
			var denominatorNumerator = _metrics.Provider.Meter.Instance((denominator as Meter)?.Options, (denominator as Meter).Tags);

			IMetricValueProvider<double> ValueProvider()
			{
				static double MeterRateFunc(MeterValue value)
				{
					return value.OneMinuteRate;
				}

				return new App.Metrics.Gauge.HitPercentageGauge(internalNumerator, denominatorNumerator, MeterRateFunc);
			}

			_metrics.Measure.Gauge.SetValue(_gaugeOptions, _metricTags, ValueProvider);
		}
	}
}
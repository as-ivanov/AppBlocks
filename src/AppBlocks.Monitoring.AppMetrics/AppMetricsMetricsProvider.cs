using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Gauge;
using App.Metrics.Histogram;
using App.Metrics.Meter;
using App.Metrics.Timer;
using AppBlocks.Monitoring.Abstractions;
using ICounter = AppBlocks.Monitoring.Abstractions.ICounter;
using IGauge = AppBlocks.Monitoring.Abstractions.IGauge;
using IHistogram = AppBlocks.Monitoring.Abstractions.IHistogram;
using IMeter = AppBlocks.Monitoring.Abstractions.IMeter;
using ITimer = AppBlocks.Monitoring.Abstractions.ITimer;

namespace AppBlocks.Monitoring.AppMetrics
{
	public class AppMetricsMetricsProvider : IMetricsProvider
	{
		private readonly IMetrics _metrics;

		public AppMetricsMetricsProvider(IMetrics metrics)
		{
			_metrics = metrics;
		}

		public ICounter CreateCounter(string contextName, string metricName, string measurementUnitName, in Tags tags)
		{
			var counterOptions = new CounterOptions
			{
				Context = contextName,
				Name = metricName,
				MeasurementUnit = measurementUnitName
			};
			return new Counter(_metrics, counterOptions, tags.ToMetricsTags());
		}

		public IHistogram CreateHistogram(string contextName, string metricName, string measurementUnitName, in Tags tags)
		{
			var histogramOptions = new HistogramOptions
			{
				Context = contextName,
				Name = metricName,
				MeasurementUnit = measurementUnitName
			};
			return new Histogram(_metrics, histogramOptions, tags.ToMetricsTags());
		}

		public IHitPercentageGauge CreateHitPercentageGauge(string contextName, string metricName, string measurementUnitName, in Tags tags)
		{
			var gaugeOptions = new GaugeOptions
			{
				Context = contextName,
				Name = metricName,
				MeasurementUnit = measurementUnitName
			};
			return new HitPercentageGauge(_metrics, gaugeOptions, tags.ToMetricsTags());
		}

		public IMeter CreateMeter(string contextName, string metricName, string measurementUnitName, in Tags tags)
		{
			var meterOptions = new MeterOptions
			{
				Context = contextName,
				Name = metricName,
				MeasurementUnit = measurementUnitName
			};
			return new Meter(_metrics, meterOptions, tags.ToMetricsTags());
		}

		public ITimer CreateTimer(string contextName, string metricName, string measurementUnitName, in Tags tags)
		{
			var timerOptions = new TimerOptions
			{
				Context = contextName,
				Name = metricName,
				MeasurementUnit = measurementUnitName
			};
			return new Timer(_metrics, timerOptions, tags.ToMetricsTags());
		}

		public IGauge CreateGauge(string contextName, string metricName, string measurementUnitName, in Tags tags)
		{
			var gaugeOptions = new GaugeOptions
			{
				Context = contextName,
				Name = metricName,
				MeasurementUnit = measurementUnitName
			};
			return new Gauge(_metrics, gaugeOptions, tags.ToMetricsTags());
		}
	}
}
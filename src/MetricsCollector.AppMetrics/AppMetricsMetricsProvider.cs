using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Gauge;
using App.Metrics.Histogram;
using App.Metrics.Meter;
using App.Metrics.Timer;
using MetricsCollector.Abstractions;
using ICounter = MetricsCollector.Abstractions.ICounter;
using IGauge = MetricsCollector.Abstractions.IGauge;
using IHistogram = MetricsCollector.Abstractions.IHistogram;
using IMeter = MetricsCollector.Abstractions.IMeter;
using ITimer = MetricsCollector.Abstractions.ITimer;

namespace MetricsCollector.AppMetrics
{
	public class AppMetricsMetricsProvider : IMetricsProvider
	{
		private readonly IMetrics _metrics;

		public AppMetricsMetricsProvider(IMetrics metrics)
		{
			_metrics = metrics;
		}

		public IMeter CreateMeter(string contextName, string name, in Tags tags)
		{
			var meterOptions = new MeterOptions
			{
				Context = contextName,
				Name = name
			};
			return new Meter(_metrics, meterOptions, tags.ToMetricsTags());
		}

		public ICounter CreateCounter(string contextName, string name, string metricUnit, in Tags tags)
		{
			var counterOptions = new CounterOptions
			{
				Context = contextName,
				Name = name
			};
			return new Counter(_metrics, counterOptions, tags.ToMetricsTags());
		}

		public IHistogram CreateHistogram(string contextName, string name, string metricUnit, in Tags tags)
		{
			var histogramOptions = new HistogramOptions
			{
				Context = contextName,
				Name = name
			};
			return new Histogram(_metrics, histogramOptions, tags.ToMetricsTags());
		}

		public IHitPercentageGauge CreateHitPercentageGauge(string contextName, string name, string metricUnit, in Tags tags)
		{
			var gaugeOptions = new GaugeOptions
			{
				Context = contextName,
				Name = name
			};
			return new HitPercentageGauge(_metrics, gaugeOptions, tags.ToMetricsTags());
		}

		public IMeter CreateMeter(string contextName, string name, string metricUnit, in Tags tags)
		{
			var meterOptions = new MeterOptions
			{
				Context = contextName,
				Name = name
			};
			return new Meter(_metrics, meterOptions, tags.ToMetricsTags());
		}

		public ITimer CreateTimer(string contextName, string name, string metricUnit, in Tags tags)
		{
			var timerOptions = new TimerOptions
			{
				Context = contextName,
				Name = name
			};
			return new Timer(_metrics, timerOptions, tags.ToMetricsTags());
		}

		public IGauge CreateGauge(string contextName, string name, string metricUnit, in Tags tags)
		{
			var gaugeOptions = new GaugeOptions
			{
				Context = contextName,
				Name = name
			};
			return new Gauge(_metrics, gaugeOptions, tags.ToMetricsTags());
		}

		public bool IsEnabled(string contextName, string name)
		{
			return true;
		}
	}
}
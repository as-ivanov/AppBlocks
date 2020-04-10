using App.Metrics;
using App.Metrics.Meter;
using MetricsCollector.Abstractions;
using IMeter = MetricsCollector.Abstractions.IMeter;

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
			return new Meter(_metrics.Provider.Meter, _metrics.Measure.Meter, meterOptions, tags.ToMetricsTags());
		}

		public bool IsEnabled(string contextName, string name)
		{
			return true;
		}
	}
}
using App.Metrics;
using App.Metrics.Counter;
using ICounter = AppBlocks.Monitoring.Abstractions.ICounter;

namespace AppBlocks.Monitoring.AppMetrics
{
	public class Counter : ICounter
	{
		private readonly CounterOptions _counterOptions;
		private readonly IMetrics _metrics;
		private readonly MetricTags _metricTags;

		public Counter(IMetrics metrics,
			CounterOptions counterOptions, in MetricTags metricTags)
		{
			_metrics = metrics;
			_counterOptions = counterOptions;
			_metricTags = metricTags;
		}


		public void Decrement(long decrement = 1)
		{
			_metrics.Measure.Counter.Decrement(_counterOptions, _metricTags, decrement);
		}

		public void Increment(long increment = 1)
		{
			_metrics.Measure.Counter.Increment(_counterOptions, _metricTags, increment);
		}

		public void Reset()
		{
			_metrics.Provider.Counter.Instance(_counterOptions, _metricTags).Reset();
		}
	}
}
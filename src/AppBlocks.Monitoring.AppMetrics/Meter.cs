using App.Metrics;
using App.Metrics.Meter;
using IMeter = AppBlocks.Monitoring.Abstractions.IMeter;

namespace AppBlocks.Monitoring.AppMetrics
{
	internal class Meter : IMeter
	{
		private readonly IMetrics _metrics;
		private readonly MeterOptions _options;
		private readonly MetricTags _tags;

		public Meter(IMetrics metrics, in MeterOptions meterOptions, in MetricTags metricTags)
		{
			_metrics = metrics;
			_options = meterOptions;
			_tags = metricTags;
		}

		public MeterOptions Options => _options;

		public MetricTags Tags => _tags;

		public void Mark()
		{
			_metrics.Measure.Meter.Mark(Options, Tags);
		}

		public void Mark(string item)
		{
			_metrics.Measure.Meter.Mark(Options, Tags, item);
		}

		public void Mark(long amount)
		{
			_metrics.Measure.Meter.Mark(Options, Tags, amount);
		}

		public void Reset()
		{
			_metrics.Provider.Meter.Instance(Options, Tags).Reset();
		}
	}
}

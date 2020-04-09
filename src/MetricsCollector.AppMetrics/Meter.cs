using App.Metrics;
using App.Metrics.Meter;
using IMeter = MetricsCollector.Abstractions.IMeter;

namespace MetricsCollector.AppMetrics
{
  internal class Meter : IMeter
  {
    private readonly IProvideMeterMetrics _providerMeter;
    private readonly IMeasureMeterMetrics _measureMeter;
    internal MeterOptions Options { get; }
    internal MetricTags Tags { get; }

    public Meter(IProvideMeterMetrics providerMeter, IMeasureMeterMetrics measureMeter, MeterOptions meterOptions, in MetricTags metricTags)
    {
      _providerMeter = providerMeter;
      _measureMeter = measureMeter;
      Options = meterOptions;
      Tags = metricTags;
    }

    public void Mark()
    {
      _measureMeter.Mark(Options, Tags);
    }

    public void Mark(string item)
    {
      _measureMeter.Mark(Options, Tags, item);
    }

    public void Mark(long amount)
    {
      _measureMeter.Mark(Options, Tags, amount);
    }

    public void Reset()
    {
      _providerMeter.Instance(Options, Tags).Reset();
    }
  }
}
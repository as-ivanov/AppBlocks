using App.Metrics;
using App.Metrics.Counter;
using ICounter = MetricsCollector.Abstractions.ICounter;

namespace MetricsCollector.AppMetrics
{
  public class Counter : ICounter
  {
    private readonly IProvideCounterMetrics _providerCounter;
    private readonly IMeasureCounterMetrics _counter;
    private readonly CounterOptions _counterOptions;
    private readonly MetricTags _metricTags;

    public Counter(IProvideCounterMetrics providerCounter, IMeasureCounterMetrics counter,
      CounterOptions counterOptions, in MetricTags metricTags)
    {
      _providerCounter = providerCounter;
      _counter = counter;
      _counterOptions = counterOptions;
      _metricTags = metricTags;
    }


    public void Decrement(long decrement = 1)
    {
      _counter.Decrement(_counterOptions, _metricTags, decrement);
    }

    public void Increment(long increment = 1)
    {
      _counter.Increment(_counterOptions, _metricTags, increment);
    }

    public void Reset()
    {
      _providerCounter.Instance(_counterOptions, _metricTags).Reset();
    }
  }
}
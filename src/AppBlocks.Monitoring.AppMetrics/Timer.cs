using System;
using App.Metrics;
using App.Metrics.Timer;

namespace AppBlocks.Monitoring.AppMetrics
{
  internal class Timer : Abstractions.ITimer
  {
    private readonly IMetrics _metrics;
    private readonly TimerOptions _timerOptions;
    private readonly MetricTags _metricTags;

    public Timer(IMetrics metrics, TimerOptions timerOptions, in MetricTags metricTags)
    {
      _metrics = metrics;
      _timerOptions = timerOptions;
      _metricTags = metricTags;
    }

    public void Record(double milliseconds)
    {
      _metrics.Provider.Timer.Instance(_timerOptions, _metricTags).Record((long)milliseconds, TimeUnit.Milliseconds);
    }

    public IDisposable Time()
    {
      return _metrics.Measure.Timer.Time(_timerOptions, _metricTags);
    }
  }
}
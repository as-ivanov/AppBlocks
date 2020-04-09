using App.Metrics;
using App.Metrics.Histogram;
using IHistogram = MetricsCollector.Abstractions.IHistogram;

namespace MetricsCollector.AppMetrics
{
  internal class Histogram : IHistogram
  {
    private readonly IMeasureHistogramMetrics _measureHistogram;
    private readonly HistogramOptions _histogramOptions;
    private readonly MetricTags _metricTags;

    public Histogram(IMeasureHistogramMetrics measureHistogram, HistogramOptions histogramOptions, in MetricTags metricTags)
    {
      _measureHistogram = measureHistogram;
      _histogramOptions = histogramOptions;
      _metricTags = metricTags;
    }


    public void Update(long value)
    {
      _measureHistogram.Update(_histogramOptions, _metricTags, value);
    }

    public void Update(long value, string userValue)
    {
      _measureHistogram.Update(_histogramOptions, _metricTags, value, userValue);
    }
  }
}
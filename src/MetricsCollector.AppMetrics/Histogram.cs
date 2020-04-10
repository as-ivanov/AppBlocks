﻿using App.Metrics;
using App.Metrics.Histogram;
using IHistogram = MetricsCollector.Abstractions.IHistogram;

namespace MetricsCollector.AppMetrics
{
  internal class Histogram : IHistogram
  {
	  private readonly IMetrics _metrics;
	  private readonly HistogramOptions _histogramOptions;
    private readonly MetricTags _metricTags;

    public Histogram(IMetrics metrics, HistogramOptions histogramOptions, in MetricTags metricTags)
    {
	    _metrics = metrics;
	    _histogramOptions = histogramOptions;
      _metricTags = metricTags;
    }


    public void Update(long value)
    {
	    _metrics.Measure.Histogram.Update(_histogramOptions, _metricTags, value);
    }

    public void Update(long value, string userValue)
    {
	    _metrics.Measure.Histogram.Update(_histogramOptions, _metricTags, value, userValue);
    }
  }
}
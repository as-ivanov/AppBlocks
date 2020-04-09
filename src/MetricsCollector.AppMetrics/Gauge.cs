using App.Metrics;
using App.Metrics.Gauge;
using IGauge = MetricsCollector.Abstractions.IGauge;

namespace MetricsCollector.AppMetrics
{
  public class Gauge : IGauge
  {
    private readonly IMeasureGaugeMetrics _measureGauge;
    private readonly GaugeOptions _gaugeOptions;
    private readonly MetricTags _metricTags;

    public Gauge(IMeasureGaugeMetrics measureGauge, GaugeOptions gaugeOptions, in MetricTags metricTags)
    {
      _measureGauge = measureGauge;
      _gaugeOptions = gaugeOptions;
      _metricTags = metricTags;
    }

    public void SetValue(double value)
    {
      _measureGauge.SetValue(_gaugeOptions, _metricTags, value);
    }
  }
}
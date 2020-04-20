using App.Metrics;
using App.Metrics.Gauge;

namespace AppBlocks.Monitoring.AppMetrics
{
  public class Gauge : Abstractions.IGauge
  {
	  private readonly IMetrics _metrics;
	  private readonly GaugeOptions _gaugeOptions;
    private readonly MetricTags _metricTags;

    public Gauge(IMetrics metrics, GaugeOptions gaugeOptions, in MetricTags metricTags)
    {
	    _metrics = metrics;
	    _gaugeOptions = gaugeOptions;
      _metricTags = metricTags;
    }

    public void SetValue(double value)
    {
	    _metrics.Measure.Gauge.SetValue(_gaugeOptions, _metricTags, value);
    }
  }
}
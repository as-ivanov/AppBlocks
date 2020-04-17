namespace MetricsCollector.Abstractions
{
  public interface IGauge
  {
    void SetValue(double value);
  }
}
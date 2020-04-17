namespace MetricsCollector.Abstractions
{
  public interface IHistogram
  {
    void Update(long value);
    void Update(long value, string userValue);
  }
}
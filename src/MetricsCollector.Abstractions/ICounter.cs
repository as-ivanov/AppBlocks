namespace MetricsCollector.Abstractions
{
  public interface ICounter
  {
    void Increment(long value = 1);
    void Decrement(long value = 1);
    void Reset();
  }
}
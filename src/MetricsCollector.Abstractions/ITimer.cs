using System;

namespace MetricsCollector.Abstractions
{
  public interface ITimer
  {
    void Record(double milliseconds);
    IDisposable Time();
  }
}
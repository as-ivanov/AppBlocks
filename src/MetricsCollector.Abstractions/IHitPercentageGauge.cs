namespace MetricsCollector.Abstractions
{
  public interface IHitPercentageGauge
  {
    void Ratio(IMeter numerator, IMeter denominator);
  }
}
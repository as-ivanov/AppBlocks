namespace MetricsCollector.Abstractions
{
	public interface IMetricsProvider
	{
		ICounter CreateCounter(string contextName, string name, string metricUnit, in Tags tags);
		IHistogram CreateHistogram(string contextName, string name, string metricUnit, in Tags tags);

		IHitPercentageGauge CreateHitPercentageGauge(string contextName, string name, string metricUnit, in Tags tags);
		IMeter CreateMeter(string contextName, string name, string metricUnit, in Tags tags);
		ITimer CreateTimer(string contextName, string name, string metricUnit, in Tags tags);

		IGauge CreateGauge(string contextName, string name, string metricUnit, in Tags tags);
		bool IsEnabled(string contextName, string name);
	}
}
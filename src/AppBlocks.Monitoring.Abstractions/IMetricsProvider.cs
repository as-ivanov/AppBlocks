namespace AppBlocks.Monitoring.Abstractions
{
	public interface IMetricsProvider
	{
		ICounter CreateCounter(string contextName, string metricName, string measurementUnitName, in Tags tags);
		IHistogram CreateHistogram(string contextName, string metricName, string measurementUnitName, in Tags tags);
		IHitPercentageGauge CreateHitPercentageGauge(string contextName, string metricName, string measurementUnitName, in Tags tags);
		IMeter CreateMeter(string contextName, string metricName, string measurementUnitName, in Tags tags);
		ITimer CreateTimer(string contextName, string metricName, string measurementUnitName, in Tags tags);
		IGauge CreateGauge(string contextName, string metricName, string measurementUnitName, in Tags tags);
	}
}
namespace MetricsCollector.Abstractions
{
	public interface IMetricsProvider
	{
		IMeter CreateMeter(string contextName, string name, in Tags tags);
	}
}
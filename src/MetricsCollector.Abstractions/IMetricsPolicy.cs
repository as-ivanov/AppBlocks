namespace MetricsCollector.Abstractions
{
	public interface IMetricsPolicy
	{
		bool IsEnabled(string contextName, string indicatorName);
	}
}
namespace AppBlocks.Monitoring.Abstractions
{
	public interface IMetricsPolicy
	{
		bool IsEnabled(string contextName, string metricName);
	}
}

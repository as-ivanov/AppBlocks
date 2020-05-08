using AppBlocks.Monitoring.Abstractions;
using AppBlocks.Monitoring.CodeGeneration.Attributes;

namespace AppBlocks.Monitoring.Sample
{
	[GenerateMetricsCollector(ContextName = "BackgroundTask", InheritedInterfaceTypes = new[]
	{
		"AppBlocks.Monitoring.Sample.ISingletonDependency",
		"AppBlocks.Monitoring.Sample.IMetricsCollectorImplementation"
	})]
	public interface IBackgroundTaskMetricsCollector
	{
		[MetricOptions(MetricName = "execution_count", MeasurementUnitName = "item")]
		IMeter ExecutionTotal(string taskName);

		[MetricOptions(MetricName = "execution_active", MeasurementUnitName = "item")]
		ICounter ExecutionActive(string taskName);

		[MetricOptions(MetricName = "execution_time", MeasurementUnitName = "ms")]
		ITimer ExecutionTime(string taskName);

		[MetricOptions(MetricName = "execution_error", MeasurementUnitName = "item")]
		IMeter ExecutionError(string key, string error);
	}
}
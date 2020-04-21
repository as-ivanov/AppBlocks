using AppBlocks.Monitoring.Abstractions;
using AppBlocks.Monitoring.CodeGeneration.Attributes;

namespace AppBlocks.Monitoring.Sample
{
	[MetricsCollectorStub("BackgroundTask", "AppBlocks.Monitoring.Sample.ISingletonDependency",
		"AppBlocks.Monitoring.Sample.IMetricsCollectorImplementation")]
	public interface IBackgroundTaskMetricsCollector
	{
			[MetricsCollectorMethodStub("execution_count", "item")]
			IMeter ExecutionTotal(string taskName);

			[MetricsCollectorMethodStub("execution_active", "item")]
			ICounter ExecutionActive(string taskName);

			[MetricsCollectorMethodStub("execution_time", "ms")]
			ITimer ExecutionTime(string taskName);

			[MetricsCollectorMethodStub("execution_error", "item")]
			IMeter ExecutionError(string key, string error);
	}
}
using System;
using CodeGeneration.Roslyn.MetricsCollector.Attributes;
using MetricsCollector.Abstractions;

namespace CodeGeneration.Roslyn.MetricsCollector.Sample
{
	[MetricsCollectorStub("BackgroundTask", "CodeGeneration.Roslyn.MetricsCollector.Sample.ISingletonDependency",
		"CodeGeneration.Roslyn.MetricsCollector.Sample.IMetricsCollectorImplementation")]
	public interface IBackgroundTaskMetricsCollector
	{
			[MetricsCollectorMethodStub("execution_count", "item")]
			IMeter ExecutionTotal(string taskName);

			[MetricsCollectorMethodStub("execution_active", "item")]
			ICounter ExecutionActive(string taskName);

			[MetricsCollectorMethodStub("execution_time", "ms")]
			ITimer ExecutionTime(string taskName);

			[MetricsCollectorMethodStub("execution_error", "item")]
			IMeter OperationError(string key, string error);
	}
}
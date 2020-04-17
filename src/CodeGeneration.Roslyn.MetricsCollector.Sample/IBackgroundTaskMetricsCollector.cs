using System;
using CodeGeneration.Roslyn.MetricsCollector.Attributes;
using MetricsCollector.Abstractions;

namespace CodeGeneration.Roslyn.MetricsCollector.Sample
{
	[MetricsCollectorStub("BackgroundTask", "CodeGeneration.Roslyn.MetricsCollector.Sample.ISingletonDependency", "CodeGeneration.Roslyn.MetricsCollector.Sample.IMetricsCollectorImplementation")]
	public interface IBackgroundTaskMetricsCollector
	{
		[MetricsCollectorMethodStub("item", "execution_count")]
		IMeter ExecutionTotal(string taskName);

		[MetricsCollectorMethodStub("item", "execution_active")]
		ICounter ExecutionActive(string taskName);

		[MetricsCollectorMethodStub("ms", "execution_time")]
		ITimer ExecutionTime(string taskName);

		[MetricsCollectorMethodStub("item", "execution_error")]
		IMeter OperationError(string key, string error);
	}
}
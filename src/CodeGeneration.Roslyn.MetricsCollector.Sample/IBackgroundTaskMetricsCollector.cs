using System;
using CodeGeneration.Roslyn.MetricsCollector.Attributes;

namespace CodeGeneration.Roslyn.MetricsCollector.Sample
{
	[MetricsCollectorStub("BackgroundTask", "CodeGeneration.Roslyn.MetricsCollector.Sample.ISingletonDependency", "CodeGeneration.Roslyn.MetricsCollector.Sample.IMetricsCollectorImplementation")]
	public interface IBackgroundTaskMetricsCollector
	{
		ICounter ExecutionCount(string taskName);

		ITimer ExecutionTime(string taskName);
	}
}
using System;
using AppBlocks.Logging.CodeGeneration.Attributes;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.Sample
{
	[GenerateLogger(InheritedInterfaceTypes = new[] {"AppBlocks.Logging.Sample.ISingletonDependency", "AppBlocks.Logging.Sample.ILoggerImplementation"})]
	public interface IBackgroundTaskManagerLogger
	{
		void ExecutionStarted(string taskName);
		void ExecutionFinished(string taskName);

		[LogOptions(Level = LogLevel.Warning, Message = "Task execution failed")]
		void ExecutionFailed(string taskName, Exception error);
	}
}
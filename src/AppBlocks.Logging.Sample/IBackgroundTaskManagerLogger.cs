using System;
using AppBlocks.Logging.CodeGeneration.Attributes;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.Sample
{
	[LoggerStub("AppBlocks.Logging.Sample.ISingletonDependency", "AppBlocks.Logging.Sample.ILoggerImplementation")]
	public interface IBackgroundTaskManagerLogger
	{
		[LoggerMethodStub(LogLevel.Information, "Task execution started")]
		void ExecutionStarted(string taskName);

		[LoggerMethodStub(LogLevel.Information, "Task execution finished")]
		void ExecutionFinished(string taskName);

		[LoggerMethodStub(LogLevel.Information, "Task execution failed")]
		void ExecutionFailed(string taskName, Exception error);
	}
}
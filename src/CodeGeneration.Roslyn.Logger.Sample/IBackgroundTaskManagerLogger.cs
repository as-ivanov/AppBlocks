using System;
using CodeGeneration.Roslyn.Logger.Attributes;
using Microsoft.Extensions.Logging;

namespace CodeGeneration.Roslyn.Logger.Sample
{
	[LoggerStub("CodeGeneration.Roslyn.Logger.Sample.ISingletonDependency", "CodeGeneration.Roslyn.Logger.Sample.ILoggerImplementation")]
	public interface IBackgroundTaskManagerLogger
	{
		[LoggerMethodStub(LogLevel.Information, "Task execution started.")]
		void ExecutionStarted(string taskName);

		[LoggerMethodStub(LogLevel.Information, "Task execution finished.")]
		void ExecutionFinished(string taskName);

		[LoggerMethodStub(LogLevel.Information, "Task execution failed.")]
		void ExecutionFailed(string taskName, Exception error);
	}
}
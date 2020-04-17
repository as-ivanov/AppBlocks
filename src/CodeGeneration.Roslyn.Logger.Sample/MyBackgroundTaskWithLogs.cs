using System;
using System.Threading.Tasks;

namespace CodeGeneration.Roslyn.Logger.Sample
{
	public class MyBackgroundTaskWithLogs
	{
		private readonly string _name;
		private readonly IBackgroundTaskManagerLogger _logger;

		public MyBackgroundTaskWithLogs(string name, IBackgroundTaskManagerLogger logger)
		{
			_name = name;
			_logger = logger;
		}

		public async Task Run()
		{
			_logger.ExecutionStarted(_name);
			try
			{
				await DoWork();
			}
			catch (Exception e)
			{
				_logger.ExecutionFailed(_name, e);
			}
			_logger.ExecutionFinished(_name);
		}

		private Task DoWork()
		{
			//...
			return Task.CompletedTask;
		}
	}
}
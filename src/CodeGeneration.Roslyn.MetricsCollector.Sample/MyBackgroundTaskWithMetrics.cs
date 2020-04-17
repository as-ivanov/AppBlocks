using System;
using System.Threading.Tasks;

namespace CodeGeneration.Roslyn.MetricsCollector.Sample
{
	public class MyBackgroundTaskWithMetrics
	{
		private readonly string _name;
		private readonly IBackgroundTaskMetricsCollector _metricsCollector;

		public MyBackgroundTaskWithMetrics(string name, IBackgroundTaskMetricsCollector metricsCollector)
		{
			_name = name;
			_metricsCollector = metricsCollector;
		}

		public async Task Run()
		{
			_metricsCollector.ExecutionTotal(_name).Mark();
			_metricsCollector.ExecutionActive(_name).Increment();
			using (_metricsCollector.ExecutionTime(_name).Time())
			{
				try
				{
					await DoWork();
				}
				catch (Exception e)
				{
					_metricsCollector.OperationError(_name, e.GetType().Name).Mark();
					throw;
				}
				finally
				{
					_metricsCollector.ExecutionActive(_name).Decrement();
				}
			}
		}

		private Task DoWork()
		{
			//...
			return Task.CompletedTask;
		}
	}
}
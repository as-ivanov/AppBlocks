using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace AppBlocks.Monitoring.Sample
{
	public class MyBackgroundTaskManagerWithMetrics : IHostedService, IDisposable
	{
		private readonly IBackgroundTaskMetricsCollector _metricsCollector;
		private Timer _timer;
		private readonly List<string> _tasks = new List<string>() { "task1", "task2", "task3"};

		public MyBackgroundTaskManagerWithMetrics(IBackgroundTaskMetricsCollector metricsCollector)
		{
			_metricsCollector = metricsCollector;
		}

		public Task StartAsync(CancellationToken stoppingToken)
		{
			_timer = new Timer(RunAll, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
			return Task.CompletedTask;
		}

		private void RunAll(object state)
		{
			foreach (var task in _tasks)
			{
				Run(task);
			}
		}

		private void Run(string name)
		{
			_metricsCollector.ExecutionTotal(name).Mark();
			_metricsCollector.ExecutionActive(name).Increment();
			using (_metricsCollector.ExecutionTime(name).Time())
			{
				try
				{
					DoWork(name);
				}
				catch (Exception e)
				{
					_metricsCollector.OperationError(name, e.GetType().Name).Mark();
					throw;
				}
				finally
				{
					_metricsCollector.ExecutionActive(name).Decrement();
				}
			}
		}

		private void DoWork(string name)
		{
			//...
		}

		public Task StopAsync(CancellationToken stoppingToken)
		{
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}
	}
}
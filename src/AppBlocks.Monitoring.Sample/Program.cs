using System;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Formatters.Json;
using AppBlocks.Monitoring.Abstractions;
using AppBlocks.Monitoring.AppMetrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppBlocks.Monitoring.Sample
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			using var host = Host.CreateDefaultBuilder(args)
				.ConfigureServices((hostContext, services) =>
				{
					services.AddHostedService<MyBackgroundTaskManagerWithMetrics>();
					services.AddSingleton<IBackgroundTaskMetricsCollector, BackgroundTaskMetricsCollector>();
					services.AddSingleton<IMetricsProvider, AppMetricsMetricsProvider>();

					var metrics = new MetricsBuilder()
						.Configuration.Configure(options =>
						{
							options.Enabled = true;
							options.ReportingEnabled = true;
						})
						.OutputMetrics.AsPlainText()
						.Report.ToConsole(
							options =>
							{
								options.FlushInterval = TimeSpan.FromSeconds(1);
								options.MetricsOutputFormatter = new MetricsJsonOutputFormatter();
							})
						.Build();

					services.AddSingleton<IMetrics>(_ => metrics);
					services.AddSingleton(_ => metrics);
				})
				.Build();
			await host.StartAsync();

			var metricsRoot = host.Services.GetRequiredService<IMetricsRoot>();

			async Task FlushMetricsReportersAsync()
			{
				await Task.WhenAll(metricsRoot.ReportRunner.RunAllAsync());
				await Task.Delay(1000);
				await FlushMetricsReportersAsync();
			}

#pragma warning disable 4014
			Task.Run(FlushMetricsReportersAsync);
#pragma warning restore 4014

			await host.WaitForShutdownAsync();
		}
	}
}
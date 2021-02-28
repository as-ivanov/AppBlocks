using System;
using App.Metrics;
using App.Metrics.Timer;
using ITimer = AppBlocks.Monitoring.Abstractions.ITimer;

namespace AppBlocks.Monitoring.AppMetrics
{
	internal class Timer : ITimer
	{
		private readonly IMetrics _metrics;
		private readonly MetricTags _metricTags;
		private readonly TimerOptions _timerOptions;

		public Timer(IMetrics metrics, TimerOptions timerOptions, in MetricTags metricTags)
		{
			_metrics = metrics;
			_timerOptions = timerOptions;
			_metricTags = metricTags;
		}

		public void Record(double milliseconds)
		{
			_metrics.Provider.Timer.Instance(_timerOptions, _metricTags).Record((long)milliseconds, TimeUnit.Milliseconds);
		}

		public IDisposable Time()
		{
			return _metrics.Measure.Timer.Time(_timerOptions, _metricTags);
		}
	}
}

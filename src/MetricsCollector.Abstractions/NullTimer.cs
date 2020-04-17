using System;

namespace MetricsCollector.Abstractions
{
	public class NullTimer : ITimer, IDisposable
	{
		public static readonly ITimer Instance = new NullTimer();

		private NullTimer()
		{
		}

		public void Record(double milliseconds)
		{
		}

		public IDisposable Time()
		{
			return this;
		}

		public void Dispose()
		{
		}
	}
}
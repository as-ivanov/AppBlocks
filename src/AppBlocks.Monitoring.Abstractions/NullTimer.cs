using System;

namespace AppBlocks.Monitoring.Abstractions
{
	public class NullTimer : ITimer, IDisposable
	{
		public static readonly ITimer Instance = new NullTimer();

		private NullTimer()
		{
		}

		public void Dispose()
		{
		}

		public void Record(double milliseconds)
		{
		}

		public IDisposable Time()
		{
			return this;
		}
	}
}

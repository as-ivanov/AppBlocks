using System;

namespace AppBlocks.Monitoring.Abstractions
{
	public interface ITimer
	{
		void Record(double milliseconds);
		IDisposable Time();
	}
}

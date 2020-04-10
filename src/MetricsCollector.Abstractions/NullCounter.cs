namespace MetricsCollector.Abstractions
{
	public class NullCounter : ICounter
	{
		public static readonly ICounter Instance = new NullCounter();

		private NullCounter()
		{
		}
		public void Increment(long value = 1)
		{
		}

		public void Decrement(long value = 1)
		{
		}

		public void Reset()
		{
		}
	}
}
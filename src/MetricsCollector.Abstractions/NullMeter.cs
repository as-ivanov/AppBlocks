namespace MetricsCollector.Abstractions
{
	public class NullMeter : IMeter
	{
		public static readonly IMeter Instance = new NullMeter();

		private NullMeter()
		{
		}

		public void Mark()
		{
		}

		public void Mark(string item)
		{
		}

		public void Mark(long amount)
		{
		}

		public void Reset()
		{
		}
	}
}
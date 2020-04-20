namespace AppBlocks.Monitoring.Abstractions
{
	public class NullHistogram : IHistogram
	{
		public static readonly IHistogram Instance = new NullHistogram();

		private NullHistogram()
		{
		}

		public void Update(long value)
		{
		}

		public void Update(long value, string userValue)
		{
		}
	}
}
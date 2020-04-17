namespace MetricsCollector.Abstractions
{
	public class NullGauge : IGauge
	{
		public static readonly IGauge Instance = new NullGauge();

		private NullGauge()
		{
		}

		public void SetValue(double value)
		{
		}
	}
}
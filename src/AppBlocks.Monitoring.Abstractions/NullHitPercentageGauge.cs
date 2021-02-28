namespace AppBlocks.Monitoring.Abstractions
{
	public class NullHitPercentageGauge : IHitPercentageGauge
	{
		public static readonly IHitPercentageGauge Instance = new NullHitPercentageGauge();

		private NullHitPercentageGauge()
		{
		}

		public void Ratio(IMeter numerator, IMeter denominator)
		{
		}
	}
}

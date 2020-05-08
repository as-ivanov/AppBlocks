namespace AppBlocks.Monitoring.Abstractions
{
	public interface IHitPercentageGauge
	{
		void Ratio(IMeter numerator, IMeter denominator);
	}
}
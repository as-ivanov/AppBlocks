namespace AppBlocks.Monitoring.Abstractions
{
	public interface IGauge
	{
		void SetValue(double value);
	}
}
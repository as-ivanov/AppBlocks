using MetricsCollector.Abstractions;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class TestMetricsProvider : IMetricsProvider
	{
		public ICounter CreateCounter(string contextName, string indicatorName, string measurementUnit, in Tags tags)
		{
			throw new System.NotImplementedException();
		}

		public IHistogram CreateHistogram(string contextName, string indicatorName, string measurementUnit, in Tags tags)
		{
			throw new System.NotImplementedException();
		}

		public IHitPercentageGauge CreateHitPercentageGauge(string contextName, string indicatorName, string measurementUnit, in Tags tags)
		{
			throw new System.NotImplementedException();
		}

		public IMeter CreateMeter(string contextName, string indicatorName, string measurementUnit, in Tags tags)
		{
			throw new System.NotImplementedException();
		}

		public ITimer CreateTimer(string contextName, string indicatorName, string measurementUnit, in Tags tags)
		{
			throw new System.NotImplementedException();
		}

		public IGauge CreateGauge(string contextName, string indicatorName, string measurementUnit, in Tags tags)
		{
			throw new System.NotImplementedException();
		}

		public bool IsEnabled(string contextName, string indicatorName)
		{
			throw new System.NotImplementedException();
		}
	}
}
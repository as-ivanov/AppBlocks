using System;

namespace MetricsCollector.Abstractions
{
	public interface IMetricsProvider
	{
		ICounter CreateCounter(string contextName, string indicatorName, string measurementUnit, in Tags tags);
		IHistogram CreateHistogram(string contextName, string indicatorName, string measurementUnit, in Tags tags);
		IHitPercentageGauge CreateHitPercentageGauge(string contextName, string indicatorName, string measurementUnit, in Tags tags);
		IMeter CreateMeter(string contextName, string indicatorName, string measurementUnit, in Tags tags);
		ITimer CreateTimer(string contextName, string indicatorName, string measurementUnit, in Tags tags);
		IGauge CreateGauge(string contextName, string indicatorName, string measurementUnit, in Tags tags);
	}
}
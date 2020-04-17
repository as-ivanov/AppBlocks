using App.Metrics;
using MetricsCollector.Abstractions;

namespace MetricsCollector.AppMetrics
{
	public static class TagsHelper
	{
		public static MetricTags ToMetricsTags(this in Tags tags)
		{
			if (tags.Key != null && tags.Value != null)
			{
				return new MetricTags(tags.Key, tags.Value);
			}
			return new MetricTags(tags.Keys, tags.Values);
		}
	}
}
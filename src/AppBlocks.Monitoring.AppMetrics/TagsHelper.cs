using App.Metrics;
using AppBlocks.Monitoring.Abstractions;

namespace AppBlocks.Monitoring.AppMetrics
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

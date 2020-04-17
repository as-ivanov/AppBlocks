using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Metrics;
using App.Metrics.Meter;
using MetricsCollector.Abstractions;
using IMeter = MetricsCollector.Abstractions.IMeter;

namespace CodeGeneration.Roslyn.MetricsCollector.AppMetrics
{
	public abstract class MetricsCollectorBase
	{
		private readonly IMetrics _metrics;
		private readonly Dictionary<Type, IGrouping<Type, IMetricsExceptionRenderer>> _metricsExceptionRenderers;

		protected MetricsCollectorBase(IMetrics metrics, IEnumerable<IMetricsExceptionRenderer> exceptionRenderers)
		{
			_metrics = metrics;
			_metricsExceptionRenderers = exceptionRenderers.GroupBy(_=>_.ExceptionType).ToDictionary(_=>_.Key);
		}

		// private static MetricTags GetMetricTags(IInvocation invocation)
		// {
		// 	var tagKeys = invocation.Method.GetParameters().Select(parameter => parameter.Name).ToArray();
		// 	var tagValues = invocation.Arguments.Select(argument => argument?.ToString() ?? "n/a").ToArray();
		// 	return new MetricTags(tagKeys, tagValues);
		// }

		protected string RenderException(Exception exception)
		{
			if (_metricsExceptionRenderers.TryGetValue(exception.GetType(), out var renderers) && renderers.Any())
			{
				var sb = new StringBuilder(exception.GetType().Name); //TODO Use Spans
				foreach (var renderer in renderers)
				{
					sb.Append("|" + renderer.Render(exception));
				}
			}
			return exception.GetType().Name;
		}

		protected IMeter CreateMeter(string contextName, string name, bool suppressExportMetrics, MetricTags tags)
		{
			var meterOptions = new MeterOptions
			{
				Context = contextName,
				Name = name,
				ReportSetItems = !suppressExportMetrics
			};
			return new Meter(_metrics.Provider.Meter, _metrics.Measure.Meter, meterOptions, tags);
		}
	}
}
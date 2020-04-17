using System;

namespace MetricsCollector.Abstractions
{
	public interface IMetricsExceptionRenderer
	{
		Type ExceptionType { get;}
		string Render(Exception error);
	}
}
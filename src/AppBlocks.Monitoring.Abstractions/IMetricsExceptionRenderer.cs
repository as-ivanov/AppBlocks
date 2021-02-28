using System;

namespace AppBlocks.Monitoring.Abstractions
{
	public interface IMetricsExceptionRenderer
	{
		Type ExceptionType { get; }
		string Render(Exception error);
	}
}

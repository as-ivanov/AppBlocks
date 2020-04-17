using System;
using System.Diagnostics;
using CodeGeneration.Roslyn.Attributes.Common;

namespace CodeGeneration.Roslyn.MetricsCollector.Attributes
{
	[Conditional(CodeGenerationAttributesConsts.CodeGenerationConditionName)]
	[AttributeUsage(AttributeTargets.Method)]
	public class MetricsCollectorMethodStubAttribute : Attribute
	{
		public MetricsCollectorMethodStubAttribute(string metricName = null, string measurementUnitName = null)
		{
		}
	}
}

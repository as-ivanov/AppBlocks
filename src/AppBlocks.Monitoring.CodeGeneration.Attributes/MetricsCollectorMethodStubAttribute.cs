using System;
using System.Diagnostics;
using AppBlocks.CodeGeneration.Attributes.Common;

namespace AppBlocks.Monitoring.CodeGeneration.Attributes
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

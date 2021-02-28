using System;
using System.Diagnostics;
using AppBlocks.CodeGeneration.Attributes.Common;

namespace AppBlocks.Monitoring.CodeGeneration.Attributes
{
	[Conditional(CodeGenerationAttributesConsts.CodeGenerationConditionName)]
	[AttributeUsage(AttributeTargets.Method)]
	public class MetricOptionsAttribute : Attribute
	{
		public string MetricName { get; set; }

		public string MeasurementUnitName { get; set; }
	}
}

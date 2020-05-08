using System;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using AppBlocks.Monitoring.CodeGeneration.Attributes;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn.Tests
{
	public class MetricsCollectorInterfaceAttributeData : ImplementInterfaceAttributeData
	{
		private readonly string _contextName;

		public MetricsCollectorInterfaceAttributeData(string contextName) : this(contextName, Array.Empty<InterfaceData>())
		{
		}

		public MetricsCollectorInterfaceAttributeData(string contextName, InterfaceData[] inheritedInterfaces) : base(nameof(GenerateMetricsCollectorAttribute), inheritedInterfaces)
		{
			_contextName = contextName;
		}

		public string ContextName => _contextName;

		public override string ToString()
		{
			var inheritedInterfacesString = GetInheritedInterfacesSetterString();
			if (!string.IsNullOrEmpty(inheritedInterfacesString))
			{
				inheritedInterfacesString = $", {inheritedInterfacesString}";
			}

			return $"[{Name}({nameof(GenerateMetricsCollectorAttribute.ContextName)} = \"{_contextName}\"{inheritedInterfacesString})]";
		}
	}
}
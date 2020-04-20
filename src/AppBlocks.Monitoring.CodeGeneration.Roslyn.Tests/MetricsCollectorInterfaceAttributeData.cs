using System;
using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn.Tests
{
	public class MetricsCollectorInterfaceAttributeData : AttributeData
	{
		private readonly string _contextName;
		private readonly InterfaceData[] _inheritedInterfaces;

		public MetricsCollectorInterfaceAttributeData(string contextName): this(contextName, Array.Empty<InterfaceData>())
		{
		}

		public MetricsCollectorInterfaceAttributeData(string contextName, InterfaceData[] inheritedInterfaces) : base(
			nameof(global::AppBlocks.Monitoring.CodeGeneration.Attributes.MetricsCollectorStubAttribute))
		{
			_contextName = contextName;
			_inheritedInterfaces = inheritedInterfaces;
		}

		public string ContextName => _contextName;

		public InterfaceData[] InheritedInterfaces => _inheritedInterfaces;

		public override string ToString()
		{
			var inheritedInterfacesString = string.Join(",", _inheritedInterfaces.Select(_ => $"\"{_.Namespace}.{_.Name}\""));
			if (!string.IsNullOrEmpty(inheritedInterfacesString))
			{
				inheritedInterfacesString = $",{inheritedInterfacesString}";
			}
			return $"[{Name}(\"{_contextName}\"{inheritedInterfacesString})]";
		}
	}
}
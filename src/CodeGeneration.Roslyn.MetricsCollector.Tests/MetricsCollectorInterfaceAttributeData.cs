using System;
using System.Linq;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class MetricsCollectorInterfaceAttributeData : AttributeData
	{
		private readonly string _contextName;
		private readonly InterfaceData[] _inheritedInterfaces;

		public MetricsCollectorInterfaceAttributeData(string contextName): this(contextName, Array.Empty<InterfaceData>())
		{

		}

		public MetricsCollectorInterfaceAttributeData(string contextName, InterfaceData[] inheritedInterfaces) : base(
			nameof(Attributes.MetricsCollectorStubAttribute))
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
using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn.Tests
{
	public class AbstractMetricsCollectorInterfaceAttributeData : ImplementInterfaceAttributeData
	{
		public AbstractMetricsCollectorInterfaceAttributeData(InterfaceData[] inheritedInterfaces) : base(nameof(Attributes.AbstractMetricsCollectorStubAttribute), inheritedInterfaces)
		{
		}

		public override string ToString()
		{
			var inheritedInterfacesString = GetInheritedInterfacesSetterString();
			return $"[{Name}({inheritedInterfacesString})] //abstract collector";
		}
	}
}
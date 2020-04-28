using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerInterfaceAttributeData: ImplementInterfaceAttributeData
	{
		public LoggerInterfaceAttributeData(InterfaceData[] inheritedInterfaces) : base(
			nameof(Attributes.GenerateLoggerAttribute), inheritedInterfaces)
		{
		}
		public override string ToString()
		{
			var inheritedInterfacesString = GetInheritedInterfacesSetterString();
			return $"[{Name}({inheritedInterfacesString})]";
		}
	}
}
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using AppBlocks.Logging.CodeGeneration.Attributes;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerInterfaceAttributeData : ImplementInterfaceAttributeData
	{
		public LoggerInterfaceAttributeData(InterfaceData[] inheritedInterfaces) : base(
			nameof(GenerateLoggerAttribute), inheritedInterfaces)
		{
		}

		public override string ToString()
		{
			var inheritedInterfacesString = GetInheritedInterfacesSetterString();
			return $"[{Name}({inheritedInterfacesString})]";
		}
	}
}

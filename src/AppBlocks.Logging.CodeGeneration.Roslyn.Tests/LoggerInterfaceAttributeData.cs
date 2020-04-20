using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerInterfaceAttributeData: AttributeData
	{
		private readonly InterfaceData[] _inheritedInterfaces;

		public LoggerInterfaceAttributeData(InterfaceData[] inheritedInterfaces) : base(
			nameof(Attributes.LoggerStubAttribute))
		{
			_inheritedInterfaces = inheritedInterfaces;
		}

		public InterfaceData[] InheritedInterfaces => _inheritedInterfaces;

		public override string ToString()
		{
			var inheritedInterfacesString = string.Join(",", _inheritedInterfaces.Select(_ => $"\"{_.Namespace}.{_.Name}\""));
			return $"[{Name}({inheritedInterfacesString})]";
		}
	}
}
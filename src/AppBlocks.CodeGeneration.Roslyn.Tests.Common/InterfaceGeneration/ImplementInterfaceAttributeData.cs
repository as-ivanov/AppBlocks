using System.Linq;
using AppBlocks.CodeGeneration.Attributes.Common;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public abstract class ImplementInterfaceAttributeData : AttributeData
	{
		private readonly InterfaceData[] _inheritedInterfaces;

		protected ImplementInterfaceAttributeData(string name, InterfaceData[] inheritedInterfaces) : base(name)
		{
			_inheritedInterfaces = inheritedInterfaces;
		}

		public InterfaceData[] InheritedInterfaces => _inheritedInterfaces;

		protected string GetInheritedInterfacesSetterString()
		{
			if (InheritedInterfaces.Length == 0)
			{
				return null;
			}

			var inheritedInterfacesString = string.Join(",", InheritedInterfaces.Select(_ => $"\"{_.Namespace}.{_.Name}\""));
			return $"{nameof(ImplementInterfaceAttribute.InheritedInterfaceTypes)} = new[] {{ {inheritedInterfacesString} }}";
		}
	}
}

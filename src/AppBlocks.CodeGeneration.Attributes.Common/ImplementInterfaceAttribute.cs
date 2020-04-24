using System;

namespace AppBlocks.CodeGeneration.Attributes.Common
{
	[AttributeUsage(AttributeTargets.Interface)]
	public abstract class ImplementInterfaceAttribute : Attribute
	{
		private readonly string[] _inheritedInterfaceTypes;

		protected ImplementInterfaceAttribute(params string[] inheritedInterfaceTypes)
		{
			_inheritedInterfaceTypes = inheritedInterfaceTypes;
		}

		public string[] InheritedInterfaceTypes => _inheritedInterfaceTypes;
	}
}
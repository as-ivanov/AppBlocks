using System;

namespace CodeGeneration.Roslyn.Attributes.Common
{
	[AttributeUsage(AttributeTargets.Interface)]
	public abstract class ImplementInterfaceAttribute : Attribute
	{
		protected ImplementInterfaceAttribute(params string[] inheritedInterfaceTypes)
		{
		}
	}
}
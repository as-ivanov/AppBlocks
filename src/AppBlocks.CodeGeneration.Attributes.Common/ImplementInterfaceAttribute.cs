using System;

namespace AppBlocks.CodeGeneration.Attributes.Common
{
	[AttributeUsage(AttributeTargets.Interface)]
	public abstract class ImplementInterfaceAttribute : Attribute
	{
		public string[] InheritedInterfaceTypes { get; set; }
		public bool AttachDebuggerOnNode { get; set; }
	}
}

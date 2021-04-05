using System;

namespace AppBlocks.CodeGeneration.Attributes.Common
{
	[AttributeUsage(AttributeTargets.Interface)]
	public abstract class ImplementInterfaceAttribute : Attribute
	{
		public string[] InheritedInterfaceTypes { get; set; }
#if DEBUG
		public bool AttachDebuggerOnNode { get; set; }
#endif
	}
}

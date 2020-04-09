using System;
using System.Diagnostics;
using CodeGeneration.Roslyn.Attributes.Common;

namespace CodeGeneration.Roslyn.Logger.Attributes
{
	[AttributeUsage(AttributeTargets.Interface)]
	[Conditional(CodeGenerationAttributesConsts.CodeGenerationConditionName)]
	[CodeGenerationAttribute("CodeGeneration.Roslyn.Logger.LoggerClassGenerator, CodeGeneration.Roslyn.Logger")]
	public class LoggerStubAttribute : ImplementInterfaceAttribute
	{
		public LoggerStubAttribute(params string[] inheritedInterfaceTypes): base(inheritedInterfaceTypes)
		{
		}
	}
}
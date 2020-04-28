using System;
using System.Diagnostics;
using AppBlocks.CodeGeneration.Attributes.Common;
using CodeGeneration.Roslyn;

namespace AppBlocks.Logging.CodeGeneration.Attributes
{
	[AttributeUsage(AttributeTargets.Interface)]
	[Conditional(CodeGenerationAttributesConsts.CodeGenerationConditionName)]
	[CodeGenerationAttribute("AppBlocks.Logging.CodeGeneration.Roslyn.LoggerClassGenerator, AppBlocks.Logging.CodeGeneration.Roslyn")]
	public class GenerateLoggerAttribute : ImplementInterfaceAttribute
	{
	}
}
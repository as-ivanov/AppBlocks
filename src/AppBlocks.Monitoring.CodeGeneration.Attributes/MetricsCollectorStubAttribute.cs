using System.Diagnostics;
using AppBlocks.CodeGeneration.Attributes.Common;
using CodeGeneration.Roslyn;

namespace AppBlocks.Monitoring.CodeGeneration.Attributes
{
	[Conditional(CodeGenerationAttributesConsts.CodeGenerationConditionName)]
  [CodeGenerationAttribute("AppBlocks.Monitoring.CodeGeneration.Roslyn.MetricsCollectorClassGenerator, AppBlocks.Monitoring.CodeGeneration.Roslyn")]
  public class MetricsCollectorStubAttribute : ImplementInterfaceAttribute
  {
    public MetricsCollectorStubAttribute(string contextName, params string[] inheritedInterfaceTypes) : base(inheritedInterfaceTypes)
    {
    }
  }
}

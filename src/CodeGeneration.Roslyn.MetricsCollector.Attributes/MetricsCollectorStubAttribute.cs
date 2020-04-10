using System.Diagnostics;
using CodeGeneration.Roslyn.Attributes.Common;

namespace CodeGeneration.Roslyn.MetricsCollector.Attributes
{
	[Conditional(CodeGenerationAttributesConsts.CodeGenerationConditionName)]
  [CodeGenerationAttribute("CodeGeneration.Roslyn.MetricsCollector.MetricsCollectorClassGenerator, CodeGeneration.Roslyn.MetricsCollector")]
  public class MetricsCollectorStubAttribute : ImplementInterfaceAttribute
  {
    public MetricsCollectorStubAttribute(string contextName, params string[] inheritedInterfaceTypes) : base(inheritedInterfaceTypes)
    {
    }
  }
}

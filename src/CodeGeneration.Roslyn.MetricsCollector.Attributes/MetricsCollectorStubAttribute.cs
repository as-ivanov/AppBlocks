using System.Diagnostics;
using CodeGeneration.Roslyn.Attributes.Common;

namespace CodeGeneration.Roslyn.MetricsCollector.Attributes
{
	[Conditional(CodeGenerationAttributesConsts.CodeGenerationConditionName)]
  [CodeGenerationAttribute("CodeGeneration.Roslyn.MetricsCollector.MetricsCollectorClassGenerator, CodeGeneration.Roslyn.MetricsCollector")]
  public class MetricsCollectorStub : ImplementInterfaceAttribute
  {
    public MetricsCollectorStub(string contextName, params string[] inheritedInterfaceTypes) : base(inheritedInterfaceTypes)
    {
    }
  }
}

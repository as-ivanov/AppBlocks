using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneration.Roslyn.MetricsCollector
{
	public class MetricsCollectorMethod
  {
    private readonly MethodDeclarationSyntax _methodDeclarationSyntax;
    private readonly MetricsCollectorType _metricsMetricsCollectorType;

    public MetricsCollectorMethod(MethodDeclarationSyntax methodDeclarationSyntax, MetricsCollectorType metricsMetricsCollectorType)
    {
      _methodDeclarationSyntax = methodDeclarationSyntax;
      _metricsMetricsCollectorType = metricsMetricsCollectorType;
    }

    public MethodDeclarationSyntax MethodDeclaration => _methodDeclarationSyntax;
    public MetricsCollectorType MetricsCollectorType => _metricsMetricsCollectorType;
  }
}
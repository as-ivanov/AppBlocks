using CodeGeneration.Roslyn.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneration.Roslyn.MetricsCollector
{
	public class MetricsCollectorMethod
  {
    private readonly MethodDeclarationSyntax _methodDeclarationSyntax;
    private readonly string _metricName;
    private readonly string _unitName;
    private readonly MetricsCollectorType _metricsMetricsCollectorType;
    private readonly string _methodName;
    private readonly string _methodNameCamelCase;

    public MetricsCollectorMethod(MethodDeclarationSyntax methodDeclarationSyntax, string metricName, string unitName, MetricsCollectorType metricsMetricsCollectorType)
    {
      _methodDeclarationSyntax = methodDeclarationSyntax;
      _metricName = metricName;
      _unitName = unitName;
      _metricsMetricsCollectorType = metricsMetricsCollectorType;
      _methodName = methodDeclarationSyntax.Identifier.WithoutTrivia().Text;
      _methodNameCamelCase = _methodName.ToCamelCase();
    }

    public MethodDeclarationSyntax MethodDeclarationSyntax => _methodDeclarationSyntax;
    public MetricsCollectorType MetricsCollectorType => _metricsMetricsCollectorType;
    public string MethodName => _methodName;
    public string MethodNameCamelCase => _methodNameCamelCase;

    public string MetricName => _metricName;

    public string UnitName => _unitName;
  }
}
using AppBlocks.CodeGeneration.Roslyn.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn
{
	public class MetricsCollectorMethod
  {
    private readonly MethodDeclarationSyntax _methodDeclarationSyntax;
    private readonly string _metricName;
    private readonly string _unitName;
    private readonly MetricsCollectorIndicatorType _metricsMetricsCollectorIndicatorType;
    private readonly string _methodKeysFieldName;

    public MetricsCollectorMethod(MethodDeclarationSyntax methodDeclarationSyntax, string metricName, string methodKeysFieldName, string unitName, MetricsCollectorIndicatorType metricsMetricsCollectorIndicatorType)
    {
      _methodDeclarationSyntax = methodDeclarationSyntax;
      _metricName = metricName;
      _methodKeysFieldName = methodKeysFieldName;
      _unitName = unitName;
      _metricsMetricsCollectorIndicatorType = metricsMetricsCollectorIndicatorType;
    }

    public MethodDeclarationSyntax MethodDeclarationSyntax => _methodDeclarationSyntax;
    public MetricsCollectorIndicatorType MetricsCollectorIndicatorType => _metricsMetricsCollectorIndicatorType;
    public string MetricName => _metricName;
    public string UnitName => _unitName;
    public string MethodKeysFieldName => _methodKeysFieldName;
  }
}
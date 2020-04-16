using System.Threading.Tasks;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Xunit;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class MetricsDisabledMetricsCollectorClassGeneratorIntegrationTests : MetricsCollectorClassGeneratorIntegrationTestBase
	{
		[Theory]
		[MemberData(nameof(Generate))]
		public Task NegativeReportingTest(ITestContext context)
		{
			return MetricsCollectorMethodGenerationTest(context, false);
		}
	}
}
using System.Threading.Tasks;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Xunit;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class MetricsEnabledMetricsCollectorClassGeneratorIntegrationTests : MetricsCollectorClassGeneratorIntegrationTestBase
	{

		[Theory]
		[MemberData(nameof(Generate))]
		public Task PositiveReportingTest(ITestGenerationContext generationContext)
		{
			return MetricsCollectorMethodGenerationTest(generationContext, true);
		}
	}
}
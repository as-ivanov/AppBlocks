using System.Threading.Tasks;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Xunit;
using Xunit.Abstractions;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class MetricsEnabledMetricsCollectorClassGeneratorIntegrationTests : MetricsCollectorClassGeneratorIntegrationTestBase
	{
		public MetricsEnabledMetricsCollectorClassGeneratorIntegrationTests(ITestOutputHelper output) : base(output)
		{
		}

		[Theory]
		[MemberData(nameof(Generate))]
		public Task PositiveReportingTest(ITestContext context)
		{
			return MetricsCollectorMethodGenerationTest(context, true);
		}
	}
}
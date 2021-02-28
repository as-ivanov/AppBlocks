using System.Threading.Tasks;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Xunit;
using Xunit.Abstractions;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn.Tests
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

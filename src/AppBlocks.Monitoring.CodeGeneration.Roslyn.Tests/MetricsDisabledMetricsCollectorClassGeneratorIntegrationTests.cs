using System.Threading.Tasks;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Xunit;
using Xunit.Abstractions;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn.Tests
{
	public class
		MetricsDisabledMetricsCollectorClassGeneratorIntegrationTests : MetricsCollectorClassGeneratorIntegrationTestBase
	{
		public MetricsDisabledMetricsCollectorClassGeneratorIntegrationTests(ITestOutputHelper output) : base(output)
		{
		}

		[Theory]
		[MemberData(nameof(Generate))]
		public Task NegativeReportingTest(ITestContext context)
		{
			return MetricsCollectorMethodGenerationTest(context, false);
		}
	}
}

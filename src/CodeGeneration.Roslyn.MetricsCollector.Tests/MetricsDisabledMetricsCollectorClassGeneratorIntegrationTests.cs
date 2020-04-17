using System.Threading.Tasks;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Xunit;
using Xunit.Abstractions;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
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
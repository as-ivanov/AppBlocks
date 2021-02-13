using System.Threading.Tasks;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Xunit;
using Xunit.Abstractions;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests.Fixtures
{
	public class LogEnabledLoggerClassGeneratorIntegrationTests : LoggerClassGeneratorIntegrationTestBase
	{
		public LogEnabledLoggerClassGeneratorIntegrationTests(ITestOutputHelper output) : base(output)
		{
		}

		[Theory]
		[MemberData(nameof(Generate))]
		public Task PositiveLoggingTest(ITestContext context)
		{
			return LoggerMethodGenerationTest(context, true);
		}
	}
}
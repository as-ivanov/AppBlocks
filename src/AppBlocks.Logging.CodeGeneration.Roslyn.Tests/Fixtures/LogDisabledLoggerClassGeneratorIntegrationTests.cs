using System.Threading.Tasks;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Xunit;
using Xunit.Abstractions;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests.Fixtures
{
	public class LogDisabledLoggerClassGeneratorIntegrationTests : LoggerClassGeneratorIntegrationTestBase
	{
		public LogDisabledLoggerClassGeneratorIntegrationTests(ITestOutputHelper output) : base(output)
		{
		}

		[Theory]
		[MemberData(nameof(Generate))]
		public Task NegativeLoggingTest(ITestContext context)
		{
			return LoggerMethodGenerationTest(context, false);
		}
	}
}

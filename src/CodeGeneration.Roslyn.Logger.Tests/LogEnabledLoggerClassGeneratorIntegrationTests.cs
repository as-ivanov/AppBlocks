using System.Threading.Tasks;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Xunit;
using Xunit.Abstractions;

namespace CodeGeneration.Roslyn.Logger.Tests
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
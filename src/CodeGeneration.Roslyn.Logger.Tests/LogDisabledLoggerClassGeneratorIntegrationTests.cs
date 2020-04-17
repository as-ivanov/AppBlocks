using System.Threading.Tasks;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Xunit;
using Xunit.Abstractions;

namespace CodeGeneration.Roslyn.Logger.Tests
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
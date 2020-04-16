using System.Threading.Tasks;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Xunit;

namespace CodeGeneration.Roslyn.Logger.Tests
{
	public class LogDisabledLoggerClassGeneratorIntegrationTests : LoggerClassGeneratorIntegrationTestBase
	{
		[Theory]
		[MemberData(nameof(Generate))]
		public Task NegativeLoggingTest(ITestContext context)
		{
			return LoggerMethodGenerationTest(context, false);
		}
	}
}
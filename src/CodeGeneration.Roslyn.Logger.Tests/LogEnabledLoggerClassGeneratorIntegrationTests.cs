using System.Threading.Tasks;
using CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Xunit;

namespace CodeGeneration.Roslyn.Logger.Tests
{
	public class LogEnabledLoggerClassGeneratorIntegrationTests : LoggerClassGeneratorIntegrationTestBase
	{
		[Theory]
		[MemberData(nameof(Generate))]
		public Task PositiveLoggingTest(ITestGenerationContext generationContext)
		{
			return LoggerMethodGenerationTest(generationContext, true);
		}
	}
}
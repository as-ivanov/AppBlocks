using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class TestLoggerFactory : ILoggerFactory
	{
		private readonly ILogger _testLogger;

		public TestLoggerFactory(ILogger testLogger)
		{
			_testLogger = testLogger;
		}

		public void Dispose()
		{
		}

		public ILogger CreateLogger(string categoryName)
		{
			return _testLogger;
		}

		public void AddProvider(ILoggerProvider provider)
		{
		}
	}
}
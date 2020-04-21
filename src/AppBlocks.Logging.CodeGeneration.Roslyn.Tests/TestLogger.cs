using System;
using System.Text;
using AppBlocks.CodeGeneration.Roslyn.Common;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class TestLogger : ILogger
	{
		private readonly EventId _eventId;
		private readonly string _methodName;
		private readonly string _message;
		private readonly Microsoft.Extensions.Logging.LogLevel _logLevel;
		private readonly MethodParameterData[] _methodParameters;
		private readonly bool _logEnabled;
		private bool _isEnabledCalled;
		private bool _logCalled;

		private Microsoft.Extensions.Logging.LogLevel _actualIsEnabledLogLevel;
		private Microsoft.Extensions.Logging.LogLevel _actualLogLogLevel;
		private EventId _actualEventId;
		private string _actualMessage;


		public TestLogger(EventId eventId, string methodName, string message,
			Microsoft.Extensions.Logging.LogLevel logLevel,
			MethodParameterData[] methodParameters, bool logEnabled)
		{
			_eventId = eventId;
			_methodName = methodName;
			_logLevel = logLevel;
			_methodParameters = methodParameters;
			_logEnabled = logEnabled;
			var sb = new StringBuilder();
			foreach (var methodParameter in methodParameters)
			{
				if (typeof(Exception).IsAssignableFrom(methodParameter.ParameterType))
				{
					continue;
				}
				if (sb.Length > 0)
				{
					sb.Append(" ");
				}
				sb.Append($"{methodParameter.Name.ToPascalCase()}: '{methodParameter.GetFormattedValue()}'");
			}
			if (sb.Length > 0)
			{
				message = $"{message} ({sb})";
			}
			_message = message;
		}

		public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state,
			Exception exception, Func<TState, Exception, string> formatter)
		{
			if (_logCalled)
			{
				throw new Exception($"{nameof(Log)} already was called");
			}

			_logCalled = true;
			_actualLogLogLevel = logLevel;
			_actualEventId = eventId;
			_actualMessage = state.ToString();
		}

		public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
		{
			_isEnabledCalled = true;
			_actualIsEnabledLogLevel = logLevel;
			return _logEnabled;
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return new Disposable();
		}

		public void Verify()
		{
			if (!_isEnabledCalled)
			{
				throw new Exception($"{nameof(IsEnabled)} was not called");
			}

			if (!_logEnabled)
			{
				if (_logCalled)
				{
					throw new Exception($"{nameof(Log)} was called with disabled logging");
				}
				return;
			}

			if (_actualIsEnabledLogLevel != _logLevel)
			{
				throw new Exception($"{nameof(IsEnabled)} was called with unexpected log level:{_actualIsEnabledLogLevel}. Expected:{_logLevel}");
			}

			if (!_logCalled)
			{
				throw new Exception($"{nameof(Log)} was not called");
			}

			if (_actualLogLogLevel != _logLevel)
			{
				throw new Exception($"{nameof(Log)} was called with unexpected log level:{_actualIsEnabledLogLevel}. Expected:{_logLevel}");
			}

			if (_actualMessage != _message)
			{
				throw new Exception($"{nameof(Log)} was called with unexpected log message:{_actualMessage}. Expected:{_message}");
			}

			if (_actualEventId.Id != _eventId.Id || _actualEventId.Name != _eventId.Name)
			{
				throw new Exception($"{nameof(Log)} was called with unexpected log eventId:{_actualEventId.Id}-{_actualEventId.Name}. Expected:{_eventId.Id}-{_eventId.Name}");
			}
		}

		private class Disposable : IDisposable
		{
			public void Dispose()
			{
			}
		}
	}
}
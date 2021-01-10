using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Roslyn
{
	public class LoggerMethodParameter
	{
		private readonly bool _isException;
		private readonly LogLevel? _minLogLevel;
		private readonly IParameterSymbol _parameterSymbol;
		private readonly ParameterSyntax _parameterSyntax;

		public LoggerMethodParameter(
			ParameterSyntax parameterSyntax,
			IParameterSymbol parameterSymbol,
			bool isException,
			LogLevel? minLogLevel
			)
		{
			_parameterSyntax = parameterSyntax;
			_parameterSymbol = parameterSymbol;
			_isException = isException;
			_minLogLevel = minLogLevel;
		}

		public ParameterSyntax ParameterSyntax => _parameterSyntax;

		public bool IsException => _isException;

		public IParameterSymbol ParameterSymbol => _parameterSymbol;

		public LogLevel? MinLogLevel => _minLogLevel;
	}
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.Logging.CodeGeneration.Roslyn
{
	public class LoggerMethodParameter
	{
		private readonly ParameterSyntax _parameterSyntax;
		private readonly bool _isException;
		private readonly IParameterSymbol _parameterSymbol;

		public LoggerMethodParameter(
		  ParameterSyntax parameterSyntax,
		  IParameterSymbol parameterSymbol,
		  bool isException)
		{
			_parameterSyntax = parameterSyntax;
			_parameterSymbol = parameterSymbol;
			_isException = isException;
		}

		public ParameterSyntax ParameterSyntax => _parameterSyntax;

		public bool IsException => _isException;

		public IParameterSymbol ParameterSymbol => _parameterSymbol;
	}
}
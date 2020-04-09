using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneration.Roslyn.Logger
{
	public class LoggerMethodParameter
	{
		private readonly ParameterSyntax _parameterSyntax;
		private readonly bool _isException;
		private readonly TypeInfo _typeInfo;

		public LoggerMethodParameter(
		  ParameterSyntax parameterSyntax,
		  TypeInfo typeInfo,
		  bool isException)
		{
			_parameterSyntax = parameterSyntax;
			_typeInfo = typeInfo;
			_isException = isException;
		}

		public ParameterSyntax ParameterSyntax => _parameterSyntax;

		public bool IsException => _isException;

		public TypeInfo Info => _typeInfo;
	}
}
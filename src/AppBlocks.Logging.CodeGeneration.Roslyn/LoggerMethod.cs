using System.Collections.Immutable;
using AppBlocks.CodeGeneration.Roslyn.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.Logging.CodeGeneration.Roslyn
{
	public class LoggerMethod
	{
		private readonly Microsoft.Extensions.Logging.LogLevel _logLevel;
		private readonly string _message;
		private readonly string _delegateFieldName;
		private readonly MethodDeclarationSyntax _methodDeclarationSyntax;
		private readonly ImmutableArray<LoggerMethodParameter> _parameters;

		public LoggerMethod(MethodDeclarationSyntax methodDeclarationSyntax, Microsoft.Extensions.Logging.LogLevel logLevel, string message, string delegateFieldName, ImmutableArray<LoggerMethodParameter> parameters)
		{
			_logLevel = logLevel;
			_message = message;
			_delegateFieldName = delegateFieldName;
			_methodDeclarationSyntax = methodDeclarationSyntax;
			_parameters = parameters;
		}

		public MethodDeclarationSyntax MethodDeclarationSyntax => _methodDeclarationSyntax;
		public ImmutableArray<LoggerMethodParameter> Parameters => _parameters;
		public Microsoft.Extensions.Logging.LogLevel Level => _logLevel;
		public string Message => _message;
		public string DelegateFieldName => _delegateFieldName;
	}
}
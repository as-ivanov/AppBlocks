using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneration.Roslyn.Logger
{
	public class LoggerMethod
	{
		private readonly Microsoft.Extensions.Logging.LogLevel _logLevel;
		private readonly string _message;
		private readonly MethodDeclarationSyntax _methodDeclarationSyntax;
		private readonly ImmutableArray<LoggerMethodParameter> _parameters;

		public LoggerMethod(MethodDeclarationSyntax methodDeclarationSyntax, Microsoft.Extensions.Logging.LogLevel logLevel, string message, ImmutableArray<LoggerMethodParameter> parameters)
		{
			_logLevel = logLevel;
			_message = message;
			_methodDeclarationSyntax = methodDeclarationSyntax;
			_parameters = parameters;
		}

		public MethodDeclarationSyntax MethodDeclarationSyntax => _methodDeclarationSyntax;
		public ImmutableArray<LoggerMethodParameter> Parameters => _parameters;
		public Microsoft.Extensions.Logging.LogLevel Level => _logLevel;
		public string Message => _message;
	}
}
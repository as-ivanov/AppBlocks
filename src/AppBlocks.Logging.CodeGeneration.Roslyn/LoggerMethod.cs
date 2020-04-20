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
		private readonly MethodDeclarationSyntax _methodDeclarationSyntax;
		private readonly ImmutableArray<LoggerMethodParameter> _parameters;
		private readonly string _methodName;
		private readonly string _methodNameCamelCase;

		public LoggerMethod(MethodDeclarationSyntax methodDeclarationSyntax, Microsoft.Extensions.Logging.LogLevel logLevel, string message, ImmutableArray<LoggerMethodParameter> parameters)
		{
			_logLevel = logLevel;
			_message = message;
			_methodDeclarationSyntax = methodDeclarationSyntax;
			_parameters = parameters;
			_methodName = methodDeclarationSyntax.Identifier.WithoutTrivia().Text;
			_methodNameCamelCase = _methodName.ToCamelCase();
		}

		public MethodDeclarationSyntax MethodDeclarationSyntax => _methodDeclarationSyntax;
		public ImmutableArray<LoggerMethodParameter> Parameters => _parameters;
		public Microsoft.Extensions.Logging.LogLevel Level => _logLevel;
		public string Message => _message;
		public string MethodName => _methodName;
		public string MethodNameCamelCase => _methodNameCamelCase;
	}
}
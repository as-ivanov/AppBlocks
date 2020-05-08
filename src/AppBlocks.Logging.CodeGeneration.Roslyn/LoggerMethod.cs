using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.CodeGeneration.Roslyn
{
	public class LoggerMethod
	{
		private readonly INamedTypeSymbol _declaredInterfaceSymbol;
		private readonly string _delegateFieldName;
		private readonly LogLevel _logLevel;
		private readonly string _message;
		private readonly MethodDeclarationSyntax _methodDeclarationSyntax;
		private readonly ImmutableArray<LoggerMethodParameter> _parameters;

		public LoggerMethod(MethodDeclarationSyntax methodDeclarationSyntax, INamedTypeSymbol declaredInterfaceSymbol,
			LogLevel logLevel, string message, string delegateFieldName, ImmutableArray<LoggerMethodParameter> parameters)
		{
			_logLevel = logLevel;
			_message = message;
			_delegateFieldName = delegateFieldName;
			_methodDeclarationSyntax = methodDeclarationSyntax;
			_declaredInterfaceSymbol = declaredInterfaceSymbol;
			_parameters = parameters;
		}

		public MethodDeclarationSyntax MethodDeclarationSyntax => _methodDeclarationSyntax;
		public ImmutableArray<LoggerMethodParameter> Parameters => _parameters;
		public LogLevel Level => _logLevel;
		public string Message => _message;
		public string DelegateFieldName => _delegateFieldName;
		public INamedTypeSymbol DeclaredInterfaceSymbol => _declaredInterfaceSymbol;
	}
}
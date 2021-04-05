using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
		private readonly LogLevel[] _subLevels;
		private readonly bool _innerLoggerMethod;

		public LoggerMethod(
			MethodDeclarationSyntax methodDeclarationSyntax,
			INamedTypeSymbol declaredInterfaceSymbol,
			LogLevel logLevel,
			string message,
			string delegateFieldName,
			ImmutableArray<LoggerMethodParameter> parameters,
			LogLevel[] subLevels)
		{

			_innerLoggerMethod = false;
			_logLevel = logLevel;
			_message = message;
			_delegateFieldName = delegateFieldName;
			_methodDeclarationSyntax = methodDeclarationSyntax;
			_declaredInterfaceSymbol = declaredInterfaceSymbol;
			_parameters = parameters;
			_subLevels = subLevels;
		}

		public LoggerMethod(
			MethodDeclarationSyntax methodDeclarationSyntax,
			INamedTypeSymbol declaredInterfaceSymbol
			)
		{
			_innerLoggerMethod = true;
			_methodDeclarationSyntax = methodDeclarationSyntax;
			_declaredInterfaceSymbol = declaredInterfaceSymbol;
		}

		public MethodDeclarationSyntax MethodDeclarationSyntax => _methodDeclarationSyntax;
		public ImmutableArray<LoggerMethodParameter> Parameters => _parameters;
		public LogLevel Level => _logLevel;
		public string Message => _message;
		public string DelegateFieldName => _delegateFieldName;
		public INamedTypeSymbol DeclaredInterfaceSymbol => _declaredInterfaceSymbol;
		public LogLevel[] SubLevels => _subLevels;
		public bool InnerLoggerMethod => _innerLoggerMethod;

		public ImmutableArray<LoggerMethodParameter> GetLoggerMethodParametersForSubLevel(LogLevel logLevel)
		{
			if (_subLevels.Length == 0)
			{
				throw new Exception();
			}

			return Parameters.Where(parameter => !parameter.MinLogLevel.HasValue || parameter.MinLogLevel >= logLevel)
				.ToImmutableArray();
		}

		public Dictionary<LogLevel, ImmutableArray<LoggerMethodParameter>> GetLoggerMethodParametersByLogLevels()
		{
			if (_subLevels.Length == 0)
			{
				throw new Exception();
			}

			return _subLevels
				.Select(_ => new { Sublevel = _, Parametes = Parameters.Where(parameter => !parameter.MinLogLevel.HasValue || parameter.MinLogLevel >= _) }).ToDictionary(_ => _.Sublevel, _ => _.Parametes.ToImmutableArray());
		}
	}
}

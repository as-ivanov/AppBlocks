using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneration.Roslyn.Logger
{
	internal static class LoggerDescriptorExtensions
	{
		public static LoggerDescriptor ToLoggerDescriptor(this TypeDeclarationSyntax typeDeclarationSyntax, TransformationContext context, AttributeData attributeData)
		{
			string baseClass = null;
			var baseList = typeDeclarationSyntax.BaseList?.Types;
			if (baseList.HasValue
				&& baseList.Value.Any())
			{
				var firstOne = baseList.Value.First();
				var fullName = firstOne.Type.ToString();
				baseClass = fullName.GetClassName();
			}

			var compilation = context.Compilation;

			var inheritedInterfaceTypes = attributeData.ConstructorArguments == null ? Array.Empty<string>() : attributeData.ConstructorArguments.SelectMany(_ => _.Values).Select(_ => _.Value as string).ToArray();

			var className = typeDeclarationSyntax.Identifier.WithoutTrivia().Text.GetClassName();

			var exceptionType = compilation.GetTypeByMetadataName(typeof(Exception).FullName);

			return new LoggerDescriptor(
			  typeDeclarationSyntax,
			  className,
			  baseClass,
			  inheritedInterfaceTypes,
			  typeDeclarationSyntax.GetLoggerMethods(context, exceptionType));
		}

		private static ImmutableArray<LoggerMethod> GetLoggerMethods(this TypeDeclarationSyntax typeDeclarationSyntax,
		  TransformationContext context, INamedTypeSymbol exceptionType)
		{
			return typeDeclarationSyntax.Members.OfType<MethodDeclarationSyntax>()
			  .Select(p => p.ToLoggerMethod(context, exceptionType))
			  .Where(_ => _ != null)
			  .ToImmutableArray();
		}

		private static LoggerMethod ToLoggerMethod(this MethodDeclarationSyntax methodDeclarationSyntax, TransformationContext context, INamedTypeSymbol exceptionType)
		{
			var attributeData = GetAttributeData(context, methodDeclarationSyntax);
			var logSeverityAttributeData =
			  attributeData.FirstOrDefault(_ => _.AttributeClass.Name == nameof(Attributes.LoggerMethodStubAttribute));

			Microsoft.Extensions.Logging.LogLevel level;
			string message;
			if (logSeverityAttributeData == null)
			{
				level = Microsoft.Extensions.Logging.LogLevel.Information;
				message = methodDeclarationSyntax.Identifier.ToFullString();
			}
			else
			{
				var constructorArguments = logSeverityAttributeData.ConstructorArguments;
				level = constructorArguments.Length > 0
					? Enum.Parse<Microsoft.Extensions.Logging.LogLevel>(constructorArguments[0].Value.ToString())
					: Microsoft.Extensions.Logging.LogLevel.Information;
				message = constructorArguments.Length > 1 ? constructorArguments[1].Value as string : methodDeclarationSyntax.Identifier.ToFullString();
			}

			var parameters = methodDeclarationSyntax.ParameterList.Parameters
				.Select(p => p.ToLoggerMethodParameter(context, exceptionType)).ToImmutableArray();

			return new LoggerMethod(
			  methodDeclarationSyntax,
			  level,
			  message,
			  parameters);
		}

		private static LoggerMethodParameter ToLoggerMethodParameter(this ParameterSyntax parameterSyntax,
		  TransformationContext context, INamedTypeSymbol exceptionType)
		{
			var typeInfo = context.SemanticModel.GetTypeInfo(parameterSyntax.Type);
			var conversionInfo = context.Compilation.ClassifyCommonConversion(typeInfo.Type, exceptionType);
			return new LoggerMethodParameter(parameterSyntax, typeInfo, conversionInfo.Exists && conversionInfo.IsImplicit);
		}

		private static ImmutableArray<AttributeData> GetAttributeData(TransformationContext context, SyntaxNode syntaxNode)
		{
			switch (syntaxNode)
			{
				case CompilationUnitSyntax syntax:
					return context.Compilation.Assembly.GetAttributes()
					  .Where(x => x.ApplicationSyntaxReference.SyntaxTree == syntax.SyntaxTree).ToImmutableArray();
				default:
					return context.SemanticModel.GetDeclaredSymbol(syntaxNode)?.GetAttributes() ?? ImmutableArray<AttributeData>.Empty;
			}
		}
	}
}
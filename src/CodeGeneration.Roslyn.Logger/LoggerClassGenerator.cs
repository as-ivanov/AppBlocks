using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeGeneration.Roslyn.Logger
{
	public class LoggerClassGenerator : IRichCodeGenerator
	{
		private const string LoggerFieldName = "_logger";
		private readonly AttributeData _attributeData;

		public LoggerClassGenerator(AttributeData attributeData)
		{
			_attributeData = attributeData ?? throw new ArgumentNullException(nameof(attributeData));
		}

		public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(
		  TransformationContext context,
		  IProgress<Diagnostic> progress,
		  CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<RichGenerationResult> GenerateRichAsync(TransformationContext context, IProgress<Diagnostic> progress,
		  CancellationToken cancellationToken)
		{
			if (!(context.ProcessingNode is TypeDeclarationSyntax tds) || !tds.IsKind(SyntaxKind.InterfaceDeclaration))
			{
				throw new Exception($"{nameof(Attributes.LoggerStubAttribute)} must be declared on interface.");
			}

			return GenerateAsync(tds, context, _attributeData);
		}

		private static Task<RichGenerationResult> GenerateAsync(TypeDeclarationSyntax typeDeclaration,
		  TransformationContext context, AttributeData attributeData)
		{
			var descriptor = typeDeclaration.ToLoggerDescriptor(context, attributeData);


			if (!(context.ProcessingNode.Parent is NamespaceDeclarationSyntax namespaceDeclarationSyntax))
			{
				throw new Exception($"Failed to determine namespace for type:'{context.ProcessingNode.Parent}'.");
			}

			var usingDirectives = GetUsingDirectives();

			var loggerClass = GetLoggerClass(descriptor);

			var @namespace = NamespaceDeclaration(namespaceDeclarationSyntax.Name)
			  .AddUsings(usingDirectives)
			  .AddMembers(loggerClass);

			var generatedMembers = new List<MemberDeclarationSyntax> { @namespace };
			var result = new RichGenerationResult { Members = new SyntaxList<MemberDeclarationSyntax>(generatedMembers) };

			return Task.FromResult(result);
		}

		private static UsingDirectiveSyntax[] GetUsingDirectives()
		{
			var list =
			  List(
				new[]
				{
			UsingDirective(ParseName(typeof(Action).Namespace)),
			UsingDirective(ParseName(typeof(ILogger).Namespace))
				});

			return list.ToArray();
		}

		private static ClassDeclarationSyntax GetLoggerClass(LoggerDescriptor loggerDescriptor)
		{
			var baseTypes = GetLoggerBaseList(loggerDescriptor, loggerDescriptor.InheritedInterfaceTypes);
			var classDeclaration = ClassDeclaration(loggerDescriptor.ClassName)
			  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
			  .AddBaseListTypes(baseTypes)
			  .AddMembers(GetGeneralLoggerFields(loggerDescriptor))
			  .AddMembers(GetLoggingDelegateLoggerFields(loggerDescriptor))
			  .AddMembers(GetLoggerConstructor(loggerDescriptor, loggerDescriptor.ClassName))
			  .AddMembers(GetLoggerMethods(loggerDescriptor));

			return classDeclaration;
		}

		private static BaseTypeSyntax[] GetLoggerBaseList(LoggerDescriptor loggerDescriptor, string[] inheritedInterfaceTypes)
		{
			var baseTypeList = new List<BaseTypeSyntax>();

			if (loggerDescriptor.BaseClass != null)
			{
				baseTypeList.Add(SimpleBaseType(IdentifierName(loggerDescriptor.BaseClass)));
			}

			baseTypeList.Add(SimpleBaseType(IdentifierName(loggerDescriptor.DeclarationSyntax.Identifier)));
			if (inheritedInterfaceTypes != null)
			{
				foreach (var inheritedInterfaceType in inheritedInterfaceTypes)
				{
					baseTypeList.Add(SimpleBaseType(IdentifierName(inheritedInterfaceType)));
				}
			}
			return baseTypeList.ToArray();
		}

		private static MemberDeclarationSyntax[] GetGeneralLoggerFields(LoggerDescriptor loggerDescriptor)
		{
			if (loggerDescriptor.BaseClass != null)
				return new MemberDeclarationSyntax[0];

			return new MemberDeclarationSyntax[]
			{
		FieldDeclaration(VariableDeclaration(IdentifierName(nameof(ILogger)))
			  .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(LoggerFieldName)))))
		  .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.ReadOnlyKeyword)))
			};
		}

		private static MemberDeclarationSyntax[] GetLoggingDelegateLoggerFields(LoggerDescriptor loggerDescriptor)
		{
			var fieldMemberDeclarations = new List<MemberDeclarationSyntax>();
			for (var index = 0; index < loggerDescriptor.Methods.Length; index++)
			{
				var method = loggerDescriptor.Methods[index];
				var defineMethodParameterTypes = GetLoggingDelegateParameterTypes(method, true);

				var sb = new StringBuilder(method.Message);
				for (var i = 0; i < method.Parameters.Length; ++i)
				{
					var parameter = method.Parameters[i];
					if (i + 1 < method.Parameters.Length || !parameter.IsException)
					{
						var pascalCaseParameter = parameter.ParameterSyntax.Identifier.WithoutTrivia().ToFullString().ToPascalCase();
						sb.Append($". {pascalCaseParameter}: \"{{{pascalCaseParameter}}}\"");
					}
				}

				var message = sb.ToString();

				var definitionMethodExpression = defineMethodParameterTypes.Arguments.Any()
				  ? MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(nameof(LoggerMessage)),
					GenericName(Identifier(nameof(LoggerMessage.Define))).WithTypeArgumentList(defineMethodParameterTypes))
				  : MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(nameof(LoggerMessage)),
					IdentifierName(nameof(LoggerMessage.Define)));
				var declaration = FieldDeclaration(
					VariableDeclaration(
						GenericName(Identifier(nameof(Action)))
						  .WithTypeArgumentList(GetLoggingDelegateParameterTypes(method, false)))
					  .WithVariables(
						SingletonSeparatedList(
						  VariableDeclarator(
							  Identifier("_" + method.MethodDeclarationSyntax.Identifier.WithoutTrivia().ToCamelCase()))
							.WithInitializer(
							  EqualsValueClause(
								InvocationExpression(definitionMethodExpression)
								  .WithArgumentList(
									ArgumentList(
									  SeparatedList<ArgumentSyntax>(
										new SyntaxNodeOrToken[]
										{
								  Argument(
									MemberAccessExpression(
									  SyntaxKind.SimpleMemberAccessExpression,
									  IdentifierName(typeof(Microsoft.Extensions.Logging.LogLevel).FullName),
									  IdentifierName(method.Level.ToString()))),
								  Token(SyntaxKind.CommaToken),
								  Argument(
									ObjectCreationExpression(
										IdentifierName(nameof(EventId)))
									  .WithArgumentList(
										ArgumentList(
										  SeparatedList<ArgumentSyntax>(
											new SyntaxNodeOrToken[]
											{
											  Argument(
												LiteralExpression(
												  SyntaxKind.NumericLiteralExpression,
												  Literal(index + 1))),
											  Token(SyntaxKind.CommaToken),
											  Argument(
												InvocationExpression(
													IdentifierName("nameof"))
												  .WithArgumentList(
													ArgumentList(
													  SingletonSeparatedList(
														Argument(
														  IdentifierName(method.MethodDeclarationSyntax.Identifier.WithoutTrivia()))))))
											})))),
								  Token(SyntaxKind.CommaToken),
								  Argument(
									LiteralExpression(
									  SyntaxKind.StringLiteralExpression,
									  Literal(message.EscapeCSharpString(), message)))
										}))))))))
				  .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.ReadOnlyKeyword)));

				fieldMemberDeclarations.Add(declaration);
			}

			return fieldMemberDeclarations.ToArray();
		}

		private static TypeArgumentListSyntax GetLoggingDelegateParameterTypes(LoggerMethod loggerMethod, bool definition)
		{
			var result = new List<SyntaxNodeOrToken>();

			if (!definition)
			{
				result.Add(IdentifierName(typeof(ILogger).FullName));
			}

			for (var index = 0; index < loggerMethod.Parameters.Length; index++)
			{
				var parameter = loggerMethod.Parameters[index];
				if (index == loggerMethod.Parameters.Length - 1 && parameter.IsException)
				{
					continue;
				}

				if (result.Any())
				{
					result.Add(Token(SyntaxKind.CommaToken));
				}

				var typeSyntax = parameter.ParameterSyntax.Type;
				if (parameter.Info.Type.TypeKind == TypeKind.TypeParameter || parameter.Info.Type.IsReferenceType)
				{
					typeSyntax = IdentifierName("object");
				}

				result.Add(IdentifierName(typeSyntax.GetText().ToString().Trim()));
			}

			if (!definition)
			{
				if (result.Any())
				{
					result.Add(Token(SyntaxKind.CommaToken));
				}

				result.Add(IdentifierName(typeof(Exception).FullName));
			}

			return TypeArgumentList(SeparatedList<TypeSyntax>(result));
		}

		private static ConstructorDeclarationSyntax GetLoggerConstructor(LoggerDescriptor loggerDescriptor, string className)
		{
			const string loggerFactoryVariableName = "loggerFactory";
			var constructorDeclaration = ConstructorDeclaration(
				Identifier(className))
			  .WithModifiers(
				TokenList(
				  Token(SyntaxKind.PublicKeyword)))
			  .WithParameterList(
				ParameterList(
				  SingletonSeparatedList(
					Parameter(
						Identifier(loggerFactoryVariableName))
					  .WithType(
						IdentifierName(typeof(ILoggerFactory).FullName)))));

			if (loggerDescriptor.BaseClass != null)
			{
				constructorDeclaration = constructorDeclaration
				  .WithInitializer(
					ConstructorInitializer(
					  SyntaxKind.BaseConstructorInitializer,
					  ArgumentList(
						SingletonSeparatedList(
						  Argument(
							IdentifierName(loggerFactoryVariableName))))))
				  .WithBody(
					Block());
			}
			else
			{
				var createLoggerInvocation = InvocationExpression(
					MemberAccessExpression(
					  SyntaxKind.SimpleMemberAccessExpression,
					  IdentifierName(loggerFactoryVariableName),
					  IdentifierName(nameof(ILoggerFactory.CreateLogger))))
				  .WithArgumentList(
					ArgumentList(
					  SingletonSeparatedList(
						Argument(
						  InvocationExpression(
							IdentifierName(nameof(Type.GetType)))))));

				constructorDeclaration = constructorDeclaration
				  .WithBody(
					Block(
					  SingletonList<StatementSyntax>(
						ExpressionStatement(
						  AssignmentExpression(
							SyntaxKind.SimpleAssignmentExpression,
							IdentifierName(LoggerFieldName),
							createLoggerInvocation
						  )))));
			}

			return constructorDeclaration;
		}

		private static MemberDeclarationSyntax[] GetLoggerMethods(LoggerDescriptor loggerDescriptor)
		{
			var publicKeywordToken = Token(SyntaxKind.PublicKeyword);

			var members = new List<MemberDeclarationSyntax>();
			foreach (var method in loggerDescriptor.Methods)
			{
				var methodParameters = method.Parameters.Select(_ => _.ParameterSyntax).ToArray();
				var methodDeclaration =
				  MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), method.MethodDeclarationSyntax.Identifier)
				  .WithTypeParameterList(method.MethodDeclarationSyntax.TypeParameterList)
				  .WithConstraintClauses(method.MethodDeclarationSyntax.ConstraintClauses)
				  .WithModifiers(method.MethodDeclarationSyntax.Modifiers)
				  .AddModifiers(publicKeywordToken)
				  .AddParameterListParameters(methodParameters)
				  .WithBody(Block(GetLoggerMethodBody(method)));
				members.Add(methodDeclaration);
			}
			return members.ToArray();
		}

		private static SyntaxList<StatementSyntax> GetLoggerMethodBody(LoggerMethod loggerMethod)
		{
			return SingletonList<StatementSyntax>(
			  IfStatement(
				InvocationExpression(
					MemberAccessExpression(
					  SyntaxKind.SimpleMemberAccessExpression,
					  IdentifierName(LoggerFieldName),
					  IdentifierName(nameof(ILogger.IsEnabled))))
				  .WithArgumentList(
					ArgumentList(
					  SingletonSeparatedList(
						Argument(
						  MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							IdentifierName(typeof(Microsoft.Extensions.Logging.LogLevel).FullName),
							IdentifierName(loggerMethod.Level.ToString())))))),
				Block(
				  SingletonList<StatementSyntax>(
					ExpressionStatement(
					  InvocationExpression(
						  IdentifierName("_" + loggerMethod.MethodDeclarationSyntax.Identifier.WithoutTrivia().ToCamelCase()))
						.WithArgumentList(GetLoggingDelegateCallArgumentList(loggerMethod)))))));
		}

		private static ArgumentListSyntax GetLoggingDelegateCallArgumentList(LoggerMethod loggerMethod)
		{
			var arguments = new List<SyntaxNodeOrToken> { Argument(IdentifierName(LoggerFieldName)) };
			foreach (var parameter in loggerMethod.Parameters)
			{
				if (arguments.Any())
				{
					arguments.Add(Token(SyntaxKind.CommaToken));
				}

				arguments.Add(Argument(IdentifierName(parameter.ParameterSyntax.Identifier.WithoutTrivia().ToCamelCase())));
			}

			var lastParameter = loggerMethod.Parameters.LastOrDefault();
			if (lastParameter == null || !lastParameter.IsException)
			{
				if (arguments.Any())
				{
					arguments.Add(Token(SyntaxKind.CommaToken));
				}

				arguments.Add(Argument(IdentifierName("null")));
			}

			return ArgumentList(SeparatedList<ArgumentSyntax>(arguments));
		}
	}
}
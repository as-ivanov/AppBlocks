using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppBlocks.CodeGeneration.Roslyn.Common;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AppBlocks.Logging.CodeGeneration.Roslyn
{
	public class LoggerClassGenerator : InterfaceImplementationGenerator<LoggerDescriptor>
	{
		private const string LoggerFieldName = "_logger";

		private static readonly TypeSyntax _loggerMessageGlobalTypeSyntax = typeof(LoggerMessage).GetGlobalTypeSyntax();
		private static readonly TypeSyntax _logLevelGlobalTypeSyntax = typeof(LogLevel).GetGlobalTypeSyntax();
		private static readonly TypeSyntax _eventIdGlobalTypeSyntax = typeof(EventId).GetGlobalTypeSyntax();
		private static readonly TypeSyntax _loggerGlobalTypeSyntax = typeof(ILogger).GetGlobalTypeSyntax();
		private static readonly TypeSyntax _exceptionGlobalTypeSyntax = typeof(Exception).GetGlobalTypeSyntax();
		private static readonly TypeSyntax _loggerFactoryGlobalTypeSyntax = typeof(ILoggerFactory).GetGlobalTypeSyntax();
		private static readonly TypeSyntax _loggerFactoryExtensionsGlobalTypeSyntax = typeof(LoggerFactoryExtensions).GetGlobalTypeSyntax();

		public LoggerClassGenerator(AttributeData attributeData) : base(attributeData, new Version(1, 0, 0))
		{
		}

		protected override LoggerDescriptor GetImplementationDescriptor(TypeDeclarationSyntax typeDeclaration,
			TransformationContext context, AttributeData attributeData)
		{
			return typeDeclaration.ToLoggerDescriptor(context, attributeData);
		}

		protected override IEnumerable<MemberDeclarationSyntax> GetFields(LoggerDescriptor loggerDescriptor)
		{
			var generalLoggerFields = GetGeneralLoggerFields(loggerDescriptor);
			var loggingDelegateLoggerFields = GetLoggingDelegateLoggerFields(loggerDescriptor);
			return generalLoggerFields.Union(loggingDelegateLoggerFields);
		}

		private static IEnumerable<MemberDeclarationSyntax> GetGeneralLoggerFields(LoggerDescriptor loggerDescriptor)
		{
			return new MemberDeclarationSyntax[]
			{
				FieldDeclaration(VariableDeclaration(_loggerGlobalTypeSyntax)
						.WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(LoggerFieldName)))))
					.WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)))
			};
		}

		private static IEnumerable<MemberDeclarationSyntax> GetLoggingDelegateLoggerFields(LoggerDescriptor loggerDescriptor)
		{
			var fieldMemberDeclarations = new List<MemberDeclarationSyntax>(loggerDescriptor.Methods.Length);
			for (var index = 0; index < loggerDescriptor.Methods.Length; index++)
			{
				var method = loggerDescriptor.Methods[index];
				var defineMethodParameterTypes = GetLoggingDelegateParameterTypes(method, true);

				var message = method.Message;
				if (method.Parameters.Length > 0)
				{
					var sb = new StringBuilder();
					for (var i = 0; i < method.Parameters.Length; ++i)
					{
						var parameter = method.Parameters[i];
						if (i + 1 < method.Parameters.Length || !parameter.IsException)
						{
							var pascalCaseParameter =
								parameter.ParameterSyntax.Identifier.WithoutTrivia().ToFullString().ToPascalCase();
							if (sb.Length > 0)
							{
								sb.Append(" ");
							}

							sb.Append($"{pascalCaseParameter}: '{{{pascalCaseParameter}}}'");
						}
					}

					if (sb.Length > 0)
					{
						message = $"{message} ({sb})";
					}
				}

				static NameSyntax GetGlobalMethodIdentifier(LoggerMethod method)
				{
					var methodIdentifier = IdentifierName(method.DeclaredInterfaceSymbol.GetFullTypeName() + "." + method.MethodDeclarationSyntax.Identifier.WithoutTrivia().ToFullString());
					return AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), methodIdentifier);
				}

				var definitionMethodExpression = defineMethodParameterTypes.Arguments.Any()
					? MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, _loggerMessageGlobalTypeSyntax,
						GenericName(Identifier(nameof(LoggerMessage.Define))).WithTypeArgumentList(defineMethodParameterTypes))
					: MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, _loggerMessageGlobalTypeSyntax,
						IdentifierName(nameof(LoggerMessage.Define)));
				var declaration = FieldDeclaration(
						VariableDeclaration(
								AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)),
									GenericName(Identifier(typeof(Action).FullName))
										.WithTypeArgumentList(GetLoggingDelegateParameterTypes(method, false))))
							.WithVariables(
								SingletonSeparatedList(
									VariableDeclarator(
											Identifier(method.DelegateFieldName))
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
																			_logLevelGlobalTypeSyntax,
																			IdentifierName(method.Level.ToString()))),
																	Token(SyntaxKind.CommaToken),
																	Argument(
																		ObjectCreationExpression(_eventIdGlobalTypeSyntax)
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
																												Argument(GetGlobalMethodIdentifier(method))))))
																						})))),
																	Token(SyntaxKind.CommaToken),
																	Argument(message.GetLiteralExpression())
																}))))))))
					.WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword),
						Token(SyntaxKind.ReadOnlyKeyword)));

				fieldMemberDeclarations.Add(declaration);
			}

			return fieldMemberDeclarations;
		}

		private static TypeArgumentListSyntax GetLoggingDelegateParameterTypes(LoggerMethod loggerMethod, bool definition)
		{
			var result = new List<SyntaxNodeOrToken>(loggerMethod.Parameters.Length);

			if (!definition)
			{
				result.Add(_loggerGlobalTypeSyntax);
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
				if (parameter.ParameterSymbol.Type.TypeKind == TypeKind.TypeParameter || parameter.ParameterSymbol.Type.IsReferenceType)
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

				result.Add(_exceptionGlobalTypeSyntax);
			}

			return TypeArgumentList(SeparatedList<TypeSyntax>(result));
		}

		protected override IEnumerable<ConstructorDeclarationSyntax> GetConstructors(LoggerDescriptor loggerDescriptor)
		{
			const string loggerFactoryVariableName = "loggerFactory";
			var constructorDeclaration = ConstructorDeclaration(
					Identifier(loggerDescriptor.ClassName))
				.WithModifiers(
					TokenList(
						Token(SyntaxKind.PublicKeyword)))
				.WithParameterList(
					ParameterList(
						SingletonSeparatedList(
							Parameter(
									Identifier(loggerFactoryVariableName))
								.WithType(_loggerFactoryGlobalTypeSyntax))));


			var createLoggerInvocation = InvocationExpression(
					MemberAccessExpression(
						SyntaxKind.SimpleMemberAccessExpression,
						_loggerFactoryExtensionsGlobalTypeSyntax,
						IdentifierName(nameof(ILoggerFactory.CreateLogger))))
				.WithArgumentList(
					ArgumentList(
						SeparatedList<ArgumentSyntax>(
							new SyntaxNodeOrToken[]
							{
								Argument(
									IdentifierName(loggerFactoryVariableName)),
								Token(SyntaxKind.CommaToken),
								Argument(
									InvocationExpression(
										IdentifierName(nameof(GetType))))
							})));

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

			yield return constructorDeclaration;
		}

		protected override IEnumerable<MemberDeclarationSyntax> GetMethods(LoggerDescriptor loggerDescriptor)
		{
			var members = new List<MemberDeclarationSyntax>(loggerDescriptor.Methods.Length);
			foreach (var method in loggerDescriptor.Methods)
			{
				var interfaceGlobalQualifiedName = AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName(method.DeclaredInterfaceSymbol.GetFullTypeName()));
				var explicitInterfaceSpecifier = ExplicitInterfaceSpecifier(interfaceGlobalQualifiedName);

				var methodConstraintClauses = method.MethodDeclarationSyntax.ConstraintClauses
					.GetAllowedImplicitImplementationConstraintClause();

				var methodParameters = method.Parameters.Select(_ => _.ParameterSyntax).ToArray();
				var methodDeclaration =
					MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), method.MethodDeclarationSyntax.Identifier)
						.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier)
						.WithTypeParameterList(method.MethodDeclarationSyntax.TypeParameterList)
						.WithConstraintClauses(methodConstraintClauses)
						.AddParameterListParameters(methodParameters)
						.WithBody(Block(GetLoggerMethodBody(method)));
				members.Add(methodDeclaration);
			}

			return members;
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
											_logLevelGlobalTypeSyntax,
											IdentifierName(loggerMethod.Level.ToString())))))),
					Block(
						SingletonList<StatementSyntax>(
							ExpressionStatement(
								InvocationExpression(
										IdentifierName(loggerMethod.DelegateFieldName))
									.WithArgumentList(GetLoggingDelegateCallArgumentList(loggerMethod)))))));
		}

		private static ArgumentListSyntax GetLoggingDelegateCallArgumentList(LoggerMethod loggerMethod)
		{
			var arguments = new List<SyntaxNodeOrToken> {Argument(IdentifierName(LoggerFieldName))};
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
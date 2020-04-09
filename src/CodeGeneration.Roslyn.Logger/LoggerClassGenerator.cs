using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeGeneration.Roslyn.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeGeneration.Roslyn.Logger
{
	public class LoggerClassGenerator : InterfaceImplementationGenerator<LoggerDescriptor>
	{
		private const string LoggerFieldName = "_logger";

		public LoggerClassGenerator(AttributeData attributeData) : base(attributeData, new Version(1, 0, 0))
		{
		}

		protected override string[] GetNamespaces()
		{
			return new[] {typeof(Action).Namespace, typeof(ILogger).Namespace};
		}

		protected override LoggerDescriptor GetImplementationDescriptor(TypeDeclarationSyntax typeDeclaration,
			TransformationContext context, AttributeData attributeData)
		{
			return typeDeclaration.ToLoggerDescriptor(context, attributeData);
		}

		protected override MemberDeclarationSyntax[] GetFields(LoggerDescriptor loggerDescriptor)
		{
			var generalLoggerFields = GetGeneralLoggerFields(loggerDescriptor);
			var loggingDelegateLoggerFields = GetLoggingDelegateLoggerFields(loggerDescriptor);
			return generalLoggerFields.Union(loggingDelegateLoggerFields).ToArray();
		}

		private static MemberDeclarationSyntax[] GetGeneralLoggerFields(LoggerDescriptor loggerDescriptor)
		{
			if (loggerDescriptor.BaseClassName != null)
			{
				return new MemberDeclarationSyntax[0];
			}

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
						var pascalCaseParameter =
							parameter.ParameterSyntax.Identifier.WithoutTrivia().ToFullString().ToPascalCase();
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
																													IdentifierName(method.MethodDeclarationSyntax.Identifier
																														.WithoutTrivia()))))))
																						})))),
																	Token(SyntaxKind.CommaToken),
																	Argument(message.GetLiteralExpression())
																}))))))))
					.WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword),
						Token(SyntaxKind.ReadOnlyKeyword)));

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

		protected override ConstructorDeclarationSyntax[] GetConstructors(LoggerDescriptor loggerDescriptor)
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
								.WithType(
									IdentifierName(typeof(ILoggerFactory).FullName)))));

			if (loggerDescriptor.BaseClassName != null)
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

			return new[] {constructorDeclaration};
		}

		protected override MemberDeclarationSyntax[] GetMethods(LoggerDescriptor loggerDescriptor)
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
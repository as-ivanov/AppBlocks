using System;
using System.Collections.Generic;
using System.Linq;
using CodeGeneration.Roslyn.Common;
using MetricsCollector.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeGeneration.Roslyn.MetricsCollector
{
	public class MetricsCollectorClassGenerator : InterfaceImplementationGenerator<MetricsCollectorDescriptor>
	{
		private const string MetricsProviderFieldName = "MetricsProvider";
		private const string ContextNameFieldName = "_contextName";
		private const string TagsVariableName = "tags";
		private const string ValuesVariableName = "values";

		public MetricsCollectorClassGenerator(AttributeData attributeData) : base(attributeData, new Version(1, 0, 0))
		{
		}

		protected override MetricsCollectorDescriptor GetImplementationDescriptor(TypeDeclarationSyntax typeDeclaration,
			TransformationContext context, AttributeData attributeData)
		{
			return typeDeclaration.ToMetricsCollectorDescriptor(context, attributeData);
		}

		protected override MemberDeclarationSyntax[] GetFields(MetricsCollectorDescriptor metricsCollectorDescriptor)
		{
			var generalMetricsCollectorFields = GetGeneralMetricsCollectorFields(metricsCollectorDescriptor);
			var metricsCollectorTagKeyFields = GetMetricsCollectorTagKeyFields(metricsCollectorDescriptor);
			return generalMetricsCollectorFields.Union(metricsCollectorTagKeyFields).ToArray();
		}

		protected override ConstructorDeclarationSyntax[] GetConstructors(
			MetricsCollectorDescriptor metricsCollectorDescriptor)
		{
			const string metricsProviderVariableName = "metricsProvider";

			var constructorDeclaration = ConstructorDeclaration(
					Identifier(metricsCollectorDescriptor.ClassName))
				.WithModifiers(
					TokenList(
						Token(SyntaxKind.PublicKeyword)))
				.WithParameterList(
					ParameterList(
						SingletonSeparatedList(
							Parameter(
									Identifier(metricsProviderVariableName))
								.WithType(typeof(IMetricsProvider).GetTypeSyntax()))));

			if (metricsCollectorDescriptor.BaseClassName != null)
			{
				constructorDeclaration = constructorDeclaration
					.WithInitializer(
						ConstructorInitializer(
							SyntaxKind.BaseConstructorInitializer,
							ArgumentList(SingletonSeparatedList(Argument(IdentifierName(metricsProviderVariableName))))))
					.WithBody(Block());
			}
			else
			{
				constructorDeclaration = constructorDeclaration.WithBody(
						Block(
							SingletonList<StatementSyntax>(
								ExpressionStatement(
									AssignmentExpression(
										SyntaxKind.SimpleAssignmentExpression,
										IdentifierName(MetricsProviderFieldName),
										IdentifierName(metricsProviderVariableName))))));
			}
			return new[] { constructorDeclaration };
		}

		protected override MemberDeclarationSyntax[] GetMethods(MetricsCollectorDescriptor metricsCollectorDescriptor)
		{
			var publicKeywordToken = Token(SyntaxKind.PublicKeyword);

			var members = new List<MemberDeclarationSyntax>();
			foreach (var method in metricsCollectorDescriptor.Methods)
			{
				var methodDeclaration =
					MethodDeclaration(method.MethodDeclarationSyntax.ReturnType, method.MethodDeclarationSyntax.Identifier)
						.WithTypeParameterList(method.MethodDeclarationSyntax.TypeParameterList)
						.WithConstraintClauses(method.MethodDeclarationSyntax.ConstraintClauses)
						.WithModifiers(method.MethodDeclarationSyntax.Modifiers)
						.AddModifiers(publicKeywordToken)
						.WithParameterList(method.MethodDeclarationSyntax.ParameterList)
						.WithBody(GetMetricsCollectorMethodBody(method));

				members.Add(methodDeclaration);
			}

			return members.ToArray();
		}

		private static BlockSyntax GetMetricsCollectorMethodBody(MetricsCollectorMethod metricsCollectorMethodDescriptor)
		{
			const string metricNameVariableName = "metricName";
			const string metricUnitVariableName = "metricUnit";

			var tagsInitialization = new List<StatementSyntax>();

			static LocalDeclarationStatementSyntax GetLocalStringDeclarationStatement(string variableName,
				string variableValue)
			{
				return LocalDeclarationStatement(
						VariableDeclaration(
								PredefinedType(
									Token(SyntaxKind.StringKeyword)))
							.WithVariables(
								SingletonSeparatedList(
									VariableDeclarator(
											Identifier(variableName))
										.WithInitializer(
											EqualsValueClause(
												variableValue == null
													? LiteralExpression(SyntaxKind.NullLiteralExpression)
													: LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(variableValue)))))))
					.WithModifiers(
						TokenList(
							Token(SyntaxKind.ConstKeyword)));
			}

			tagsInitialization.Add(GetLocalStringDeclarationStatement(metricNameVariableName,
				metricsCollectorMethodDescriptor.MetricName));
			tagsInitialization.Add(
				GetLocalStringDeclarationStatement(metricUnitVariableName, metricsCollectorMethodDescriptor.UnitName));

			StatementSyntax GetEnabledCheckStatement()
			{
				return IfStatement(
					PrefixUnaryExpression(
						SyntaxKind.LogicalNotExpression,
						InvocationExpression(
								MemberAccessExpression(
									SyntaxKind.SimpleMemberAccessExpression,
									IdentifierName(MetricsProviderFieldName),
									IdentifierName(nameof(IMetricsProvider.IsEnabled))))
							.WithArgumentList(
								ArgumentList(
									SeparatedList<ArgumentSyntax>(
										new SyntaxNodeOrToken[]
										{
											Argument(
												IdentifierName(ContextNameFieldName)),
											Token(SyntaxKind.CommaToken),
											Argument(
												IdentifierName(metricNameVariableName))
										})))),
					Block(
						SingletonList<StatementSyntax>(
							ReturnStatement(
								MemberAccessExpression(
									SyntaxKind.SimpleMemberAccessExpression,
									AliasQualifiedName(
										IdentifierName(Token(SyntaxKind.GlobalKeyword)),
										IdentifierName(typeof(IMetricsProvider).Namespace + ".Null" +
																 metricsCollectorMethodDescriptor.MetricsCollectorIndicatorType)),
									IdentifierName("Instance"))))));
			}

			tagsInitialization.Add(GetEnabledCheckStatement());

			if (metricsCollectorMethodDescriptor.MethodDeclarationSyntax.ParameterList.Parameters.Any())
			{
				static ExpressionSyntax GetToStringExpression(string parameterName)
				{
					return InvocationExpression(
						MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							IdentifierName(parameterName),
							IdentifierName(nameof(ToString))));
				}


				IEnumerable<LocalDeclarationStatementSyntax> tagsInitializationStatements;
				if (metricsCollectorMethodDescriptor.MethodDeclarationSyntax.ParameterList.Parameters.Count == 1)
				{
					tagsInitializationStatements = GetSingleTagInitializationStatement(metricsCollectorMethodDescriptor.MethodDeclarationSyntax.ParameterList.Parameters[0]);
				}
				else
				{
					var values = metricsCollectorMethodDescriptor.MethodDeclarationSyntax.ParameterList.Parameters
						.Select(_ => GetToStringExpression(_.Identifier.WithoutTrivia().Text)).ToSeparatedList();
					tagsInitializationStatements =
						GetMultipleTagsInitializationStatement(metricsCollectorMethodDescriptor, values);
				}

				foreach (var tagsDeclarationStatement in tagsInitializationStatements)
				{
					tagsInitialization.Add(tagsDeclarationStatement);
				}
			}
			else
			{
				tagsInitialization.Add(LocalDeclarationStatement(
					VariableDeclaration(
							IdentifierName("var"))
						.WithVariables(
							SingletonSeparatedList(
								VariableDeclarator(
										Identifier(TagsVariableName))
									.WithInitializer(
										EqualsValueClause(
											MemberAccessExpression(
												SyntaxKind.SimpleMemberAccessExpression,
												typeof(Tags).GetTypeSyntax(),
												IdentifierName(nameof(Tags.Empty)))))))));
			}


			StatementSyntax GetReturnStatement()
			{
				return ReturnStatement(
					InvocationExpression(
							MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								IdentifierName(MetricsProviderFieldName),
								IdentifierName("Create" + metricsCollectorMethodDescriptor.MetricsCollectorIndicatorType)))
						.WithArgumentList(
							ArgumentList(
								SeparatedList<ArgumentSyntax>(
									new SyntaxNodeOrToken[]
									{
										Argument(
											IdentifierName(ContextNameFieldName)),
										Token(SyntaxKind.CommaToken),
										Argument(
											IdentifierName(metricNameVariableName)),
										Token(SyntaxKind.CommaToken),
										Argument(
											IdentifierName(metricUnitVariableName)),
										Token(SyntaxKind.CommaToken),
										Argument(
											IdentifierName(TagsVariableName))
									}))));
			}

			tagsInitialization.Add(GetReturnStatement());

			return Block(tagsInitialization.ToArray());
		}

		private static IEnumerable<LocalDeclarationStatementSyntax> GetSingleTagInitializationStatement(ParameterSyntax parameter)
		{
			yield return LocalDeclarationStatement(
				VariableDeclaration(
						IdentifierName("var"))
					.WithVariables(
						SingletonSeparatedList(
							VariableDeclarator(
									Identifier(TagsVariableName))
								.WithInitializer(
									EqualsValueClause(
										ObjectCreationExpression(typeof(Tags).GetTypeSyntax())
											.WithArgumentList(
												ArgumentList(SeparatedList<ArgumentSyntax>(
													new SyntaxNodeOrToken[]
													{
														Argument(
															LiteralExpression(
																SyntaxKind.StringLiteralExpression,
																Literal(parameter.Identifier.WithoutTrivia().Text))),
														Token(SyntaxKind.CommaToken),
														Argument(
															InvocationExpression(
																MemberAccessExpression(
																	SyntaxKind.SimpleMemberAccessExpression,
																	IdentifierName(parameter.Identifier),
																	IdentifierName(nameof(ToString)))))
													}))))))));
		}

		private static IEnumerable<LocalDeclarationStatementSyntax> GetMultipleTagsInitializationStatement(
			MetricsCollectorMethod metricsCollectorMethodDescriptor, SeparatedSyntaxList<ExpressionSyntax> values)
		{
			yield return LocalDeclarationStatement(
				VariableDeclaration(
						IdentifierName("var"))
					.WithVariables(
						SingletonSeparatedList(
							VariableDeclarator(
									Identifier(ValuesVariableName))
								.WithInitializer(
									EqualsValueClause(
										ArrayCreationExpression(
												ArrayType(
														PredefinedType(
															Token(SyntaxKind.StringKeyword)))
													.WithRankSpecifiers(
														SingletonList(
															ArrayRankSpecifier(
																SingletonSeparatedList<ExpressionSyntax>(
																	OmittedArraySizeExpression())))))
											.WithInitializer(
												InitializerExpression(SyntaxKind.ArrayInitializerExpression, values)))))));

			yield return LocalDeclarationStatement(
				VariableDeclaration(
						IdentifierName("var"))
					.WithVariables(
						SingletonSeparatedList(
							VariableDeclarator(
									Identifier(TagsVariableName))
								.WithInitializer(
									EqualsValueClause(
										ObjectCreationExpression(typeof(Tags).GetTypeSyntax())
											.WithArgumentList(
												ArgumentList(
													SeparatedList<ArgumentSyntax>(
														new SyntaxNodeOrToken[]
														{
															Argument(
																IdentifierName("_" + metricsCollectorMethodDescriptor.MethodNameCamelCase)),
															Token(SyntaxKind.CommaToken),
															Argument(
																IdentifierName(ValuesVariableName))
														}))))))));
		}

		private static MemberDeclarationSyntax[] GetGeneralMetricsCollectorFields(
			MetricsCollectorDescriptor metricsCollectorDescriptor)
		{
			var contextNameFieldDeclaration = FieldDeclaration(
					VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)))
						.WithVariables(
							SingletonSeparatedList(
								VariableDeclarator(
										Identifier(ContextNameFieldName))
									.WithInitializer(
										EqualsValueClause(
											LiteralExpression(
												SyntaxKind.StringLiteralExpression,
												Literal(metricsCollectorDescriptor.ContextName)))))))
				.WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ConstKeyword)));

			if (metricsCollectorDescriptor.BaseClassName != null)
			{
				return new MemberDeclarationSyntax[]
				{
					contextNameFieldDeclaration
				};
			}

			var metricsProviderFieldDeclaration = FieldDeclaration(
					VariableDeclaration(typeof(IMetricsProvider).GetTypeSyntax())
						.WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(MetricsProviderFieldName)))))
				.WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.ReadOnlyKeyword)));

			return new MemberDeclarationSyntax[]
			{
				contextNameFieldDeclaration,
				metricsProviderFieldDeclaration
			};
		}

		private static MemberDeclarationSyntax[] GetMetricsCollectorTagKeyFields(
			MetricsCollectorDescriptor metricsCollectorDescriptor)
		{
			var fieldMemberDeclarations = new List<MemberDeclarationSyntax>(metricsCollectorDescriptor.Methods.Length);
			for (var index = 0; index < metricsCollectorDescriptor.Methods.Length; index++)
			{
				var method = metricsCollectorDescriptor.Methods[index];

				if (method.MethodDeclarationSyntax.ParameterList.Parameters.Count <= 1) // use Tags(string key, string value) constructor with constant key instead
				{
					continue;
				}

				var values = method.MethodDeclarationSyntax.ParameterList.Parameters
					.Select(_ => _.Identifier.WithoutTrivia().Text.GetLiteralExpression()).ToSeparatedList<ExpressionSyntax>();

				var declaration = FieldDeclaration(
						VariableDeclaration(
								ArrayType(
										PredefinedType(
											Token(SyntaxKind.StringKeyword)))
									.WithRankSpecifiers(
										SingletonList(
											ArrayRankSpecifier(
												SingletonSeparatedList<ExpressionSyntax>(
													OmittedArraySizeExpression())))))
							.WithVariables(
								SingletonSeparatedList(
									VariableDeclarator(
											Identifier("_" + method.MethodNameCamelCase))
										.WithInitializer(
											EqualsValueClause(
												InitializerExpression(
													SyntaxKind.ArrayInitializerExpression,
													values))))))
					.WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword),
						Token(SyntaxKind.ReadOnlyKeyword)));

				fieldMemberDeclarations.Add(declaration);
			}

			return fieldMemberDeclarations.ToArray();
		}
	}
}
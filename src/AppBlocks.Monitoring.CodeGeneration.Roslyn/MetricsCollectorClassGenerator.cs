using System;
using System.Collections.Generic;
using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Common;
using CodeGeneration.Roslyn;
using AppBlocks.Monitoring.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn
{
	public class MetricsCollectorClassGenerator : InterfaceImplementationGenerator<MetricsCollectorDescriptor>
	{
		private const string MetricsProviderFieldName = "_metricsProvider";
		private const string MetricsPolicyFieldName = "_metricsPolicy";
		private const string ContextNameFieldName = "_contextName";
		private const string TagsVariableName = "tags";
		private const string ValuesVariableName = "values";


		private static readonly TypeSyntax _metricsProviderGlobalTypeSyntax = typeof(IMetricsProvider).GetGlobalTypeSyntax();
		private static readonly TypeSyntax _metricsPolicyGlobalTypeSyntax = typeof(IMetricsPolicy).GetGlobalTypeSyntax();
		private static readonly TypeSyntax _tagsGlobalTypeSyntax = typeof(Tags).GetGlobalTypeSyntax();

		public MetricsCollectorClassGenerator(AttributeData attributeData) : base(attributeData, new Version(1, 0, 0))
		{
		}

		protected override MetricsCollectorDescriptor GetImplementationDescriptor(TypeDeclarationSyntax typeDeclaration,
			TransformationContext context, AttributeData attributeData)
		{
			return typeDeclaration.ToMetricsCollectorDescriptor(context, attributeData);
		}

		protected override IEnumerable<MemberDeclarationSyntax> GetFields(
			MetricsCollectorDescriptor metricsCollectorDescriptor)
		{
			var generalMetricsCollectorFields = GetGeneralMetricsCollectorFields(metricsCollectorDescriptor);
			var metricsCollectorTagKeyFields = GetMetricsCollectorTagKeyFields(metricsCollectorDescriptor);
			return generalMetricsCollectorFields.Union(metricsCollectorTagKeyFields);
		}

		protected override IEnumerable<ConstructorDeclarationSyntax> GetConstructors(
			MetricsCollectorDescriptor metricsCollectorDescriptor)
		{
			const string metricsProviderVariableName = "metricsProvider";
			const string metricsPolicyVariableName = "metricsPolicy";

			var constructorParameters = new List<ParameterSyntax>
			{
				Parameter(Identifier(metricsProviderVariableName))
					.WithType(_metricsProviderGlobalTypeSyntax),
				Parameter(Identifier(metricsPolicyVariableName))
					.WithType(_metricsPolicyGlobalTypeSyntax)
					.WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.NullLiteralExpression)))
			};

			var constructorDeclaration = ConstructorDeclaration(
					Identifier(metricsCollectorDescriptor.ClassName))
				.WithModifiers(
					TokenList(
						Token(SyntaxKind.PublicKeyword)))
				.WithParameterList(
					ParameterList(constructorParameters.ToSeparatedList()));

			var statements = new List<StatementSyntax>
			{
				ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
					IdentifierName(MetricsProviderFieldName), IdentifierName(metricsProviderVariableName))),
				ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
					IdentifierName(MetricsPolicyFieldName), IdentifierName(metricsPolicyVariableName)))
			};

			constructorDeclaration = constructorDeclaration.WithBody(
				Block(statements));

			yield return constructorDeclaration;
		}

		protected override IEnumerable<MemberDeclarationSyntax> GetMethods(
			MetricsCollectorDescriptor metricsCollectorDescriptor)
		{
			var members = new List<MemberDeclarationSyntax>(metricsCollectorDescriptor.Methods.Length);
			foreach (var method in metricsCollectorDescriptor.Methods)
			{
				var interfaceGlobalQualifiedName = AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName(method.DeclaringInterfaceSymbol.GetFullTypeName()));
				var explicitInterfaceSpecifier = ExplicitInterfaceSpecifier(interfaceGlobalQualifiedName);

				var returnTypeNameWithoutNamespaces = method.MethodDeclarationSyntax.ReturnType.WithoutTrivia().ToFullString()
					.GetTypeNameWithoutNamespaces();
				var returnTypeIdentifier = IdentifierName(typeof(IMetricsProvider).Namespace + "." + returnTypeNameWithoutNamespaces);
				var returnType = AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), returnTypeIdentifier);

				var methodConstraintClauses = method.MethodDeclarationSyntax.ConstraintClauses
					.GetAllowedImplicitImplementationConstraintClause();

				var methodDeclaration =
					MethodDeclaration(method.MethodDeclarationSyntax.ReturnType, method.MethodDeclarationSyntax.Identifier)
						.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier)
						.WithTypeParameterList(method.MethodDeclarationSyntax.TypeParameterList)
						.WithConstraintClauses(methodConstraintClauses)
						.WithParameterList(method.MethodDeclarationSyntax.ParameterList)
						.WithReturnType(returnType)
						.WithBody(GetMetricsCollectorMethodBody(method));

				members.Add(methodDeclaration);
			}

			return members;
		}

		private static BlockSyntax GetMetricsCollectorMethodBody(MetricsCollectorMethod metricsCollectorMethodDescriptor)
		{
			const string metricNameVariableName = "metricName";
			const string metricUnitVariableName = "metricUnit";

			var statements = new List<StatementSyntax>();

			static LocalDeclarationStatementSyntax GetLocalConstStringDeclarationStatement(string variableName,
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
											EqualsValueClause(variableValue.GetLiteralExpression())))))
					.WithModifiers(
						TokenList(
							Token(SyntaxKind.ConstKeyword)));
			}

			statements.Add(GetLocalConstStringDeclarationStatement(metricNameVariableName,
				metricsCollectorMethodDescriptor.MetricName));
			statements.Add(
				GetLocalConstStringDeclarationStatement(metricUnitVariableName, metricsCollectorMethodDescriptor.UnitName));

			StatementSyntax GetEnabledCheckStatement()
			{
				var condition = BinaryExpression(
					SyntaxKind.LogicalAndExpression,
					BinaryExpression(
						SyntaxKind.NotEqualsExpression,
						IdentifierName(MetricsPolicyFieldName),
						LiteralExpression(
							SyntaxKind.NullLiteralExpression)),
					PrefixUnaryExpression(
						SyntaxKind.LogicalNotExpression,
						InvocationExpression(
								MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(MetricsPolicyFieldName),
									IdentifierName(nameof(IMetricsPolicy.IsEnabled))))
							.WithArgumentList(
								ArgumentList(
									SeparatedList<ArgumentSyntax>(
										new SyntaxNodeOrToken[]
										{
											Argument(IdentifierName(ContextNameFieldName)),
											Token(SyntaxKind.CommaToken),
											Argument(IdentifierName(metricNameVariableName))
										})))));
				return IfStatement(
					condition,
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

			statements.Add(GetEnabledCheckStatement());

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
					tagsInitializationStatements =
						GetSingleTagInitializationStatement(metricsCollectorMethodDescriptor.MethodDeclarationSyntax.ParameterList
							.Parameters[0]);
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
					statements.Add(tagsDeclarationStatement);
				}
			}
			else
			{
				statements.Add(LocalDeclarationStatement(
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
												_tagsGlobalTypeSyntax,
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

			statements.Add(GetReturnStatement());

			return Block(statements.ToArray());
		}

		private static IEnumerable<LocalDeclarationStatementSyntax> GetSingleTagInitializationStatement(
			ParameterSyntax parameter)
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
										ObjectCreationExpression(_tagsGlobalTypeSyntax)
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
										ObjectCreationExpression(_tagsGlobalTypeSyntax)
											.WithArgumentList(
												ArgumentList(
													SeparatedList<ArgumentSyntax>(
														new SyntaxNodeOrToken[]
														{
															Argument(IdentifierName(metricsCollectorMethodDescriptor.MethodKeysFieldName)),
															Token(SyntaxKind.CommaToken),
															Argument(
																IdentifierName(ValuesVariableName))
														}))))))));
		}

		private static MemberDeclarationSyntax[] GetGeneralMetricsCollectorFields(MetricsCollectorDescriptor descriptor)
		{
			var contextNameFieldNameInitializer = EqualsValueClause(descriptor.ContextName.GetLiteralExpression());

			var contextNameFieldDeclaration = FieldDeclaration(
					VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)))
						.WithVariables(
							SingletonSeparatedList(
								VariableDeclarator(Identifier(ContextNameFieldName)).WithInitializer(contextNameFieldNameInitializer)
								)))
				.WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ConstKeyword)));


			var metricsProviderFieldDeclaration = FieldDeclaration(
					VariableDeclaration(_metricsProviderGlobalTypeSyntax)
						.WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(MetricsProviderFieldName)))))
				.WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)));

			var metricsPolicyFieldDeclaration = FieldDeclaration(
					VariableDeclaration(_metricsPolicyGlobalTypeSyntax)
						.WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(MetricsPolicyFieldName)))))
				.WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)));

			return new MemberDeclarationSyntax[]
			{
				contextNameFieldDeclaration,
				metricsProviderFieldDeclaration,
				metricsPolicyFieldDeclaration
			};
		}

		private static MemberDeclarationSyntax[] GetMetricsCollectorTagKeyFields(
			MetricsCollectorDescriptor metricsCollectorDescriptor)
		{
			var fieldMemberDeclarations = new List<MemberDeclarationSyntax>(metricsCollectorDescriptor.Methods.Length);
			for (var index = 0; index < metricsCollectorDescriptor.Methods.Length; index++)
			{
				var method = metricsCollectorDescriptor.Methods[index];

				if (method.MethodDeclarationSyntax.ParameterList.Parameters.Count <= 1
				) // use Tags(string key, string value) constructor with constant key instead
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
											Identifier(method.MethodKeysFieldName))
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
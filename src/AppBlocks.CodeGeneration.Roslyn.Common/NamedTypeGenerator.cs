using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	internal static class NamedTypeGenerator
	{
		public static TypeDeclarationSyntax GetInterfaceDeclarationSyntax(
			INamedTypeSymbol namedType)
		{
			static BaseListSyntax GenerateBaseList(INamedTypeSymbol namedType)
			{
				var types = new List<BaseTypeSyntax>();
				if (namedType.TypeKind == TypeKind.Class && namedType.BaseType != null &&
				    namedType.BaseType.SpecialType != SpecialType.System_Object)
				{
					types.Add(SyntaxFactory.SimpleBaseType(namedType.BaseType.GenerateTypeSyntax()));
				}

				foreach (var type in namedType.Interfaces)
				{
					types.Add(SyntaxFactory.SimpleBaseType(type.GenerateTypeSyntax()));
				}

				if (types.Count == 0)
				{
					return null;
				}

				return SyntaxFactory.BaseList(SyntaxFactory.SeparatedList(types));
			}

			return SyntaxFactory.TypeDeclaration(SyntaxKind.InterfaceDeclaration, namedType.Name)
				.WithAttributeLists(GenerateAttributeDeclarations(namedType))
				.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
				.WithTypeParameterList(GenerateTypeParameterList(namedType.TypeParameters))
				.WithBaseList(GenerateBaseList(namedType))
				.WithConstraintClauses(GenerateConstraintClauses(namedType.TypeParameters))
				.AddMembers(GetMembers(namedType));
		}

		private static TypeParameterListSyntax GenerateTypeParameterList(
			ImmutableArray<ITypeParameterSymbol> typeParameters)
		{
			static TypeParameterSyntax GenerateTypeParameter(ITypeParameterSymbol symbol)
			{
				var varianceKeyword =
					symbol.Variance == VarianceKind.In ? SyntaxFactory.Token(SyntaxKind.InKeyword) :
					symbol.Variance == VarianceKind.Out ? SyntaxFactory.Token(SyntaxKind.OutKeyword) : default;

				return SyntaxFactory.TypeParameter(
					AttributeGenerator.GenerateAttributeLists(symbol.GetAttributes()),
					varianceKeyword,
					SyntaxFactory.Identifier(symbol.Name));
			}

			return typeParameters.Length == 0
				? null
				: SyntaxFactory.TypeParameterList(
					SyntaxFactory.SeparatedList(typeParameters.Select(GenerateTypeParameter)));
		}


		private static MemberDeclarationSyntax[] GetMembers(INamedTypeSymbol namedType)
		{
			static TypeSyntax GenerateReturnTypeSyntax(IMethodSymbol method)
			{
				var returnType = method.ReturnType;
				return SyntaxFactory.IdentifierName(returnType.Name);
			}


			static ParameterListSyntax GenerateParameterList(
				IEnumerable<IParameterSymbol> parameterDefinitions)
			{
				var result = new List<ParameterSyntax>();
				var seenOptional = false;
				var isFirstParam = true;

				static ParameterSyntax GetParameter(IParameterSymbol p, bool isFirstParam, bool seenOptional)
				{
					static SyntaxTokenList GetParameterModifiers(RefKind refKind)
					{
						return refKind switch
						{
							RefKind.None => new SyntaxTokenList(),
							RefKind.Out => SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OutKeyword)),
							RefKind.Ref => SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.RefKeyword)),
							RefKind.In => SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InKeyword)),
							_ => throw new Exception($"Unexpected kind:{refKind}"),
						};
					}

					static SyntaxTokenList GenerateModifiers(
						IParameterSymbol parameter)
					{
						var list = GetParameterModifiers(parameter.RefKind);
						if (parameter.IsParams)
						{
							list = list.Add(SyntaxFactory.Token(SyntaxKind.ParamsKeyword));
						}

						return list;
					}

					static EqualsValueClauseSyntax GenerateEqualsValueClause(
						IParameterSymbol parameter,
						bool seenOptional)
					{
						if (!parameter.IsParams && !(parameter.RefKind == RefKind.Ref || parameter.RefKind == RefKind.Out))
						{
							if (parameter.HasExplicitDefaultValue || seenOptional)
							{
								var defaultValue = parameter.HasExplicitDefaultValue ? parameter.ExplicitDefaultValue : null;
								if (defaultValue is DateTime)
								{
									return null;
								}

								return SyntaxFactory.EqualsValueClause(
									ExpressionGenerator.GenerateExpression(parameter.Type, defaultValue, true));
							}
						}

						return null;
					}

					return SyntaxFactory.Parameter(SyntaxFactory.Identifier(p.Name))
						.WithAttributeLists(AttributeGenerator.GenerateAttributeLists(p.GetAttributes()))
						.WithModifiers(GenerateModifiers(p))
						.WithType(p.Type.GenerateTypeSyntax())
						.WithDefault(GenerateEqualsValueClause(p, seenOptional));
				}

				foreach (var p in parameterDefinitions)
				{
					var parameter = GetParameter(p, isFirstParam, seenOptional);
					result.Add(parameter);
					seenOptional = seenOptional || parameter.Default != null;
					isFirstParam = false;
				}

				return SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(result));
			}

			static MemberDeclarationSyntax CreateMemberDeclarationSyntax(IMethodSymbol method)
			{
				return SyntaxFactory.MethodDeclaration(
					GenerateAttributeDeclarations(method),
					default,
					GenerateReturnTypeSyntax(method),
					default,
					SyntaxFactory.Identifier(method.Name),
					GenerateTypeParameterList(method.TypeParameters),
					GenerateParameterList(method.Parameters),
					GenerateConstraintClauses(method.TypeParameters),
					null,
					null,
					SyntaxFactory.Token(SyntaxKind.SemicolonToken));
			}

			return namedType.GetMembers()
				.OfType<IMethodSymbol>()
				.Select(CreateMemberDeclarationSyntax)
				.ToArray();
		}

		private static SyntaxList<TypeParameterConstraintClauseSyntax> GenerateConstraintClauses(
			ImmutableArray<ITypeParameterSymbol> typeParameters)
		{
			var clauses = new List<TypeParameterConstraintClauseSyntax>();

			static void AddConstraintClauses(
				List<TypeParameterConstraintClauseSyntax> clauses,
				ITypeParameterSymbol typeParameter)
			{
				var constraints = new List<TypeParameterConstraintSyntax>();

				if (typeParameter.HasReferenceTypeConstraint)
				{
					constraints.Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.ClassConstraint));
				}
				else if (typeParameter.HasUnmanagedTypeConstraint)
				{
					constraints.Add(SyntaxFactory.TypeConstraint(SyntaxFactory.IdentifierName("unmanaged")));
				}
				else if (typeParameter.HasValueTypeConstraint)
				{
					constraints.Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.StructConstraint));
				}
				else if (typeParameter.HasNotNullConstraint)
				{
					constraints.Add(SyntaxFactory.TypeConstraint(SyntaxFactory.IdentifierName("notnull")));
				}

				var constraintTypes =
					typeParameter.ConstraintTypes.Where(t => t.TypeKind == TypeKind.Class).Concat(
						typeParameter.ConstraintTypes.Where(t => t.TypeKind == TypeKind.Interface).Concat(
							typeParameter.ConstraintTypes.Where(t =>
								t.TypeKind != TypeKind.Class && t.TypeKind != TypeKind.Interface)));

				foreach (var type in constraintTypes)
				{
					if (type.SpecialType != SpecialType.System_Object)
					{
						constraints.Add(SyntaxFactory.TypeConstraint(type.GenerateTypeSyntax()));
					}
				}

				if (typeParameter.HasConstructorConstraint)
				{
					constraints.Add(SyntaxFactory.ConstructorConstraint());
				}

				if (constraints.Count == 0)
				{
					return;
				}

				clauses.Add(SyntaxFactory.TypeParameterConstraintClause(
					SyntaxFactory.IdentifierName(typeParameter.Name),
					SyntaxFactory.SeparatedList(constraints)));
			}

			foreach (var typeParameter in typeParameters)
			{
				AddConstraintClauses(clauses, typeParameter);
			}

			return clauses.Count == 0 ? default : SyntaxFactory.List(clauses);
		}

		private static SyntaxList<AttributeListSyntax> GenerateAttributeDeclarations(ISymbol namedType)
		{
			return AttributeGenerator.GenerateAttributeLists(namedType.GetAttributes());
		}
	}
}
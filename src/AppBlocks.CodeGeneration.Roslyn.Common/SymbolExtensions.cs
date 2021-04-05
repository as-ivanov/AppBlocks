using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public static class SymbolExtensions
	{
		private static readonly SymbolDisplayFormat _format = new SymbolDisplayFormat(SymbolDisplayGlobalNamespaceStyle.Included,
			SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
			SymbolDisplayGenericsOptions.IncludeTypeParameters,
			miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable);

		public static string GetFullTypeName(this ITypeSymbol symbol)
		{
			return symbol.ToDisplayString(_format);
		}


		public static IEnumerable<(MethodDeclarationSyntax MethodDeclaration, IMethodSymbol methodSymbol, TypeDeclarationSyntax DeclaredInterface, INamedTypeSymbol DeclaredInterfaceSymbol)>
			GetAllMethodDeclarations(this TypeDeclarationSyntax typeDeclarationSyntax, TransformationContext context)
		{
			var typeFullName = typeDeclarationSyntax.GetFullTypeName();
			var targetInterfaceSymbol = context.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax) as INamedTypeSymbol;
			if (targetInterfaceSymbol == null)
			{
				throw new Exception($"{typeFullName} not found in assembly.");
			}

			static IEnumerable<INamedTypeSymbol> GetAllInterfaceSymbolsRecursive(INamedTypeSymbol typeSymbol)
			{
				yield return typeSymbol;
				foreach (var inheritedInterfaceSymbol in typeSymbol.Interfaces)
				{
					foreach (var item in GetAllInterfaceSymbolsRecursive(inheritedInterfaceSymbol))
					{
						yield return item;
					}
				}
			}

			var allInterfaceSymbols = GetAllInterfaceSymbolsRecursive(targetInterfaceSymbol);

			foreach (var interfaceSymbol in allInterfaceSymbols.Distinct())
			{
				var interfaceMethodSymbols = interfaceSymbol.GetMembers().OfType<IMethodSymbol>().ToArray();
				var interfaceDeclarations = interfaceSymbol.DeclaringSyntaxReferences.SelectMany(_ => _.SyntaxTree.GetRoot().DescendantNodesAndSelf())
					.OfType<TypeDeclarationSyntax>().Where(_ => _.GetFullTypeName() == interfaceSymbol.OriginalDefinition.ToDisplayString()).ToList();
				if (!interfaceDeclarations.Any()) // interface declared in another assembly
				{
					var interfaceDeclaration = NamedTypeGenerator.GetInterfaceDeclarationSyntax(interfaceSymbol); // reconstruct declaration from symbol
					interfaceDeclarations.Add(interfaceDeclaration);
				}

				foreach (var interfaceDeclaration in interfaceDeclarations)
				{
					var methodDeclarations = interfaceDeclaration.Members.OfType<MethodDeclarationSyntax>();
					var index = -1;
					foreach (var methodDeclaration in methodDeclarations)
					{
						index++;
						var methodSymbol = interfaceMethodSymbols[index];
						yield return (methodDeclaration, methodSymbol, interfaceDeclaration, interfaceSymbol);
					}
				}
			}
		}
	}
}

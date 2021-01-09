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
		public static string GetFullTypeName(this ITypeSymbol symbol)
		{
			var nameBuilder = new StringBuilder();
			ISymbol symbolOrParent = symbol;
			while (symbolOrParent != null && !string.IsNullOrEmpty(symbolOrParent.Name))
			{
				if (nameBuilder.Length > 0)
				{
					nameBuilder.Insert(0, CSharpConst.NamespaceClassDelimiter);
				}

				nameBuilder.Insert(0, symbolOrParent.Name);
				symbolOrParent = symbolOrParent.ContainingSymbol;
			}

			return nameBuilder.ToString();
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

			foreach (var interfaceSymbol in allInterfaceSymbols)
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
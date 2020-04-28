using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public static class SymbolExtensions
	{
		public static string GetFullTypeName(this INamedTypeSymbol symbol)
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

		public static IEnumerable<INamedTypeSymbol> GetInheritedInterfaceSymbolsWithMeRecursive(this INamedTypeSymbol typeSymbol)
		{
			yield return typeSymbol;
			foreach (var inheritedInterfaceSymbol in typeSymbol.Interfaces)
			{
				foreach (var item in GetInheritedInterfaceSymbolsWithMeRecursive(inheritedInterfaceSymbol))
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<(MethodDeclarationSyntax MethodDeclaration, TypeDeclarationSyntax TypeDeclaration)> GetAllMethodDeclarations(
			this IEnumerable<INamedTypeSymbol> interfaceSymbols)
		{
			foreach (var interfaceSymbol in interfaceSymbols)
			{
				var interfaceDeclarations  = interfaceSymbol.DeclaringSyntaxReferences.SelectMany(_ => _.SyntaxTree.GetRoot().DescendantNodesAndSelf())
					.OfType<TypeDeclarationSyntax>().Where(_ => _.GetFullTypeName() == interfaceSymbol.OriginalDefinition.ToDisplayString());
				foreach (var interfaceDeclaration in interfaceDeclarations)
				{
					foreach (var member in interfaceDeclaration.Members.OfType<MethodDeclarationSyntax>())
					{
						yield return (member, interfaceDeclaration);
					}
				}
			}
		}
	}
}
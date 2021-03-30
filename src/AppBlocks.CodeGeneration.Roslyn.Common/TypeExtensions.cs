using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public static class TypeExtensions
	{
		public static AliasQualifiedNameSyntax GetGlobalTypeSyntax(this Type type)
		{
			return AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName(type.FullName));
		}

		public static string GetFriendlyName(this Type type)
		{
			var friendlyName = type.Name;
			if (type.IsGenericType)
			{
				var iBacktick = friendlyName.IndexOf('`');
				if (iBacktick > 0)
				{
					friendlyName = friendlyName.Remove(iBacktick);
				}

				friendlyName += "<";
				var typeParameters = type.GetGenericArguments();
				for (var i = 0; i < typeParameters.Length; ++i)
				{
					var typeParamName = GetFriendlyName(typeParameters[i]);
					friendlyName += i == 0 ? typeParamName : "," + typeParamName;
				}

				friendlyName += ">";
			}

			return friendlyName;
		}
	}
}

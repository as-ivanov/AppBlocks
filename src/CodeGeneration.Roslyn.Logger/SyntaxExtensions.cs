using Microsoft.CodeAnalysis;

namespace CodeGeneration.Roslyn.Logger
{
	internal static class SyntaxExtensions
	{
		public static string ToCamelCase(this SyntaxToken syntaxToken)
		{
			var id = (string)syntaxToken.Value;
			return id.ToCamelCase();
		}
	}
}
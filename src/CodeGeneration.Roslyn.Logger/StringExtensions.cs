using System.Linq;

namespace CodeGeneration.Roslyn.Logger
{
	public static class StringExtensions
	{
		public static string ToPascalCase(this string word)
		{
			return string.Join(" ", word.Split(' ')
			  .Select(w => w.Trim())
			  .Where(w => w.Length > 0)
			  .Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1)));
		}

		public static string ToCamelCase(this string input)
		{
			return char.ToLowerInvariant(input[0]) + input.Substring(1);
		}

		public static string GetClassName(this string word)
		{
			if (word.StartsWith('I'))
			{
				word = word.Substring(1);
			}
			return word;
		}

		public static string EscapeCSharpString(this string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return "\"\"";
			}

			return "@\"" + input.Replace("\"", "\"\"") + "\"";
		}
	}
}
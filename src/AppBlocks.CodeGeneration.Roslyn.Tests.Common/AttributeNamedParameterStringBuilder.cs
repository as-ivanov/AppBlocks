using System.Text;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common
{
	public class AttributeNamedParameterStringBuilder
	{
		private readonly StringBuilder _stringBuilder;

		public AttributeNamedParameterStringBuilder()
		{
			_stringBuilder = new StringBuilder();
		}

		public void Append(string parameterName, string parameterValue)
		{
			if (_stringBuilder.Length > 0)
			{
				_stringBuilder.Append(", ");
			}

			_stringBuilder.Append($"{parameterName} = {parameterValue}");
		}

		public override string ToString()
		{
			return _stringBuilder.ToString();
		}
	}
}

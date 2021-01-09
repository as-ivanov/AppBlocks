using System;
using System.Text;
using System.Text.RegularExpressions;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common
{
	internal class CSharpBlockStringBuilder
	{
		private static readonly Regex _newLineRegex = new Regex(@"^", RegexOptions.Compiled | RegexOptions.Multiline);
		private readonly StringBuilder _stringBuilder;
		private int _currentLevel;

		public CSharpBlockStringBuilder()
		{
			_stringBuilder = new StringBuilder();
		}

		public IDisposable Block()
		{
			Append("{");
			_currentLevel += 1;
			AppendLine();

			void CloseBlock()
			{
				_currentLevel -= 1;
				AppendLine("}");
			}

			return new DisposableAction(CloseBlock);
		}

		public IDisposable Block(string text)
		{
			AppendLine(text);
			return Block();
		}

		public void Append(string text)
		{
			text = _newLineRegex.Replace(text, new string('	', _currentLevel));
			_stringBuilder.Append(text);
		}

		public void AppendLine(string text)
		{
			Append(text);
			AppendLine();
		}

		public void AppendLineIfNotEmpty(string @string)
		{
			if (string.IsNullOrEmpty(@string))
			{
				return;
			}
			AppendLine(@string);
		}

		public void AppendLine()
		{
			_stringBuilder.AppendLine();
		}

		public override string ToString()
		{
			return _stringBuilder.ToString();
		}

		private class DisposableAction : IDisposable
		{
			private readonly Action _action;

			public DisposableAction(Action action)
			{
				_action = action;
			}

			public void Dispose()
			{
				_action();
			}
		}
	}
}
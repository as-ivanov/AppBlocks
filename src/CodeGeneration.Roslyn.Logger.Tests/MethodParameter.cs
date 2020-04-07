using System;

namespace CodeGeneration.Roslyn.Logger.Tests
{
	public class MethodParameter
	{
		private readonly string _name;
		private readonly Type _type;

		public MethodParameter(string name, Type type)
		{
			_name = name;
			_type = type;
		}

		public string Name => _name;

		public Type Type => _type;
	}
}
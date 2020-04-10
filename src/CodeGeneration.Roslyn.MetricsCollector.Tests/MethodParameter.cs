using System;
using System.Globalization;

namespace CodeGeneration.Roslyn.MetricsCollector.Tests
{
	public class MethodParameter
	{
		private readonly string _name;
		private readonly Type _type;
		private readonly object _value;

		public MethodParameter(string name, Type type)
		{
			_name = name;
			_type = type;
			_value = Type.IsValueType ? Activator.CreateInstance(Type) : null;
		}

		public string Name => _name;

		public Type Type => _type;

		public object Value => _value;
	}
}
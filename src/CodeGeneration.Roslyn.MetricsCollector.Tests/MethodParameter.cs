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

		public string FormattedValue
		{
			get
			{
				if (_value == null)
				{
					return "(null)";
				}

				if (_value is DateTime dateTime)
				{
					return dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
				}

				return _value.ToString();
			}
		}
	}
}
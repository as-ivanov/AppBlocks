using System;
using System.Collections.Generic;

namespace CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public class MethodParameterData
	{
		private readonly string _name;
		private readonly Type _type;
		private readonly object _value;

		private MethodParameterData(string name, Type type, object value)
		{
			_name = name;
			_type = type;
			_value = value;
		}

		public object Value => _value;

		public override string ToString()
		{
			return $"{_type} {_name}";
		}

		public static IEnumerable<MethodParameterData> GetAllCombinations(ITestInterfaceGenerationOptions options)
		{
			var index = 0;
			foreach (var type in options.MethodParameterTypes)
			{
				var values = GetValues(type);
				foreach (var value in values)
				{
					index++;
					yield return new MethodParameterData("param" + index, type, value);
				}
			}
		}

		private static IEnumerable<object> GetValues(Type type)
		{
			if (!type.IsValueType)
			{
				yield return null;
			}
			else
			{
				yield return Activator.CreateInstance(type);
			}

			if (type == typeof(string))
			{
				yield return Guid.NewGuid().ToString();
			}
			else if (type == typeof(char))
			{
				yield return Guid.NewGuid().ToString()[0];
			}
			else if (type.IsAssignableFrom(typeof(float)))
			{
				yield return (float)Guid.NewGuid().GetHashCode() / Guid.NewGuid().GetHashCode();
			}
			else if (type.IsAssignableFrom(typeof(uint)))
			{
				yield return Guid.NewGuid().GetHashCode();
			}
			else if (type == typeof(Exception))
			{
				yield return new Exception(Guid.NewGuid().ToString());
			}
		}
	}
}
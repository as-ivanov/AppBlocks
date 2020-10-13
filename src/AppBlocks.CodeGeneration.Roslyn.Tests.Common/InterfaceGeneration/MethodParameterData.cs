using System;
using System.Collections.Generic;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
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

		public string Name => _name;

		public Type ParameterType => _type;

		public override string ToString()
		{
			return $"{ParameterType} {Name}";
		}

		public static IEnumerable<MethodParameterData> GetPossibleVariations(ITestInterfaceGenerationOptions options)
		{
			var index = 0;
			foreach (var type in options.MethodParameterTypes)
			{
				var values = options.ParameterValuesBuilder.GetValues(type);
				foreach (var value in values)
				{
					index++;
					var prefix = index % 2 == 0 ? "@" : "";
					yield return new MethodParameterData( prefix + "param" + index, type, value);
				}
			}
		}
	}
}
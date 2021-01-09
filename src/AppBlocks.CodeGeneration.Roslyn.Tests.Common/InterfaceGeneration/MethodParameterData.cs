using System;
using System.Collections.Generic;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public class MethodParameterData
	{
		private readonly string _name;
		private readonly Type _type;
		private readonly object _value;
		private readonly string _aliasTypeName;

		private MethodParameterData(string name, Type type, object value, string aliasTypeName)
		{
			_name = name;
			_type = type;
			_value = value;
			_aliasTypeName = aliasTypeName;
		}

		public object Value => _value;

		public string Name => _name;

		public Type ParameterType => _type;

		public string AliasTypeName => _aliasTypeName;

		public override string ToString()
		{
			return $"{_aliasTypeName ?? ParameterType.Name.ToString()} {Name}";
		}

		public TypeNameAliasUsingData GetTypeNameAliasUsingData()
		{
			if (_aliasTypeName == null)
			{
				return TypeNameAliasUsingData.Empty;
			}

			return new TypeNameAliasUsingData(_type, _aliasTypeName);
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
					var aliasTypeName = index % 1 == 0 ? "AliasType" + index : null;
					yield return new MethodParameterData(prefix + "param" + index, type, value, aliasTypeName);
				}
			}
		}
	}
}
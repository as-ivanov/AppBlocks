using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public class MethodParameterData
	{
		private readonly string _name;
		private readonly Type _type;
		private readonly object _value;
		private readonly string _aliasTypeName;
		private readonly AttributeData[] _attributeDataList;

		private MethodParameterData(string name, Type type, object value, string aliasTypeName, AttributeData[] attributeDataList)
		{
			_name = name;
			_type = type;
			_value = value;
			_aliasTypeName = aliasTypeName;
			_attributeDataList = attributeDataList;
		}

		public object Value => _value;

		public string Name => _name;

		public Type ParameterType => _type;

		public string AliasTypeName => _aliasTypeName;

		public AttributeData[] AttributeDataList => _attributeDataList;

		public override string ToString()
		{
			var attributes = string.Join(Environment.NewLine, AttributeDataList.Select(_ => _.ToString()));
			return $"{attributes} {_aliasTypeName ?? ParameterType.Name.ToString()} {Name}";
		}

		public TypeNameAliasUsingData GetTypeNameAliasUsingData()
		{
			if (_aliasTypeName == null)
			{
				return TypeNameAliasUsingData.Empty;
			}

			return new TypeNameAliasUsingData(_type, _aliasTypeName);
		}

		public static IEnumerable<Func<ITestContext, MethodParameterData>> GetPossibleVariations(ITestInterfaceGenerationOptions options)
		{
			var index = 0;
			foreach (var parameterAttributeCombination in
				options.ParameterAttributeDataBuilder.GetPossibleCombinations(options))
			{
				foreach (var type in options.MethodParameterTypes)
				{
					var values = options.ParameterValuesBuilder.GetValues(type);
					foreach (var value in values)
					{
						index++;
						var prefix = index % 2 == 0 ? "@" : "";
						var aliasTypeName = index % 1 == 0 ? "AliasType" + index : null;
						yield return (context) => new MethodParameterData(prefix + "param" + context.NextId(), type, value, aliasTypeName, parameterAttributeCombination(context).ToArray());
					}
				}
			}
		}
	}
}

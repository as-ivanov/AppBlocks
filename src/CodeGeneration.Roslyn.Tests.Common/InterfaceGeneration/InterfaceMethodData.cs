using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public class InterfaceMethodData
	{
		private readonly Type _returnType;
		private readonly string _name;
		private readonly AttributeData[] _attributeDataList;
		private readonly MethodParameterData[] _parameters;

		private InterfaceMethodData(Type returnType, string name, AttributeData[] attributeDataList,
			MethodParameterData[] parameters)
		{
			_returnType = returnType;
			_name = name;
			_attributeDataList = attributeDataList;
			_parameters = parameters;
		}

		public static IEnumerable<Func<ITestGenerationContext, InterfaceMethodData>> GetAllCombinations(ITestInterfaceGenerationOptions options)
		{
			var index = 0;
			var methodAttributeCombinations =  options.MethodAttributeDataBuilder.GetCombinations(options);
			foreach (var attributeData in methodAttributeCombinations)
			{
				foreach (var returnType in options.InterfaceMethodReturnTypes)
				{
					foreach (var parametersCount in options.MethodParameterNumbers)
					{
						foreach (var parameters in MethodParameterData.GetAllCombinations(options).ToList().Combinations(parametersCount))
						{
							index++;
							yield return (context) => new InterfaceMethodData(returnType, "Method" + index, attributeData(context).ToArray(), parameters.ToArray());
						}
					}
				}
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach (var attributeData in _attributeDataList)
			{
				sb.AppendLine(attributeData.ToString());
			}
			sb.AppendLine($"{_returnType} {_name}({string.Join(",", _parameters.Select(_ => _.ToString()))});");
			return sb.ToString();
		}
	}
}
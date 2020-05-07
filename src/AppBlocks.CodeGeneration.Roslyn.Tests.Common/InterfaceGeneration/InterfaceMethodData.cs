using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
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

		public string Name => _name;

		public Type ReturnType => _returnType;

		public AttributeData[] AttributeDataList => _attributeDataList;

		public MethodParameterData[] Parameters => _parameters;

		public static IEnumerable<Func<ITestContext, InterfaceMethodData>> GetPossibleVariations(
			ITestInterfaceGenerationOptions options)
		{
			var methodAttributeCombinations = options.MethodAttributeDataBuilder.GetPossibleCombinations(options);
			var methodParameterPossibleVariations = MethodParameterData.GetPossibleVariations(options).ToList();
			foreach (var attributeData in methodAttributeCombinations)
			{
				foreach (var returnType in options.InterfaceMethodReturnTypes)
				{
					foreach (var parametersCount in options.MethodParameterCounts)
					{
						Func<ITestContext, InterfaceMethodData> CreateInterfaceMethodDataVariation(IEnumerable<MethodParameterData> parameters)
						{
							return (context) => new InterfaceMethodData(returnType, "Method" + context.NextId(), attributeData(context).ToArray(), parameters.ToArray());
						}
						if (parametersCount == 1)
						{
							foreach (var parameters in methodParameterPossibleVariations
								.GetPossibleCombinations(parametersCount))
							{
								yield return CreateInterfaceMethodDataVariation(parameters);
							}
						}
						else
						{
							var parameters = methodParameterPossibleVariations.Take(parametersCount);
							yield return CreateInterfaceMethodDataVariation(parameters);
						}
					}
				}
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"//interface method with {Parameters.Length} parameters.");
			foreach (var attributeData in AttributeDataList)
			{
				sb.AppendLine(attributeData.ToString());
			}
			var returnType = ReturnType == typeof(void) ? "void" : ReturnType.FullName;
			sb.AppendLine($"{returnType} {Name}<TParam1, TParam2>({string.Join(",", Parameters.Select(_ => _.ToString()))}) where TParam1 : class, IComparable<TParam1> where TParam2 : struct, IEquatable<TParam2>;");
			return sb.ToString();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public class InterfaceData : IMemberData
	{
		private readonly AttributeData[] _attributeDataList;
		private readonly InterfaceData[] _inheritedInterfaces;
		private readonly bool _isSut;
		private readonly InterfaceMethodData[] _methods;
		private readonly string _name;
		private readonly string _namespace;

		public InterfaceData(string name, string @namespace) : this(name, @namespace, Array.Empty<AttributeData>(), Array.Empty<InterfaceMethodData>(), Array.Empty<InterfaceData>(), false)
		{
		}

		public InterfaceData(string name, string @namespace, AttributeData[] attributeDataList, InterfaceMethodData[] methods,
			InterfaceData[] inheritedInterfaces, bool isSut)
		{
			_name = name;
			_namespace = @namespace;
			_attributeDataList = attributeDataList;
			_methods = methods;
			_inheritedInterfaces = inheritedInterfaces;
			_isSut = isSut;
		}

		public InterfaceData[] InheritedInterfaces => _inheritedInterfaces;

		public AttributeData[] AttributeDataList => _attributeDataList;

		public InterfaceMethodData[] Methods => _methods;

		public string Name => _name;

		public string Namespace => _namespace;

		public bool IsSut => _isSut;

		public static IEnumerable<Func<ITestContext, InterfaceData>> GetPossibleVariations(ITestInterfaceGenerationOptions options)
		{
			var interfaceMethodPossibleVariations = InterfaceMethodData.GetPossibleVariations(options).ToList();

			var interfaceAttributeDataCombinations = options.InterfaceAttributeDataBuilder.GetPossibleCombinations(options);
			foreach (var attributeData in interfaceAttributeDataCombinations)
			{
				foreach (var inheritedInterfaceCount in options.InheritedInterfaceCounts)
				{
					var inheritedInterfaces = options.InheritanceListBuilder.GetInheritedInterfaces(options, inheritedInterfaceCount);

					foreach (var methodsCount in options.InterfaceMethodsCounts)
					{
						Func<ITestContext, InterfaceData> CreateInterfaceDataVariation(IEnumerable<Func<ITestContext, InterfaceMethodData>> methods)
						{
							return context => new InterfaceData("ITestInterface" + context.NextId(), options.InterfaceNamespace, attributeData(context).ToArray(), methods.Select(_ => _.Invoke(context)).ToArray(), inheritedInterfaces(context), true);
						}

						if (methodsCount == 1)
						{
							foreach (var methods in interfaceMethodPossibleVariations
								.GetPossibleCombinations(methodsCount))
							{
								yield return CreateInterfaceDataVariation(methods);
							}
						}
						else
						{
							var methods = interfaceMethodPossibleVariations.Take(methodsCount);
							yield return CreateInterfaceDataVariation(methods);
						}
					}
				}
			}
		}


		public override string ToString()
		{
			var sb = new CSharpBlockStringBuilder();
			sb.AppendLine($"//interface with {Methods.Length} methods.");
			foreach (var attributeData in _attributeDataList)
			{
				sb.AppendLine(attributeData.ToString());
			}

			using (sb.Block($"interface {_name}" + (InheritedInterfaces.Any() ? $" : {string.Join(",", InheritedInterfaces.Select(_ => _.Namespace + "." + _.Name))}" : string.Empty)))
			{
				foreach (var method in Methods)
				{
					sb.AppendLine(method.ToString());
				}
			}

			return sb.ToString();
		}
	}
}
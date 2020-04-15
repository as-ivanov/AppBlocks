using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public class InterfaceData : IMemberData
	{
		private readonly string _name;
		private readonly string _namespace;
		private readonly AttributeData[] _attributeDataList;
		private readonly InterfaceMethodData[] _methods;
		private readonly InterfaceData[] _inheritedInterfaces;
		private readonly bool _isSut;

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

		public string Name => _name;

		public string Namespace => _namespace;

		public bool IsSut => _isSut;

		public AttributeData[] AttributeDataList => _attributeDataList;

		public static IEnumerable<Func<ITestGenerationContext, InterfaceData>> GetAllCombinations(ITestInterfaceGenerationOptions options)
		{
			var index = 0;
			var interfaceAttributeDataCombinations = options.InterfaceAttributeDataBuilder.GetCombinations(options);
			foreach (var attributeData in interfaceAttributeDataCombinations)
			{
				foreach (var inheritedInterfaceCount in options.InheritedInterfaceNumbers)
				{
					var inheritedInterfaces = options.InheritanceListBuilder.GetInheritedInterfaces(options, inheritedInterfaceCount);

					foreach (var methodsCount in options.InterfaceMethodsNumbers)
					{
						foreach (var methods in InterfaceMethodData
							.GetAllCombinations(options).ToList()
							.Combinations(methodsCount))
						{
							index++;
							yield return (context) => { return new InterfaceData("ITestInterface" + index, options.InterfaceNamespace, attributeData(context).ToArray(), methods.Select(_ => _.Invoke(context)).ToArray(), inheritedInterfaces(context), true); };
						}
					}
				}
			}
		}


		public override string ToString()
		{
			var sb = new CSharpBlockStringBuilder();
			foreach (var attributeData in _attributeDataList)
			{
				sb.AppendLine(attributeData.ToString());
			}
			using (sb.Block($"interface {_name}" + (InheritedInterfaces.Any() ? $" : {string.Join(",", InheritedInterfaces.Select(_ => _.Namespace + "." + _.Name))}" : string.Empty)))
			{
				foreach (var method in _methods)
				{
					sb.AppendLine(method.ToString());
				}
			}

			return sb.ToString();
		}
	}
}
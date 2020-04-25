using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public static class AttributeDataExtensions
	{
		public static string[] GetInheritedInterfaceTypes(this AttributeData attributeData)
		{
			return attributeData.GetNamedArgumentValueArray(nameof(Attributes.Common.ImplementInterfaceAttribute
				.InheritedInterfaceTypes));
		}

		public static string GetNamedArgumentValue(this AttributeData attributeData, string name, string @default = default)
		{
			return GetNamedArgumentValue<string>(attributeData, name, @default);
		}

		public static T GetNamedArgumentValue<T>(this AttributeData attributeData, string name, T @default = default)
		{
			if (attributeData == null)
			{
				return @default;
			}

			var namedArgument = attributeData.NamedArguments.FirstOrDefault(_ => _.Key == name);
			if (namedArgument.Equals(default(KeyValuePair<string, TypedConstant>)))
			{
				return @default;
			}

			return (T) namedArgument.Value.Value;
		}

		public static string[] GetNamedArgumentValueArray(this AttributeData attributeData, string name,
			string @default = default)
		{
			return GetNamedArgumentValueArray<string>(attributeData, name, @default);
		}

		public static T[] GetNamedArgumentValueArray<T>(this AttributeData attributeData, string name, T @default = default)
		{
			if (attributeData == null)
			{
				return Array.Empty<T>();
			}

			var namedArgument = attributeData.NamedArguments.FirstOrDefault(_ => _.Key == name);
			if (namedArgument.Equals(default(KeyValuePair<string, TypedConstant>)))
			{
				return Array.Empty<T>();
			}

			return namedArgument.Value.Values.Select(_ => (T) _.Value).ToArray();
		}
	}
}
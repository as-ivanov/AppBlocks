using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public static class AttributeDataExtensions
	{
		public static string[] GetInheritedInterfaceTypes(this AttributeData attributeData)
		{
			return attributeData.ConstructorArguments == null
				? Array.Empty<string>()
				: attributeData.ConstructorArguments
					.Where(_ => _.Kind == TypedConstantKind.Array).SelectMany(_ => _.Values).Select(_ => _.Value as string).ToArray();
		}
	}
}
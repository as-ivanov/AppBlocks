using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	public static class ParameterSyntaxExtensions
	{
		public static ParameterSyntax OmitNullableAttribute(this ParameterSyntax parameterSyntax)
		{
			var resultedAttributeLists = new List<AttributeListSyntax>();
			foreach (var attributeList in parameterSyntax.AttributeLists)
			{
				var attributes = attributeList.Attributes.Where(_ => _.Name.ToString().Replace("global::", string.Empty) != CSharpConst.NullableAttributeName);
				var resultedAttributeList = AttributeList(attributes.ToSeparatedList());
				if (resultedAttributeList.Attributes.Any())
				{
					resultedAttributeLists.Add(resultedAttributeList);
				}
			}

			return parameterSyntax
				.WithType(NullableType(parameterSyntax.Type))
				.WithAttributeLists(List(resultedAttributeLists));
		}
	}


}

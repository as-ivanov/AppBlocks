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
			var makeNullable = false;
			foreach (var attributeList in parameterSyntax.AttributeLists)
			{
				var attributes = attributeList.Attributes.Where(_ => _.Name.ToString().Replace("global::", string.Empty) != CSharpConst.NullableAttributeName);
				makeNullable = makeNullable || attributes.Count() < attributeList.Attributes.Count;
				var resultedAttributeList = AttributeList(attributes.ToSeparatedList());
				if (resultedAttributeList.Attributes.Any())
				{
					resultedAttributeLists.Add(resultedAttributeList);
				}
			}

			return parameterSyntax
				.WithType(makeNullable ? NullableType(parameterSyntax.Type) : parameterSyntax.Type)
				.WithAttributeLists(List(resultedAttributeLists));
		}
	}


}

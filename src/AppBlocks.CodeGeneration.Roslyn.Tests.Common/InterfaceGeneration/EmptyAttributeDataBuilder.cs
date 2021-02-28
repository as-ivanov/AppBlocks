using System;
using System.Collections.Generic;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public class EmptyAttributeDataBuilder : IAttributeDataBuilder
	{
		public IEnumerable<Func<ITestContext, IEnumerable<AttributeData>>> GetPossibleCombinations(ITestInterfaceGenerationOptions options)
		{
			yield break;
		}
	}
}

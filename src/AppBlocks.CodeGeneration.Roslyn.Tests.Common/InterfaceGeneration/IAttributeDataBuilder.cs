using System;
using System.Collections.Generic;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public interface IAttributeDataBuilder
	{
		IEnumerable<Func<ITestContext, IEnumerable<AttributeData>>> GetPossibleCombinations(ITestInterfaceGenerationOptions options);
	}
}
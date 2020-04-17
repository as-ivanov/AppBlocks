using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public interface IAttributeDataBuilder
	{
		IEnumerable<Func<ITestContext, IEnumerable<AttributeData>>> GetPossibleCombinations(ITestInterfaceGenerationOptions options);
	}
}
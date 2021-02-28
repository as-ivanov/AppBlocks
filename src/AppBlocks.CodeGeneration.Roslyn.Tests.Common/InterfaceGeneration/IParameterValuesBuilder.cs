using System;
using System.Collections.Generic;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public interface IParameterValuesBuilder
	{
		IEnumerable<object> GetValues(Type type);
	}
}

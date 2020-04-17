using System;
using System.Collections.Generic;

namespace CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public interface IParameterValuesBuilder
	{
		IEnumerable<object> GetValues(Type type);
	}
}
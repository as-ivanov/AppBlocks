using System;
using System.Collections.Generic;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;

namespace AppBlocks.Logging.CodeGeneration.Roslyn.Tests
{
	public class LoggerInterfaceParameterValuesBuilder : IParameterValuesBuilder
	{
		public IEnumerable<object> GetValues(Type type)
		{
			if (!type.IsValueType)
			{
				yield return null;
			}
			else
			{
				yield return Activator.CreateInstance(type);
			}

			if (type == typeof(string))
			{
				yield return Guid.NewGuid().ToString();
			}
			else if (type == typeof(char))
			{
				yield return Guid.NewGuid().ToString()[0];
			}
			else if (type.IsAssignableFrom(typeof(float)))
			{
				yield return (float) Guid.NewGuid().GetHashCode() / Guid.NewGuid().GetHashCode();
			}
			else if (type.IsAssignableFrom(typeof(uint)))
			{
				yield return Guid.NewGuid().GetHashCode();
			}
			else if (type == typeof(Exception))
			{
				yield return new Exception(Guid.NewGuid().ToString());
			}
		}
	}
}
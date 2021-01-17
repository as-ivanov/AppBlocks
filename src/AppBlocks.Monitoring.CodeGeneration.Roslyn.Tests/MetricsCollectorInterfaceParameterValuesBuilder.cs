using System;
using System.Collections.Generic;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common;
using AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration;
using Castle.DynamicProxy.Internal;

namespace AppBlocks.Monitoring.CodeGeneration.Roslyn.Tests
{
	public class MetricsCollectorInterfaceParameterValuesBuilder : IParameterValuesBuilder
	{
		public IEnumerable<object> GetValues(Type type)
		{
			if (type.IsNullableType())
			{
				var underlyingType = Nullable.GetUnderlyingType(type);
				return GetValuesInternal(underlyingType);
			}
			return GetValuesInternal(type);
		}

		private IEnumerable<object> GetValuesInternal(Type type)
		{
			if (type.IsValueType)
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
			else if (type.IsFloatingNumericType())
			{
				yield return (float) Guid.NewGuid().GetHashCode() / Guid.NewGuid().GetHashCode();
			}
			else if (type.IsUnsignedNumericType())
			{
				yield return Math.Abs(Guid.NewGuid().GetHashCode());
			}
			else if (type.IsSignedNumericType())
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
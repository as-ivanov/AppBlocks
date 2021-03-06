using System;
using Microsoft.CodeAnalysis;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	internal static class EnumUtilities
	{
		internal static ulong ConvertEnumUnderlyingTypeToUInt64(object value, SpecialType specialType)
		{
			unchecked
			{
				switch (specialType)
				{
					case SpecialType.System_SByte:
						return (ulong)(sbyte)value;
					case SpecialType.System_Int16:
						return (ulong)(short)value;
					case SpecialType.System_Int32:
						return (ulong)(int)value;
					case SpecialType.System_Int64:
						return (ulong)(long)value;
					case SpecialType.System_Byte:
						return (byte)value;
					case SpecialType.System_UInt16:
						return (ushort)value;
					case SpecialType.System_UInt32:
						return (uint)value;
					case SpecialType.System_UInt64:
						return (ulong)value;

					default:
						// not using ExceptionUtilities.UnexpectedValue() because this is used by the Services layer
						// which doesn't have those utilities.
						throw new InvalidOperationException($"{specialType} is not a valid underlying type for an enum");
				}
			}
		}
	}
}

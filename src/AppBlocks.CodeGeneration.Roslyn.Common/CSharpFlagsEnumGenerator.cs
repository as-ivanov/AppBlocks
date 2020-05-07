using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	internal static class CSharpFlagsEnumGenerator
	{
		internal static SyntaxNode CreateEnumConstantValue(INamedTypeSymbol enumType, object constantValue)
		{
			// Code copied from System.Enum.
			var isFlagsEnum = IsFlagsEnum(enumType);
			if (isFlagsEnum)
			{
				return CreateFlagsEnumConstantValue(enumType, constantValue);
			}
			else
			{
				// Try to see if its one of the enum values.  If so, add that.  Otherwise, just add
				// the literal value of the enum.
				return CreateNonFlagsEnumConstantValue(enumType, constantValue);
			}
		}

		private static SyntaxNode CreateNonFlagsEnumConstantValue(INamedTypeSymbol enumType, object constantValue)
		{
			var underlyingSpecialType = enumType.EnumUnderlyingType.SpecialType;
			var constantValueULong = EnumUtilities.ConvertEnumUnderlyingTypeToUInt64(constantValue, underlyingSpecialType);

			// See if there's a member with this value.  If so, then use that.
			foreach (var member in enumType.GetMembers())
			{
				if (member.Kind == SymbolKind.Field)
				{
					var field = (IFieldSymbol) member;
					if (field.HasConstantValue)
					{
						var fieldValue =
							EnumUtilities.ConvertEnumUnderlyingTypeToUInt64(field.ConstantValue, underlyingSpecialType);
						if (constantValueULong == fieldValue)
						{
							return CreateMemberAccessExpression(field, enumType, underlyingSpecialType);
						}
					}
				}
			}

			// Otherwise, just add the enum as a literal.
			return CreateExplicitlyCastedLiteralValue(enumType, underlyingSpecialType, constantValue);
		}

		private static SyntaxNode CreateMemberAccessExpression(
			IFieldSymbol field, INamedTypeSymbol enumType, SpecialType underlyingSpecialType)
		{
			if (SyntaxFacts.IsValidIdentifier(field.Name))
			{
				return CSharpSyntaxGenerator.MemberAccessExpression(
					enumType.GenerateTypeSyntax(),
					SyntaxFactory.IdentifierName(field.Name));
			}
			else
			{
				return CreateExplicitlyCastedLiteralValue(enumType, underlyingSpecialType, field.ConstantValue);
			}
		}

		private static SyntaxNode CreateExplicitlyCastedLiteralValue(
			INamedTypeSymbol enumType,
			SpecialType underlyingSpecialType,
			object constantValue)
		{
			var expression = ExpressionGenerator.GenerateNonEnumValueExpression(
				enumType.EnumUnderlyingType, constantValue, canUseFieldReference: true);

			var constantValueULong = EnumUtilities.ConvertEnumUnderlyingTypeToUInt64(constantValue, underlyingSpecialType);
			if (constantValueULong == 0)
			{
				// 0 is always convertible to an enum type without needing a cast.
				return expression;
			}

			return SyntaxFactory.CastExpression((TypeSyntax) enumType, expression.Parenthesize());
		}

		private static void GetSortedEnumFieldsAndValues(
			INamedTypeSymbol enumType,
			List<(IFieldSymbol field, ulong value)> allFieldsAndValues)
		{
			var underlyingSpecialType = enumType.EnumUnderlyingType.SpecialType;
			foreach (var member in enumType.GetMembers())
			{
				if (member.Kind == SymbolKind.Field)
				{
					var field = (IFieldSymbol) member;
					if (field.HasConstantValue)
					{
						var value = EnumUtilities.ConvertEnumUnderlyingTypeToUInt64(field.ConstantValue, underlyingSpecialType);
						allFieldsAndValues.Add((field, value));
					}
				}
			}

			allFieldsAndValues.Sort(new EnumFieldsSorter());
		}

		private static SyntaxNode CreateFlagsEnumConstantValue(INamedTypeSymbol enumType, object constantValue)
		{
			// These values are sorted by value. Don't change this.
			var allFieldsAndValues = new List<(IFieldSymbol field, ulong value)>();
			GetSortedEnumFieldsAndValues(enumType, allFieldsAndValues);

			var usedFieldsAndValues = new List<(IFieldSymbol field, ulong value)>();
			return CreateFlagsEnumConstantValue(enumType, constantValue, allFieldsAndValues, usedFieldsAndValues);
		}

		private static SyntaxNode CreateFlagsEnumConstantValue(
			INamedTypeSymbol enumType,
			object constantValue,
			List<(IFieldSymbol field, ulong value)> allFieldsAndValues,
			List<(IFieldSymbol field, ulong value)> usedFieldsAndValues)
		{
			var underlyingSpecialType = enumType.EnumUnderlyingType.SpecialType;
			var constantValueULong = EnumUtilities.ConvertEnumUnderlyingTypeToUInt64(constantValue, underlyingSpecialType);

			var result = constantValueULong;

			// We will not optimize this code further to keep it maintainable. There are some
			// boundary checks that can be applied to minimize the comparisons required. This code
			// works the same for the best/worst case. In general the number of items in an enum are
			// sufficiently small and not worth the optimization.
			for (var index = allFieldsAndValues.Count - 1; index >= 0 && result != 0; index--)
			{
				var fieldAndValue = allFieldsAndValues[index];
				var valueAtIndex = fieldAndValue.value;

				if (valueAtIndex != 0 && (result & valueAtIndex) == valueAtIndex)
				{
					result -= valueAtIndex;
					usedFieldsAndValues.Add(fieldAndValue);
				}
			}

			//var syntaxFactory = GetSyntaxGenerator();

			// We were able to represent this number as a bitwise OR of valid flags.
			if (result == 0 && usedFieldsAndValues.Count > 0)
			{
				// We want to emit the fields in lower to higher value.  So we walk backward.
				SyntaxNode finalNode = null;
				for (var i = usedFieldsAndValues.Count - 1; i >= 0; i--)
				{
					var field = usedFieldsAndValues[i];
					var node = CreateMemberAccessExpression(field.field, enumType, underlyingSpecialType);
					if (finalNode == null)
					{
						finalNode = node;
					}
					else
					{
						finalNode = CSharpSyntaxGenerator.BitwiseOrExpression(finalNode, node);
					}
				}

				return finalNode;
			}
			else
			{
				// We couldn't find fields to OR together to make the value.

				// If we had 0 as the value, and there's an enum value equal to 0, then use that.
				var zeroField = GetZeroField(allFieldsAndValues);
				if (constantValueULong == 0 && zeroField != null)
				{
					return CreateMemberAccessExpression(zeroField, enumType, underlyingSpecialType);
				}
				else
				{
					// Add anything else in as a literal value.
					return CreateExplicitlyCastedLiteralValue(enumType, underlyingSpecialType, constantValue);
				}
			}
		}

		private static IFieldSymbol GetZeroField(List<(IFieldSymbol field, ulong value)> allFieldsAndValues)
		{
			for (var i = allFieldsAndValues.Count - 1; i >= 0; i--)
			{
				var (field, value) = allFieldsAndValues[i];
				if (value == 0)
				{
					return field;
				}
			}

			return null;
		}

		private static bool IsFlagsEnum(INamedTypeSymbol typeSymbol)
		{
			if (typeSymbol.TypeKind != TypeKind.Enum)
			{
				return false;
			}

			foreach (var attribute in typeSymbol.GetAttributes())
			{
				var ctor = attribute.AttributeConstructor;
				if (ctor != null)
				{
					var type = ctor.ContainingType;
					if (!ctor.Parameters.Any() && type.Name == "FlagsAttribute")
					{
						var containingSymbol = type.ContainingSymbol;
						if (containingSymbol.Kind == SymbolKind.Namespace &&
						    containingSymbol.Name == "System" &&
						    ((INamespaceSymbol) containingSymbol.ContainingSymbol).IsGlobalNamespace)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		private class EnumFieldsSorter : IComparer<(IFieldSymbol field, ulong value)>
		{
			public int Compare((IFieldSymbol field, ulong value) x, (IFieldSymbol field, ulong value) y)
			{
				unchecked
				{
					return
						(long) x.value < (long) y.value
							? -1
							: (long) x.value > (long) y.value
								? 1
								: -x.field.Name.CompareTo(y.field.Name);
				}
			}
		}
	}
}
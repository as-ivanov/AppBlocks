using System;
using System.Linq;
using AppBlocks.CodeGeneration.Roslyn.Common;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public class TypeNameAliasUsingData
	{
		public static readonly TypeNameAliasUsingData Empty = new TypeNameAliasUsingData();

		private readonly Type _type;
		private readonly string _alias;
		private readonly bool _isEmpty;

		private TypeNameAliasUsingData()
		{
			_isEmpty = true;
		}

		public TypeNameAliasUsingData(Type type, string alias)
		{
			_type = type;
			_alias = alias;
		}

		public override string ToString()
		{
			if (_isEmpty)
			{
				return string.Empty;
			}

			return $"using {_alias} = {_type.GetFriendlyName()};";
		}
	}
}

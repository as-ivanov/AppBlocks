using AppBlocks.CodeGeneration.Roslyn.Common;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public class NamespaceData
	{
		private readonly string _name;
		private readonly IMemberData[] _members;

		public NamespaceData(string name, params IMemberData[] members)
		{
			_name = name;
			_members = members;
		}

		public IMemberData[] Members => _members;

		public string Name => _name;


		public override string ToString()
		{
			var sb = new CSharpBlockStringBuilder();
			using (sb.Block($"namespace {Name}"))
			{
				foreach (var member in Members)
				{
					sb.AppendLine(member.ToString());
				}
			}

			return sb.ToString();
		}
	}
}
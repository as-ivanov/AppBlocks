namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public class NamespaceData
	{
		private readonly IMemberData[] _members;
		private readonly string _name;

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

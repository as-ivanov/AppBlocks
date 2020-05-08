namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public class CompilationEntryData
	{
		private readonly NamespaceData[] _namespaces;
		private readonly string[] _usingNamespaces;

		public CompilationEntryData(string[] usingNamespaces, params NamespaceData[] namespaces)
		{
			_usingNamespaces = usingNamespaces;
			_namespaces = namespaces;
		}

		public string[] UsingNamespaces => _usingNamespaces;

		public NamespaceData[] Namespaces => _namespaces;

		public override string ToString()
		{
			var sb = new CSharpBlockStringBuilder();
			foreach (var usingNamespace in UsingNamespaces)
			{
				sb.AppendLine($"using {usingNamespace};");
			}

			foreach (var @namespace in _namespaces)
			{
				sb.AppendLine(@namespace.ToString());
			}

			return sb.ToString();
		}
	}
}
namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public abstract class AttributeData
	{
		private readonly string _name;

		public AttributeData(string name)
		{
			_name = name;
		}

		public string Name => _name;
	}
}

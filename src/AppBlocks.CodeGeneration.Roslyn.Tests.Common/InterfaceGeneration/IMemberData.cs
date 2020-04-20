namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public interface IMemberData
	{
		string Name { get; }
		string Namespace { get; }
		bool IsSut { get; }
	}
}
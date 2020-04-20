namespace AppBlocks.Monitoring.Abstractions
{
  public interface IMeter
  {
    void Mark();
    void Mark(string item);
    void Mark(long amount);

    void Reset();
  }
}
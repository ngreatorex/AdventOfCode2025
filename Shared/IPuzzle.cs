
namespace AdventOfCode2025.Shared;

public interface IPuzzle<TSelf, TInstruction>
    where TSelf : Puzzle<TSelf, TInstruction>, new()
    where TInstruction : IParsable<TInstruction>, new()
{
    static abstract Task<TSelf> LoadAsync<TInput>(string fileName);
}
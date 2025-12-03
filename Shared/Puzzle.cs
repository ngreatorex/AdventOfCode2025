using Serilog;

namespace AdventOfCode2025.Shared;

public abstract class Puzzle<TSelf, TInstruction> : IPuzzle<TSelf, TInstruction> where TInstruction : IParsable<TInstruction>, new()
    where TSelf : Puzzle<TSelf, TInstruction>, new()
{
    public List<TInstruction> Instructions { get; init; } = [];

    public virtual void Run()
    {
        foreach (var instruction in Instructions)
        {
            ProcessInstruction(instruction);
        }
    }

    public abstract void PrintResult();

    protected virtual IEnumerable<string> Split(string input) => input.Split('\n');

    protected abstract void ProcessInstruction(TInstruction instruction);

    public static async Task<TSelf> LoadAsync<TInput>(string fileName)
    {
        using var streamReader = new StreamReader(fileName);
        var result = new TSelf();

        Log.Debug("Reading {FileName}", fileName);
        var inputString = await streamReader.ReadToEndAsync();

        result.Instructions.AddRange(
            result.Split(inputString)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim('\r'))
                .Select(s => TInstruction.Parse(s, null))
        );

        return result;
    }
}

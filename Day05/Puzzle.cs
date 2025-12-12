using AdventOfCode2025.Shared;
using Serilog;

namespace AdventOfCode2025.Day05;

public class Puzzle : Puzzle<Puzzle, Instruction>
{
    public List<Instruction> FreshRanges = [];
    public List<long> Available = [];

    public override void LogState(bool isInitial)
    {
        var freshCount = Available.Count(a => FreshRanges.Any(r => r.Contains(a)));

        Log.Information("{Count} available ingredients are fresh", freshCount);

        var firstIngredient = FreshRanges.Min(r => r.Start);
        var lastIngredient = FreshRanges.Max(r => r.End);

        Log.Debug("Ingredient IDs range from {Min} to {Max} ({Count} IDs)", firstIngredient, lastIngredient, lastIngredient - firstIngredient);

        var processedRanges = new List<Instruction>();
        foreach (var range in FreshRanges)
        {
            if (range.Start is null || range.End is null)
                throw new InvalidOperationException();

            var overlappingRanges = processedRanges.Where(r => range.Start <= r.End && range.End >= r.Start).ToList();
            if (overlappingRanges.Count > 0)
            {
                foreach (var r in overlappingRanges)
                    processedRanges.Remove(r);

                processedRanges.Add(new Instruction
                {
                    Type = Instruction.InstructionType.Freshness,
                    Start = overlappingRanges.Append(range).Min(r => r.Start),
                    End = overlappingRanges.Append(range).Max(r => r.End)
                });
            }
            else
            {
                processedRanges.Add(range);
            }
        }

        var total = processedRanges.Sum(r => r.End - r.Start + 1);

        Log.Debug("Merged ranges: {Ranges}", processedRanges.Select(r => $"{r.Start}-{r.End}"));
        Log.Information("{Count} IDs represent fresh ingredients", total);
    }

    protected override Task ProcessInstruction(Instruction instruction)
    {
        switch (instruction.Type)
        {
            case Instruction.InstructionType.Freshness:
                Log.Debug("Adding fresh ingredient range from {Start} to {End}", instruction.Start, instruction.End);
                FreshRanges.Add(instruction);
                break;

            case Instruction.InstructionType.Availablility:
                if (instruction.Start == null)
                    throw new InvalidDataException();
                Log.Debug("Adding available ingredient {ID}", instruction.Start);
                Available.Add(instruction.Start.Value);
                break;

            default:
                Log.Debug("Ignoring instruction of type {Type}", instruction.Type);
                break;
        }

        return Task.CompletedTask;
    }
}

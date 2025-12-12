using AdventOfCode2025.Shared;
using Serilog;
using Serilog.Context;

namespace AdventOfCode2025.Day02;

public class Puzzle : Puzzle<Puzzle, PuzzleInstruction>
{
    private HashSet<long> RuleOneInvalidIDs = [];
    private HashSet<long> RuleTwoInvalidIDs = [];

    public override void LogState(bool isInitial)
    {
        Log.Information("Invalid IDs by rule 1: {IDs}", RuleOneInvalidIDs);
        Log.Information("Invalid IDs by rule 2: {IDs}", RuleTwoInvalidIDs);
        Log.Information("Sum of invalid IDs by rule 1: {Sum}", RuleOneInvalidIDs.Sum());
        Log.Information("Sum of invalid IDs by rule 2: {Sum}", RuleTwoInvalidIDs.Sum());
    }

    protected override Task ProcessInstruction(PuzzleInstruction instruction)
    {
        for (long id = instruction.StartID; id <= instruction.EndID; id++)
        {
            using var d1 = LogContext.PushProperty("ID", id, false);
            var idString = id.ToString();

            for (var chunkSize = 1; chunkSize <= idString.Length / 2; chunkSize++)
            {
                using var d2 = LogContext.PushProperty("ChunkSize", chunkSize, false);

                if (idString.Length % chunkSize != 0)
                {
                    Log.Debug("String with length {Length} cannot be split into parts of length {ChunkSize}",
                        idString.Length, chunkSize);
                    continue;
                }

                var parts = Enumerable.Range(0, idString.Length / chunkSize)
                    .Select(i => idString.Substring(i * chunkSize, chunkSize))
                    .ToList();

                Log.Debug("Chunks: {Chunks}", parts);

                if (parts.All(p => p == parts[0]))
                {
                    Log.Debug("ID {ID} is invalid by rule two", id);
                    RuleTwoInvalidIDs.Add(id);

                    if (parts.Count == 2)
                    {
                        Log.Debug("ID {ID} is invalid by rule one", id);
                        RuleOneInvalidIDs.Add(id);
                    }
                }
            }

        }

        return Task.CompletedTask;
    }

    protected override IEnumerable<string> Split(string input, bool _) => input.Split(',');
}

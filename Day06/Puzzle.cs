using AdventOfCode2025.Shared;
using Serilog;
using System.Diagnostics;
using System.Text;

namespace AdventOfCode2025.Day06;

public class Puzzle : Puzzle<Puzzle, Problem>
{
    public long AnswerSum { get; private set; }

    protected override IEnumerable<string> Split(string input, bool isPartTwo)
    {
        var lines = input.Split('\n').Select(s => s.Trim('\r')).ToList();
        var lineLength = lines[0].Length;
        Debug.Assert(lines.All(l => l.Length == lineLength));

        var problemStart = 0;

        if (isPartTwo)
        {
            problemStart = 0;
            for (var i = 0; i < lineLength; i++)
            {
                var isSeparator = lines.All(l => l[i] == ' ');
                if (isSeparator || i == lineLength - 1)
                {
                    var problemEnd = i == lineLength - 1 ? lineLength : i;
                    var parts = new StringBuilder[problemEnd - problemStart + 1];

                    for (var j = problemStart; j < problemEnd; j++)
                    {
                        var outputIndex = j - problemStart;

                        for (var k = 0; k < lines.Count - 1; k++)
                        {
                            if (parts[outputIndex] == null)
                                parts[outputIndex] = new StringBuilder();

                            if (lines[k][j] == ' ')
                                continue;

                            parts[outputIndex].Append(lines[k][j]);
                        }
                    }
                    parts[parts.Length - 1] = new StringBuilder(lines[lines.Count - 1].Substring(problemStart, problemEnd - problemStart).Trim());

                    Log.Debug("Split problem into {Parts}", parts);

                    var result = string.Join("\n", parts.Select(s => s.ToString().Trim()));
                    yield return result;

                    problemStart = i + 1;
                }
            }
        }
        else
        {
            for (var i = 0; i < lineLength; i++)
            {
                var isSeparator = lines.All(l => l[i] == ' ');
                if (isSeparator || i == lineLength - 1)
                {
                    var problemEnd = i == lineLength - 1 ? lineLength : i;
                    var parts = lines.Select(l => l.Substring(problemStart, problemEnd - problemStart).Trim()).ToList();

                    Log.Debug("Split problem into {Parts}", parts);

                    var result = string.Join("\n", parts);
                    yield return result;

                    problemStart = i;
                }
            }
        }
    }

    public override void LogState(bool isInitial)
    {
        Log.Information("Sum of all puzzles is {Sum}", AnswerSum);
    }

    protected override Task ProcessInstruction(Problem instruction)
    {
        AnswerSum += instruction.Answer;

        return Task.CompletedTask;
    }
}

using AdventOfCode2025.Shared;
using Serilog;

namespace AdventOfCode2025.Day01;

public class Puzzle : Puzzle<Puzzle, PuzzleInstruction>
{
    private const int MAX_POSITION = 99;

    public long CurrentPosition { get; private set; } = 50;
    public long TimesPointingAtZeroAfterInstruction { get; private set; } = 0;
    public long TimesPointingAtZero { get; private set; } = 0;


    protected override Task ProcessInstruction(PuzzleInstruction instruction)
    {
        var startingPosition = CurrentPosition;

        for (int i = 0; i < instruction.Turns; i++)
        {
            SingleMove(instruction.Direction);
        }

        if (CurrentPosition == 0)
            TimesPointingAtZeroAfterInstruction++;

        Log.Debug("Processed move {Direction} {Turns}. Position: {startingPosition} -> {CurrentPosition}. Passing = {TimesPointingAtZero}, Pointing = {TimesPointingAtZeroAfterInstruction}",
            instruction.Direction, instruction.Turns, startingPosition, CurrentPosition, TimesPointingAtZero, TimesPointingAtZeroAfterInstruction);

        return Task.CompletedTask;
    }

    private void SingleMove(TurnDirection direction)
    {
        if (direction == TurnDirection.Left)
            CurrentPosition--;
        else
            CurrentPosition++;

        if (CurrentPosition > MAX_POSITION)
            CurrentPosition = 0;
        if (CurrentPosition < 0)
            CurrentPosition = MAX_POSITION;

        if (CurrentPosition == 0)
            TimesPointingAtZero++;
    }

    public override void LogState(bool isInitial)
    {
        Log.Information("Puzzle with {Count} instructions. Current position = {CurrentPosition}, Passing = {Passing}, Pointing = {Pointing}",
            Instructions.Count, CurrentPosition, TimesPointingAtZero, TimesPointingAtZeroAfterInstruction);
    }
}

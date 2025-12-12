using AdventOfCode2025.Day02;
using AdventOfCode2025.Shared;

partial class Program : Program<Program, Puzzle, PuzzleInstruction>
{
    public Program()
    {
        LogOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{InputFile}] [{ID}] [{ChunkSize}] {Message:lj}{NewLine}{Exception}";
    }

    public static new async Task Main(string[] args) => await Program<Program, Puzzle, PuzzleInstruction>.Main(args);
}
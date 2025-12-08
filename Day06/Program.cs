using AdventOfCode2025.Shared;

namespace AdventOfCode2025.Day06;

partial class Program : Program<Program, Puzzle, Problem>
{
    protected override bool PartTwoRequiresRerun => true;

    public static new async Task Main(string[] args) => await Program<Program, Puzzle, Problem>.Main(args);
}
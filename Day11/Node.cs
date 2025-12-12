namespace AdventOfCode2025.Day11;

public class Node
{
    public required string DeviceID { get; init; }

    public override string ToString() => $"{DeviceID}";
}

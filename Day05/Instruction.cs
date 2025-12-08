using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AdventOfCode2025.Day05;

public partial class Instruction : IParsable<Instruction>
{
    public InstructionType Type { get; init; }

    public long? Start { get; init; }
    public long? End { get; init; }

    public bool Contains(long id) => id >= Start && id <= End;

    public override string ToString() => $"{Type} {Start} - {End}";

    public static Instruction Parse(string s, IFormatProvider? provider)
    {
        var freshnessMatch = FreshnessFormat().Match(s);
        if (freshnessMatch.Success)
        {
            return new Instruction
            {
                Type = InstructionType.Freshness,
                Start = long.Parse(freshnessMatch.Groups["start"].Value),
                End = long.Parse(freshnessMatch.Groups["end"].Value)
            };
        }

        var availabilityMatch = AvailabilityFormat().Match(s);
        if (availabilityMatch.Success)
        {
            return new Instruction
            {
                Type = InstructionType.Availablility,
                Start = long.Parse(availabilityMatch.Value)
            };
        }

        return new Instruction
        {
            Type = InstructionType.Separator
        };
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Instruction result) => throw new NotImplementedException();


    [GeneratedRegex(@"^(?<start>\d+)-(?<end>\d+)$")]
    private static partial Regex FreshnessFormat();

    [GeneratedRegex(@"^\d+$")]
    private static partial Regex AvailabilityFormat();


    public enum InstructionType
    {
        Freshness,
        Separator,
        Availablility
    }
}

using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2025.Day03;

public partial record BatteryBank : IParsable<BatteryBank>
{
    [GeneratedRegex(@"^[0-9]+$")]
    private static partial Regex LineRegex();

    public List<int> BatteryJoltages { get; init; } = [];

    public long MaxJoltageFromTwoBatteries => GetMaxJoltage(2);

    public long MaxJoltageFromTwelveBatteries => GetMaxJoltage(12);

    public override string ToString()
    {
        var sb = new StringBuilder(BatteryJoltages.Count);
        foreach (var joltage in BatteryJoltages)
            sb.Append(joltage.ToString());
        return sb.ToString();
    }

    public long GetMaxJoltage(int numberOfBatteries)
    {
        Log.Debug("Calculating max joltage for {Count} batteries in bank {Bank}", numberOfBatteries, ToString());
        return GetJoltage(BatteryJoltages, numberOfBatteries);
    }

    public static BatteryBank Parse(string s, IFormatProvider? provider)
    {
        var match = LineRegex().Match(s);

        if (!match.Success)
            throw new ArgumentOutOfRangeException(nameof(s), "Invalid instruction");

        return new BatteryBank()
        {
            BatteryJoltages = [.. (s.Select(c => int.Parse(c.ToString())))]
        };
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out BatteryBank result) => throw new NotImplementedException();

    private static long GetJoltage(List<int> batteryBank, int batteriesToFind = 2)
    {
        var numDigitsAfterThis = batteriesToFind - 1;

        // We can't look further than numDigitsAfterThis from the end (as we still need to add that many digits)
        var numDigitsToLookahead = batteryBank.Count - numDigitsAfterThis;
        var lookaheadRange = batteryBank.Take(numDigitsToLookahead).ToList();

        // Find the position of the first occurrence of the maxmimum digit in our lookahead range
        var (bestDigit, bestDigitPos) = lookaheadRange
            .Select((value, index) => (value, index))
            .MaxBy(x => x.value);  // Relies on MaxBy returning the *first* instance of the match. Is this part of the contract? Who knows ¯\_(ツ)_/¯

        var joltage = bestDigit * (long)Math.Pow(10, numDigitsAfterThis);

        Log.Debug("Found digit {Digit} at position {DigitPos} out of {LookaheadCount} possibilities. {StillToFind} digits still to find.",
            bestDigit, (bestDigitPos + 1), numDigitsToLookahead, numDigitsAfterThis);

        // If we still need more digits, recurse...
        if (numDigitsAfterThis > 0)
            joltage += GetJoltage([.. batteryBank.Skip(bestDigitPos + 1)], numDigitsAfterThis);

        return joltage;
    }
}

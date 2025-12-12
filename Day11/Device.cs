using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AdventOfCode2025.Day11;

public partial record Device : IParsable<Device>
{
    public required string ID { get; init; }
    public List<string> ConnectedDeviceIDs { get; init; } = [];

    public static Device Parse(string s, IFormatProvider? provider)
    {
        var match = DeviceRegex().Match(s);

        if (!match.Success)
            throw new ArgumentException("Invalid device definition", nameof(s));

        var id = match.Groups["id"].Value;
        var connectedIDs = match.Groups["connectedDevice"].Captures.Select(c => c.Value).ToList();

        return new Device
        {
            ID = id,
            ConnectedDeviceIDs = connectedIDs
        };
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Device result) => throw new NotImplementedException();

    [GeneratedRegex("(?<id>[a-z]{3}):(?: (?<connectedDevice>[a-z]{3}))+")]
    private static partial Regex DeviceRegex();
}

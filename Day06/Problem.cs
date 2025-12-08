using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode2025.Day06;

public record Problem : IParsable<Problem>
{
    public List<int> Operands { get; init; } = [];

    public required char Operator { get; set; }

    public long Answer
    {
        get
        {
            return Operator switch
            {
                '*' => Operands.Select(i => (long)i).Aggregate((a, b) => a * b),
                '+' => Operands.Select(i => (long)i).Sum(),
                _ => throw new NotImplementedException(),
            };
        }
    }

    public static Problem Parse(string s, IFormatProvider? provider)
    {
        var parts = s.Split('\n');
        var operands = new List<int>();

        for (var i = 0; i < parts.Length; i++)
        {
            if (int.TryParse(parts[i], out var value))
                operands.Add(value);
            else if (i == parts.Length - 1)
                return new Problem
                {
                    Operands = operands,
                    Operator = parts[i].Single(),
                };
            else
                throw new InvalidDataException();
        }

        throw new InvalidDataException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Problem result) => throw new NotImplementedException();
}

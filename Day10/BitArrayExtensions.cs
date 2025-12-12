using System.Collections;
using System.Text;

namespace AdventOfCode2025.Day10;

public static class BitArrayExtensions
{
    public static byte[] ToByteArray(this BitArray bits)
    {
        byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
        bits.CopyTo(ret, 0);
        return ret;
    }

    public static string ToStateString(this BitArray bits)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < bits.Count; i++)
        {
            sb.Append(bits[i] ? '#' : '.');
        }

        return sb.ToString();
    }

    public static bool ValueEquals(this BitArray bits1, BitArray bits2) => Enumerable.SequenceEqual(bits1.ToByteArray(), bits2.ToByteArray());
}

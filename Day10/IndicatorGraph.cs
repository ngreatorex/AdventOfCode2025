using AdventOfCode2025.Shared.Graphs;
using System.Collections;

namespace AdventOfCode2025.Day10;

public record IndicatorNode(BitArray State) : IEquatable<IndicatorNode>
{
    public virtual bool Equals(IndicatorNode? other)
    {
        if (other == null)
            return false;

        return State.ValueEquals(other.State);
    }

    public override int GetHashCode()
    {
        int hash = 0;
        if (State != null)
        {
            hash = (hash * 17) + State.Length;
            for (var i = 0; i < State.Length; i++)
            {
                hash *= 17;
                hash += State[i].GetHashCode();
            }
        }
        return hash;
    }
}

public record IndicatorEdge : Edge<IndicatorNode>
{
    public required Button Button { get; init; }
}

public record IndicatorGraph : Graph<IndicatorNode, IndicatorEdge>
{

}
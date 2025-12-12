using AdventOfCode2025.Shared.Graphs;

namespace AdventOfCode2025.Day10;

public record JoltageNode(List<int> State) : IEquatable<JoltageNode>
{
    public virtual bool Equals(JoltageNode? other)
    {
        if (other == null)
            return false;

        return Enumerable.SequenceEqual(State, other.State);
    }

    public override int GetHashCode()
    {
        int hash = 0;
        if (State != null)
        {
            hash = (hash * 17) + State.Count;
            for (var i = 0; i < State.Count; i++)
            {
                hash *= 17;
                hash += State[i];
            }
        }
        return hash;
    }
}

public record JoltageEdge : Edge<JoltageNode>
{
    public required Button Button { get; init; }
}

public record JoltageGraph : Graph<JoltageNode, JoltageEdge>
{

}
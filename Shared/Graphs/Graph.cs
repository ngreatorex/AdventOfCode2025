using Serilog;
using System.Collections.Concurrent;

namespace AdventOfCode2025.Shared.Graphs;


public record Edge<T>
{
    public required T Source { get; init; }
    public required T Target { get; init; }

    public override string ToString() => $"{Source} => {Target}";
}

public partial record Graph<TNode, TEdge>
    where TNode : class
    where TEdge : Edge<TNode>
{
    public ConcurrentHashSet<TNode> Nodes { get; init; } = [];
    public List<TEdge> Edges { get; init; } = [];

    public required TNode Start { get; init; }

    public long CountPathsSatisfyingPredicate(Func<TNode, bool> endNodeIdentifier, Func<List<TEdge>, bool> pathChecker)
    {
        return DFS(endNodeIdentifier, pathChecker, true).Count();
    }

    public List<string> GetPaths(TNode goal)
    {
        var memo = new ConcurrentDictionary<TNode, List<string>>();
        return GetPathsFromStart(goal, memo);
    }

    public long CountPaths(Func<TNode, bool> endNodeIdentifier)
    {
        var endNodes = Nodes.Where(endNodeIdentifier).ToList();
        var memo = new ConcurrentDictionary<TNode, long>();
        return endNodes.AsParallel().Sum(n => CountPathsFromStart(n, []));
    }

    public IEnumerable<List<TEdge>> GenerativeSearch(TreeSearchMode mode,
        Func<TNode, IEnumerable<TEdge>> edgeGenerator,
        Func<TNode, bool> goalChecker,
        Func<List<TEdge>, bool> pathChecker,
        bool isExhaustive = false)
    {
        var visited = new ConcurrentHashSet<(TNode node, List<TEdge>? path)>();
        var tsc = new TreeSearchCollection(mode);
        tsc.Add((Start, []));

        var stack = new ConcurrentStack<(TNode node, List<TEdge> path)>();
        stack.Push((Start, []));

        ConcurrentBag<List<TEdge>> output = [];
        long visitedStates = 0;
        long foundPaths = 0;

        var degreeOfParallelism = Environment.ProcessorCount * 4;
        Parallel.For(0, degreeOfParallelism, (n, p) =>
        {
            while (tsc.TryIterate(out var state))
            {
                var visitCount = Interlocked.Increment(ref visitedStates);
                if (visitCount % 100000 == 0)
                {
                    Log.Debug("Visited {Count:N0} states", visitCount);
                }

                Log.Verbose("Visiting state {State}", state);

                if (goalChecker(state.node))
                {
                    if (pathChecker(state.path))
                    {
                        Log.Verbose("Found end state {Node} with path {Path}", state.node, state.path);
                        output.Add(state.path);

                        var outputCount = Interlocked.Increment(ref foundPaths);
                        if (outputCount % 100000 == 0)
                            Log.Debug("Found {Count:N0} paths to an end state", outputCount);

                        if (!isExhaustive)
                            p.Break();
                    }

                    if (isExhaustive)
                        continue;
                }

                foreach (var edge in edgeGenerator(state.node))
                {
                    if (state.path.Contains(edge))
                    {
                        Log.Debug("Loop detected!");
                        continue;
                    }

                    List<TEdge> newPath = [.. state.path, edge];
                    if (visited.TryAdd((edge.Target, isExhaustive ? newPath : null)))
                    {
                        tsc.Add((edge.Target, newPath));
                    }
                    else
                    {
                        Log.Debug("Already visited node {Node}", edge.Target);
                    }
                }
            }
        });

        return output;
    }


    public bool CanReach(TNode source, TNode goal)
    {
        var edgeDict = Edges.GroupBy(e => e.Target).ToDictionary(g => g.Key, g => g.ToList());
        return CanReach(edgeDict, goal, source);
    }

    private bool CanReach(Dictionary<TNode, List<TEdge>> edgeDict, TNode source, TNode goal)
    {
        if (source == goal)
            return true;

        return edgeDict.ContainsKey(source) ? edgeDict[source].Any(e => CanReach(edgeDict, e.Source, goal)) : false;
    }

    protected IEnumerable<List<TEdge>> StandardEdgeGeneratorSearch(TreeSearchMode mode, Func<TNode, bool> goalChecker, Func<List<TEdge>, bool> pathChecker, bool isExhaustive)
    {
        var edgeDict = Edges.GroupBy(e => e.Source).ToDictionary(g => g.Key, g => g.ToList());
        return GenerativeSearch(mode, n => edgeDict.TryGetValue(n, out var r) ? r : [], goalChecker, pathChecker, isExhaustive);
    }

    public IEnumerable<List<TEdge>> DFS(Func<TNode, bool> goalChecker, Func<List<TEdge>, bool> pathChecker, bool isExhaustive = false) => StandardEdgeGeneratorSearch(TreeSearchMode.DepthFirst, goalChecker, pathChecker, isExhaustive);
    public IEnumerable<List<TEdge>> BFS(Func<TNode, bool> goalChecker, Func<List<TEdge>, bool> pathChecker, bool isExhaustive = false) => StandardEdgeGeneratorSearch(TreeSearchMode.BreadthFirst, goalChecker, pathChecker, isExhaustive);

    protected long CountPathsFromStart(TNode node, ConcurrentDictionary<TNode, long> memo) => CountPathsBetween(Start, node, memo);
    protected List<string> GetPathsFromStart(TNode node, ConcurrentDictionary<TNode, List<string>> memo) => GetPathsBetween(Start, node, memo);

    protected long CountPathsBetween(TNode currentNode, TNode destination, ConcurrentDictionary<TNode, long> memo)
    {
        if (currentNode == destination)
            return 1;

        if (memo.TryGetValue(currentNode, out long value))
            return value;

        long count = 0;
        foreach (var edge in Edges.Where(e => e.Source == currentNode))
        {
            count = checked(count + CountPathsBetween(edge.Target, destination, memo));
        }

        return memo[currentNode] = count;
    }

    protected List<string> GetPathsBetween(TNode currentNode, TNode destination, ConcurrentDictionary<TNode, List<string>> memo)
    {
        if (currentNode == destination)
            return [""];

        if (memo.TryGetValue(currentNode, out var value))
            return value;

        List<string> paths = [];
        foreach (var edge in Edges.Where(e => e.Source == currentNode))
        {
            paths.AddRange(GetPathsBetween(edge.Target, destination, memo).Select(p => $"{currentNode}{p}"));
        }

        return memo[currentNode] = paths;
    }


    internal class TreeSearchCollection
    {
        protected ConcurrentQueue<(TNode node, List<TEdge> path)> _queue = new();
        protected ConcurrentStack<(TNode node, List<TEdge> path)> _stack = new();

        internal TreeSearchCollection(TreeSearchMode mode)
        {
            Mode = mode;
        }

        internal TreeSearchMode Mode { get; }

        internal void Add((TNode node, List<TEdge> path) state)
        {
            if (Mode == TreeSearchMode.DepthFirst)
                _stack.Push(state);
            else if (Mode == TreeSearchMode.BreadthFirst)
                _queue.Enqueue(state);
            else
                throw new NotImplementedException();
        }

        internal bool TryIterate(out (TNode node, List<TEdge> path) state)
        {
            return Mode switch
            {
                TreeSearchMode.DepthFirst => _stack.TryPop(out state),
                TreeSearchMode.BreadthFirst => _queue.TryDequeue(out state),
                _ => throw new NotImplementedException(),
            };
        }
    }

}
using AdventOfCode2025.Shared;
using AdventOfCode2025.Shared.Graphs;
using Serilog;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace AdventOfCode2025.Day07;

public class Puzzle : Puzzle<Puzzle, Line>
{
    public char[,] Grid { get; private set; } = new char[0, 0];

    private int Height => Grid.GetLength(0);
    private int Width => Grid.GetLength(1);

    public override void LogState(bool isInitial)
    {
        Log.Information("Current state: \n{State}", ToString());
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Grid.GetLength(0); i++)
        {
            for (var j = 0; j < Grid.GetLength(1); j++)
            {
                sb.Append(Grid[i, j]);
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public override async Task Run()
    {
        await base.Run();

        var graph = TranslateToGraph();
        Log.Debug("Created DAG with {Nodes} nodes and {Edges} edges", graph.Nodes.Count, graph.Edges.Count);

        var splitCount = graph.Nodes.AsEnumerable().Count(n => n.IsSplitter == true);
        Log.Information("Split count = {Splits}", splitCount);

        var pathCount = graph.CountPaths(Height);
        Log.Information("Path count = {Count}", pathCount);
    }

    private Graph TranslateToGraph()
    {
        var startPosition = (-1, -1);

        for (var i = 0; i < Height; i++)
        {
            for (var j = 0; j < Width; j++)
            {
                if (Grid[i, j] == 'S')
                {
                    startPosition = (i, j);
                    break;
                }
            }
            if (startPosition != (-1, -1))
                break;
        }

        if (startPosition == (-1, -1))
            throw new InvalidOperationException("Can't find start");

        var startNode = new Node()
        {
            Position = startPosition
        };

        var graph = new Graph()
        {
            Start = startNode,
            Nodes = [startNode]
        };
        var visited = new HashSet<Node>();
        var nodeLookup = new ConcurrentDictionary<(int y, int x), Node>();
        nodeLookup[startPosition] = startNode;
        var stack = new Stack<((int y, int x) position, Node node)>();
        stack.Push((startPosition, startNode));

        while (stack.TryPop(out var currentState))
        {
            if (!visited.Add(currentState.node))
                continue;

            var currentChar = Grid[currentState.position.y, currentState.position.x];
            if (currentChar == 'S' || currentChar == '.')
            {
                if (currentState.position.y < Height - 1)
                {
                    AddNewEdge(currentState.node, currentState.position.y + 1, currentState.position.x);
                }
            }
            else if (currentChar == '^')
            {
                currentState.node.IsSplitter = true;
                if (currentState.position.x > 0)
                {
                    AddNewEdge(currentState.node, currentState.position.y, currentState.position.x - 1);
                }

                if (currentState.position.x < Width - 1)
                {
                    AddNewEdge(currentState.node, currentState.position.y, currentState.position.x + 1);
                }
            }
            else
                throw new InvalidOperationException($"Unknown character '{currentChar}' in data");
        }

        return graph;

        void AddNewEdge(Node currentNode, int y, int x)
        {
            var newPosition = (y, x);
            var target = nodeLookup.GetOrAdd(newPosition, new Node { Position = newPosition });
            graph.Nodes.Add(target);

            var edge = new Edge<Node> { Source = currentNode, Target = target };
            graph.Edges.Add(edge);

            stack.Push((newPosition, target));
        }
    }

    protected override Task ProcessInstruction(Line instruction)
    {
        if (Grid.GetLength(0) == 0)
        {
            Grid = new char[1, instruction.Cells.Length];
        }
        else
        {
            var temp = new char[Height + 1, Width];
            Array.ConstrainedCopy(Grid, 0, temp, 0, Grid.Length);
            Grid = temp;
        }

        Debug.Assert(Width == instruction.Cells.Length);

        for (var i = 0; i < instruction.Cells.Length; i++)
            Grid[Height - 1, i] = instruction.Cells[i];

        return Task.CompletedTask;
    }
}

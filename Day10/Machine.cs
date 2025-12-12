using AdventOfCode2025.Shared.Graphs;
using Microsoft.Z3;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Log = Serilog.Log;

namespace AdventOfCode2025.Day10;

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> input) where T : class
    {
        return (IEnumerable<T>)input.Where(i => i != null);
    }
}

public partial class Machine : IParsable<Machine>
{
    public required BitArray GoalState { get; init; }
    public required List<Button> Buttons { get; init; }
    public required List<int> JoltageGoal { get; init; }

    public int FindJoltagePathByLinearAlgebra()
    {
        using Context ctx = new Context(new Dictionary<string, string>() { { "model", "true" } });
        using var optimizer = ctx.MkOptimize();
        var zero = ctx.MkInt(0);

        var buttonVariables = new Dictionary<IntExpr, Button>();
        for (int i = 0; i < Buttons.Count; i++)
        {
            var intConst = ctx.MkIntConst(i.ToString());
            buttonVariables[intConst] = Buttons[i];
            optimizer.Add(ctx.MkGe(intConst, zero));
        }

        for (var i = 0; i < JoltageGoal.Count; i++)
        {
            var applicableButtons = buttonVariables
                .Where(kvp => kvp.Value.ToggledLightIndexes.Contains(i))
                .Select(kvp => kvp.Key);

            var target = JoltageGoal[i];

            if (applicableButtons.Any())
            {
                var sum = ctx.MkAdd(applicableButtons);
                optimizer.Add(ctx.MkEq(sum, ctx.MkInt(target)));
            }
        }

        var total = ctx.MkAdd(buttonVariables.Keys);
        var min = optimizer.MkMinimize(total);

        var type = optimizer.Check();

        if (type != Status.SATISFIABLE)
        {
            throw new InvalidOperationException("Unknown answer");
        }

        var model = optimizer.Model;
        var results = model.Consts
            .Where(kvp => kvp.Value.IsIntNum)
            .ToDictionary(
                kvp => Buttons[int.Parse(kvp.Key.Name.ToString())],
                kvp => int.Parse(kvp.Value.SExpr())
            );

        Log.Debug("Found a solution! {Buttons}",
            results.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value));

        return results.Values.Sum();
    }

    public IEnumerable<Button> FindJoltagePathByBFS()
    {
        var limit = JoltageGoal.Max();

        var node = new JoltageNode([.. Enumerable.Repeat(0, GoalState.Count)]);
        var graph = new JoltageGraph()
        {
            Nodes = [node],
            Start = node
        };

        Log.Debug("Finding shortest path to {GoalState}", JoltageGoal);

        var path = graph.GenerativeSearch(TreeSearchMode.BreadthFirst,
            node => Buttons.Select(b => (button: b, newState: b.ApplyJoltages(node.State))).Select(s =>
        {
            var newNode = new JoltageNode(s.newState);
            if (graph.Nodes.Add(newNode))
            {
                Log.Debug("Generating new possible state of {State}", newNode.State);
            }

            Log.Debug("Generating edge from {OldState} to {NewState} by pushing button {Button}",
                node.State, s.newState, s.button);

            if (s.newState.Where((val, index) => val > JoltageGoal[index]).Count() > 0)
                return null;
            return new JoltageEdge
            {
                Button = s.button,
                Source = node,
                Target = newNode
            };
        }).WhereNotNull(), b =>
        {
            Log.Debug("Checking if state {Current} is equal to goal {Goal} is {Value)",
                b.State, JoltageGoal, Enumerable.SequenceEqual(b.State, JoltageGoal));
            return Enumerable.SequenceEqual(b.State, JoltageGoal);
        }, _ => true);

        if (path == null)
            throw new InvalidOperationException("No solution found?");

        return path.Single().Select(e => e.Button);
    }

    public IEnumerable<Button> FindPathToGoal()
    {
        var node = new IndicatorNode(new BitArray(GoalState.Count));
        var graph = new IndicatorGraph()
        {
            Nodes = [node],
            Start = node
        };

        Log.Debug("Finding shortest path to {GoalState}", GoalState.ToStateString());

        var path = graph.GenerativeSearch(TreeSearchMode.BreadthFirst, node => Buttons.Select(b => (button: b, newState: b.Apply(node.State))).Select(s =>
        {
            var newNode = new IndicatorNode(s.newState);
            if (graph.Nodes.Add(newNode))
            {
                Log.Debug("Generating new possible state of {State}", newNode.State.ToStateString());
            }

            Log.Debug("Generating edge from {OldState} to {NewState} by pushing button {Button}",
                node.State.ToStateString(), s.newState.ToStateString(), s.button);

            return new IndicatorEdge
            {
                Button = s.button,
                Source = node,
                Target = newNode
            };
        }), b =>
        {
            Log.Debug("Checking if state {Current} is equal to goal {Goal} is {Value)",
                b.State.ToStateString(),
                GoalState.ToStateString(),
                b.State.ValueEquals(GoalState));
            return b.State.ValueEquals(GoalState);
        }, _ => true);

        if (path == null)
            throw new InvalidOperationException("No solution found?");

        return path.Single().Select(e => e.Button);
    }

    public void LogState(bool isInitial)
    {
        var lights = GoalState.ToStateString();
        var buttons = string.Join(" ", Buttons.Select(b => b.ToString()));

        Log.Information("[{Lights}] {Buttons} {{{Joltages}}}", lights, buttons, JoltageGoal);
    }


    public static Machine Parse(string s, IFormatProvider? provider)
    {
        var match = MachineRegex().Match(s);

        if (!match.Success)
            throw new ArgumentOutOfRangeException(nameof(s));

        var numberOfLights = match.Groups["lights"].Length;
        var lights = new BitArray(numberOfLights);
        for (var i = 0; i < numberOfLights; i++)
            lights[i] = match.Groups["lights"].Value[i] == '#';

        var buttonParts = match.Groups["buttons"].Value.Split(' ');

        return new Machine
        {
            GoalState = lights,
            Buttons = [.. buttonParts.Select(s => Button.Parse(s, numberOfLights))],
            JoltageGoal = [.. match.Groups["joltages"].Value.Split(',').Select(int.Parse)]
        };
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Machine result) => throw new NotImplementedException();

    [GeneratedRegex(@"\[(?<lights>[.#]*)\] (?<buttons>(?:\(\d+(?:,\d+)*\) ?)*) {(?<joltages>\d+(?:,\d+)*)}")]
    private static partial Regex MachineRegex();
}

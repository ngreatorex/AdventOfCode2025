
using Microsoft.Z3;
using System.Diagnostics;
using Log = Serilog.Log;

namespace AdventOfCode2025.Day12;

public record Problem
{
    public int Width { get; init; }
    public int Height { get; init; }

    public required List<int> ShapeCounts { get; init; }

    public bool? Solvable { get; init; }

    public Problem Solve(List<Polyomino> shapes)
    {
        Debug.Assert(shapes.Count == ShapeCounts.Count);
        Debug.Assert(shapes.All(s => s.Number == shapes.IndexOf(s)));

        if (checked(Width * Height) < shapes.Sum(s => s.Coords.Count * ShapeCounts[s.Number]))
            return this with { Solvable = false };
        if (checked(Width * Height) >= shapes.Sum(s => s.Area * ShapeCounts[s.Number]))
            return this with { Solvable = true };

        Log.Debug("This problem doesn't have a trivial solution? Uh oh!");
        if (Width * Height > 64)
            throw new InvalidOperationException("Can't solve such a large problem");

        return RealSolver(shapes);
    }

    public Problem RealSolver(List<Polyomino> shapes)
    {

        var output = new List<string>();

        var shapeData = shapes
            .ToDictionary(s => s.Number, s =>
            {
                var configs = s.GeneratePossiblePlacements(Height, Width).ToList();
                return Enumerable.Range(0, ShapeCounts[s.Number])
                    .ToDictionary(n => n, n => configs.ToList());
            });

        var nameIndex = shapeData.SelectMany(a => a.Value.SelectMany(b => b.Value.Select((c, i) => (name: $"p_{a.Key}_{b.Key}_{i}", shape: c))))
            .ToDictionary(d => d.name, d => d.shape);

        foreach (var (shapeIndex, instances) in shapeData)
        {
            foreach (var (instanceNumber, configs) in instances)
            {
                for (var configIndex = 0; configIndex < configs.Count; configIndex++)
                {
                    var name = $"p_{shapeIndex}_{instanceNumber}_{configIndex}";

                    output.Add(GenerateLine("declare-const", name, "Bool"));
                }
            }
        }

        foreach (var (shapeIndex, instances) in shapeData)
        {
            foreach (var (instanceNumber, configs) in instances)
            {
                var cs = configs.Select((c, i) => $"p_{shapeIndex}_{instanceNumber}_{i}");

                output.Add(Assert(GenerateLine([GenerateLine("_", "at-most", "1"), .. cs])));
                output.Add(Assert(GenerateLine(["or", .. cs])));
            }
        }

        foreach (var (shapeIndex, instances) in shapeData)
        {
            foreach (var (instanceNumber, configs) in instances)
            {
                for (var configIndex = 0; configIndex < configs.Count; configIndex++)
                {
                    var config = configs[configIndex];
                    var disallowedOverlaps = new List<string>();

                    foreach (var (otherShapeIndex, otherInstances) in shapeData)
                    {
                        foreach (var (otherInstanceNumber, otherConfigs) in otherInstances)
                        {
                            if (shapeIndex == otherShapeIndex && instanceNumber == otherInstanceNumber)
                                continue;

                            for (var otherConfigIndex = 0; otherConfigIndex < otherConfigs.Count; otherConfigIndex++)
                            {
                                var otherConfig = otherConfigs[otherConfigIndex];
                                if (otherConfig.Overlaps(config))
                                {
                                    disallowedOverlaps.Add($"p_{otherShapeIndex}_{otherInstanceNumber}_{otherConfigIndex}");
                                }
                            }
                        }
                    }

                    if (disallowedOverlaps.Count > 0)
                    {
                        output.Add(Assert(GenerateLine("=>", $"p_{shapeIndex}_{instanceNumber}_{configIndex}", GenerateLine("not", GenerateLine(["or", .. disallowedOverlaps])))));
                    }
                }
            }
        }

        output.Add(GenerateLine("check-sat-using", GenerateLine("then", "aig", "nnf", "sat")));

        var prog = string.Join("\n", output);
        Log.Verbose("Program: \n{Program}", prog);
        File.WriteAllLines($"executed_program_{Width}x{Height}_{ShapeCounts.Sum()}.smt2", output);

        using var ctx = new Context(new Dictionary<string, string>() {
            { "model", "true" }
        });
        var aig = ctx.MkTactic("aig");
        var nnf = ctx.MkTactic("nnf");
        var sat = ctx.MkTactic("sat");
        var tactic = ctx.Then(aig, nnf, sat);
        var solver = ctx.MkSolver(tactic);

        solver.Add(ctx.ParseSMTLIB2String(prog));

        Log.Debug("Checking for solution...");

        var status = solver.Check();

        if (status != Status.SATISFIABLE)
        {
            Log.Debug("... no solution found");
            return this with
            {
                Solvable = false
            };
        }

        var solutionShapeNames = solver.Model.Consts.Where(c => c.Value.IsTrue).Select(c => c.Key.Name.ToString());
        var solutionShapes = solutionShapeNames.Select(n => nameIndex[n]);

        Log.Debug("... found a solution:");
        foreach (var str in solutionShapes.ToStrings())
            Log.Debug(str);

        return this with
        {
            Solvable = true
        };
    }

    private static string Assert(string e) => GenerateLine("assert", e);
    private static string GenerateLine(params string[] input) => GenerateLineEnumerable(input);
    private static string GenerateLineEnumerable(IEnumerable<string> input) => $"( {string.Join(' ', input)} )";

    internal void LogState(bool isInitial)
    {
        Log.Information("{Width}x{Height}: {ShapeCounts}. Is solvable? {Solvable}",
            Width, Height, string.Join(' ', ShapeCounts), Solvable == null ? "unknown" : Solvable.Value ? "true" : "false");
    }

    public override string ToString() => $"{Width}x{Height}: {string.Join(' ', ShapeCounts)}";
}

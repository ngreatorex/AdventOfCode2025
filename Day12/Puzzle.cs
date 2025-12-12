using AdventOfCode2025.Shared;
using Serilog;
using Serilog.Context;
using System.Collections.Concurrent;

namespace AdventOfCode2025.Day12;

public class Puzzle : Puzzle<Puzzle, Instruction>
{
    public List<Polyomino> Shapes { get; init; } = [];
    public List<Problem> Problems { get; private set; } = [];

    public override void LogState(bool isInitial)
    {
        Log.Information("Shapes:");
        foreach (var presentShape in Shapes)
        {
            presentShape.LogState(isInitial);
        }

        Log.Information("Problems:");
        foreach (var problem in Problems)
        {
            problem.LogState(isInitial);
        }
    }

    public override async Task Run()
    {
        await base.Run();

        var solutions = new Problem[Problems.Count];
        var started = 0;
        var completed = 0;

        var queue = new ConcurrentQueue<(Problem p, int i)>();
        for (var i = 0; i < Problems.Count; i++)
        {
            queue.Enqueue((Problems[i], i));
        }

        var threads = new Thread[8];
        var waitEvents = new ManualResetEvent[threads.Length];

        for (var i = 0; i < threads.Length; i++)
        {
            waitEvents[i] = new ManualResetEvent(false);
            threads[i] = new Thread(o =>
            {
                if (o is null)
                    throw new InvalidOperationException();

                int threadNum = (int)o;

                while (queue.TryDequeue(out var tuple))
                {
                    using var _ = LogContext.PushProperty("Problem", tuple.i);
                    Interlocked.Increment(ref started);
                    solutions[tuple.i] = tuple.p.Solve(Shapes);
                    Interlocked.Increment(ref completed);
                }

                waitEvents[threadNum].Set();
            });
            threads[i].Start(i);
        }

        var total = Problems.Count;

        while (!WaitHandle.WaitAll(waitEvents, TimeSpan.FromSeconds(10)))
        {
            Log.Information("{Count} / {Total} tasks completed. {Started} started.", completed, total, started);
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
        foreach (var waitEvent in waitEvents)
        {
            waitEvent.Dispose();
        }

        Log.Information("All problems complete! {Count} are possible", solutions.Count(s => s.Solvable.HasValue && s.Solvable.Value));

        Problems = [.. solutions];
    }

    protected override IEnumerable<string> Split(string input, bool isPartTwo)
    {
        return input.Split($"{Environment.NewLine}{Environment.NewLine}");
    }

    protected override Task ProcessInstruction(Instruction instruction)
    {
        if (instruction is Polyomino p)
            Shapes.Add(p);
        else if (instruction is ProblemList r)
            Problems.AddRange(r.Grids);
        else
            throw new NotImplementedException();


        return Task.CompletedTask;
    }
}
using System;
using System.Diagnostics;

public class CodeTimer(string name) : IDisposable
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly string _name = name;

    public void Dispose()
    {
        _stopwatch.Stop();
        Console.WriteLine($"{_name} took {_stopwatch.Elapsed.TotalSeconds:F2} seconds");
    }
}

using Seekatar.Tests;

namespace ObjectFactoryTestWorkers;

[Worker(Name = "times")]
public class TestMultiplier : ITestWorker
{
    public int RunWorker(int a, int b) => a * b;
}

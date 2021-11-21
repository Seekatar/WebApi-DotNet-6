using Seekatar.Tests;

namespace ObjectFactoryTestWorkers;

public class TestMultiplier : ITestWorker
{
    public int RunWorker(int a, int b) => a * b;
}

using System.Threading;

namespace Seekatar.Tests;

[Worker(Name = "add")]
class TestAdder : ITestWorker
{
    public int RunWorker(int a, int b) => a + b;
}

class TestSubtracter : ITestWorker
{
    public int RunWorker(int a, int b) => a - b;
}
class TestSummer : ITestWorker
{
    private int _sum;

    public int RunWorker(int a, int b) => Interlocked.Add(ref _sum, a + b);
}

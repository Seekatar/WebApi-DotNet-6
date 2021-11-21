using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Seekatar.Tools;
using Shouldly;
using System.Threading;

namespace Seekatar.Tests;
class TestAdder : ITestWorker
{
    public int RunWorker(int a, int b) => a + b;
}

class TestSubtracter : ITestWorker
{
    public int RunWorker(int a, int b) => a - b;
}
class TestSummer: ITestWorker
{
    private int _sum;

    public int RunWorker(int a, int b) => Interlocked.Add(ref _sum, a + b);
}

public class ObjectFactoryTests
{
    private ServiceProvider? _provider;
    private ObjectFactory<ITestWorker>? _factory;

    [SetUp]
    public void Setup()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<ObjectFactory<ITestWorker>>();
        serviceCollection.AddSingleton<ITestWorker,TestSummer>();

        serviceCollection.AddOptions<ObjectFactoryOptions>().Configure(options => {
            options.AssemblyNameMask = "O*";
        });

        _provider = serviceCollection.BuildServiceProvider();
        _provider.ShouldNotBeNull();

        _factory = _provider!.GetService<ObjectFactory<ITestWorker>>();
        _factory.ShouldNotBeNull();
    }

    [Test]
    public void TestAdd()
    {
        var worker = _factory!.GetInstance(typeof(TestAdder).Name);
        worker.ShouldNotBeNull();
        worker.RunWorker(1, 4).ShouldBe(5);
    }
    [Test]
    public void TestSubtract()
    {
        var worker = _factory!.GetInstance(typeof(TestSubtracter).Name);
        worker.ShouldNotBeNull();
        worker.RunWorker(10, 1).ShouldBe(9);
    }
    [Test]
    public void TestSum()
    {
        var worker = _factory!.GetInstance(typeof(TestSummer).Name);
        worker.ShouldNotBeNull();
        worker.RunWorker(1, 1).ShouldBe(2);
        worker.RunWorker(1, 1).ShouldBe(4);
    }
    [Test]
    public void TestMultiplierFromNuGet()
    {
        // TestMultiplier not reference here to avoid it gettting loaded automatically
        // that way this tests the ObjectFactory.LoadAssemblies method
        var worker = _factory!.GetInstance("TestMultiplier");
        worker.ShouldNotBeNull();
        worker.RunWorker(5, 6).ShouldBe(30);
    }
    [Test]
    public void TestMissing()
    {
        var worker = _factory!.GetInstance("NotFound");
        worker.ShouldBeNull();
    }
}

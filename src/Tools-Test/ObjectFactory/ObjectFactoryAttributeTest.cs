using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Seekatar.Tools;
using Shouldly;
using System;
using System.Linq;
using System.Threading;

namespace Seekatar.Tests;

class WorkerAttributeFactory : ObjectFactory<ITestWorker>
{
    public WorkerAttributeFactory(IServiceProvider provider, IOptions<ObjectFactoryOptions> options) : base(provider, options)
    { }

    protected override bool Predicate(Type type)
    {
        if (base.Predicate(type))
        {
            bool ret = type.GetCustomAttributes(typeof(WorkerAttribute), false).Any();
            return ret;
        }
        else
        {
            return false;
        }
    }

    protected override string ObjectName(Type type) => (type.GetCustomAttributes(typeof(WorkerAttribute), false).FirstOrDefault() as WorkerAttribute)!.Name;
}

public class ObjectFactoryAttributeTest
{
    private ServiceProvider? _provider;
    private ObjectFactory<ITestWorker>? _factory;

    [SetUp]
    public void Setup()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<ObjectFactory<ITestWorker>, WorkerAttributeFactory>();
        serviceCollection.AddSingleton<ITestWorker, TestSummer>();

        serviceCollection.AddOptions<ObjectFactoryOptions>().Configure(options =>
        {
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
        var worker = _factory!.GetInstance("add");
        worker.ShouldNotBeNull();
        worker.RunWorker(1, 4).ShouldBe(5);
    }
    [Test]
    public void TestSubtract()
    {
        var worker = _factory!.GetInstance(typeof(TestSubtracter).Name);
        worker.ShouldBeNull();
    }
    [Test]
    public void TestMultiplierFromNuGet()
    {
        // TestMultiplier not reference here to avoid it gettting loaded automatically
        // that way this tests the ObjectFactory.LoadAssemblies method
        var worker = _factory!.GetInstance("times");
        worker.ShouldNotBeNull();
        worker.RunWorker(5, 6).ShouldBe(30);
    }
}

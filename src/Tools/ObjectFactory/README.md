# ObjectFactory

This tool will create objects by a name of a given `Type`. Registered as a singleton, it will scan loaded assemblies for `Types`, and track ones that you specify. Then you will call `GetInstance` to have the factory return an instance of the class.

This uses the .NET [IServiceProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iserviceprovider?view=net-6.0) to create the instances so they support injecting whatever they need.

> If you can use dependency injection to get your objects, or an IEnumerable&lt;baseClass&gt; that is preferred to a factory. Since this is a Service Resolver (anti)-pattern

## Default Configuration

To use the default configuration, simply register it as a singleton with the base type or interface you will want to create. (All of these examples are from the unit tests.)

```csharp
serviceCollection.AddSingleton<ObjectFactory<ITestWorker>>();
```

When created, the factory will scan all loaded assemblies for classes implementing `ITestWorker`, and then you can inject the factory into your classes and use it to get an instance using the class name.

```csharp
_factory.GetInstance(typeof(TestAdder).Name);
```

## Loading Assemblies

If you have a nuget package that has implementations in it, it may loaded at startup if nothing references it. The factory can optionally load them so it can register them. For instance if the assembly names matched `ObjectFactory*` you would do this. (The unit test `ObjectFactoryTestWorkers` project demonstrates this.)

```csharp
serviceCollection.AddOptions<ObjectFactoryOptions>().Configure(options => {
    options.AssemblyNameMask = "ObjectFactory*";
    options.AssemblyNameRegEx = "(ObjectFactory.*|Tools-Test)";
});
```

The `AssemblyNameMask` is a file mask for loading assemblies from the `bin` folder. If left empty, it will not load any additional assemblies.

`AssemblyNameRegEx` is just an optimization to scan for types in only assemblies that match that expression. In the example above it looks in the `ObjectWorker.*` assemblies, and the root assembly of `Tools-Test`. If left empty, the factory will scan _all_ loaded assemblies for types.

## Using a Different Name

As mentioned above, the default configuration checks for derived types of the generic parameter and uses the `Type.Name` when looking up the `Type` to create. You can derive from `ObjectFactory` and change that behavior. In the unit test example below, I override the `Predicate` to only include implementations that have a `WorkerAttribute`. Then I override `ObjectName` to use the `Name` of the attribute as the lookup value (See `WorkerAttributeFactory` in unit tests.)

```csharp
// only include ones that have WorkerAttribute
protected override bool Predicate(Type type) => base.Predicate(type) &&
                type.GetCustomAttributes(typeof(WorkerAttribute), false).Any();

// use WorkerAttribute.Name in GetInstance() instead of Type.Name
protected override string ObjectName(Type type) =>
        (type.GetCustomAttributes(typeof(WorkerAttribute), false)
            .FirstOrDefault() as WorkerAttribute)!.Name;
```

# Seekatar's .NET Tools

This repo contains various .NET tools for .NET 6

## ObjectFactory

Yet another object factory class. This will scan loaded assemblies (and load ones that may not automatically load), and track ones that you specify. Then `GetInstance` can be called to have the factory return an instance of the class.

To use the default factory, simply register it as a singleton with the type you want to create later. (All of these examples are from the unit tests.)

```csharp
serviceCollection.AddSingleton<ObjectFactory<ITestWorker>>();
```

When created, the factory will scan all loaded assemblies for classes implementing `ITestWorker`, and then you can inject the factory into your classes and get an instance using the class name. Full dependency injection is supported in the instances.

```csharp
_factory.GetInstance(typeof(MyClass).Name);
```

If you have a nuget package that has implementations in it, it may loaded at startup if nothing references it. The factory can optionally load them so it can register them. For instance if the assembly names matched `ObjectFactory*` you would do this.

```csharp
serviceCollection.AddOptions<ObjectFactoryOptions>().Configure(options => {
    options.AssemblyNameMask = "ObjectFactory*";
    options.AssemblyNameRegEx = "(ObjectFactory.*|Tools-Test)";
});
```

The `AssemblyNameMask` is a file mask for loading assemblies from the `bin` folder. If left empty, it will not load any additional assemblies.

`AssemblyNameRegEx` is just an optimization to scan for types in only assemblies that match that expression. In the example above it looks in the `ObjectWorker.*` assemblies, and the root assembly of `Tools-Test`. If left empty, the factory will scan _all_ loaded assemblies for types.

As mentioned above, by default the factory checks for types assignable from the generic parameter and uses the `Type.Name` when looking up the `Type`. You can derive from `ObjectFactory` and change that behavior. For example, to only include implementations that have an attribute, and use the name of the attribute as the lookup value, in the derived class override two methods (See `WorkerAttributeFactory` in unit tests.)

```csharp
// only include ones that have WorkerAttribute
protected override bool Predicate(Type type) => base.Predicate(type) &&
                type.GetCustomAttributes(typeof(WorkerAttribute), false).Any();

// use WorkerAttribute.Name in GetInstance() instead of Type.Name
protected override string ObjectName(Type type) =>
        (type.GetCustomAttributes(typeof(WorkerAttribute), false)
            .FirstOrDefault() as WorkerAttribute)!.Name;

```

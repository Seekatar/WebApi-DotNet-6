namespace Seekatar.Tests;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class WorkerAttribute : Attribute
{
    public string Name { get; set; } = "";
}


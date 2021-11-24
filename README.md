# Seekatar's .NET Tools

This repo contains various .NET tools for .NET 6

[ObjectFactory](src/Tools/ObjectFactory/README.md) discovers derived `Types` and allows them to be created later.

## Pulling From GitHub NuGet Repo

Add a new nuget source

```PowerShell
dotnet nuget add source --username $username --password $pat --store-password-in-clear-text --name github "https://nuget.pkg.github.com/seekatar/index.json"
```

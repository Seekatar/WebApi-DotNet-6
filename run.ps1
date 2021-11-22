param (
    [ValidateSet('ObjectFactoryBuild','ObjectFactoryTest')]
    [string[]] $Task
)

function executeSB
{
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [scriptblock] $ScriptBlock,
    [string] $WorkingDirectory
)
    Set-StrictMode -Version Latest

    if ($WorkingDirectory) {
        Push-Location $WorkingDirectory
    }
    try {
        Invoke-Command -ScriptBlock $ScriptBlock

        if ($LASTEXITCODE -ne 0) {
            throw "Error executing command, last exit $LASTEXITCODE"
        }
    } finally {
        if ($WorkingDirectory) {
            Pop-Location
        }
    }
}

foreach ($t in $Task) {

    try {

        $prevPref = $ErrorActionPreference
        $ErrorActionPreference = "Stop"

        switch ($t) {
            'ObjectFactoryBuild' {
                executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/src/Tools') {
                    dotnet build
                    }
            }
            'ObjectFactoryTest' {
                executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/tests/ObjectFactoryTests/ObjectFactoryTestInterface') {
                    dotnet build
                    }
                executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/tests/ObjectFactoryTests/ObjectFactoryTestWorkers') {
                    $localNuget = dotnet nuget list source | Select-String "Local \[Enabled" -Context 0,1
                    if ($localNuget) {
                        dotnet pack -o ($localNuget.Context.PostContext.Trim()) --include-source -p:Version=1.0.1 -p:AssemblyVersion=1.0.1
                    } else {
                        throw "Must have a Local NuGet source for testing. e.g. nuget sources add -name Local -source c:\nupkgs"
                    }
                    }
                executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/tests/ObjectFactoryTests/unit') {
                    dotnet test
                    }
            }
            Default {}
        }

    } finally {
        $ErrorActionPreference = $prevPref
    }
}
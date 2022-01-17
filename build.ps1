param(
    [switch] $Install
)
end {
    if (-not $env:ETG_DATA) {
        throw 'Please set %ETG_DATA% environment variable to your "Enter the Gungeon\EtG_Data" folder.'
    }

    if (-not (Test-Path $env:ETG_DATA -PathType Container)) {
        throw 'Environment variable %ETG_DATA% is not valid.'
    }

    $dotnet = Get-Command dotnet -CommandType Application -ErrorAction Ignore
    if (-not ($dotnet -and $dotnet.Version.Major -ge 6)) {
        throw 'dotnet SDK 6+ is required.'
    }

    & $dotnet publish --configuration release
    $item = Get-Item -LiteralPath $PSScriptRoot\Release -ErrorAction Ignore

    # Sanity check the hell out of the recursive folder deletion
    if ($item) {
        if ($item.Name -ne 'Release' -or -not $item.PSIsContainer) {
            throw 'unexpected'
        }

        $item.Delete($true)
    }

    New-Item $PSScriptRoot\Release -ItemType Directory | Out-Null

    $target = "$PSScriptRoot\src\GunBinds\bin\Debug\net35\publish"
    (Get-Item $target\GunBinds.dll),
    (Get-Item $target\metadata.txt),
    (Get-Item $target\Newtonsoft.Json.dll),
    (Get-Item $target\MonoMod.RuntimeDetour.dll),
    (Get-Item $target\MonoMod.Utils.dll),
    (Get-Item $target\0Harmony.dll) |
        Compress-Archive -DestinationPath $PSScriptRoot\Release\GunBinds.zip

    if ($Install) {
        $modFolder = $env:ETG_DATA | Split-Path | Join-Path -ChildPath 'Mods'
        if (Test-Path -LiteralPath $modFolder\GunBinds.zip) {
            Remove-Item -LiteralPath $modFolder\GunBinds.zip
        }

        if (Test-Path -LiteralPath $modFolder\mods.txt) {
            Remove-Item -LiteralPath $modFolder\mods.txt
        }

        Copy-Item -LiteralPath $PSScriptRoot\Release\GunBinds.zip -Destination $modFolder
    }
}

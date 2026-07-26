$ErrorActionPreference = 'Stop'
$ProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$Source = Join-Path $ProjectRoot 'src\Program.cs'
$Manifest = Join-Path $ProjectRoot 'src\app.manifest'
$Icon = Join-Path $ProjectRoot 'src\app.ico'
$Build = Join-Path $ProjectRoot 'build'
$Compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'

New-Item -ItemType Directory -Force -Path $Build | Out-Null
& $Compiler /nologo /target:winexe /optimize+ /platform:anycpu /win32manifest:$Manifest /win32icon:$Icon `
    /reference:System.dll /reference:System.Core.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll `
    /out:"$Build\JackdawFlagPatcher.exe" $Source
if ($LASTEXITCODE -ne 0) { throw "Compilation failed." }

Copy-Item (Join-Path $ProjectRoot 'vendor\DirectXTex\texconv.exe') $Build -Force
Copy-Item (Join-Path $ProjectRoot 'vendor\DirectXTex\LICENSE.txt') (Join-Path $Build 'DIRECTXTEX-LICENSE.txt') -Force
Copy-Item (Join-Path $ProjectRoot 'README.md') $Build -Force
Copy-Item (Join-Path $ProjectRoot 'CHANGELOG.md') $Build -Force
Copy-Item (Join-Path $ProjectRoot 'LICENSE') (Join-Path $Build 'LICENSE.txt') -Force
Copy-Item (Join-Path $ProjectRoot 'THIRD-PARTY-NOTICES.txt') $Build -Force

Write-Host "Built $Build"

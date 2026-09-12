$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$out = Join-Path $root 'dist'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path $compiler)) { $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe' }
if (-not (Test-Path $compiler)) { throw 'ไม่พบ C# compiler ของ .NET Framework 4.x' }
$framework = Split-Path -Parent $compiler
$wpf = Join-Path $framework 'WPF'
New-Item -ItemType Directory -Path $out -Force | Out-Null
$source = Join-Path $root 'src\SignatureStudio\Program.cs'
$compilerArgs = @('/nologo','/target:winexe','/optimize+',('/out:' + (Join-Path $out 'SignatureStudio.exe')),$source,('/r:' + (Join-Path $wpf 'PresentationCore.dll')),('/r:' + (Join-Path $wpf 'PresentationFramework.dll')),('/r:' + (Join-Path $wpf 'WindowsBase.dll')),('/r:' + (Join-Path $framework 'System.dll')),('/r:' + (Join-Path $framework 'System.Xaml.dll')),('/r:' + (Join-Path $framework 'System.Xml.dll')),('/r:' + (Join-Path $framework 'System.Core.dll')))
& $compiler @compilerArgs
if ($LASTEXITCODE -ne 0) { throw "Build failed with exit code $LASTEXITCODE" }
Copy-Item (Join-Path $root 'README-GUI.md') (Join-Path $out 'README-GUI.md') -Force
Write-Host (Join-Path $out 'SignatureStudio.exe')

[CmdletBinding()]
param(
    [switch]$ResetDatabase
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$apiProject = Join-Path $repoRoot 'api\ReqFlow.Api'
$uiDirectory = Join-Path $repoRoot 'ui'

function Require-Command {
    param([string]$Name)

    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        throw "Required command '$Name' was not found. Install it and run this script again."
    }
}

function Quote-PowerShellLiteral {
    param([string]$Value)

    return "'$($Value.Replace("'", "''"))'"
}

Require-Command 'dotnet'
Require-Command 'npm'

if ($ResetDatabase) {
    Require-Command 'sqlcmd'

    Write-Host 'Resetting and seeding the ReqFlow LocalDB database...' -ForegroundColor Cyan
    & sqlcmd -S '(localdb)\MSSQLLocalDB' -E -b -i (Join-Path $repoRoot 'sql\001_create_reqflow_schema.sql')
    & sqlcmd -S '(localdb)\MSSQLLocalDB' -E -b -i (Join-Path $repoRoot 'sql\002_seed_sample_data.sql')
}

if (-not (Test-Path (Join-Path $uiDirectory 'node_modules'))) {
    Write-Host 'Installing UI packages...' -ForegroundColor Cyan
    & npm install --prefix $uiDirectory
}

$apiPath = Quote-PowerShellLiteral $apiProject
$uiPath = Quote-PowerShellLiteral $uiDirectory
$apiCommand = @"
`$host.UI.RawUI.WindowTitle = 'ReqFlow API'
Set-Location $apiPath
Write-Host 'ReqFlow API starting at http://localhost:5000' -ForegroundColor Cyan
Write-Host 'Press Ctrl+C in this window to stop the API.' -ForegroundColor Yellow
dotnet run --launch-profile http
"@
$uiCommand = @"
`$host.UI.RawUI.WindowTitle = 'ReqFlow UI'
Set-Location $uiPath
Write-Host 'ReqFlow UI starting at http://localhost:5173' -ForegroundColor Cyan
Write-Host 'Press Ctrl+C in this window to stop the UI.' -ForegroundColor Yellow
npm run dev
"@

Start-Process powershell.exe -ArgumentList '-NoExit', '-ExecutionPolicy', 'Bypass', '-Command', $apiCommand
Start-Process powershell.exe -ArgumentList '-NoExit', '-ExecutionPolicy', 'Bypass', '-Command', $uiCommand

Write-Host ''
Write-Host 'ReqFlow is starting in two PowerShell windows.' -ForegroundColor Green
Write-Host 'Open http://localhost:5173 when the UI terminal reports that Vite is ready.'
Write-Host 'To stop the app, press Ctrl+C in both the ReqFlow API and ReqFlow UI windows.'

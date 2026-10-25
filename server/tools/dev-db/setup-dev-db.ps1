# Create the minimal dev IRS-COMMON on a local SQL Server, register a SAP B1 company DB,
# and point .dev-site\Web.config at it. Take a backup before running (see report).
#   powershell -File setup-dev-db.ps1 [-CompanyDb SBODemoAU] [-SqlServer localhost]
# Prints the generated passwords once; they are stored only encrypted in Web.config / APZ_SLSP.
param(
    [string]$CompanyDb = "SBODemoAU",
    [string]$SqlServer = "localhost",
    [string]$DevLogin = "irs_dev",
    [string]$SuperUser = "apzadmin",  # must NOT exist in OUSR (CheckAdministrator rejects that)
    [string]$Domain = "ILD"           # certificate domain shown on the client login screen
)
$ErrorActionPreference = "Stop"
$apiRoot = Resolve-Path (Join-Path $PSScriptRoot "..\..")
$tool = Join-Path $apiRoot "tools\IrsConfigTool\bin\Debug\net462\IrsConfigTool.exe"
$webConfig = Join-Path $apiRoot ".dev-site\Web.config"
if (-not (Test-Path $tool)) { throw "Build tools\IrsConfigTool first" }
if (-not (Test-Path $webConfig)) { throw "Run run-dev.ps1 once so .dev-site\Web.config exists" }

function New-Password {
    $chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789"
    -join (1..14 | ForEach-Object { $chars[(Get-Random -Maximum $chars.Length)] }) + "#7a"
}
function Enc([string]$text) { (& $tool encrypt $text).Trim() }

$dbPassword = New-Password
$superPassword = New-Password
$version = "2.10.776"   # Versions.ApplicationVersion (2.10) + BuildVersion (776) of the recovered server

# sqlcmd picks scripting variables up from the environment; -v cannot carry base64 values ending in '='
$vars = @{
    CompanyDb = $CompanyDb; DevLogin = $DevLogin; DevPassword = $dbPassword; Domain = $Domain
    EncServer = Enc $SqlServer; EncUser = Enc $DevLogin; EncPass = Enc $dbPassword; EncVersion = Enc $version
}
try {
    foreach ($k in $vars.Keys) { Set-Item "env:$k" $vars[$k] }
    & sqlcmd -S $SqlServer -E -C -b -i (Join-Path $PSScriptRoot "setup-irs-common-dev.sql")
    if ($LASTEXITCODE -ne 0) { throw "sqlcmd failed ($LASTEXITCODE)" }
    & sqlcmd -S $SqlServer -E -C -b -i (Join-Path $apiRoot "db-scripts\sqlserver\license\APZ_LICENSEMANAGER_SAVELIC.sql")
    if ($LASTEXITCODE -ne 0) { throw "license procedure setup failed ($LASTEXITCODE)" }
}
finally {
    foreach ($k in $vars.Keys) { Remove-Item "env:$k" -ErrorAction SilentlyContinue }
}

& $tool set-config $webConfig "CommonDbServer=$SqlServer" "CommonDb=IRS-COMMON" "CommonDbUserName=$DevLogin" `
    "CommonDbPassword=$dbPassword" "SuperUser=$SuperUser" "Pass=$superPassword" | Out-Null
if ($LASTEXITCODE -ne 0) { throw "set-config failed" }

Write-Host ""
Write-Host "Dev DB ready. Restart IIS Express (run-dev.ps1) so the API reloads Web.config."
Write-Host "  SQL login : $DevLogin / $dbPassword"
Write-Host "  IRS login : $SuperUser / $superPassword  (database $CompanyDb)"

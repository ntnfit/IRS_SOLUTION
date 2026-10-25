# Run the recovered ApiIRS locally on IIS Express (dev).
#  - builds ApiIRS.sln (skip with -NoBuild)
#  - assembles .dev-site from the deployed site's static files + freshly built Apzon.* DLLs
#  - creates .dev-site\Web.config from deploy\Web.Dev.config on first run (later edits are kept)
#  - starts IIS Express on a fixed port
param(
    [int]$Port = 18011,
    [string]$SiteSource = (Join-Path $PSScriptRoot "site-template"),
    [switch]$NoBuild
)
$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$site = Join-Path $root ".dev-site"
$buildOut = Join-Path $root "src\Apzon.Api\bin\Debug\net462"

if (-not $NoBuild) {
    dotnet build (Join-Path $root "ApiIRS.sln") -c Debug -v q -nologo
    if ($LASTEXITCODE -ne 0) { throw "build failed" }
}

# static site content (Global.asax, Views, third-party bin) from the deployed site; App_Data/Log are not needed for dev
robocopy $SiteSource $site /E /XD App_Data Log /XF Web.config Web_backup.config /NFL /NDL /NJH /NJS /NP | Out-Null
if ($LASTEXITCODE -ge 8) { throw "robocopy failed ($LASTEXITCODE)" }
Copy-Item (Join-Path $buildOut "Apzon.*") (Join-Path $site "bin") -Force

# The recovered deployment has ILD templates but no SBODemoAU folder. Map those assets for dev.
$ildAppData = Join-Path $SiteSource "App_Data\SBO_ILD_GOLIVE"
$devAppData = Join-Path $site "App_Data\SBODemoAU"
if (Test-Path $ildAppData) {
    New-Item -ItemType Directory -Force -Path (Join-Path $devAppData "Report"), (Join-Path $devAppData "Action") | Out-Null
    Copy-Item (Join-Path $ildAppData "Report\*.rpt") (Join-Path $devAppData "Report") -Force -ErrorAction SilentlyContinue
    Copy-Item (Join-Path $ildAppData "Action\*.rpt") (Join-Path $devAppData "Action") -Force -ErrorAction SilentlyContinue
}

# POS report sources were lost with the old server. The recovered client carries
# SQL-backed replacement layouts; publish the same layouts into the dev company.
$posReportAssets = Join-Path $root "..\client\src\Apzon.Forms\Report\SBODemoAU"
if (Test-Path $posReportAssets) {
    New-Item -ItemType Directory -Force -Path (Join-Path $devAppData "Report") | Out-Null
    Copy-Item (Join-Path $posReportAssets "POS Invoice.rpt") (Join-Path $devAppData "Report") -Force -ErrorAction SilentlyContinue
    Copy-Item (Join-Path $posReportAssets "Sales and Taxes Summary.rpt") (Join-Path $devAppData "Report") -Force -ErrorAction SilentlyContinue
}

$webConfig = Join-Path $site "Web.config"
if (-not (Test-Path $webConfig)) {
    Copy-Item (Join-Path $root "deploy\Web.SqlServer.config") $webConfig
}

$busy = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue
if ($busy) { throw "Port $Port is already in use (PID $($busy.OwningProcess)). Stop that process first." }

Write-Host "ApiIRS dev: http://localhost:$Port/  (site: $site)"
& "C:\Program Files\IIS Express\iisexpress.exe" "/path:$site" "/port:$Port" /systray:false

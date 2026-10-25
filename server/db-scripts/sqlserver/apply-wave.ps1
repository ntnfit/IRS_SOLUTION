# Apply one converted wave to a SQL Server company DB: COPY_ONLY backup first, then tables, functions, procedures.
#   powershell -File apply-wave.ps1 -Wave wave-1 [-CompanyDb SBODemoAU] [-SqlServer localhost] [-NoBackup]
# Each file runs on its own (QUOTED_IDENTIFIER ON) so one failure does not stop the wave; failures are listed at the end.
param(
    [Parameter(Mandatory)] [string]$Wave,
    [string]$CompanyDb = "SBODemoAU",
    [string]$SqlServer = "localhost",
    [switch]$NoBackup
)
$ErrorActionPreference = "Stop"
$waveDir = Join-Path $PSScriptRoot $Wave
if (-not (Test-Path $waveDir)) { throw "No folder $waveDir" }

if (-not $NoBackup) {
    $backup = "DECLARE @f nvarchar(500) = CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS nvarchar(400)) + N'\$CompanyDb" + "_before_$Wave" + "_' + FORMAT(GETDATE(),'yyyyMMdd_HHmmss') + N'.bak';" +
              " BACKUP DATABASE [$CompanyDb] TO DISK = @f WITH COPY_ONLY, COMPRESSION, CHECKSUM, INIT; RESTORE VERIFYONLY FROM DISK = @f WITH CHECKSUM; PRINT @f;"
    & sqlcmd -S $SqlServer -E -C -b -Q $backup | Where-Object { $_ -notmatch '^Processed' }
    if ($LASTEXITCODE -ne 0) { throw "backup failed" }
}

# functions first (procedures call them); a file is a function when it creates one
$tables = Get-ChildItem (Join-Path $waveDir "tables") -Filter *.sql -ErrorAction SilentlyContinue
$modules = Get-ChildItem (Join-Path $waveDir "procedures") -Filter *.sql -ErrorAction SilentlyContinue
$functions = $modules | Where-Object { (Get-Content $_.FullName -Raw) -match 'CREATE OR ALTER FUNCTION' }
$procedures = $modules | Where-Object { $functions -notcontains $_ }
# functions that call other functions go last
$functions = $functions | Sort-Object { if ((Get-Content $_.FullName -Raw) -match 'dbo\.apz_fun') { 1 } else { 0 } }

$failed = @()
foreach ($f in @($tables) + @($functions) + @($procedures)) {
    $out = & sqlcmd -S $SqlServer -E -C -b -I -d $CompanyDb -i $f.FullName 2>&1
    if ($LASTEXITCODE -ne 0) { $failed += [pscustomobject]@{ File = $f.Name; Error = ($out -join ' ') } }
}
"applied: $(@($tables).Count) tables, $(@($functions).Count) functions, $(@($procedures).Count) procedures; failed: $($failed.Count)"
$failed | Format-List

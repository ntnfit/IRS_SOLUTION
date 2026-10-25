"""Generate a read-only T-SQL script that checks whether a SQL Server SAP B1 install is ready for the
recovered ApiIRS running with DatabaseSystemType = SQL.

It scans the SQL Server code path of the decompiled sources (everything except HanaRepository/HDBConnection)
for stored procedures called with CommandType.StoredProcedure and APZ_/@ tables used in inline SQL,
then emits a script comparing them with what exists in the company database.

Usage: python generate-sqlserver-readiness-check.py <ApiIRS/src> <out.sql>
"""
import re
import sys
from pathlib import Path

src, out = Path(sys.argv[1]), Path(sys.argv[2])
procs, tables = set(), set()
for p in src.rglob("*.cs"):
    if {"obj", "bin"} & set(p.parts) or "HanaRepository" in str(p) or "HDBConnection" in str(p):
        continue
    text = p.read_text(encoding="utf-8", errors="replace")
    procs |= {m.upper() for m in re.findall(r'"(APZ_[A-Za-z0-9_]+)"\s*,\s*CommandType\.StoredProcedure', text, re.I)}
    tables |= {m.upper() for m in re.findall(r'\b(?:from|join|into|update)\s+\[?"?(APZ_[A-Za-z0-9_]+|@[A-Za-z0-9_]+)', text, re.I)}


def inserts(table, names):
    # SQL Server allows at most 1000 rows per INSERT ... VALUES
    rows = sorted(names)
    return "\n".join(
        f"INSERT INTO {table} VALUES\n" + ",\n".join(f"(N'{n}')" for n in rows[i:i + 900]) + ";"
        for i in range(0, len(rows), 900))


sql = f"""-- ApiIRS (SQL Server mode) readiness check. READ-ONLY: only SELECTs, nothing is created or changed.
-- Generated from the recovered sources: {len(procs)} stored procedures, {len(tables)} APZ_/@ tables.
-- 1) Set the two names below, 2) run the whole script in SSMS.
DECLARE @CommonDb sysname = N'IRS-COMMON';   -- value of CommonDb in Web.config
DECLARE @CompanyDb sysname = N'SBO_ILD_GOLIVE'; -- SAP B1 company database to check

SET NOCOUNT ON;
SELECT 'IRS common DB exists' AS [Check], CASE WHEN DB_ID(@CommonDb) IS NULL THEN 'NO' ELSE 'yes' END AS Result
UNION ALL SELECT 'Company DB exists', CASE WHEN DB_ID(@CompanyDb) IS NULL THEN 'NO' ELSE 'yes' END
UNION ALL SELECT 'SBO-COMMON exists', CASE WHEN DB_ID(N'SBO-COMMON') IS NULL THEN 'NO' ELSE 'yes' END;

DECLARE @sql nvarchar(max);
DECLARE @qCommon nvarchar(300) = QUOTENAME(@CommonDb), @qCompany nvarchar(300) = QUOTENAME(@CompanyDb);

-- company registration in the IRS common DB (credentials columns are not selected)
IF DB_ID(@CommonDb) IS NOT NULL
BEGIN
    SET @sql = N'IF OBJECT_ID(N''' + @qCommon + N'.dbo.APZ_SLSP'') IS NULL
                   SELECT ''APZ_SLSP missing in common DB'' AS Problem
               ELSE
                   SELECT CmpDB, Status FROM ' + @qCommon + N'.dbo.APZ_SLSP';
    EXEC(@sql);
END;

CREATE TABLE #needProc (Name sysname PRIMARY KEY);
CREATE TABLE #needTable (Name sysname PRIMARY KEY);
{inserts("#needProc", procs)}
{inserts("#needTable", tables)}

CREATE TABLE #haveProc (Name sysname);
CREATE TABLE #haveTable (Name sysname);
IF DB_ID(@CompanyDb) IS NOT NULL
BEGIN
    SET @sql = N'SELECT UPPER(name) FROM ' + @qCompany + N'.sys.procedures';
    INSERT INTO #haveProc EXEC(@sql);
    SET @sql = N'SELECT UPPER(name) FROM ' + @qCompany + N'.sys.tables';
    INSERT INTO #haveTable EXEC(@sql);
END;

SELECT 'stored procedures' AS Kind, (SELECT COUNT(*) FROM #needProc) AS Needed,
       (SELECT COUNT(*) FROM #needProc n WHERE EXISTS (SELECT 1 FROM #haveProc h WHERE h.Name = n.Name)) AS Present
UNION ALL
SELECT 'APZ_/@ tables', (SELECT COUNT(*) FROM #needTable),
       (SELECT COUNT(*) FROM #needTable n WHERE EXISTS (SELECT 1 FROM #haveTable h WHERE h.Name = n.Name));

SELECT TOP 50 'missing proc' AS Kind, Name FROM #needProc n WHERE NOT EXISTS (SELECT 1 FROM #haveProc h WHERE h.Name = n.Name)
UNION ALL
SELECT TOP 50 'missing table', Name FROM #needTable n WHERE NOT EXISTS (SELECT 1 FROM #haveTable h WHERE h.Name = n.Name)
ORDER BY 1, 2;

IF DB_ID(@CompanyDb) IS NOT NULL
BEGIN
    SET @sql = N'IF OBJECT_ID(N''' + @qCompany + N'.dbo.APZ_CINF'') IS NULL SELECT ''APZ_CINF missing: IRS not installed in company DB'' AS Problem';
    EXEC(@sql);
END;

DROP TABLE #needProc, #needTable, #haveProc, #haveTable;
"""
out.write_text(sql, encoding="utf-8")
print(f"wrote {out}: {len(procs)} procs, {len(tables)} tables")

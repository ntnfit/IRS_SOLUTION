"""Convert only straightforward HANA SQLScript procedures to T-SQL.

Complex control flow and dynamic SQL are deliberately rejected for manual porting.
Usage: python hana-simple-procedure-to-tsql.py <export-dir> <closure.txt> <out-dir>
"""
import re
import sys
from pathlib import Path

export_dir, closure_path, out_dir = map(Path, sys.argv[1:4])
out_dir.mkdir(parents=True, exist_ok=True)
unsupported = re.compile(
    r"(?is)CREATE\s+LOCAL\s+TEMPORARY|DECLARE\s+\w+\s+TABLE|\bUPSERT\b|EXECUTE\s+IMMEDIATE|"
    r"\bFOR\b.+?\bDO\b|\bWHILE\b|\bMERGE\b|\bIF\b.+?\bTHEN\b|\bSIGNAL\b|"
    r"\bLIMIT\b|\bARRAY\b|SERIES_GENERATE|CE_|ADD_DAYS|TO_DATE|TO_TIMESTAMP"
)


def split_params(text):
    parts, current, depth = [], [], 0
    for char in text:
        depth += char == "("
        depth -= char == ")"
        if char == "," and depth == 0:
            parts.append("".join(current).strip())
            current = []
        else:
            current.append(char)
    if current:
        parts.append("".join(current).strip())
    return parts


def parameter(part):
    match = re.match(r'(?is)(?:IN\s+)?"?([A-Za-z_][A-Za-z0-9_]*)"?\s+([A-Za-z]+(?:\s*\([^)]*\))?)(?:\s+DEFAULT\s+(.+))?$', part)
    if not match:
        raise ValueError(f"unsupported parameter: {part}")
    name, sql_type, default = match.groups()
    sql_type = re.sub(r"(?i)NCLOB", "nvarchar(max)", sql_type)
    sql_type = re.sub(r"(?i)CLOB", "varchar(max)", sql_type)
    return f"@{name} {sql_type}" + (f" = {default}" if default else "")


written, skipped = 0, []
for line in closure_path.read_text(encoding="utf-8").splitlines():
    kind, name, relative = line.split("\t")[:3]
    if kind != "PROCEDURE":
        continue
    source = (export_dir / relative / "create.sql").read_text(encoding="utf-8", errors="replace").strip()
    if unsupported.search(source):
        skipped.append(name)
        continue
    match = re.match(
        r'(?is)^\s*CREATE\s+PROCEDURE\s+"?([A-Za-z_][A-Za-z0-9_]*)"?\s*(?:\((.*?)\))?\s*'
        r'(?:LANGUAGE\s+SQLSCRIPT\s*)?(?:SQL\s+SECURITY\s+\w+\s*)?(?:READS\s+SQL\s+DATA\s*)?AS\s*BEGIN\s*(.*)\s*END\s*;?\s*$',
        source,
    )
    if not match:
        skipped.append(name)
        continue
    proc_name, params, body = match.groups()
    try:
        converted_params = ",\n    ".join(parameter(p) for p in split_params(params or "") if p)
    except ValueError:
        skipped.append(name)
        continue
    body = re.sub(r'"([^"]+)"', r'[\1]', body)
    body = re.sub(r'(?<!@):([A-Za-z_][A-Za-z0-9_]*)', r'@\1', body)
    body = re.sub(r'(?i)\bIFNULL\s*\(', 'ISNULL(', body)
    body = re.sub(r'(?i)\bCURRENT_DATE\b', 'CAST(GETDATE() AS date)', body)
    body = re.sub(r'(?i)\bCURRENT_TIMESTAMP\b', 'SYSDATETIME()', body)
    body = re.sub(r'(?i)\s+FROM\s+DUMMY\b', '', body)
    body = body.replace('||', '+')
    signature = f"\n    {converted_params}\n" if converted_params else "\n"
    output = f"USE [SBODemoAU];\nGO\nCREATE OR ALTER PROCEDURE dbo.[{proc_name}]{signature}AS\nBEGIN\n    SET NOCOUNT ON;\n{body.strip()}\nEND;\nGO\n"
    (out_dir / f"{name}.sql").write_text(output, encoding="utf-8")
    written += 1

print(f"simple procedures written: {written}; manual conversion required: {len(skipped)}")
if skipped:
    print("\n".join(skipped))

"""Convert HANA `CREATE [COLUMN|ROW] TABLE` statements from a catalog export into re-runnable T-SQL.

Usage: python hana-table-to-tsql.py <hana-export dir> <wave-closure.txt> <out dir> [--skip NAME,NAME]
Writes <out dir>/<NAME>.sql per table and prints anything it could not map (review before applying).
SAP standard tables (no APZ_/IRS_/RA_/@ prefix) are skipped: they belong to SAP B1, not to IRS.
"""
import re
import sys
from pathlib import Path

export_dir, closure_file, out_dir = Path(sys.argv[1]), Path(sys.argv[2]), Path(sys.argv[3])
out_dir.mkdir(parents=True, exist_ok=True)
IRS_PREFIX = re.compile(r"^(APZ_|IRS_|RA_|@)", re.I)

# HANA type -> SQL Server type (length/precision kept where present)
TYPE_MAP = {
    "NVARCHAR": "nvarchar", "VARCHAR": "varchar", "NCHAR": "nchar", "CHAR": "char", "ALPHANUM": "nvarchar",
    "SHORTTEXT": "nvarchar", "INTEGER": "int", "INT": "int", "BIGINT": "bigint", "SMALLINT": "smallint",
    "TINYINT": "tinyint", "DECIMAL": "decimal", "SMALLDECIMAL": "decimal(38,10)", "DOUBLE": "float",
    # SAP B1 on SQL Server stores dates as datetime; keep IRS tables consistent with it
    "FLOAT": "float", "REAL": "real", "DATE": "datetime", "TIME": "time", "TIMESTAMP": "datetime",
    "LONGDATE": "datetime", "SECONDDATE": "datetime", "NCLOB": "nvarchar(max)", "CLOB": "varchar(max)", "TEXT": "nvarchar(max)",
    "BLOB": "varbinary(max)", "VARBINARY": "varbinary", "BOOLEAN": "bit",
}
NO_ARGS = {"SMALLDECIMAL", "NCLOB", "CLOB", "TEXT", "BLOB", "BOOLEAN", "DATE", "TIME", "TIMESTAMP", "SECONDDATE", "LONGDATE",
           "INTEGER", "INT", "BIGINT", "SMALLINT", "TINYINT", "DOUBLE", "REAL"}


def split_top_level(s):
    parts, depth, cur, quote = [], 0, "", None
    for ch in s:
        if quote:
            cur += ch
            if ch == quote:
                quote = None
            continue
        if ch in "'\"":
            quote = ch
        elif ch == "(":
            depth += 1
        elif ch == ")":
            depth -= 1
        if ch == "," and depth == 0:
            parts.append(cur.strip())
            cur = ""
        else:
            cur += ch
    if cur.strip():
        parts.append(cur.strip())
    return parts


def convert_column(col, problems, table):
    m = re.match(r'"([^"]+)"\s+([A-Z]+)\s*(\([^)]*\))?(.*)$', col, re.S | re.I)
    if not m:
        problems.append(f"{table}: cannot parse column: {col[:80]}")
        return None
    name, htype, args, rest = m.group(1), m.group(2).upper(), m.group(3) or "", m.group(4)
    if htype not in TYPE_MAP:
        problems.append(f"{table}.{name}: unknown type {htype}")
        return None
    sqltype = TYPE_MAP[htype] + ("" if htype in NO_ARGS or "(" in TYPE_MAP[htype] else args)
    size = re.match(r"\(\s*(\d+)\s*\)", args)
    if size and ((htype in ("NVARCHAR", "NCHAR", "ALPHANUM", "SHORTTEXT") and int(size.group(1)) > 4000)
                 or (htype in ("VARCHAR", "CHAR", "VARBINARY") and int(size.group(1)) > 8000)):
        sqltype = TYPE_MAP[htype] + "(max)"  # SQL Server caps sized (n)varchar at 4000/8000
    if htype in ("NVARCHAR", "VARCHAR", "VARBINARY", "ALPHANUM", "SHORTTEXT") and not args:
        sqltype += "(max)" if htype != "ALPHANUM" else "(127)"
    if htype == "DECIMAL" and not args:
        sqltype = "decimal(38,10)"  # HANA floating DECIMAL
    rest = re.sub(r"\bCS_[A-Z_]+\b|\bMEMORY\s+THRESHOLD\s+\d+", "", rest, flags=re.I)  # column-store storage hints
    rest = re.sub(r"\bGENERATED\s+(BY DEFAULT|ALWAYS)\s+AS\s+IDENTITY(\s*\([^)]*\))?", " IDENTITY(1,1)", rest, flags=re.I)
    default = re.search(r"\bDEFAULT\s+('(?:[^']|'')*'|-?[\d.]+|CURRENT_TIMESTAMP|CURRENT_DATE|NULL)", rest, re.I)
    not_null = re.search(r"\bNOT\s+NULL\b", rest, re.I)
    identity = "IDENTITY(1,1)" in rest
    leftover = re.sub(r"\bDEFAULT\s+('(?:[^']|'')*'|-?[\d.]+|CURRENT_TIMESTAMP|CURRENT_DATE|NULL)|\bNOT\s+NULL\b|\bNULL\b|IDENTITY\(1,1\)", "", rest, flags=re.I).strip()
    if leftover:
        problems.append(f"{table}.{name}: unmapped column option '{leftover}'")
    d = default.group(1) if default else None
    if d and d.upper() == "CURRENT_TIMESTAMP":
        d = "SYSDATETIME()"
    elif d and d.upper() == "CURRENT_DATE":
        d = "CAST(GETDATE() AS date)"
    out = f"[{name}] {sqltype}"
    if identity:
        out += " IDENTITY(1,1)"
    out += " NOT NULL" if not_null else " NULL"
    if d and d.upper() != "NULL":
        out += f" DEFAULT {d}"
    return out


def convert(sql, table, problems):
    m = re.search(r'CREATE\s+(?:COLUMN\s+|ROW\s+)?TABLE\s+"[^"]+"\."([^"]+)"\s*\((.*)\)\s*(?:UNLOAD|AUTO|PARTITION|WITH|$)', sql, re.S | re.I)
    if not m:
        problems.append(f"{table}: cannot parse CREATE TABLE")
        return None
    name, body = m.group(1), m.group(2)
    cols, constraints = [], []
    for part in split_top_level(body):
        pk = re.match(r'(?:CONSTRAINT\s+"[^"]+"\s+)?PRIMARY\s+KEY\s*(?:INVERTED\s+\w+\s*)?\(([^)]*)\)', part, re.I)
        uq = re.match(r'(?:CONSTRAINT\s+"[^"]+"\s+)?UNIQUE\s*(?:INVERTED\s+\w+\s*)?\(([^)]*)\)', part, re.I)
        if pk:
            constraints.append(f"CONSTRAINT [PK_{name}] PRIMARY KEY ({pk.group(1).replace(chr(34), '')})".replace(",", ", "))
        elif uq:
            constraints.append(f"UNIQUE ({uq.group(1).replace(chr(34), '')})")
        else:
            c = convert_column(part, problems, name)
            if c:
                cols.append(c)
    # HANA makes primary-key columns NOT NULL implicitly; SQL Server needs it spelled out
    pk_cols = set()
    for c in constraints:
        if "PRIMARY KEY" in c:
            pk_cols |= {x.strip() for x in c[c.rindex("(") + 1:c.rindex(")")].split(",")}
    cols = [re.sub(r"\bNULL\b", "NOT NULL", c, count=1) if c.split("]")[0].lstrip("[") in pk_cols and " NOT NULL" not in c else c
            for c in cols]
    lines = ",\n    ".join(cols + constraints)
    extra = sql[m.end():]
    for idx in re.finditer(r'CREATE\s+(UNIQUE\s+)?(?:\w+\s+)?INDEX\s+"[^"]+"\."([^"]+)"\s+ON\s+"[^"]+"\."[^"]+"\s*\(([^)]*)\)', extra, re.I):
        lines += ""  # indexes appended below
    script = (f"-- converted from HANA catalog export ({table}); review before production use\n"
              f"IF OBJECT_ID(N'dbo.[{name}]') IS NULL\nCREATE TABLE dbo.[{name}] (\n    {lines}\n);\n")
    for idx in re.finditer(r'CREATE\s+(UNIQUE\s+)?(?:\w+\s+)?INDEX\s+"[^"]+"\."([^"]+)"\s+ON\s+"[^"]+"\."[^"]+"\s*\(([^)]*)\)', sql, re.I):
        uniq, iname, icols = idx.group(1) or "", idx.group(2), idx.group(3).replace('"', "")
        script += (f"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'{iname}' AND object_id = OBJECT_ID(N'dbo.[{name}]'))\n"
                   f"    CREATE {uniq}INDEX [{iname}] ON dbo.[{name}] ({icols});\n")
    return script


problems, written, skipped = [], 0, []
skip = set(sys.argv[sys.argv.index("--skip") + 1].upper().split(",")) if "--skip" in sys.argv else set()
for line in closure_file.read_text(encoding="utf-8").splitlines():
    kind, name, path = line.split("\t")[:3]
    if kind != "TABLE":
        continue
    if not IRS_PREFIX.match(name) or name.upper() in skip:
        skipped.append(name)
        continue
    sql = (export_dir / path / "create.sql").read_text(encoding="utf-8", errors="replace")
    script = convert(sql, name, problems)
    if script:
        (out_dir / f"{name}.sql").write_text(script, encoding="utf-8")
        written += 1
print(f"tables written: {written}; skipped (SAP standard or --skip): {skipped}")
print("\n".join(problems) if problems else "no unmapped syntax")

"""Turn a ';'-separated seed file (first line = column names) into a re-runnable T-SQL script that
replaces the table's rows. Empty fields become NULL for numeric columns and '' for text columns.

Usage: python load-seed.py <seed.txt> <table> <out.sql> [--numeric Col1,Col2]
"""
import sys
from pathlib import Path

seed, table, out = Path(sys.argv[1]), sys.argv[2], Path(sys.argv[3])
numeric = set(sys.argv[sys.argv.index("--numeric") + 1].split(",")) if "--numeric" in sys.argv else set()

lines = [l for l in seed.read_text(encoding="utf-8").splitlines() if l.strip()]
cols = [c.strip() for c in lines[0].split(";")]
rows = []
for n, line in enumerate(lines[1:], start=2):
    vals = [v.strip() for v in line.split(";")]
    if len(vals) != len(cols):
        raise SystemExit(f"{seed}:{n}: expected {len(cols)} fields, got {len(vals)}: {line}")
    rows.append(vals)


def lit(col, v):
    if col in numeric:
        return v if v != "" else "NULL"
    return "N'" + v.replace("'", "''") + "'"


col_list = ", ".join(f"[{c}]" for c in cols)
batches = []
for i in range(0, len(rows), 500):  # T-SQL allows 1000 rows per VALUES; stay well below
    values = ",\n".join("(" + ", ".join(lit(c, v) for c, v in zip(cols, r)) + ")" for r in rows[i:i + 500])
    batches.append(f"INSERT INTO dbo.[{table}] ({col_list}) VALUES\n{values};")

out.write_text(
    f"-- generated from {seed.name} by tools/load-seed.py: replaces all rows of {table}\n"
    "SET NOCOUNT ON;\nSET XACT_ABORT ON;\nBEGIN TRAN;\n"
    f"DELETE FROM dbo.[{table}];\n" + "\n".join(batches) +
    f"\nCOMMIT;\nSELECT COUNT(*) AS rows_in_{table.replace('@', '')} FROM dbo.[{table}];\n",
    encoding="utf-8")
print(f"{out}: {len(rows)} rows")

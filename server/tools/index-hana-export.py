"""Index a HANA catalog export (EXPORT ... AS BINARY/CATALOG) and compare it with what the
SQL Server code path of ApiIRS needs.

Usage: python index-hana-export.py <hana-export/SCHEMA> <check-sqlserver-readiness.sql> [api-log-for-wave]
Writes <hana-export/SCHEMA>/../index.csv and prints coverage numbers.
"""
import csv
import re
import sys
from collections import Counter
from pathlib import Path
from urllib.parse import unquote

schema_dir, readiness = Path(sys.argv[1]), Path(sys.argv[2])
wave_log = Path(sys.argv[3]) if len(sys.argv) > 3 else None

rows = []
for sql in schema_dir.rglob("create.sql"):
    # catalog objects carry create.info; column tables (exported under index/) only have create.sql
    info = sql.with_name("create.info")
    if info.exists():
        kind = info.read_text(encoding="utf-8", errors="replace").splitlines()[0].strip()
    else:
        m = re.search(r"CREATE\s+(?:COLUMN\s+|ROW\s+|GLOBAL\s+TEMPORARY\s+(?:COLUMN\s+)?)?(TABLE|VIEW|PROCEDURE|FUNCTION|SEQUENCE|TYPE)",
                      sql.read_text(encoding="utf-8", errors="replace"), re.I)
        kind = m.group(1).upper() if m else "UNKNOWN"
    rows.append({"name": unquote(sql.parent.name), "kind": kind, "path": str(sql.parent.relative_to(schema_dir.parent))})
rows.sort(key=lambda r: (r["kind"], r["name"].upper()))
with open(schema_dir.parent / "index.csv", "w", newline="", encoding="utf-8") as f:
    w = csv.DictWriter(f, fieldnames=["name", "kind", "path"])
    w.writeheader()
    w.writerows(rows)

have = {r["name"].upper(): r["kind"] for r in rows}
need = set(re.findall(r"\(N'([^']+)'\)", readiness.read_text(encoding="utf-8")))
print("export objects by kind:", dict(Counter(r["kind"] for r in rows)))
print("IRS-like objects (APZ_/IRS_/RA_/@):", sum(1 for n in have if re.match(r"(APZ|IRS|RA)_|@", n)))
print(f"needed by SQL path: {len(need)} | present in export: {len(need & have.keys())} | missing: {len(need - have.keys())}")
missing = sorted(need - have.keys())
(schema_dir.parent / "missing-vs-sqlserver-path.txt").write_text("\n".join(missing), encoding="utf-8")
print("first missing:", missing[:20])
if wave_log:
    log = wave_log.read_text(encoding="utf-8", errors="replace")
    wave = {m.upper().replace("DBO.", "") for m in re.findall(r"(?:stored procedure|object name) '([^']+)'", log)}
    print(f"main-screen wave: {len(wave)} | present in export: {len(wave & have.keys())} | missing: {sorted(wave - have.keys())}")

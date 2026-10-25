"""Resolve the dependency closure of a wave: every exported HANA object referenced by the wave's
procedures/views/functions that does not already exist in the SQL Server company DB.

Usage: python resolve-wave-deps.py <wave.txt> <hana-export dir> <existing-objects.txt> <out.txt>
  wave.txt              lines "KIND<TAB>NAME<TAB>PATH" (from contracts/wave-*.txt)
  existing-objects.txt  upper-case names already in the target DB (one per line)
Writes the closure (wave + deps) in dependency order: TABLE, SEQUENCE, VIEW, FUNCTION, PROCEDURE.
"""
import csv
import re
import sys
from pathlib import Path

wave_file, export_dir, existing_file, out_file = map(Path, sys.argv[1:5])
index = {r["name"].upper(): r for r in csv.DictReader(open(export_dir / "index.csv", encoding="utf-8"))}
existing = {l.strip().upper() for l in existing_file.read_text(encoding="utf-8").splitlines() if l.strip()}
ident = re.compile(r'"?(@?[A-Za-z_][A-Za-z0-9_]*)"?')


def body(name):
    return (export_dir / index[name]["path"] / "create.sql").read_text(encoding="utf-8", errors="replace")


todo = [l.split("\t")[1].upper() for l in wave_file.read_text(encoding="utf-8").splitlines() if l.strip()]
closure, reasons = [], {}
while todo:
    name = todo.pop()
    if name in closure or name not in index:
        continue
    closure.append(name)
    if index[name]["kind"] in ("PROCEDURE", "VIEW", "FUNCTION"):
        for tok in {t.upper() for t in ident.findall(body(name))}:
            if tok in index and tok not in existing and tok not in closure and tok != name:
                reasons.setdefault(tok, name)
                todo.append(tok)

order = {"TABLE": 0, "SEQUENCE": 1, "VIEW": 2, "FUNCTION": 3, "PROCEDURE": 4}
closure.sort(key=lambda n: (order.get(index[n]["kind"], 9), n))
out_file.write_text("\n".join(f'{index[n]["kind"]}\t{index[n]["name"]}\t{index[n]["path"]}\t{reasons.get(n, "wave")}'
                              for n in closure), encoding="utf-8")
kinds = {}
for n in closure:
    kinds[index[n]["kind"]] = kinds.get(index[n]["kind"], 0) + 1
print(f"closure: {len(closure)} objects {kinds}; added as dependencies: {len(reasons)}")

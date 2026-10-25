"""Build a HANA-export wave manifest from APZ object names referenced by C# files.

Usage: python build-module-wave.py <source-dir> <hana-index.csv> <out.txt>
Only names present in the export are emitted. Missing names are printed for review.
"""
import csv
import re
import sys
from pathlib import Path

source_dir, index_path, output_path = map(Path, sys.argv[1:4])
index = {row["name"].upper(): row for row in csv.DictReader(index_path.open(encoding="utf-8"))}
pattern = re.compile(r"(?i)(?<![A-Za-z0-9_])(@?APZ_[A-Za-z0-9_]+)")

names = set()
for source in source_dir.rglob("*.cs"):
    names.update(match.upper() for match in pattern.findall(source.read_text(encoding="utf-8", errors="replace")))

found = [index[name] for name in sorted(names) if name in index]
missing = sorted(name for name in names if name not in index)
output_path.parent.mkdir(parents=True, exist_ok=True)
output_path.write_text(
    "\n".join(f'{row["kind"]}\t{row["name"]}\t{row["path"]}' for row in found) + "\n",
    encoding="utf-8",
)
print(f"referenced: {len(names)}; in export: {len(found)}; missing from export: {len(missing)}")
if missing:
    print("\n".join(missing))

"""Show how the SQL Server code path calls given stored procedures: repository method, SqlParameter
names and the columns read from the result. Used to make converted T-SQL match the C# callers.

Usage: python extract-call-contracts.py <src dir> <wave-closure.txt> <out.json>
"""
import json
import re
import sys
from pathlib import Path

src, closure_file, out = Path(sys.argv[1]), Path(sys.argv[2]), Path(sys.argv[3])
procs = [l.split("\t")[1] for l in closure_file.read_text(encoding="utf-8").splitlines() if l.split("\t")[0] == "PROCEDURE"]
files = [p for p in src.rglob("*.cs") if not {"obj", "bin"} & set(p.parts) and "HanaRepository" not in str(p)]
texts = {p: p.read_text(encoding="utf-8", errors="replace") for p in files}

result = {}
for proc in procs:
    calls = []
    pat = re.compile(r'"' + re.escape(proc) + r'"', re.I)
    for p, t in texts.items():
        for m in pat.finditer(t):
            start = t.rfind("\n\tpublic ", 0, m.start())
            end = t.find("\n\t}", m.end())
            method = t[start:end]
            sig = method.strip().splitlines()[0].strip() if method.strip() else ""
            params = sorted(set(re.findall(r'SqlParameter\(\s*"(@?\w+)"', method) + re.findall(r'AddWithValue\(\s*"(@?\w+)"', method)))
            cols = sorted(set(re.findall(r'\[\s*"(\w+)"\s*\]', method)))
            calls.append({"file": str(p.relative_to(src)), "method": sig, "parameters": params, "columnsRead": cols,
                          "commandType": "StoredProcedure" if "CommandType.StoredProcedure" in t[m.start():m.start() + 300] else "?"})
    result[proc] = calls
out.write_text(json.dumps(result, indent=1), encoding="utf-8")
for proc, calls in result.items():
    print(proc, "|", "; ".join(f'{c["parameters"]} {c["commandType"]}' for c in calls) or "NO C# CALLER")

import json, glob
from pathlib import Path
from graphify.cache import save_semantic_cache

ROOT = Path(r"C:\Users\jorj\Projects\MinimalShop")
SPEC = Path(r"C:\Users\jorj\.config\opencode\skills\graphify\references\extraction-spec.md")

chunks = sorted(glob.glob("graphify-out/.graphify_chunk_*.json"))
all_nodes, all_edges, all_hyperedges = [], [], []
total_in, total_out = 0, 0
for c in chunks:
    d = json.loads(Path(c).read_text(encoding="utf-8"))
    all_nodes += d.get("nodes", [])
    all_edges += d.get("edges", [])
    all_hyperedges += d.get("hyperedges", [])
    total_in += d.get("input_tokens", 0)
    total_out += d.get("output_tokens", 0)
    # normalize: some chunks wrote hyperedges with 'members' instead of 'nodes'
    for h in all_hyperedges:
        if "members" in h and "nodes" not in h:
            h["nodes"] = h["members"]

Path("graphify-out/.graphify_semantic_new.json").write_text(json.dumps({
    "nodes": all_nodes, "edges": all_edges, "hyperedges": all_hyperedges,
    "input_tokens": total_in, "output_tokens": total_out,
}, indent=2, ensure_ascii=False), encoding="utf-8")
print(f"Chunks: {len(chunks)} -> {len(all_nodes)} nodes, {len(all_edges)} edges")

# B3.5 coverage check
covered = {}
for c in chunks:
    d = json.loads(Path(c).read_text(encoding="utf-8"))
    for src, n in (d.get("coverage") or {}).items():
        covered[src] = max(covered.get(src, 0), int(n))
partial = []
for src, n in sorted(covered.items()):
    p = Path(src)
    if not p.is_file():
        continue
    if src.lower().endswith(('.jpg', '.png')):
        total = 1
    else:
        total = sum(1 for _ in p.open(encoding="utf-8", errors="replace"))
    if total and n < total:
        partial.append((src, n, total))
Path("graphify-out/.graphify_partial.txt").write_text("\n".join(s for s, _, _ in partial), encoding="utf-8")
if partial:
    print(f"WARNING: {len(partial)} file(s) partly read:")
    for src, n, total in partial:
        print(f"  {src}: {n} of {total}")
else:
    print("Coverage OK")

# save cache for new results
new = json.loads(Path("graphify-out/.graphify_semantic_new.json").read_text(encoding="utf-8"))
uncached = [l for l in Path("graphify-out/.graphify_uncached.txt").read_text(encoding="utf-8").splitlines() if l]
saved = save_semantic_cache(new.get("nodes", []), new.get("edges", []), new.get("hyperedges", []), root=str(ROOT), allowed_source_files=uncached, prompt_file=str(SPEC), partial_source_files=partial)
print(f"Cached {saved} files")

# merge cached + new
cached = json.loads(Path("graphify-out/.graphify_cached.json").read_text(encoding="utf-8")) if Path("graphify-out/.graphify_cached.json").exists() else {"nodes": [], "edges": [], "hyperedges": []}
all_n = cached.get("nodes", []) + new.get("nodes", [])
all_e = cached.get("edges", []) + new.get("edges", [])
all_h = (cached.get("hyperedges") or []) + (new.get("hyperedges") or [])
seen, deduped = set(), []
for n in all_n:
    if n["id"] not in seen:
        seen.add(n["id"])
        deduped.append(n)
merged = {"nodes": deduped, "edges": all_e, "hyperedges": all_h, "input_tokens": new.get("input_tokens", 0), "output_tokens": new.get("output_tokens", 0)}
Path("graphify-out/.graphify_semantic.json").write_text(json.dumps(merged, indent=2, ensure_ascii=False), encoding="utf-8")
print(f"Semantic merge: {len(deduped)} nodes, {len(all_e)} edges ({len(cached.get('nodes', []))} from cache, {len(new.get('nodes', []))} new)")

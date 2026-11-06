import json
from pathlib import Path
from graphify.build import build_from_json
from graphify.cluster import cluster, score_all
from graphify.analyze import god_nodes, surprising_connections, suggest_questions
from graphify.report import generate
from graphify.export import to_json
from graphify.diagnostics import diagnose_extraction, format_diagnostic_report

ROOT_STR = r"C:\Users\jorj\Projects\MinimalShop"
ROOT = Path(ROOT_STR)

ast = json.loads(Path("graphify-out/.graphify_ast.json").read_text(encoding="utf-8"))
sem = json.loads(Path("graphify-out/.graphify_semantic.json").read_text(encoding="utf-8"))

for n in sem["nodes"]:
    n.setdefault("_origin", "semantic")
for e in sem["edges"]:
    e.setdefault("_origin", "semantic")

seen = {n["id"] for n in ast["nodes"]}
merged_nodes = list(ast["nodes"])
for n in sem["nodes"]:
    if n["id"] not in seen:
        merged_nodes.append(n)
        seen.add(n["id"])
merged_edges = ast["edges"] + sem["edges"]
merged = {"nodes": merged_nodes, "edges": merged_edges, "hyperedges": sem.get("hyperedges", []),
          "input_tokens": sem.get("input_tokens", 0), "output_tokens": sem.get("output_tokens", 0)}
Path("graphify-out/.graphify_extract.json").write_text(json.dumps(merged, indent=2, ensure_ascii=False), encoding="utf-8")
print(f'Merged: {len(merged_nodes)} nodes, {len(merged_edges)} edges ({len(ast["nodes"])} AST + {len(sem["nodes"])} semantic)')

G = build_from_json(merged, root=ROOT_STR, directed=False)
if G.number_of_nodes() == 0:
    print("ERROR: Graph is empty")
    raise SystemExit(1)

# diagnostics
detection = json.loads(Path("graphify-out/.graphify_detect.json").read_text(encoding="utf-8"))
summary = diagnose_extraction(merged, directed=False, root=ROOT_STR)
print(format_diagnostic_report(summary))

communities = cluster(G)
cohesion = score_all(G, communities)
tokens = {"input": merged.get("input_tokens", 0), "output": merged.get("output_tokens", 0)}
gods = god_nodes(G)
surprises = surprising_connections(G, communities)
labels = {cid: "Community " + str(cid) for cid in communities}
questions = suggest_questions(G, communities, labels)

wrote = to_json(G, communities, "graphify-out/graph.json", force=False)
if not wrote:
    print("ERROR: refused to shrink graph.json")
    raise SystemExit(1)
report = generate(G, communities, cohesion, labels, gods, surprises, detection, tokens, ROOT_STR, suggested_questions=questions)
Path("graphify-out/GRAPH_REPORT.md").write_text(report, encoding="utf-8")
analysis = {"communities": {str(k): v for k, v in communities.items()},
            "cohesion": {str(k): v for k, v in cohesion.items()},
            "gods": gods, "surprises": surprises, "questions": questions}
Path("graphify-out/.graphify_analysis.json").write_text(json.dumps(analysis, indent=2, ensure_ascii=False), encoding="utf-8")
print(f"Graph: {G.number_of_nodes()} nodes, {G.number_of_edges()} edges, {len(communities)} communities")

import json
from pathlib import Path

ROOT = Path(r"C:\Users\jorj\Projects\MinimalShop")
chunk4 = json.loads(Path("graphify-out/.graphify_chunk_04.json").read_text(encoding="utf-8")) if Path("graphify-out/.graphify_chunk_04.json").exists() else None

nodes = [
    {"id": "wwwroot_images_products_spark_65w_charger_product_photo", "label": "Product photo: white wall charger with European two-pin plug (spark-65w-charger.jpg)", "file_type": "image", "source_file": "C:\\Users\\jorj\\Projects\\MinimalShop\\wwwroot\\images\\products\\spark-65w-charger.jpg", "source_location": None, "source_url": None, "captured_at": None, "author": None, "contributor": None},
    {"id": "concept_spark_65w_charger_product", "label": "Spark 65W Charger product", "file_type": "concept", "source_file": "C:\\Users\\jorj\\Projects\\MinimalShop\\wwwroot\\images\\products\\spark-65w-charger.jpg", "source_location": None, "source_url": None, "captured_at": None, "author": None, "contributor": None},
    {"id": "concept_wall_power_adapter", "label": "Wall-mount USB power adapter", "file_type": "concept", "source_file": "C:\\Users\\jorj\\Projects\\MinimalShop\\wwwroot\\images\\products\\spark-65w-charger.jpg", "source_location": None, "source_url": None, "captured_at": None, "author": None, "contributor": None},
    {"id": "concept_european_two_pin_plug", "label": "European (Europlug) two-pin wall plug", "file_type": "concept", "source_file": "C:\\Users\\jorj\\Projects\\MinimalShop\\wwwroot\\images\\products\\spark-65w-charger.jpg", "source_location": None, "source_url": None, "captured_at": None, "author": None, "contributor": None},
    {"id": "concept_ecommerce_product_catalog_imagery", "label": "E-commerce product catalog imagery", "file_type": "concept", "source_file": "C:\\Users\\jorj\\Projects\\MinimalShop\\wwwroot\\images\\products\\spark-65w-charger.jpg", "source_location": None, "source_url": None, "captured_at": None, "author": None, "contributor": None},
]
edges = [
    {"source": "wwwroot_images_products_spark_65w_charger_product_photo", "target": "concept_spark_65w_charger_product", "relation": "references", "confidence": "EXTRACTED", "confidence_score": 1.0, "source_file": "C:\\Users\\jorj\\Projects\\MinimalShop\\wwwroot\\images\\products\\spark-65w-charger.jpg", "source_location": None, "weight": 1.0},
    {"source": "wwwroot_images_products_spark_65w_charger_product_photo", "target": "concept_wall_power_adapter", "relation": "references", "confidence": "EXTRACTED", "confidence_score": 1.0, "source_file": "C:\\Users\\jorj\\Projects\\MinimalShop\\wwwroot\\images\\products\\spark-65w-charger.jpg", "source_location": None, "weight": 1.0},
    {"source": "wwwroot_images_products_spark_65w_charger_product_photo", "target": "concept_european_two_pin_plug", "relation": "references", "confidence": "EXTRACTED", "confidence_score": 1.0, "source_file": "C:\\Users\\jorj\\Projects\\MinimalShop\\wwwroot\\images\\products\\spark-65w-charger.jpg", "source_location": None, "weight": 1.0},
    {"source": "concept_spark_65w_charger_product", "target": "concept_wall_power_adapter", "relation": "conceptually_related_to", "confidence": "INFERRED", "confidence_score": 0.95, "source_file": "C:\\Users\\jorj\\Projects\\MinimalShop\\wwwroot\\images\\products\\spark-65w-charger.jpg", "source_location": None, "weight": 1.0},
    {"source": "concept_wall_power_adapter", "target": "concept_european_two_pin_plug", "relation": "conceptually_related_to", "confidence": "INFERRED", "confidence_score": 1.0, "source_file": "C:\\Users\\jorj\\Projects\\MinimalShop\\wwwroot\\images\\products\\spark-65w-charger.jpg", "source_location": None, "weight": 1.0},
    {"source": "wwwroot_images_products_spark_65w_charger_product_photo", "target": "concept_ecommerce_product_catalog_imagery", "relation": "conceptually_related_to", "confidence": "INFERRED", "confidence_score": 1.0, "source_file": "C:\\Users\\jorj\\Projects\\MinimalShop\\wwwroot\\images\\products\\spark-65w-charger.jpg", "source_location": None, "weight": 1.0},
]
hyperedges = [
    {"id": "hyperedge_spark_charger_product_presentation", "label": "65W wall charger product presentation", "nodes": ["wwwroot_images_products_spark_65w_charger_product_photo", "concept_spark_65w_charger_product", "concept_wall_power_adapter", "concept_european_two_pin_plug"], "relation": "participate_in", "confidence": "INFERRED", "confidence_score": 0.85, "source_file": "C:\\Users\\jorj\\Projects\\MinimalShop\\wwwroot\\images\\products\\spark-65w-charger.jpg"},
]

if chunk4 is not None:
    nodes, edges, hyperedges = chunk4.get("nodes", nodes), chunk4.get("edges", edges), chunk4.get("hyperedges", hyperedges)

sem = json.loads(Path("graphify-out/.graphify_semantic.json").read_text(encoding="utf-8"))
existing_ids = {n["id"] for n in sem["nodes"]}
added_n = added_e = 0
for n in nodes:
    if n["id"] not in existing_ids:
        n.setdefault("_origin", "semantic")
        sem["nodes"].append(n)
        existing_ids.add(n["id"])
        added_n += 1
have_e = {(e["source"], e["target"], e["relation"]) for e in sem["edges"]}
for e in edges:
    if (e["source"], e["target"], e["relation"]) not in have_e:
        e.setdefault("_origin", "semantic")
        sem["edges"].append(e)
        added_e += 1
if hyperedges and not sem.get("hyperedges"):
    sem["hyperedges"] = []
have_h = {h["id"] for h in sem.get("hyperedges", [])}
for h in hyperedges:
    if h["id"] not in have_h:
        sem["hyperedges"].append(h)

Path("graphify-out/.graphify_semantic.json").write_text(json.dumps(sem, indent=2, ensure_ascii=False), encoding="utf-8")
print(f"chunk_04 merged: +{added_n} nodes, +{added_e} edges -> total {len(sem['nodes'])} nodes, {len(sem['edges'])} edges")

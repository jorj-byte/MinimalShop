import json
from pathlib import Path
from graphify.detect import detect
from graphify.paths import out_path
from graphify.watch import _read_build_excludes, _read_build_gitignore

result = detect(Path(r"C:\Users\jorj\Projects\MinimalShop"), extra_excludes=_read_build_excludes(out_path()), gitignore=_read_build_gitignore(out_path()))
Path("graphify-out/.graphify_detect.json").write_text(json.dumps(result, ensure_ascii=False), encoding="utf-8")
print(f'Detected {result["total_files"]} files, ~{result.get("total_words", 0)} words')
for cat, files in result["files"].items():
    if files:
        print(f'  {cat}: {len(files)}')
if result.get("skipped_sensitive"):
    print(f'  skipped_sensitive: {len(result["skipped_sensitive"])} -> {result["skipped_sensitive"][:5]}')

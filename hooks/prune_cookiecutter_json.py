#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
from pathlib import Path


def _load_json(path: Path) -> object:
    return json.loads(path.read_text(encoding="utf-8"))


def _write_json(path: Path, data: object) -> None:
    path.write_text(json.dumps(data, indent=4) + "\n", encoding="utf-8")


def load_deprecation_keys(dep_path: Path) -> set[str]:
    """
    Supports:
      - {"deprecatedKey": "anything", "deprecatedKey2": "..."}   (your current format)
      - {"remove": ["deprecatedKey", ...]}
      - ["deprecatedKey", ...]
    """
    raw = _load_json(dep_path)

    if isinstance(raw, dict):
        remove = raw.get("remove")
        if isinstance(remove, list):
            return {str(k).strip() for k in remove if str(k).strip()}
        # your current format: keys are deprecated
        return {str(k).strip() for k in raw.keys() if str(k).strip()}

    if isinstance(raw, list):
        return {str(k).strip() for k in raw if str(k).strip()}

    return set()


def prune_cookiecutter_file(cookie_path: Path, deprecated_keys: set[str]) -> list[str]:
    """
    Removes deprecated keys from:
      - data["cookiecutter"][key]
      - and also top-level data[key] (defensive for older formats)
    Returns the keys actually removed.
    """
    if not cookie_path.exists() or not deprecated_keys:
        return []

    data = _load_json(cookie_path)
    if not isinstance(data, dict):
        return []

    removed: list[str] = []

    cc = data.get("cookiecutter")
    if isinstance(cc, dict):
        for k in sorted(deprecated_keys):
            if k in cc:
                del cc[k]
                removed.append(k)

    # Defensive: remove from top-level as well
    for k in sorted(deprecated_keys):
        if k in data:
            del data[k]
            if k not in removed:
                removed.append(k)

    if removed:
        _write_json(cookie_path, data)

    return removed


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--cookie", default=".cookiecutter.json", help="Path to .cookiecutter.json")
    ap.add_argument("--deprecations", default="templates/deprecations.json", help="Path to deprecations file")
    args = ap.parse_args()

    cookie_path = Path(args.cookie).resolve()
    dep_path = Path(args.deprecations).resolve()

    if not dep_path.exists():
        print(f"ℹ️  No deprecations file found at {dep_path}; skipping.")
        return 0

    try:
        keys = load_deprecation_keys(dep_path)
    except Exception as e:
        print(f"⚠️  Failed to read deprecations from {dep_path}: {e}")
        return 0

    if not keys:
        print(f"ℹ️  Deprecations file found but no keys detected: {dep_path}")
        return 0

    try:
        removed = prune_cookiecutter_file(cookie_path, keys)
    except Exception as e:
        print(f"⚠️  Failed to prune {cookie_path}: {e}")
        return 0

    if removed:
        print(f"🧹 Pruned deprecated cookiecutter keys: {', '.join(removed)}")
    else:
        print("ℹ️  No deprecated keys present in .cookiecutter.json")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())

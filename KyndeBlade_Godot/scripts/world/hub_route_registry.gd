extends RefCounted
class_name HubRouteRegistry
## Loads [`data/world/hub_route_map.json`](../../data/world/hub_route_map.json) for hub fog + edges.

const PATH := "res://data/world/hub_route_map.json"

static var _doc: Dictionary = {}


static func document() -> Dictionary:
	if not _doc.is_empty():
		return _doc
	if not FileAccess.file_exists(PATH):
		push_warning("HubRouteRegistry: missing %s" % PATH)
		return {}
	var f := FileAccess.open(PATH, FileAccess.READ)
	var raw: Variant = JSON.parse_string(f.get_as_text())
	if typeof(raw) != TYPE_DICTIONARY:
		push_warning("HubRouteRegistry: invalid JSON")
		return {}
	_doc = raw
	return _doc


static func nodes() -> Array:
	var d := document()
	var n: Variant = d.get("nodes", [])
	return n if n is Array else []


static func edges() -> Array:
	var d := document()
	var e: Variant = d.get("edges", [])
	return e if e is Array else []


static func fog_config() -> Dictionary:
	var d := document()
	var f: Variant = d.get("fog", {})
	return f if typeof(f) == TYPE_DICTIONARY else {}


static func node_by_id(node_id: String) -> Dictionary:
	var want := node_id.strip_edges()
	for item in nodes():
		if typeof(item) != TYPE_DICTIONARY:
			continue
		if str(item.get("id", "")) == want:
			return item
	return {}


static func neighbor_ids(from_id: String) -> PackedStringArray:
	var out := PackedStringArray()
	var from := from_id.strip_edges()
	for e in edges():
		if typeof(e) != TYPE_DICTIONARY:
			continue
		var a := str(e.get("from", ""))
		var b := str(e.get("to", ""))
		if a == from and not b.is_empty() and not out.has(b):
			out.append(b)
		elif b == from and not a.is_empty() and not out.has(a):
			out.append(a)
	return out

extends RefCounted
class_name PiersSymbolCatalog
## Loads [`data/world/piers_symbols.json`](../../data/world/piers_symbols.json) — symbol hooks for locations and beats (see [`docs/PIERS_SOURCE_VERSIONING.md`](../../docs/PIERS_SOURCE_VERSIONING.md)).

const SYMBOLS_PATH := "res://data/world/piers_symbols.json"

static var _doc_cache: Dictionary = {}
static var _by_id: Dictionary = {}


static func _load_document() -> Dictionary:
	if not _doc_cache.is_empty():
		return _doc_cache
	if not FileAccess.file_exists(SYMBOLS_PATH):
		push_warning("PiersSymbolCatalog: missing %s" % SYMBOLS_PATH)
		return {}
	var f := FileAccess.open(SYMBOLS_PATH, FileAccess.READ)
	var raw: Variant = JSON.parse_string(f.get_as_text())
	if typeof(raw) != TYPE_DICTIONARY:
		push_warning("PiersSymbolCatalog: invalid JSON")
		return {}
	_doc_cache = raw
	_by_id.clear()
	var list: Variant = _doc_cache.get("symbols", [])
	if typeof(list) == TYPE_ARRAY:
		for item in list:
			if typeof(item) != TYPE_DICTIONARY:
				continue
			var id: String = str(item.get("id", "")).strip_edges()
			if not id.is_empty():
				_by_id[id] = item
	return _doc_cache


static func clear_cache() -> void:
	_doc_cache.clear()
	_by_id.clear()


## Returns a copy of the symbol row, or empty if missing.
static func get_symbol(symbol_id: String) -> Dictionary:
	_load_document()
	var id := symbol_id.strip_edges()
	var row: Variant = _by_id.get(id, {})
	return (row.duplicate(true) if typeof(row) == TYPE_DICTIONARY else {})


## Symbol ids listed on the location row in `locations_registry.json`, plus any symbol that lists this location in `related_location_ids`.
static func symbols_for_location(location_id: String) -> PackedStringArray:
	var loc := location_id.strip_edges()
	var out: PackedStringArray = PackedStringArray()
	var seen := {}
	for sid in LocationRegistry.get_piers_symbol_ids(loc):
		if not seen.has(sid):
			seen[sid] = true
			out.append(sid)
	_load_document()
	for sym_id in _by_id:
		var row: Dictionary = _by_id[sym_id]
		var rel: Variant = row.get("related_location_ids", [])
		if typeof(rel) != TYPE_ARRAY:
			continue
		for x in rel:
			if str(x).strip_edges() == loc and not seen.has(sym_id):
				seen[sym_id] = true
				out.append(str(sym_id))
	return out


static func list_all_symbol_ids() -> PackedStringArray:
	_load_document()
	var arr: Array = _by_id.keys()
	arr.sort()
	var out := PackedStringArray()
	for k in arr:
		out.append(str(k))
	return out

extends RefCounted
class_name LocationRegistry
## Loads [`data/world/locations_registry.json`](../../data/world/locations_registry.json) — full campaign location spine (Unity `LocationNode` parity).

const REGISTRY_PATH := "res://data/world/locations_registry.json"

static var _cache: Dictionary = {}


static func get_document() -> Dictionary:
	if not _cache.is_empty():
		return _cache
	if not FileAccess.file_exists(REGISTRY_PATH):
		push_warning("LocationRegistry: missing %s" % REGISTRY_PATH)
		return {}
	var f := FileAccess.open(REGISTRY_PATH, FileAccess.READ)
	var raw: Variant = JSON.parse_string(f.get_as_text())
	if typeof(raw) != TYPE_DICTIONARY:
		push_warning("LocationRegistry: invalid JSON")
		return {}
	_cache = raw
	return _cache


static func default_shell_scene() -> String:
	var doc := get_document()
	return str(doc.get("default_location_shell_scene", "res://scenes/world/location_shell.tscn"))


static func list_location_ids_sorted() -> PackedStringArray:
	var locs: Dictionary = get_locations_map()
	var arr: Array = locs.keys()
	arr.sort()
	var out := PackedStringArray()
	for k in arr:
		out.append(str(k))
	return out


static func get_locations_map() -> Dictionary:
	var doc := get_document()
	var raw: Variant = doc.get("locations", {})
	return raw if typeof(raw) == TYPE_DICTIONARY else {}


static func get_location(location_id: String) -> Dictionary:
	var locs := get_locations_map()
	var row: Variant = locs.get(location_id.strip_edges(), {})
	return row if typeof(row) == TYPE_DICTIONARY else {}


static func get_display_name(location_id: String) -> String:
	var row := get_location(location_id)
	var n: String = str(row.get("display_name", ""))
	if n.is_empty():
		return location_id
	return n


static func get_description(location_id: String) -> String:
	return str(get_location(location_id).get("description", ""))


static func get_next_ids(location_id: String) -> PackedStringArray:
	var row := get_location(location_id)
	var nxt: Variant = row.get("next_location_ids", [])
	var out := PackedStringArray()
	if typeof(nxt) != TYPE_ARRAY:
		return out
	for x in nxt:
		var s := str(x).strip_edges()
		if not s.is_empty():
			out.append(s)
	return out


static func get_implementation_status(location_id: String) -> String:
	return str(get_location(location_id).get("implementation_status", "placeholder"))


static func get_encounter_resource_path(location_id: String) -> String:
	var row := get_location(location_id)
	var enc: Variant = row.get("encounter", null)
	if typeof(enc) != TYPE_DICTIONARY:
		return ""
	return str(enc.get("godot_resource", "")).strip_edges()


static func get_writer_notes(location_id: String) -> String:
	return str(get_location(location_id).get("writer_notes", ""))


## Optional Piers symbol ids from `piers_symbols` field (see `piers_symbols.json`).
static func get_piers_symbol_ids(location_id: String) -> PackedStringArray:
	var row := get_location(location_id)
	var raw: Variant = row.get("piers_symbols", [])
	var out := PackedStringArray()
	if typeof(raw) != TYPE_ARRAY:
		return out
	for x in raw:
		var s := str(x).strip_edges()
		if not s.is_empty():
			out.append(s)
	return out


static func get_text_edition_notes(location_id: String) -> String:
	return str(get_location(location_id).get("text_edition_notes", ""))


## B-text-oriented passus label for writers, if set (may be empty).
static func get_passus_anchor_b(location_id: String) -> String:
	var v: Variant = get_location(location_id).get("passus_anchor_b", "")
	if v == null:
		return ""
	return str(v)


static func clear_cache() -> void:
	_cache.clear()

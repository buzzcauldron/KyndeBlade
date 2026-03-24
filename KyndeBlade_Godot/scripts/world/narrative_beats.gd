extends RefCounted
class_name NarrativeBeats
## Loads `narrative_beats_skeleton.json` for arrival copy when Unity export rows are empty.

const PATH := "res://data/world/narrative_beats_skeleton.json"

static var _by_location: Dictionary = {}


static func arrival_placeholder_for_location(location_id: String) -> String:
	_ensure_loaded()
	var lid := location_id.strip_edges()
	if lid.is_empty():
		return ""
	return str(_by_location.get(lid, ""))


## Strips internal authoring tag for player-facing panels.
static func player_facing_arrival_line(location_id: String) -> String:
	var raw := arrival_placeholder_for_location(location_id)
	if raw.is_empty():
		return ""
	var s := raw.replace("[AUTHOR]", "").strip_edges()
	return s


static func _ensure_loaded() -> void:
	if not _by_location.is_empty():
		return
	_by_location = {}
	if not FileAccess.file_exists(PATH):
		push_warning("NarrativeBeats: missing %s" % PATH)
		return
	var f := FileAccess.open(PATH, FileAccess.READ)
	var doc: Variant = JSON.parse_string(f.get_as_text())
	if typeof(doc) != TYPE_DICTIONARY:
		push_warning("NarrativeBeats: invalid JSON")
		return
	var beats: Variant = doc.get("beats", [])
	if typeof(beats) != TYPE_ARRAY:
		return
	for b in beats:
		if typeof(b) != TYPE_DICTIONARY:
			continue
		var lid := str(b.get("location_id", "")).strip_edges()
		if lid.is_empty() or _by_location.has(lid):
			continue
		_by_location[lid] = str(b.get("placeholder_arrival", ""))

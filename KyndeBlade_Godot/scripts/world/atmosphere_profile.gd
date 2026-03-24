extends RefCounted
class_name AtmosphereProfile
## Loads [`data/world/atmosphere_profiles.json`](../../data/world/atmosphere_profiles.json) and applies knobs to hub crawl + combat backdrop.

const PATH := "res://data/world/atmosphere_profiles.json"

static var _doc: Dictionary = {}


static func _load() -> Dictionary:
	if not _doc.is_empty():
		return _doc
	if not FileAccess.file_exists(PATH):
		push_warning("AtmosphereProfile: missing %s" % PATH)
		return {}
	var f := FileAccess.open(PATH, FileAccess.READ)
	var raw: Variant = JSON.parse_string(f.get_as_text())
	if typeof(raw) != TYPE_DICTIONARY:
		push_warning("AtmosphereProfile: invalid JSON")
		return {}
	_doc = raw
	return _doc


static func profile_for_location(location_id: String) -> Dictionary:
	var doc := _load()
	var profiles: Variant = doc.get("profiles", {})
	if typeof(profiles) != TYPE_DICTIONARY:
		return {}
	var lid := location_id.strip_edges()
	if lid.is_empty():
		lid = str(doc.get("default_location_id", "default"))
	var row: Variant = profiles.get(lid, null)
	if row == null or typeof(row) != TYPE_DICTIONARY:
		row = profiles.get(str(doc.get("default_location_id", "default")), {})
	if typeof(row) != TYPE_DICTIONARY:
		return {}
	return row


static func apply_to_combat_backdrop(backdrop: Control, location_id: String) -> void:
	if backdrop == null:
		return
	var p := profile_for_location(location_id)
	if p.is_empty():
		return
	if "jewel_wash_strength" in p and backdrop.get("jewel_wash_strength") != null:
		backdrop.jewel_wash_strength = float(p["jewel_wash_strength"])
	if "jewel_wash_ultramarine_mix" in p and backdrop.get("jewel_wash_ultramarine_mix") != null:
		backdrop.jewel_wash_ultramarine_mix = float(p["jewel_wash_ultramarine_mix"])
	if "procedural_sky_alpha" in p and backdrop.get("procedural_sky_alpha") != null:
		backdrop.procedural_sky_alpha = float(p["procedural_sky_alpha"])
	if "parallax_speed_scale" in p and backdrop.get("parallax_speed_scale") != null:
		backdrop.parallax_speed_scale = float(p["parallax_speed_scale"])
	if "manuscript_void_blend" in p and backdrop.get("manuscript_void_blend") != null:
		backdrop.manuscript_void_blend = float(p["manuscript_void_blend"])


static func apply_to_hub_crawl(crawl: Node, location_id: String) -> void:
	if crawl == null:
		return
	var p := profile_for_location(location_id)
	if p.is_empty():
		return
	if crawl.get("parallax_speed_scale") != null and "parallax_speed_scale" in p:
		crawl.parallax_speed_scale = float(p["parallax_speed_scale"])


static func weather_preset_for_location(location_id: String) -> Dictionary:
	var p := profile_for_location(location_id)
	var pid := str(p.get("weather_preset_id", "mist_calm"))
	var intensity := clampf(float(p.get("weather_intensity", 1.0)), 0.0, 2.0)
	return {"preset_id": pid, "intensity": intensity}

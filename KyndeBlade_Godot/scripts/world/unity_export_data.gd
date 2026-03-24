class_name UnityExportData
extends RefCounted
## Parses `data/exported_from_unity.json` produced by Unity menu **KyndeBlade → Export Slice Data for Godot**.

const EXPORT_PATH := "res://data/exported_from_unity.json"


static func load_export() -> Dictionary:
	if not FileAccess.file_exists(EXPORT_PATH):
		return {}
	var f := FileAccess.open(EXPORT_PATH, FileAccess.READ)
	var data = JSON.parse_string(f.get_as_text())
	if typeof(data) != TYPE_DICTIONARY:
		return {}
	return data


static func location_by_id(export_data: Dictionary, id: String) -> Dictionary:
	var locs: Variant = export_data.get("locations", [])
	if typeof(locs) != TYPE_ARRAY:
		return {}
	for item: Variant in locs:
		if typeof(item) != TYPE_DICTIONARY:
			continue
		if str(item.get("location_id", "")) == id:
			return item
	return {}


static func encounter_for_asset_name(export_data: Dictionary, asset_name: String) -> Dictionary:
	var encs: Variant = export_data.get("encounters", [])
	if typeof(encs) != TYPE_ARRAY:
		return {}
	for item: Variant in encs:
		if typeof(item) != TYPE_DICTIONARY:
			continue
		if str(item.get("asset_name", "")) == asset_name:
			return item
	return {}

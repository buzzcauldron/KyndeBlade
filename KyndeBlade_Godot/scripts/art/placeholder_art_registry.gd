class_name PlaceholderArtRegistry
## Loads [`data/art/placeholder_registry.json`](../../data/art/placeholder_registry.json) — art-bible tokens for [`PlaceholderSilhouetteLibrary`](placeholder_silhouette_library.gd) + [`PlaceholderLocationBackdrop`](placeholder_location_backdrop.gd).

const REGISTRY_PATH := "res://data/art/placeholder_registry.json"

static var _doc: Dictionary = {}


static func get_document() -> Dictionary:
	if not _doc.is_empty():
		return _doc
	if not FileAccess.file_exists(REGISTRY_PATH):
		push_warning("PlaceholderArtRegistry: missing %s" % REGISTRY_PATH)
		return {}
	var f := FileAccess.open(REGISTRY_PATH, FileAccess.READ)
	var raw: Variant = JSON.parse_string(f.get_as_text())
	if typeof(raw) != TYPE_DICTIONARY:
		push_warning("PlaceholderArtRegistry: invalid JSON")
		return {}
	_doc = raw
	return _doc


static func clear_cache() -> void:
	_doc.clear()


static func color_from_token(token: String) -> Color:
	var t := token.strip_edges()
	if t.is_empty():
		return Color.WHITE
	if t.begins_with("#") and t.length() >= 7:
		return Color.html(t)
	# Map palette constant names → Color
	match t:
		"INK_PRIMARY":
			return KyndeBladeArtPalette.INK_PRIMARY
		"INK_SECONDARY":
			return KyndeBladeArtPalette.INK_SECONDARY
		"PARCHMENT_LIGHT":
			return KyndeBladeArtPalette.PARCHMENT_LIGHT
		"PARCHMENT":
			return KyndeBladeArtPalette.PARCHMENT
		"PARCHMENT_AGED":
			return KyndeBladeArtPalette.PARCHMENT_AGED
		"GOLD":
			return KyndeBladeArtPalette.GOLD
		"GOLD_DARK":
			return KyndeBladeArtPalette.GOLD_DARK
		"RUBRICATION":
			return KyndeBladeArtPalette.RUBRICATION
		"BORDER_RED":
			return KyndeBladeArtPalette.BORDER_RED
		"LAPIS":
			return KyndeBladeArtPalette.LAPIS
		"COMBAT_VOID":
			return KyndeBladeArtPalette.COMBAT_VOID
		"COMBAT_VOID_COOL":
			return KyndeBladeArtPalette.COMBAT_VOID_COOL
		"HUB_TWILIGHT":
			return KyndeBladeArtPalette.HUB_TWILIGHT
		"HUB_MIST":
			return KyndeBladeArtPalette.HUB_MIST
		"HI_BIT_SKY_TEAL":
			return KyndeBladeArtPalette.HI_BIT_SKY_TEAL
		"HI_BIT_SKY_MIST":
			return KyndeBladeArtPalette.HI_BIT_SKY_MIST
		"HI_BIT_SKY_PEACH":
			return KyndeBladeArtPalette.HI_BIT_SKY_PEACH
		"HI_BIT_SILHOUETTE_FAR":
			return KyndeBladeArtPalette.HI_BIT_SILHOUETTE_FAR
		"HI_BIT_SILHOUETTE_MID":
			return KyndeBladeArtPalette.HI_BIT_SILHOUETTE_MID
		"HI_BIT_TERRACOTTA":
			return KyndeBladeArtPalette.HI_BIT_TERRACOTTA
		"HI_BIT_SAGE":
			return KyndeBladeArtPalette.HI_BIT_SAGE
		"HI_BIT_TEAL_SHADOW":
			return KyndeBladeArtPalette.HI_BIT_TEAL_SHADOW
		"BOSS_GREEN_KNIGHT":
			return KyndeBladeArtPalette.BOSS_GREEN_KNIGHT
		"BOSS_HUNGER":
			return KyndeBladeArtPalette.BOSS_HUNGER
		"BOSS_PRIDE":
			return KyndeBladeArtPalette.BOSS_PRIDE
		"JEWEL_CRIMSON":
			return KyndeBladeArtPalette.JEWEL_CRIMSON
		"JEWEL_EMERALD":
			return KyndeBladeArtPalette.JEWEL_EMERALD
		"JEWEL_ULTRAMARINE":
			return KyndeBladeArtPalette.JEWEL_ULTRAMARINE
		"JEWEL_VIOLET_SHADOW":
			return KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW
		"VISTA_GOLD_TITLE":
			return KyndeBladeArtPalette.VISTA_GOLD_TITLE
		"VISTA_BODY":
			return KyndeBladeArtPalette.VISTA_BODY
		"skin_umber":
			return Color(0.36, 0.28, 0.2, 1.0)
		"cowl_dusk":
			return Color(0.22, 0.2, 0.26, 1.0)
		_:
			push_warning("PlaceholderArtRegistry: unknown color token '%s'" % t)
			return Color(0.5, 0.5, 0.5, 1.0)


static func character_entry(character_id: String) -> Dictionary:
	var ch: Variant = get_document().get("characters", {})
	if typeof(ch) != TYPE_DICTIONARY:
		return {}
	var row: Variant = ch.get(character_id, {})
	return row if typeof(row) == TYPE_DICTIONARY else {}


static func level_entry(location_id: String) -> Dictionary:
	var lv: Variant = get_document().get("levels", {})
	if typeof(lv) != TYPE_DICTIONARY:
		return {}
	var row: Variant = lv.get(location_id, {})
	return row if typeof(row) == TYPE_DICTIONARY else {}


static func level_preset(location_id: String) -> String:
	return str(level_entry(location_id).get("preset", "twilight_tower_mist"))


static func level_jewel_wash(location_id: String) -> float:
	return float(level_entry(location_id).get("jewel_wash", 0.06))


static func level_preview_character(location_id: String) -> String:
	return str(level_entry(location_id).get("preview_character", ""))


## Editor + headless: every `locations_registry` id must have a `levels` row.
static func validate_coverage() -> bool:
	var doc := get_document()
	if doc.is_empty():
		push_error("PlaceholderArtRegistry: empty document")
		return false
	var levels: Dictionary = doc.get("levels", {}) as Dictionary
	var ok := true
	for loc_id in LocationRegistry.list_location_ids_sorted():
		if not levels.has(loc_id):
			push_error("PlaceholderArtRegistry: missing levels[%s]" % loc_id)
			ok = false
	return ok


static func validate_characters_known() -> bool:
	var doc := get_document()
	var ch: Dictionary = doc.get("characters", {}) as Dictionary
	var need := [
		"will_dreamer",
		"langage_false",
		"green_knight",
		"lady_mede",
		"wrath_sin",
		"hunger_person",
		"piers_ploughman",
		"orfeo_threshold",
		"crowd_silhouette",
		"grace_waiting",
	]
	var ok := true
	for id in need:
		if not ch.has(id):
			push_error("PlaceholderArtRegistry: missing characters[%s]" % id)
			ok = false
	return ok

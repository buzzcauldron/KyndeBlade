extends RefCounted
class_name MedievalTextCatalog
## Loads [`data/medieval_text_unlocks.json`](../../data/medieval_text_unlocks.json) for medieval-text → moveset grants.

const PATH := "res://data/medieval_text_unlocks.json"

static var _cache: Dictionary = {}


static func get_catalog() -> Dictionary:
	if not _cache.is_empty():
		return _cache
	if not FileAccess.file_exists(PATH):
		push_warning("MedievalTextCatalog: missing %s" % PATH)
		return {}
	var f := FileAccess.open(PATH, FileAccess.READ)
	var raw: Variant = JSON.parse_string(f.get_as_text())
	if typeof(raw) != TYPE_DICTIONARY:
		push_warning("MedievalTextCatalog: invalid JSON")
		return {}
	_cache = raw
	return _cache


static func list_text_ids() -> PackedStringArray:
	var cat := get_catalog()
	var texts: Variant = cat.get("medieval_texts", [])
	var out := PackedStringArray()
	if typeof(texts) != TYPE_ARRAY:
		return out
	for t in texts:
		if typeof(t) == TYPE_DICTIONARY:
			var id: String = str(t.get("id", ""))
			if not id.is_empty():
				out.append(id)
	return out


static func aggregate_totals(read_ids: PackedStringArray) -> Dictionary:
	var cat := get_catalog()
	var texts: Array = cat.get("medieval_texts", []) as Array
	var code_table: Dictionary = cat.get("moveset_codes", {}) as Dictionary
	var totals := {
		"strike_stamina_cost_delta": 0.0,
		"strike_damage_delta": 0.0,
		"feint_pattern_offset_delta": 0,
		"dodge_stamina_flat_extra": 0.0,
		"fae_chance_delta": 0.0,
		"green_knight_random_weight_delta": 0.0,
		"parry_window_ms_penalty": 0,
	}
	var granted: Dictionary = {}
	for tid in read_ids:
		for item in texts:
			if typeof(item) != TYPE_DICTIONARY:
				continue
			if str(item.get("id", "")) != tid:
				continue
			var codes: Variant = item.get("grants_codes", [])
			if typeof(codes) != TYPE_ARRAY:
				continue
			for c in codes:
				var code: String = str(c)
				if code.is_empty() or granted.has(code):
					continue
				granted[code] = true
				var m: Variant = code_table.get(code, {})
				if typeof(m) != TYPE_DICTIONARY:
					continue
				totals["strike_stamina_cost_delta"] += float(m.get("strike_stamina_cost_delta", 0.0))
				totals["strike_damage_delta"] += float(m.get("strike_damage_delta", 0.0))
				totals["feint_pattern_offset_delta"] += int(m.get("feint_pattern_offset_delta", 0))
				totals["dodge_stamina_flat_extra"] += float(m.get("dodge_stamina_flat_extra", 0.0))
				totals["fae_chance_delta"] += float(m.get("fae_chance_delta", 0.0))
				totals["green_knight_random_weight_delta"] += float(m.get("green_knight_random_weight_delta", 0.0))
				totals["parry_window_ms_penalty"] += int(m.get("parry_window_ms_penalty", 0))
	return totals


## Move codes granted by read texts, in discovery order (first text wins for duplicates).
static func list_granted_codes_in_order(read_ids: PackedStringArray) -> PackedStringArray:
	var cat := get_catalog()
	var texts: Array = cat.get("medieval_texts", []) as Array
	var out := PackedStringArray()
	var seen: Dictionary = {}
	for tid in read_ids:
		for item in texts:
			if typeof(item) != TYPE_DICTIONARY:
				continue
			if str(item.get("id", "")) != tid:
				continue
			var codes: Variant = item.get("grants_codes", [])
			if typeof(codes) != TYPE_ARRAY:
				continue
			for c in codes:
				var code: String = str(c)
				if code.is_empty() or seen.has(code):
					continue
				seen[code] = true
				out.append(code)
	return out

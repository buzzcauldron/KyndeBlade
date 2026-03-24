extends Node
## Session + cross-scene flags for the Godot Steam / retail slice.

var current_location_id: String = "tour"
var fair_field_cleared: bool = false
## Mirrors Unity DemoTestHelper-adjacent flags (W7 meta-progression subset).
var ethical_misstep_count: int = 0
var has_ever_had_hunger: bool = false
## Completed medieval text / beat ids (see `data/medieval_text_unlocks.json`).
var read_medieval_text_ids: PackedStringArray = PackedStringArray()
## Piers / Langland text witness for writer-facing branching (default B-text).
var piers_text_edition: String = "B"
## Campaign phase id — aligns with `text_version_manifest.json` (`visio`, `vita_dowel`, …).
var narrative_phase_id: String = "visio"
## `location_id` → visit count (arrivals / hub travel). Persisted as pipe string in save.
var location_visit_counts: Dictionary = {}
## How many times the player has **returned** to the fair field after an earlier visit.
var fair_field_return_count: int = 0
## Scripted dream boundaries — bump when narrative fires a major wake / vision reset.
var dream_iteration: int = 0


func reset_from_new_game() -> void:
	current_location_id = "tour"
	fair_field_cleared = false
	ethical_misstep_count = 0
	has_ever_had_hunger = false
	read_medieval_text_ids = PackedStringArray()
	piers_text_edition = "B"
	narrative_phase_id = "visio"
	location_visit_counts = {}
	fair_field_return_count = 0
	dream_iteration = 0


func apply_loaded(data: Dictionary) -> void:
	if data.is_empty():
		reset_from_new_game()
		return
	current_location_id = str(data.get("location_id", "tour"))
	fair_field_cleared = bool(data.get("fair_field_cleared", false))
	ethical_misstep_count = int(data.get("ethical_misstep_count", 0))
	has_ever_had_hunger = bool(data.get("has_ever_had_hunger", false))
	_decode_read_text_ids(str(data.get("read_medieval_text_ids", "")))
	piers_text_edition = str(data.get("piers_text_edition", "B")).strip_edges()
	if piers_text_edition.is_empty():
		piers_text_edition = "B"
	narrative_phase_id = str(data.get("narrative_phase_id", "visio")).strip_edges()
	if narrative_phase_id.is_empty():
		narrative_phase_id = "visio"
	_decode_location_visit_counts(str(data.get("location_visit_counts", "")))
	fair_field_return_count = int(data.get("fair_field_return_count", 0))
	dream_iteration = int(data.get("dream_iteration", 0))


func _decode_read_text_ids(pipe_str: String) -> void:
	read_medieval_text_ids = PackedStringArray()
	if pipe_str.is_empty():
		return
	for part in pipe_str.split("|", false):
		var t := part.strip_edges()
		if not t.is_empty() and not read_medieval_text_ids.has(t):
			read_medieval_text_ids.append(t)


func _decode_location_visit_counts(pipe_str: String) -> void:
	location_visit_counts = {}
	if pipe_str.is_empty():
		return
	for part in pipe_str.split("|", false):
		var chunk := part.strip_edges()
		if chunk.is_empty():
			continue
		var bits := chunk.split(":", true, 1)
		if bits.size() < 2:
			continue
		var lid := str(bits[0]).strip_edges()
		if lid.is_empty():
			continue
		location_visit_counts[lid] = int(bits[1])


func encode_read_text_ids() -> String:
	if read_medieval_text_ids.is_empty():
		return ""
	return "|".join(read_medieval_text_ids)


func encode_location_visit_counts() -> String:
	if location_visit_counts.is_empty():
		return ""
	var keys: Array = location_visit_counts.keys()
	keys.sort()
	var parts: PackedStringArray = PackedStringArray()
	for k in keys:
		parts.append("%s:%d" % [str(k), int(location_visit_counts[k])])
	return "|".join(parts)


func mark_medieval_text_read(text_id: String) -> void:
	var t := text_id.strip_edges()
	if t.is_empty() or read_medieval_text_ids.has(t):
		return
	read_medieval_text_ids.append(t)
	sync_to_save()


func has_read_medieval_text(text_id: String) -> bool:
	return read_medieval_text_ids.has(text_id.strip_edges())


## Increments visit count for `loc_id`. When revisiting **fayre_felde**, bumps `fair_field_return_count`.
func record_location_visit(loc_id: String) -> void:
	var lid := loc_id.strip_edges()
	if lid.is_empty():
		return
	var prev: int = int(location_visit_counts.get(lid, 0))
	location_visit_counts[lid] = prev + 1
	if lid == "fayre_felde" and prev >= 1:
		fair_field_return_count += 1


func bump_dream_iteration() -> void:
	dream_iteration += 1


func sync_to_save() -> void:
	SaveService.save_progress(
			current_location_id,
			fair_field_cleared,
			ethical_misstep_count,
			has_ever_had_hunger,
			encode_read_text_ids(),
			piers_text_edition,
			narrative_phase_id,
			encode_location_visit_counts(),
			fair_field_return_count,
			dream_iteration,
	)


func on_victory_fair_field() -> void:
	current_location_id = "fayre_felde"
	fair_field_cleared = true
	sync_to_save()


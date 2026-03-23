extends Node
## Session + cross-scene flags for the Godot Steam / retail slice.

var current_location_id: String = "tour"
var fair_field_cleared: bool = false
## Mirrors Unity DemoTestHelper-adjacent flags (W7 meta-progression subset).
var ethical_misstep_count: int = 0
var has_ever_had_hunger: bool = false
## Completed medieval text / beat ids (see `data/medieval_text_unlocks.json`).
var read_medieval_text_ids: PackedStringArray = PackedStringArray()


func reset_from_new_game() -> void:
	current_location_id = "tour"
	fair_field_cleared = false
	ethical_misstep_count = 0
	has_ever_had_hunger = false
	read_medieval_text_ids = PackedStringArray()


func apply_loaded(data: Dictionary) -> void:
	if data.is_empty():
		reset_from_new_game()
		return
	current_location_id = str(data.get("location_id", "tour"))
	fair_field_cleared = bool(data.get("fair_field_cleared", false))
	ethical_misstep_count = int(data.get("ethical_misstep_count", 0))
	has_ever_had_hunger = bool(data.get("has_ever_had_hunger", false))
	_decode_read_text_ids(str(data.get("read_medieval_text_ids", "")))


func _decode_read_text_ids(pipe_str: String) -> void:
	read_medieval_text_ids = PackedStringArray()
	if pipe_str.is_empty():
		return
	for part in pipe_str.split("|", false):
		var t := part.strip_edges()
		if not t.is_empty() and not read_medieval_text_ids.has(t):
			read_medieval_text_ids.append(t)


func encode_read_text_ids() -> String:
	if read_medieval_text_ids.is_empty():
		return ""
	return "|".join(read_medieval_text_ids)


func mark_medieval_text_read(text_id: String) -> void:
	var t := text_id.strip_edges()
	if t.is_empty() or read_medieval_text_ids.has(t):
		return
	read_medieval_text_ids.append(t)
	sync_to_save()


func has_read_medieval_text(text_id: String) -> bool:
	return read_medieval_text_ids.has(text_id.strip_edges())


func sync_to_save() -> void:
	SaveService.save_progress(
			current_location_id,
			fair_field_cleared,
			ethical_misstep_count,
			has_ever_had_hunger,
			encode_read_text_ids(),
	)


func on_victory_fair_field() -> void:
	current_location_id = "fayre_felde"
	fair_field_cleared = true
	sync_to_save()

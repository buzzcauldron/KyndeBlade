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
## Hub route map fog: which `hub_route_map.json` node ids are visible / clickable.
var hub_revealed_node_ids: PackedStringArray = PackedStringArray()
## Runtime only: set before `ManuscriptNav.turn_page_to(combat)` to override default Fair Field encounter.
var pending_combat_encounter_path: String = ""
## Act 1: shallow dongeoun fight cleared (second combat).
var dongeoun_gate_cleared: bool = false
## One-time combat HUD line (feint vs true edge); cleared after first defensive wind-up.
var combat_defense_tip_ack: bool = false
## Main-menu **Combat gauntlet (greybox)**: chained fights without advancing campaign on wins.
var demo_gauntlet_active: bool = false
var demo_gauntlet_next_index: int = 0
## After final gauntlet win or abandon on defeat, outcome primary button returns to main menu.
var demo_gauntlet_exit_to_menu: bool = false

## Not `const` — GDScript requires a constant expression for const arrays here.
var DEMO_GAUNTLET_ENCOUNTERS: PackedStringArray = PackedStringArray(
		[
			"res://data/encounter_fair_field.tres",
			"res://data/encounter_fair_field.tres",
			"res://data/encounter_dongeoun_gate.tres",
		]
)


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
	dongeoun_gate_cleared = false
	combat_defense_tip_ack = false
	pending_combat_encounter_path = ""
	_clear_demo_gauntlet()
	_init_hub_revealed_from_registry()


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
	dongeoun_gate_cleared = bool(data.get("dongeoun_gate_cleared", false))
	combat_defense_tip_ack = bool(data.get("combat_defense_tip_ack", false))
	_decode_hub_revealed(str(data.get("hub_revealed_nodes", "")))
	if hub_revealed_node_ids.is_empty():
		_migrate_hub_revealed_from_legacy_progress()
	_clear_demo_gauntlet()


func begin_demo_gauntlet() -> void:
	demo_gauntlet_active = true
	demo_gauntlet_next_index = 0
	demo_gauntlet_exit_to_menu = false
	pending_combat_encounter_path = DEMO_GAUNTLET_ENCOUNTERS[0]


func _clear_demo_gauntlet() -> void:
	demo_gauntlet_active = false
	demo_gauntlet_next_index = 0
	demo_gauntlet_exit_to_menu = false


## Call after each gauntlet round victory (before outcome UI). Sets next encounter or ends run.
func demo_gauntlet_after_victory() -> void:
	demo_gauntlet_next_index += 1
	if demo_gauntlet_next_index >= DEMO_GAUNTLET_ENCOUNTERS.size():
		demo_gauntlet_active = false
		demo_gauntlet_exit_to_menu = true
		pending_combat_encounter_path = ""
	else:
		pending_combat_encounter_path = DEMO_GAUNTLET_ENCOUNTERS[demo_gauntlet_next_index]


func end_demo_gauntlet_from_defeat() -> void:
	demo_gauntlet_active = false
	demo_gauntlet_next_index = 0
	pending_combat_encounter_path = ""
	demo_gauntlet_exit_to_menu = true


func demo_gauntlet_refresh_pending_for_current_round() -> void:
	if not demo_gauntlet_active:
		return
	if demo_gauntlet_next_index >= 0 and demo_gauntlet_next_index < DEMO_GAUNTLET_ENCOUNTERS.size():
		pending_combat_encounter_path = DEMO_GAUNTLET_ENCOUNTERS[demo_gauntlet_next_index]


func finalize_demo_gauntlet_menu_return() -> void:
	demo_gauntlet_exit_to_menu = false
	demo_gauntlet_next_index = 0


func _init_hub_revealed_from_registry() -> void:
	hub_revealed_node_ids = PackedStringArray()
	var fog := HubRouteRegistry.fog_config()
	var init_ids: Variant = fog.get("initial_revealed", PackedStringArray(["tour"]))
	if init_ids is Array:
		for x in init_ids:
			var s := str(x).strip_edges()
			if not s.is_empty():
				hub_revealed_node_ids.append(s)
	elif str(init_ids).strip_edges() != "":
		hub_revealed_node_ids.append(str(init_ids).strip_edges())
	if hub_revealed_node_ids.is_empty():
		hub_revealed_node_ids.append("tour")


func _decode_hub_revealed(pipe_str: String) -> void:
	hub_revealed_node_ids = PackedStringArray()
	if pipe_str.is_empty():
		return
	for part in pipe_str.split("|", false):
		var t := part.strip_edges()
		if not t.is_empty() and not hub_revealed_node_ids.has(t):
			hub_revealed_node_ids.append(t)


func encode_hub_revealed() -> String:
	if hub_revealed_node_ids.is_empty():
		return ""
	return "|".join(hub_revealed_node_ids)


func is_hub_node_revealed(node_id: String) -> bool:
	return hub_revealed_node_ids.has(node_id.strip_edges())


func reveal_hub_node(node_id: String) -> void:
	var nid := node_id.strip_edges()
	if nid.is_empty() or hub_revealed_node_ids.has(nid):
		return
	hub_revealed_node_ids.append(nid)


## Reveal neighbors of `from_id` except **dongeoun** until [member fair_field_cleared]
## (slice pacing).
func reveal_hub_neighbors(from_id: String) -> void:
	var fog := HubRouteRegistry.fog_config()
	if not bool(fog.get("reveal_neighbors_on_visit", true)):
		return
	var from := from_id.strip_edges()
	for nid in HubRouteRegistry.neighbor_ids(from):
		if nid == "dongeoun" and not fair_field_cleared:
			continue
		reveal_hub_node(nid)


func apply_hub_reveal_flags() -> void:
	var fog := HubRouteRegistry.fog_config()
	var rules: Variant = fog.get("reveal_when_flag", [])
	if rules is Array:
		for rule in rules:
			if typeof(rule) != TYPE_DICTIONARY:
				continue
			if bool(rule.get("requires_fair_field_cleared", false)) and fair_field_cleared:
				reveal_hub_node(str(rule.get("node_id", "")))


func hub_on_map_opened() -> void:
	reveal_hub_neighbors(current_location_id)
	apply_hub_reveal_flags()


func _migrate_hub_revealed_from_legacy_progress() -> void:
	_init_hub_revealed_from_registry()
	reveal_hub_node("tour")
	if fair_field_cleared or int(location_visit_counts.get("fayre_felde", 0)) > 0:
		reveal_hub_node("fayre_felde")
	if fair_field_cleared:
		reveal_hub_node("dongeoun")


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


## Increments visit count for `loc_id`. When revisiting **fayre_felde**,
## bumps `fair_field_return_count`.
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
			encode_hub_revealed(),
			dongeoun_gate_cleared,
			combat_defense_tip_ack,
	)


func on_victory_fair_field() -> void:
	current_location_id = "fayre_felde"
	fair_field_cleared = true
	sync_to_save()


func on_victory_dongeoun_gate() -> void:
	current_location_id = "dongeoun"
	dongeoun_gate_cleared = true
	sync_to_save()


extends Node
## Headless smoke tests for TDAD godot-parity-slice / CI (see docs/CI_GODOT_TESTS.md).
## Run via `res://tests/headless_main.tscn` (not `--script` on a SceneTree):
## Godot 4.6 resolves autoload singletons after the main scene tree exists;
## a SceneTree entry script compiles too early.
## Uses [/root/…] lookups + preloaded autoload scripts as types so tooling works when the workspace
## root is the monorepo (not only [code]KyndeBlade_Godot[/code]).

const PHeadlessSave := preload("res://scripts/save_service.gd")
const PHeadlessState := preload("res://scripts/game_state.gd")
const PHeadlessWorldNav := preload("res://scripts/world/world_nav.gd")
const _CombatScenarioRunner := preload("res://tests/combat_scenarios.gd")
const _NarrativeBeats := preload("res://scripts/world/narrative_beats.gd")

var _save: PHeadlessSave
var _state: PHeadlessState
var _world_nav: PHeadlessWorldNav


func _ready() -> void:
	var root := get_tree().root
	_save = root.get_node("/root/SaveService") as PHeadlessSave
	_state = root.get_node("/root/GameState") as PHeadlessState
	_world_nav = root.get_node("/root/WorldNav") as PHeadlessWorldNav
	get_tree().process_frame.connect(_kickoff, CONNECT_ONE_SHOT)


func _kickoff() -> void:
	var ok := true
	ok = _headless_step("slice_locations_json", _test_slice_locations_json(), ok)
	ok = _headless_step("unity_export_json", _test_unity_export_json(), ok)
	ok = _headless_step("save_roundtrip_service", _test_save_roundtrip_service(), ok)
	ok = _headless_step("dongeoun_gate_save_roundtrip", _test_dongeoun_gate_save_roundtrip(), ok)
	ok = _headless_step("combat_defense_tip_save_roundtrip", _test_combat_defense_tip_save_roundtrip(), ok)
	ok = _headless_step("narrative_beats_skeleton_lines", _test_narrative_beats_skeleton_lines(), ok)
	ok = _headless_step("meta_flags_roundtrip", _test_meta_flags_roundtrip(), ok)
	ok = _headless_step("settings_volume", _test_settings_volume(), ok)
	ok = _headless_step("audio_buses", _test_audio_buses(), ok)
	ok = _headless_step("encounter_resource", _test_encounter_resource(), ok)
	ok = _headless_step("hazard_stub_damage", _test_hazard_stub_damage(), ok)
	ok = _headless_step("swing_pattern_static", _test_swing_pattern_static(), ok)
	ok = _headless_step("game_state_tour", _test_game_state_tour(), ok)
	ok = _headless_step(
			"read_medieval_text_ids_roundtrip", _test_read_medieval_text_ids_roundtrip(), ok
	)
	ok = _headless_step("medieval_catalog_aggregate", _test_medieval_catalog_aggregate(), ok)
	ok = _headless_step("replay_moveset_matrix", _test_replay_moveset_matrix(), ok)
	ok = _headless_step("autosave_mirror_exists", _test_autosave_mirror_exists(), ok)
	ok = _headless_step("load_fallback_autosave", _test_load_fallback_autosave(), ok)
	ok = _headless_step("legacy_demo_save_migrates", _test_legacy_demo_save_migrates(), ok)
	ok = _headless_step("medieval_list_granted_codes", _test_medieval_list_granted_codes(), ok)
	ok = _headless_step(
			"piers_symbol_catalog_fayre_felde", _test_piers_symbol_catalog_fayre_felde(), ok
	)
	ok = _headless_step("piers_state_save_roundtrip", _test_piers_state_save_roundtrip(), ok)
	ok = _headless_step(
			"narrative_context_arrival_variant", _test_narrative_context_arrival_variant(), ok
	)
	ok = _headless_step(
			"atmosphere_weather_presets_resolve", _test_atmosphere_weather_presets_resolve(), ok
	)
	ok = _headless_step("hub_revealed_save_roundtrip", _test_hub_revealed_save_roundtrip(), ok)
	ok = _headless_step("placeholder_art_registry", _test_placeholder_art_registry(), ok)
	ok = _headless_step(
			"victory_fair_field_updates_gamestate", _test_victory_fair_field_updates_gamestate(), ok
	)
	ok = _headless_step("settings_fullscreen_roundtrip", _test_settings_fullscreen_roundtrip(), ok)
	ok = _headless_step(
			"main_menu_continue_gated_smoke", await _test_main_menu_continue_gated_smoke(), ok
	)
	ok = _headless_step("hub_counsel_gate_smoke", await _test_hub_counsel_gate_smoke(), ok)
	ok = _headless_step(
			"combat_pause_freezes_window_tick_smoke",
			await _test_combat_pause_freezes_window_tick_smoke(),
			ok,
	)
	ok = _headless_step("world_atlas_scene_smoke", await _test_world_atlas_scene_smoke(), ok)
	ok = _headless_step(
			"location_shell_scene_smoke", await _test_location_shell_scene_smoke(), ok
	)
	ok = _headless_step(
			"slice_open_yard_scene_smoke", await _test_slice_open_yard_scene_smoke(), ok
	)
	ok = _headless_step("scene_transition_smoke", await _test_scene_transition_smoke(), ok)
	ok = _headless_step("crawl_overworld_scene_smoke", await _test_crawl_overworld_scene_smoke(), ok)
	ok = _headless_step("nav_test_yard_smoke", await _test_nav_test_yard_smoke(), ok)
	ok = _headless_step("hub_route_map_pin_geo_consistency", _test_hub_route_map_pin_geo_consistency(), ok)
	if not ok:
		_finish(false)
		return
	_save.write_new_game()
	_state.reset_from_new_game()
	var runner := _CombatScenarioRunner.new()
	ok = _headless_step("combat_scenarios", await runner.run_all(get_tree().root), ok)
	_finish(ok)


func _headless_step(step_name: String, step_ok: bool, ok_so_far: bool) -> bool:
	if not step_ok:
		printerr("HEADLESS step failed: ", step_name)
	return ok_so_far and step_ok


func _finish(passed: bool) -> void:
	print("HEADLESS_TESTS: ", "PASS" if passed else "FAIL")
	_save.write_new_game()
	_state.reset_from_new_game()
	get_tree().quit(0 if passed else 1)


func _test_slice_locations_json() -> bool:
	var path := "res://data/slice_locations.json"
	if not FileAccess.file_exists(path):
		push_error("missing slice_locations.json")
		return false
	var f := FileAccess.open(path, FileAccess.READ)
	var data = JSON.parse_string(f.get_as_text())
	if typeof(data) != TYPE_DICTIONARY:
		return false
	var tour: Variant = data.get("tour", {})
	if typeof(tour) != TYPE_DICTIONARY:
		return false
	var nxt: Variant = tour.get("next", [])
	if typeof(nxt) != TYPE_ARRAY:
		return false
	return nxt.has("fayre_felde")


func _test_unity_export_json() -> bool:
	var data: Dictionary = UnityExportData.load_export()
	if data.is_empty():
		push_error("missing or invalid exported_from_unity.json")
		return false
	if int(data.get("schema_version", 0)) < 2:
		return false
	var locs: Variant = data.get("locations", [])
	if typeof(locs) != TYPE_ARRAY or (locs as Array).size() < 2:
		return false
	var tour: Dictionary = UnityExportData.location_by_id(data, "tour")
	var tn: Variant = tour.get("next_location_ids", [])
	if typeof(tn) != TYPE_ARRAY or not tn.has("fayre_felde"):
		return false
	var fayre: Dictionary = UnityExportData.location_by_id(data, "fayre_felde")
	var encn: String = str(fayre.get("encounter_asset_name", ""))
	var enc: Dictionary = UnityExportData.encounter_for_asset_name(data, encn)
	var types: Variant = enc.get("enemy_character_types", [])
	if typeof(types) != TYPE_ARRAY or not types.has("False"):
		return false
	if str(tour.get("arrival_beat_id", "")) != "tower_vista":
		return false
	return not str(tour.get("arrival_beat_text", "")).is_empty()


func _test_dongeoun_gate_save_roundtrip() -> bool:
	_save.write_new_game()
	_state.reset_from_new_game()
	_state.dongeoun_gate_cleared = true
	_state.sync_to_save()
	var d: Dictionary = _save.load_save()
	return bool(d.get("dongeoun_gate_cleared", false))


func _test_combat_defense_tip_save_roundtrip() -> bool:
	_save.write_new_game()
	_state.reset_from_new_game()
	_state.combat_defense_tip_ack = true
	_state.sync_to_save()
	var d: Dictionary = _save.load_save()
	if not bool(d.get("combat_defense_tip_ack", false)):
		return false
	_state.apply_loaded(d)
	return _state.combat_defense_tip_ack


func _test_narrative_beats_skeleton_lines() -> bool:
	var f: String = _NarrativeBeats.player_facing_arrival_line("fayre_felde")
	var dg: String = _NarrativeBeats.player_facing_arrival_line("dongeoun")
	var tour_line: String = _NarrativeBeats.player_facing_arrival_line("tour")
	return not f.is_empty() and not dg.is_empty() and not tour_line.is_empty()


func _test_save_roundtrip_service() -> bool:
	_save.write_new_game()
	var d: Dictionary = _save.load_save()
	if str(d.get("location_id", "")) != "tour":
		return false
	if not FileAccess.file_exists("user://kyndeblade_save.cfg"):
		return false
	_save.save_progress("fayre_felde", true)
	d = _save.load_save()
	return str(d.get("location_id", "")) == "fayre_felde" and bool(d.get("fair_field_cleared", false))


func _test_meta_flags_roundtrip() -> bool:
	_save.write_new_game()
	_save.save_progress("tour", false, 2, true)
	var d: Dictionary = _save.load_save()
	if int(d.get("ethical_misstep_count", -1)) != 2:
		return false
	if bool(d.get("has_ever_had_hunger", false)) != true:
		return false
	_state.apply_loaded(d)
	return _state.ethical_misstep_count == 2 and _state.has_ever_had_hunger


func _test_settings_volume() -> bool:
	_save.save_master_volume(0.42)
	return is_equal_approx(_save.load_master_volume(), 0.42)


func _test_audio_buses() -> bool:
	return AudioServer.get_bus_index("Music") >= 0 and AudioServer.get_bus_index("SFX") >= 0


func _test_encounter_resource() -> bool:
	var enc := load("res://data/encounter_fair_field.tres") as EncounterDef
	if enc == null:
		return false
	var ok_ff: bool = (
			enc.enemy_id == "false"
			and enc.encounter_id == "fayre_felde"
			and str(enc.enemy_display_name).findn("langage") >= 0
	)
	var dg := load("res://data/encounter_dongeoun_gate.tres") as EncounterDef
	if dg == null:
		return false
	var ok_dg: bool = (
			dg.encounter_id == "dongeoun_gate"
			and dg.defensive_windup_sec > 0.0
			and dg.enemy_turn_bleed_damage > 0.0
	)
	return ok_ff and ok_dg


func _test_hazard_stub_damage() -> bool:
	var c := CombatManager.new()
	var hz: float = c.apply_piers_hazard_damage()
	c.queue_free()
	return hz == 0.0


func _test_swing_pattern_static() -> bool:
	var a := CombatManager.enemy_swing_is_hit_for_window_index(0) == true
	var b := CombatManager.enemy_swing_is_hit_for_window_index(1) == false
	var c := CombatManager.enemy_swing_is_hit_for_window_index(0, 1) == false
	var d := CombatManager.enemy_swing_is_hit_for_window_index(1, 1) == true
	return a and b and c and d


func _test_game_state_tour() -> bool:
	_state.reset_from_new_game()
	var ok1 := _state.current_location_id == "tour" and _state.fair_field_cleared == false
	_state.read_medieval_text_ids.append("tower_vista")
	_state.reset_from_new_game()
	var ok2 := _state.read_medieval_text_ids.is_empty()
	return ok1 and ok2


func _test_read_medieval_text_ids_roundtrip() -> bool:
	_save.write_new_game()
	_save.save_progress("tour", false, 0, false, "tower_vista|FayreFelde")
	var d: Dictionary = _save.load_save()
	if str(d.get("read_medieval_text_ids", "")) != "tower_vista|FayreFelde":
		return false
	_state.apply_loaded(d)
	return (
			_state.read_medieval_text_ids.size() == 2
			and _state.read_medieval_text_ids.has("tower_vista")
			and _state.read_medieval_text_ids.has("FayreFelde")
	)


func _test_medieval_catalog_aggregate() -> bool:
	_state.reset_from_new_game()
	_state.read_medieval_text_ids = PackedStringArray(["tower_vista"])
	var t: Dictionary = MedievalTextCatalog.aggregate_totals(_state.read_medieval_text_ids)
	# dreamer_ledger_stride: +5 stam cost, +4 damage
	if not is_equal_approx(float(t.get("strike_stamina_cost_delta", 0.0)), 5.0):
		return false
	if not is_equal_approx(float(t.get("strike_damage_delta", 0.0)), 4.0):
		return false
	_state.reset_from_new_game()
	return true


func _mirror_combat_feint_pattern_offset() -> int:
	var d: int = PlayerMovesetModifiers.feint_pattern_offset_delta()
	return posmod(int(_state.ethical_misstep_count) + d, 2)


func _test_replay_moveset_matrix() -> bool:
	_state.reset_from_new_game()
	var base_parry: int = PlayerMovesetModifiers.parry_window_ms()
	if base_parry < 170 or base_parry > 230:
		return false
	if _mirror_combat_feint_pattern_offset() != 0:
		return false
	_state.has_ever_had_hunger = true
	var hunger_parry: int = PlayerMovesetModifiers.parry_window_ms()
	if hunger_parry > base_parry:
		return false
	var st1: float = PlayerMovesetModifiers.parry_stamina_total()
	var st2: float = PlayerMovesetModifiers.parry_stamina_total()
	if not is_equal_approx(st1, st2):
		return false
	_state.has_ever_had_hunger = false
	_state.ethical_misstep_count = 1
	if _mirror_combat_feint_pattern_offset() != 1:
		return false
	_state.ethical_misstep_count = 0
	_state.read_medieval_text_ids = PackedStringArray(["FayreFelde"])
	if PlayerMovesetModifiers.feint_pattern_offset_delta() != 1:
		return false
	if _mirror_combat_feint_pattern_offset() != 1:
		return false
	_state.ethical_misstep_count = 1
	if _mirror_combat_feint_pattern_offset() != 0:
		return false
	_state.has_ever_had_hunger = true
	_state.current_location_id = "fayre_felde"
	_state.fair_field_return_count = 2
	_state.dream_iteration = 3
	var w1: int = PlayerMovesetModifiers.parry_window_ms()
	var w2: int = PlayerMovesetModifiers.parry_window_ms()
	if w1 != w2:
		return false
	_state.reset_from_new_game()
	_state.record_location_visit("fayre_felde")
	if NarrativeContext.arrival_variant_key("fayre_felde") != "first":
		return false
	_state.record_location_visit("fayre_felde")
	if NarrativeContext.arrival_variant_key("fayre_felde") != "return_2":
		return false
	_state.reset_from_new_game()
	return true


func _test_autosave_mirror_exists() -> bool:
	_save.write_new_game()
	return FileAccess.file_exists("user://kyndeblade_save.cfg") and FileAccess.file_exists(
			"user://kyndeblade_autosave.cfg"
	)


func _test_load_fallback_autosave() -> bool:
	_save.write_new_game()
	_save.save_progress("fayre_felde", true, 1, false, "")
	var d := DirAccess.open("user://")
	if d == null:
		return false
	if d.file_exists("kyndeblade_save.cfg"):
		d.remove("kyndeblade_save.cfg")
	var loaded: Dictionary = _save.load_save()
	var ok: bool = (
			str(loaded.get("location_id", "")) == "fayre_felde"
			and bool(loaded.get("fair_field_cleared", false))
	)
	_save.write_new_game()
	_state.reset_from_new_game()
	return ok


func _test_legacy_demo_save_migrates() -> bool:
	_save.delete_save()
	var d := DirAccess.open("user://")
	if d != null:
		for f: String in ["kyndeblade_save.cfg", "kyndeblade_autosave.cfg", "kyndeblade_demo_save.cfg"]:
			if d.file_exists(f):
				d.remove(f)
	var cfg := ConfigFile.new()
	cfg.set_value("save", "version", 1)
	cfg.set_value("save", "location_id", "tour")
	cfg.set_value("save", "fair_field_cleared", false)
	cfg.set_value("save", "ethical_misstep_count", 7)
	cfg.set_value("save", "has_ever_had_hunger", true)
	cfg.set_value("save", "read_medieval_text_ids", "tower_vista")
	if cfg.save("user://kyndeblade_demo_save.cfg") != OK:
		return false
	var loaded: Dictionary = _save.load_save()
	if int(loaded.get("ethical_misstep_count", -1)) != 7:
		_save.delete_save()
		return false
	var has_new: bool = FileAccess.file_exists("user://kyndeblade_save.cfg")
	var legacy_removed: bool = not FileAccess.file_exists("user://kyndeblade_demo_save.cfg")
	_save.delete_save()
	return has_new and legacy_removed


func _test_medieval_list_granted_codes() -> bool:
	var codes := MedievalTextCatalog.list_granted_codes_in_order(
			PackedStringArray(["tower_vista", "FayreFelde"])
	)
	return (
			codes.size() == 2
			and str(codes[0]) == "dreamer_ledger_stride"
			and str(codes[1]) == "crowd_surge"
	)


func _test_piers_symbol_catalog_fayre_felde() -> bool:
	PiersSymbolCatalog.clear_cache()
	LocationRegistry.clear_cache()
	var row: Dictionary = PiersSymbolCatalog.get_symbol("field_full_of_folk")
	if row.is_empty() or str(row.get("id", "")) != "field_full_of_folk":
		return false
	var ids: PackedStringArray = PiersSymbolCatalog.symbols_for_location("fayre_felde")
	return ids.has("field_full_of_folk") and ids.has("four_types_of_seeds")


func _test_piers_state_save_roundtrip() -> bool:
	_save.write_new_game()
	_state.reset_from_new_game()
	_state.piers_text_edition = "C"
	_state.narrative_phase_id = "vita_dowel"
	_state.record_location_visit("tour")
	_state.record_location_visit("fayre_felde")
	_state.record_location_visit("fayre_felde")
	_state.fair_field_return_count = 2
	_state.dream_iteration = 4
	_state.sync_to_save()
	var d: Dictionary = _save.load_save()
	if str(d.get("piers_text_edition", "")) != "C":
		return false
	if str(d.get("narrative_phase_id", "")) != "vita_dowel":
		return false
	if int(d.get("fair_field_return_count", -1)) != 2:
		return false
	if int(d.get("dream_iteration", -1)) != 4:
		return false
	_state.reset_from_new_game()
	_state.apply_loaded(d)
	return (
			_state.piers_text_edition == "C"
			and _state.narrative_phase_id == "vita_dowel"
			and int(_state.location_visit_counts.get("tour", 0)) == 1
			and int(_state.location_visit_counts.get("fayre_felde", 0)) == 2
			and _state.fair_field_return_count == 2
			and _state.dream_iteration == 4
	)


func _test_narrative_context_arrival_variant() -> bool:
	_state.reset_from_new_game()
	if NarrativeContext.arrival_variant_key("tour") != "first":
		return false
	_state.record_location_visit("tour")
	if NarrativeContext.arrival_variant_key("tour") != "first":
		return false
	_state.record_location_visit("tour")
	return NarrativeContext.arrival_variant_key("tour") == "return_2"


func _test_atmosphere_weather_presets_resolve() -> bool:
	if HubRouteRegistry.nodes().is_empty():
		push_error("atmosphere: hub_route_map nodes missing")
		return false
	for lid in ["tour", "fayre_felde", "dongeoun", "nonexistent_slice_id"]:
		var wx: Dictionary = AtmosphereProfile.weather_preset_for_location(lid)
		var pid: String = str(wx.get("preset_id", ""))
		if pid.is_empty():
			push_error("atmosphere: empty weather preset for location %s" % lid)
			return false
		if not WeatherParticles.preset_exists(pid):
			push_error("atmosphere: weather_presets.json missing %s (from %s)" % [pid, lid])
			return false
	return true


func _test_hub_revealed_save_roundtrip() -> bool:
	_save.write_new_game()
	_state.reset_from_new_game()
	_state.reveal_hub_node("fayre_felde")
	_state.sync_to_save()
	var d: Dictionary = _save.load_save()
	if str(d.get("hub_revealed_nodes", "")).find("fayre_felde") < 0:
		_save.write_new_game()
		_state.reset_from_new_game()
		return false
	_state.reset_from_new_game()
	_state.apply_loaded(d)
	var ok := _state.is_hub_node_revealed("fayre_felde")
	_save.write_new_game()
	_state.reset_from_new_game()
	return ok


func _test_hub_route_map_pin_geo_consistency() -> bool:
	var bounds := HubGeoProject.basemap_bounds_from_registry()
	if bounds.is_empty():
		push_error("hub_route_map: basemap.bounds missing for pin geo check")
		return false
	var eps := 0.0001
	for item in HubRouteRegistry.nodes():
		if typeof(item) != TYPE_DICTIONARY:
			continue
		var row: Dictionary = item
		if not row.has("lon") or not row.has("lat"):
			continue
		var v := HubGeoProject.lon_lat_to_normalized(
				float(row["lon"]), float(row["lat"]), bounds
		)
		var ex := float(row.get("x", -1.0))
		var ey := float(row.get("y", -1.0))
		if absf(v.x - ex) > eps or absf(v.y - ey) > eps:
			push_error(
					"hub_route_map pin drift id=%s norm=(%s,%s) json xy=(%s,%s)"
					% [row.get("id", "?"), v.x, v.y, ex, ey]
			)
			return false
	return true


func _test_placeholder_art_registry() -> bool:
	PlaceholderArtRegistry.clear_cache()
	return (
			PlaceholderArtRegistry.validate_coverage()
			and PlaceholderArtRegistry.validate_characters_known()
	)


func _test_victory_fair_field_updates_gamestate() -> bool:
	_save.write_new_game()
	_state.reset_from_new_game()
	_state.on_victory_fair_field()
	if _state.current_location_id != "fayre_felde" or not _state.fair_field_cleared:
		_save.write_new_game()
		_state.reset_from_new_game()
		return false
	var d: Dictionary = _save.load_save()
	var ok: bool = (
			str(d.get("location_id", "")) == "fayre_felde"
			and bool(d.get("fair_field_cleared", false))
	)
	_save.write_new_game()
	_state.reset_from_new_game()
	return ok


func _test_settings_fullscreen_roundtrip() -> bool:
	var before: bool = _save.load_fullscreen()
	_save.save_fullscreen(not before)
	if _save.load_fullscreen() != (not before):
		_save.save_fullscreen(before)
		return false
	_save.save_fullscreen(before)
	return _save.load_fullscreen() == before


func _test_main_menu_continue_gated_smoke() -> bool:
	_save.delete_save()
	_state.reset_from_new_game()
	var menu_path := "res://scenes/main_menu.tscn"
	if not FileAccess.file_exists(menu_path):
		push_error("main menu smoke: missing tscn")
		_save.write_new_game()
		_state.reset_from_new_game()
		return false
	var ps: PackedScene = load(menu_path) as PackedScene
	var mm: Control = ps.instantiate() as Control
	if mm == null:
		_save.write_new_game()
		_state.reset_from_new_game()
		return false
	get_tree().root.add_child(mm)
	await get_tree().process_frame
	var cont: Button = mm.get_node("%ContinueButton") as Button
	if cont == null:
		push_error("main menu smoke: missing %ContinueButton")
		mm.queue_free()
		await get_tree().process_frame
		_save.write_new_game()
		_state.reset_from_new_game()
		return false
	if not cont.disabled:
		push_error("main menu smoke: Continue should be disabled without save")
		mm.queue_free()
		await get_tree().process_frame
		_save.write_new_game()
		_state.reset_from_new_game()
		return false
	_save.write_new_game()
	await get_tree().process_frame
	if cont.disabled:
		push_error("main menu smoke: Continue should enable after save")
		mm.queue_free()
		await get_tree().process_frame
		_state.reset_from_new_game()
		return false
	mm.queue_free()
	await get_tree().process_frame
	_state.reset_from_new_game()
	return true


func _test_hub_counsel_gate_smoke() -> bool:
	_save.write_new_game()
	_state.reset_from_new_game()
	var hub_path := "res://scenes/hub_map.tscn"
	var ps: PackedScene = load(hub_path) as PackedScene
	var hub: Control = ps.instantiate() as Control
	if hub == null:
		return false
	get_tree().root.add_child(hub)
	await get_tree().process_frame
	var fair: Button = hub.get_node("%FairFieldButton") as Button
	var proceed: Button = hub.get_node("%EnterCombat") as Button
	var counsel: Button = hub.get_node("%CounselTrewthe") as Button
	var cancel: Button = hub.get_node("FlavorPanel/FVBox/CancelFlavor") as Button
	if fair == null or proceed == null or counsel == null or cancel == null:
		push_error("hub counsel smoke: expected hub nodes missing")
		hub.queue_free()
		await get_tree().process_frame
		return false
	if hub.has_method("travel_to_fair_field_for_tests"):
		hub.call("travel_to_fair_field_for_tests")
	else:
		fair.pressed.emit()
	await get_tree().process_frame
	if not proceed.disabled:
		push_error("hub counsel smoke: Proceed should stay disabled before counsel")
		hub.queue_free()
		await get_tree().process_frame
		return false
	counsel.pressed.emit()
	await get_tree().process_frame
	if proceed.disabled:
		push_error("hub counsel smoke: Proceed should enable after counsel")
		hub.queue_free()
		await get_tree().process_frame
		return false
	cancel.pressed.emit()
	await get_tree().process_frame
	hub.queue_free()
	await get_tree().process_frame
	return true


func _test_combat_pause_freezes_window_tick_smoke() -> bool:
	var combat_path := "res://scenes/combat.tscn"
	var ps: PackedScene = load(combat_path) as PackedScene
	var combat_inst: Node = ps.instantiate()
	if combat_inst == null:
		return false
	var cm: CombatManager = combat_inst.get_node_or_null("CombatManager") as CombatManager
	if cm == null:
		push_error("combat pause smoke: CombatManager missing")
		return false
	cm.use_instant_resolution_for_tests = true
	get_tree().root.add_child(combat_inst)
	await get_tree().process_frame
	if cm.state != CombatManager.State.WAITING_PLAYER:
		push_error("combat pause smoke: expected WAITING_PLAYER")
		combat_inst.queue_free()
		await get_tree().process_frame
		return false
	cm.player_dodge()
	if cm.state != CombatManager.State.REAL_TIME_WINDOW:
		push_error("combat pause smoke: expected REAL_TIME_WINDOW after dodge")
		combat_inst.queue_free()
		await get_tree().process_frame
		return false
	var r0: float = cm.window_remaining
	if r0 <= 0.0001:
		push_error("combat pause smoke: expected positive window_remaining")
		combat_inst.queue_free()
		await get_tree().process_frame
		return false
	get_tree().paused = true
	for _i in 5:
		await get_tree().process_frame
	var r1: float = cm.window_remaining
	get_tree().paused = false
	combat_inst.queue_free()
	await get_tree().process_frame
	if not is_equal_approx(r0, r1):
		push_error("combat pause smoke: window_remaining changed while paused (%s vs %s)" % [r0, r1])
		return false
	return true


func _test_world_atlas_scene_smoke() -> bool:
	var path := "res://scenes/world/world_atlas.tscn"
	if not FileAccess.file_exists(path):
		push_error("world atlas smoke: missing tscn")
		return false
	var ps: PackedScene = load(path) as PackedScene
	var inst: Control = ps.instantiate() as Control
	if inst == null:
		return false
	get_tree().root.add_child(inst)
	await get_tree().process_frame
	var list: VBoxContainer = inst.get_node_or_null("%LocationList") as VBoxContainer
	if list == null or list.get_child_count() < 1:
		push_error("world atlas smoke: LocationList should list at least one location")
		inst.queue_free()
		await get_tree().process_frame
		return false
	inst.queue_free()
	await get_tree().process_frame
	return true


func _test_location_shell_scene_smoke() -> bool:
	_world_nav.pending_location_id = "tour"
	var path := "res://scenes/world/location_shell.tscn"
	if not FileAccess.file_exists(path):
		push_error("location shell smoke: missing tscn")
		return false
	var ps: PackedScene = load(path) as PackedScene
	var inst: Control = ps.instantiate() as Control
	if inst == null:
		return false
	get_tree().root.add_child(inst)
	await get_tree().process_frame
	var title: Label = inst.get_node_or_null("%TitleLabel") as Label
	if title == null or str(title.text).strip_edges().is_empty():
		push_error("location shell smoke: title not populated for tour")
		inst.queue_free()
		await get_tree().process_frame
		return false
	inst.queue_free()
	await get_tree().process_frame
	return true


func _test_slice_open_yard_scene_smoke() -> bool:
	var path := "res://scenes/slice_open_yard.tscn"
	if not FileAccess.file_exists(path):
		push_error("slice open yard smoke: missing tscn")
		return false
	var ps: PackedScene = load(path) as PackedScene
	var inst: Control = ps.instantiate() as Control
	if inst == null:
		return false
	get_tree().root.add_child(inst)
	await get_tree().process_frame
	var yt: Label = inst.get_node_or_null("%YardTitle") as Label
	if yt == null or str(yt.text).strip_edges().is_empty():
		push_error("slice open yard smoke: title missing")
		inst.queue_free()
		await get_tree().process_frame
		return false
	var world: Node = inst.get_node_or_null(
			"SheetMargin/ManuscriptPanel/PageInner/VBox/SubViewportContainer/SubViewport/World"
	)
	if world == null:
		push_error("slice open yard smoke: world missing")
		inst.queue_free()
		await get_tree().process_frame
		return false
	inst.queue_free()
	await get_tree().process_frame
	return true


func _test_scene_transition_smoke() -> bool:
	## Crawl prototype will pop out to combat; prove hub + combat scenes load and mount headless.
	var hub_path := "res://scenes/hub_map.tscn"
	var combat_path := "res://scenes/combat.tscn"
	if not FileAccess.file_exists(hub_path) or not FileAccess.file_exists(combat_path):
		push_error("scene transition smoke: missing tscn path")
		return false
	var hub_ps: PackedScene = load(hub_path) as PackedScene
	var combat_ps: PackedScene = load(combat_path) as PackedScene
	if hub_ps == null or combat_ps == null:
		push_error("scene transition smoke: PackedScene load failed")
		return false
	var hub_inst: Node = hub_ps.instantiate()
	var combat_inst: Node = combat_ps.instantiate()
	if hub_inst == null or combat_inst == null:
		return false
	get_tree().root.add_child(hub_inst)
	await get_tree().process_frame
	hub_inst.queue_free()
	await get_tree().process_frame
	get_tree().root.add_child(combat_inst)
	await get_tree().process_frame
	combat_inst.queue_free()
	await get_tree().process_frame
	return true


func _test_crawl_overworld_scene_smoke() -> bool:
	## Phase C0 — fake-voxel crawl shell loads headless (DESIGN_CRAWL_VOXEL_SHADER_CI_M6 §1).
	var path := "res://scenes/crawl_overworld.tscn"
	if not FileAccess.file_exists(path):
		push_error("crawl overworld smoke: missing tscn")
		return false
	var ps: PackedScene = load(path) as PackedScene
	if ps == null:
		push_error("crawl overworld smoke: PackedScene load failed")
		return false
	var inst: Node = ps.instantiate()
	if inst == null:
		return false
	var label: Label = inst.get_node_or_null("CrawlShellLabel") as Label
	if label == null:
		push_error("crawl overworld smoke: CrawlShellLabel missing")
		inst.queue_free()
		await get_tree().process_frame
		return false
	get_tree().root.add_child(inst)
	await get_tree().process_frame
	inst.queue_free()
	await get_tree().process_frame
	return true


func _test_nav_test_yard_smoke() -> bool:
	var path := "res://scenes/nav_test_yard.tscn"
	if not FileAccess.file_exists(path):
		push_error("nav test yard smoke: missing tscn")
		return false
	var ps: PackedScene = load(path) as PackedScene
	if ps == null:
		return false
	var inst: Node = ps.instantiate()
	if inst == null:
		return false
	get_tree().root.add_child(inst)
	await get_tree().physics_frame
	await get_tree().physics_frame
	await get_tree().physics_frame
	var agent: NavigationAgent3D = inst.get_node_or_null("%NavAgent") as NavigationAgent3D
	var player: CharacterBody3D = inst.get_node_or_null("%Player") as CharacterBody3D
	if agent == null or player == null:
		push_error("nav test yard smoke: missing NavAgent or Player")
		inst.queue_free()
		await get_tree().process_frame
		return false
	# Short hop on the left island (gate closed): avoids long runs that stall in headless.
	var target := Vector3(-3.5, 0.05, 0.0)
	var d0: float = player.global_position.distance_to(target)
	agent.target_position = target
	for _j in 8:
		await get_tree().physics_frame
	var path_pts: PackedVector3Array = agent.get_current_navigation_path()
	if path_pts.size() < 2:
		push_error(
				"nav test yard smoke: path too short (%s) finished=%s"
				% [path_pts.size(), agent.is_navigation_finished()]
		)
		inst.queue_free()
		await get_tree().process_frame
		return false
	var best: float = d0
	for _i in 120:
		await get_tree().physics_frame
		best = minf(best, player.global_position.distance_to(target))
	var ok_move: bool = best <= d0 - 0.06
	if not ok_move:
		push_error(
				"nav test yard smoke: did not approach target (d0=%s best=%s finished=%s dist_tgt=%s next=%s pos=%s)"
				% [
						d0,
						best,
						agent.is_navigation_finished(),
						agent.distance_to_target(),
						agent.get_next_path_position(),
						player.global_position,
				]
		)
	inst.queue_free()
	await get_tree().process_frame
	return ok_move

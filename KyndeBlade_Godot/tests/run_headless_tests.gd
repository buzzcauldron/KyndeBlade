extends SceneTree
## Headless smoke tests for TDAD godot-parity-slice / CI (see docs/CI_GODOT_TESTS.md).

const _CombatScenarioRunner := preload("res://tests/combat_scenarios.gd")


func _init() -> void:
	process_frame.connect(_kickoff, CONNECT_ONE_SHOT)


func _kickoff() -> void:
	var ok := true
	ok = ok and _test_slice_locations_json()
	ok = ok and _test_unity_export_json()
	ok = ok and _test_save_roundtrip_service()
	ok = ok and _test_meta_flags_roundtrip()
	ok = ok and _test_settings_volume()
	ok = ok and _test_audio_buses()
	ok = ok and _test_encounter_resource()
	ok = ok and _test_hazard_stub_damage()
	ok = ok and _test_swing_pattern_static()
	ok = ok and _test_game_state_tour()
	ok = ok and _test_read_medieval_text_ids_roundtrip()
	ok = ok and _test_medieval_catalog_aggregate()
	ok = ok and _test_autosave_mirror_exists()
	ok = ok and _test_load_fallback_autosave()
	ok = ok and _test_legacy_demo_save_migrates()
	ok = ok and _test_medieval_list_granted_codes()
	if not ok:
		_finish(false)
		return
	SaveService.write_new_game()
	GameState.reset_from_new_game()
	var runner := _CombatScenarioRunner.new()
	ok = await runner.run_all(root)
	_finish(ok)


func _finish(passed: bool) -> void:
	print("HEADLESS_TESTS: ", "PASS" if passed else "FAIL")
	SaveService.write_new_game()
	GameState.reset_from_new_game()
	quit(0 if passed else 1)


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


func _test_save_roundtrip_service() -> bool:
	SaveService.write_new_game()
	var d: Dictionary = SaveService.load_save()
	if str(d.get("location_id", "")) != "tour":
		return false
	if not FileAccess.file_exists("user://kyndeblade_save.cfg"):
		return false
	SaveService.save_progress("fayre_felde", true)
	d = SaveService.load_save()
	return str(d.get("location_id", "")) == "fayre_felde" and bool(d.get("fair_field_cleared", false))


func _test_meta_flags_roundtrip() -> bool:
	SaveService.write_new_game()
	SaveService.save_progress("tour", false, 2, true)
	var d: Dictionary = SaveService.load_save()
	if int(d.get("ethical_misstep_count", -1)) != 2:
		return false
	if bool(d.get("has_ever_had_hunger", false)) != true:
		return false
	GameState.apply_loaded(d)
	return GameState.ethical_misstep_count == 2 and GameState.has_ever_had_hunger


func _test_settings_volume() -> bool:
	SaveService.save_master_volume(0.42)
	return is_equal_approx(SaveService.load_master_volume(), 0.42)


func _test_audio_buses() -> bool:
	return AudioServer.get_bus_index("Music") >= 0 and AudioServer.get_bus_index("SFX") >= 0


func _test_encounter_resource() -> bool:
	var enc := load("res://data/encounter_fair_field.tres") as EncounterDef
	if enc == null:
		return false
	return (
			enc.enemy_id == "false"
			and enc.encounter_id == "fayre_felde"
			and str(enc.enemy_display_name).findn("langage") >= 0
	)


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
	GameState.reset_from_new_game()
	var ok1 := GameState.current_location_id == "tour" and GameState.fair_field_cleared == false
	GameState.read_medieval_text_ids.append("tower_vista")
	GameState.reset_from_new_game()
	var ok2 := GameState.read_medieval_text_ids.is_empty()
	return ok1 and ok2


func _test_read_medieval_text_ids_roundtrip() -> bool:
	SaveService.write_new_game()
	SaveService.save_progress("tour", false, 0, false, "tower_vista|FayreFelde")
	var d: Dictionary = SaveService.load_save()
	if str(d.get("read_medieval_text_ids", "")) != "tower_vista|FayreFelde":
		return false
	GameState.apply_loaded(d)
	return (
			GameState.read_medieval_text_ids.size() == 2
			and GameState.read_medieval_text_ids.has("tower_vista")
			and GameState.read_medieval_text_ids.has("FayreFelde")
	)


func _test_medieval_catalog_aggregate() -> bool:
	GameState.reset_from_new_game()
	GameState.read_medieval_text_ids = PackedStringArray(["tower_vista"])
	var t: Dictionary = MedievalTextCatalog.aggregate_totals(GameState.read_medieval_text_ids)
	# dreamer_ledger_stride: +5 stam cost, +4 damage
	if not is_equal_approx(float(t.get("strike_stamina_cost_delta", 0.0)), 5.0):
		return false
	if not is_equal_approx(float(t.get("strike_damage_delta", 0.0)), 4.0):
		return false
	GameState.reset_from_new_game()
	return true


func _test_autosave_mirror_exists() -> bool:
	SaveService.write_new_game()
	return FileAccess.file_exists("user://kyndeblade_save.cfg") and FileAccess.file_exists(
			"user://kyndeblade_autosave.cfg"
	)


func _test_load_fallback_autosave() -> bool:
	SaveService.write_new_game()
	SaveService.save_progress("fayre_felde", true, 1, false, "")
	var d := DirAccess.open("user://")
	if d == null:
		return false
	if d.file_exists("kyndeblade_save.cfg"):
		d.remove("kyndeblade_save.cfg")
	var loaded: Dictionary = SaveService.load_save()
	var ok: bool = str(loaded.get("location_id", "")) == "fayre_felde" and bool(loaded.get("fair_field_cleared", false))
	SaveService.write_new_game()
	GameState.reset_from_new_game()
	return ok


func _test_legacy_demo_save_migrates() -> bool:
	SaveService.delete_save()
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
	var loaded: Dictionary = SaveService.load_save()
	if int(loaded.get("ethical_misstep_count", -1)) != 7:
		SaveService.delete_save()
		return false
	var has_new: bool = FileAccess.file_exists("user://kyndeblade_save.cfg")
	var legacy_removed: bool = not FileAccess.file_exists("user://kyndeblade_demo_save.cfg")
	SaveService.delete_save()
	return has_new and legacy_removed


func _test_medieval_list_granted_codes() -> bool:
	var codes := MedievalTextCatalog.list_granted_codes_in_order(PackedStringArray(["tower_vista", "FayreFelde"]))
	return codes.size() == 2 and str(codes[0]) == "dreamer_ledger_stride" and str(codes[1]) == "crowd_surge"

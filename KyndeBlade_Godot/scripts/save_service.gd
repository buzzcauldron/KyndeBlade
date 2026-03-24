extends Node
## Persists save + settings under user:// (**Steam / retail** paths).
## **Primary slot** + **autosave mirror** (same schema). One-time import from legacy **`kyndeblade_demo_*`** demo-era filenames.

const SAVE_PATH := "user://kyndeblade_save.cfg"
const SAVE_FILE := "kyndeblade_save.cfg"
const AUTOSAVE_PATH := "user://kyndeblade_autosave.cfg"
const AUTOSAVE_FILE := "kyndeblade_autosave.cfg"
const SETTINGS_PATH := "user://kyndeblade_settings.cfg"
const SETTINGS_FILE := "kyndeblade_settings.cfg"
## Pre–Steam-build demo package (Godot); migrated automatically on first load.
const LEGACY_SAVE_PATH := "user://kyndeblade_demo_save.cfg"
const LEGACY_SAVE_FILE := "kyndeblade_demo_save.cfg"
const LEGACY_SETTINGS_PATH := "user://kyndeblade_demo_settings.cfg"
const LEGACY_SETTINGS_FILE := "kyndeblade_demo_settings.cfg"
const SAVE_VERSION := 2
const PERIODIC_AUTOSAVE_SEC := 90.0

signal save_changed


func _ready() -> void:
	var t := Timer.new()
	t.wait_time = PERIODIC_AUTOSAVE_SEC
	t.timeout.connect(_on_periodic_autosave)
	t.autostart = true
	add_child(t)
	var w: Window = get_tree().root.get_window()
	if w != null:
		w.focus_exited.connect(_on_window_focus_exited)


func _on_periodic_autosave() -> void:
	if not has_save():
		return
	GameState.sync_to_save()


func _on_window_focus_exited() -> void:
	if not has_save():
		return
	GameState.sync_to_save()


func has_save() -> bool:
	return (
			FileAccess.file_exists(SAVE_PATH)
			or FileAccess.file_exists(AUTOSAVE_PATH)
			or FileAccess.file_exists(LEGACY_SAVE_PATH)
	)


func delete_save() -> void:
	var d := DirAccess.open("user://")
	if d != null:
		for f: String in [SAVE_FILE, AUTOSAVE_FILE, LEGACY_SAVE_FILE]:
			if d.file_exists(f):
				d.remove(f)
	save_changed.emit()


func write_new_game() -> void:
	_remove_legacy_demo_save_if_present()
	var cfg := ConfigFile.new()
	cfg.set_value("save", "version", SAVE_VERSION)
	cfg.set_value("save", "location_id", "tour")
	cfg.set_value("save", "fair_field_cleared", false)
	cfg.set_value("save", "ethical_misstep_count", 0)
	cfg.set_value("save", "has_ever_had_hunger", false)
	cfg.set_value("save", "read_medieval_text_ids", "")
	cfg.set_value("save", "piers_text_edition", "B")
	cfg.set_value("save", "narrative_phase_id", "visio")
	cfg.set_value("save", "location_visit_counts", "")
	cfg.set_value("save", "fair_field_return_count", 0)
	cfg.set_value("save", "dream_iteration", 0)
	_persist_save(cfg)
	save_changed.emit()


func _remove_legacy_demo_save_if_present() -> void:
	var d := DirAccess.open("user://")
	if d != null and d.file_exists(LEGACY_SAVE_FILE):
		d.remove(LEGACY_SAVE_FILE)


func save_progress(
		location_id: String,
		fair_field_cleared: bool,
		ethical_misstep_count: int = 0,
		has_ever_had_hunger: bool = false,
		read_medieval_text_ids_pipe: String = "",
		piers_text_edition: String = "B",
		narrative_phase_id: String = "visio",
		location_visit_counts_pipe: String = "",
		fair_field_return_count: int = 0,
		dream_iteration: int = 0,
) -> void:
	var cfg := ConfigFile.new()
	var src: String = _try_load_save_cfg(cfg)
	if src.is_empty():
		cfg.set_value("save", "version", SAVE_VERSION)
	cfg.set_value("save", "version", SAVE_VERSION)
	cfg.set_value("save", "location_id", location_id)
	cfg.set_value("save", "fair_field_cleared", fair_field_cleared)
	cfg.set_value("save", "ethical_misstep_count", ethical_misstep_count)
	cfg.set_value("save", "has_ever_had_hunger", has_ever_had_hunger)
	cfg.set_value("save", "read_medieval_text_ids", read_medieval_text_ids_pipe)
	cfg.set_value("save", "piers_text_edition", piers_text_edition)
	cfg.set_value("save", "narrative_phase_id", narrative_phase_id)
	cfg.set_value("save", "location_visit_counts", location_visit_counts_pipe)
	cfg.set_value("save", "fair_field_return_count", fair_field_return_count)
	cfg.set_value("save", "dream_iteration", dream_iteration)
	cfg.set_value("save", "last_save_unix", int(Time.get_unix_time_from_system()))
	_persist_save(cfg)
	if src == LEGACY_SAVE_PATH:
		_remove_legacy_demo_save_if_present()
	save_changed.emit()


func _try_load_save_cfg(cfg: ConfigFile) -> String:
	if cfg.load(SAVE_PATH) == OK:
		return SAVE_PATH
	if cfg.load(AUTOSAVE_PATH) == OK:
		return AUTOSAVE_PATH
	if cfg.load(LEGACY_SAVE_PATH) == OK:
		return LEGACY_SAVE_PATH
	return ""


func _persist_save(cfg: ConfigFile) -> void:
	var e1 := cfg.save(SAVE_PATH)
	if e1 != OK:
		push_error("SaveService: failed primary save: %s" % e1)
	var e2 := cfg.save(AUTOSAVE_PATH)
	if e2 != OK:
		push_error("SaveService: failed autosave mirror: %s" % e2)


func load_save() -> Dictionary:
	var cfg := ConfigFile.new()
	var src: String = _try_load_save_cfg(cfg)
	if src.is_empty():
		return {}
	var ver: int = int(cfg.get_value("save", "version", 0))
	if ver != SAVE_VERSION:
		pass
	var out := {
		"location_id": str(cfg.get_value("save", "location_id", "tour")),
		"fair_field_cleared": bool(cfg.get_value("save", "fair_field_cleared", false)),
		"ethical_misstep_count": int(cfg.get_value("save", "ethical_misstep_count", 0)),
		"has_ever_had_hunger": bool(cfg.get_value("save", "has_ever_had_hunger", false)),
		"read_medieval_text_ids": str(cfg.get_value("save", "read_medieval_text_ids", "")),
		"piers_text_edition": str(cfg.get_value("save", "piers_text_edition", "B")),
		"narrative_phase_id": str(cfg.get_value("save", "narrative_phase_id", "visio")),
		"location_visit_counts": str(cfg.get_value("save", "location_visit_counts", "")),
		"fair_field_return_count": int(cfg.get_value("save", "fair_field_return_count", 0)),
		"dream_iteration": int(cfg.get_value("save", "dream_iteration", 0)),
	}
	if src == LEGACY_SAVE_PATH:
		_persist_save(cfg)
		_remove_legacy_demo_save_if_present()
	return out


func load_master_volume() -> float:
	var cfg := ConfigFile.new()
	var migrated := false
	if cfg.load(SETTINGS_PATH) != OK:
		if cfg.load(LEGACY_SETTINGS_PATH) == OK:
			migrated = true
		else:
			return 1.0
	var linear := clampf(float(cfg.get_value("audio", "master_linear", 1.0)), 0.0, 1.0)
	if migrated:
		cfg.save(SETTINGS_PATH)
		var d := DirAccess.open("user://")
		if d != null and d.file_exists(LEGACY_SETTINGS_FILE):
			d.remove(LEGACY_SETTINGS_FILE)
	return linear


func save_master_volume(linear: float) -> void:
	var cfg := ConfigFile.new()
	cfg.load(SETTINGS_PATH)
	cfg.set_value("audio", "master_linear", clampf(linear, 0.0, 1.0))
	cfg.save(SETTINGS_PATH)
	_apply_volume()


func load_fullscreen() -> bool:
	var cfg := ConfigFile.new()
	var migrated := false
	if cfg.load(SETTINGS_PATH) != OK:
		if cfg.load(LEGACY_SETTINGS_PATH) == OK:
			migrated = true
		else:
			return false
	var on := bool(cfg.get_value("display", "fullscreen", false))
	if migrated:
		cfg.save(SETTINGS_PATH)
		var d := DirAccess.open("user://")
		if d != null and d.file_exists(LEGACY_SETTINGS_FILE):
			d.remove(LEGACY_SETTINGS_FILE)
	return on


func save_fullscreen(on: bool) -> void:
	var cfg := ConfigFile.new()
	cfg.load(SETTINGS_PATH)
	cfg.set_value("display", "fullscreen", on)
	cfg.save(SETTINGS_PATH)
	DisplayServer.window_set_mode(
			DisplayServer.WINDOW_MODE_FULLSCREEN if on else DisplayServer.WINDOW_MODE_WINDOWED
	)


## When **false**, enemy turn applies damage immediately after telegraph timers (legacy). When **true**, enemy strike opens a reaction window first (parity: Unity `EnableRealTimeDefenseWindows`).
func load_enable_real_time_defense_on_enemy_turn() -> bool:
	var cfg := ConfigFile.new()
	if cfg.load(SETTINGS_PATH) != OK:
		if cfg.load(LEGACY_SETTINGS_PATH) != OK:
			return true
	var v: Variant = cfg.get_value("combat", "enable_real_time_defense_on_enemy_turn", true)
	return bool(v)


func save_enable_real_time_defense_on_enemy_turn(on: bool) -> void:
	var cfg := ConfigFile.new()
	cfg.load(SETTINGS_PATH)
	cfg.set_value("combat", "enable_real_time_defense_on_enemy_turn", on)
	cfg.save(SETTINGS_PATH)


func apply_stored_settings() -> void:
	_apply_volume()
	if load_fullscreen():
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)


func _apply_volume() -> void:
	var linear := load_master_volume()
	var db: float = -80.0 if linear <= 0.0001 else 20.0 * log(linear) / log(10.0)
	AudioServer.set_bus_volume_db(AudioServer.get_bus_index("Master"), db)

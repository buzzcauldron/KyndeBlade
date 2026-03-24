extends RefCounted
## Deterministic **combat scenario** suite for headless CI.
## Mirrors `PLAYABLE_SLICE.md` / False encounter combat loop + `CombatManager`
## (strike, enemy turn, dodge/parry windows).
## Requires `CombatManager.use_instant_resolution_for_tests` so turns do not depend on wall-clock
## timers.

const PHeadlessState := preload("res://scripts/bootstrap/game_state.gd")


func _game_state(tree: SceneTree) -> PHeadlessState:
	return tree.root.get_node("/root/GameState") as PHeadlessState


func run_all(p_root: Window) -> bool:
	_game_state(p_root.get_tree()).reset_from_new_game()
	# One frame: CombatManager._ready + _start_combat after add_child
	await p_root.get_tree().process_frame
	var ok := true
	ok = _cs_step(
			"victory_false_by_strikes_only", await victory_false_by_strikes_only(p_root), ok
	)
	ok = _cs_step(
			"default_encounter_loads_when_null",
			await default_encounter_loads_when_null(p_root),
			ok,
	)
	ok = _cs_step("dodge_mitigates_first_swing", await dodge_mitigates_first_swing(p_root), ok)
	ok = _cs_step("parry_reduces_swing_damage", await parry_reduces_swing_damage(p_root), ok)
	ok = _cs_step("parry_window_ms_stays_in_band", parry_window_ms_stays_in_band(), ok)
	ok = _cs_step("feint_chips_if_you_defend", await feint_chips_if_you_defend(p_root), ok)
	ok = _cs_step(
			"enemy_turn_reaction_dodge_partial",
			await enemy_turn_reaction_dodge_partial(p_root),
			ok,
	)
	ok = _cs_step(
			"enemy_turn_reaction_parry_partial_and_riposte",
			await enemy_turn_reaction_parry_partial_and_riposte(p_root),
			ok,
	)
	ok = _cs_step("defeat_on_brutal_enemy_turn", await defeat_on_brutal_enemy_turn(p_root), ok)
	ok = _cs_step(
			"strike_ignored_during_defensive_window",
			await strike_ignored_during_defensive_window(p_root),
			ok,
	)
	ok = _cs_step(
			"strike_fails_when_stamina_too_low", await strike_fails_when_stamina_too_low(p_root), ok
	)
	ok = _cs_step(
			"misstep_inverts_first_defensive_window",
			await misstep_inverts_first_defensive_window(p_root),
			ok,
	)
	return ok


func _cs_step(name: String, step_ok: bool, ok_so_far: bool) -> bool:
	if not step_ok:
		printerr("COMBAT_SCENARIOS step failed: ", name)
	return ok_so_far and step_ok


func _spawn_cm(p_root: Window, enc: EncounterDef = null) -> CombatManager:
	var c := CombatManager.new()
	c.use_instant_resolution_for_tests = true
	if enc != null:
		c.encounter = enc
	p_root.add_child(c)
	return c


func default_encounter_loads_when_null(p_root: Window) -> bool:
	var c := _spawn_cm(p_root, null)
	await p_root.get_tree().process_frame
	var ok := c.encounter != null and c.encounter.encounter_id == "fayre_felde"
	ok = ok and is_equal_approx(c.enemy_max_hp, 80.0)
	ok = ok and is_equal_approx(c.player_strike_damage, 15.0)
	_fail_cleanup(c)
	return ok


func victory_false_by_strikes_only(p_root: Window) -> bool:
	var c := _spawn_cm(p_root, null)
	await p_root.get_tree().process_frame
	if c.state != CombatManager.State.WAITING_PLAYER:
		_fail_cleanup(c)
		return false
	if not is_equal_approx(c.enemy_max_hp, 80.0):
		_fail_cleanup(c)
		return false
	# 5 strikes leave 5 HP; 6th kills before enemy counter
	for i in 6:
		if c.state != CombatManager.State.WAITING_PLAYER:
			_fail_cleanup(c)
			return false
		c.player_strike()
	var ok: bool = c.state == CombatManager.State.ENDED and is_equal_approx(c.enemy_hp, 0.0)
	_fail_cleanup(c)
	return ok


func dodge_mitigates_first_swing(p_root: Window) -> bool:
	var c := _spawn_cm(p_root, null)
	await p_root.get_tree().process_frame
	c.player_dodge()
	if not is_equal_approx(c.window_duration, 1.5):
		_fail_cleanup(c)
		return false
	if c.state != CombatManager.State.REAL_TIME_WINDOW:
		_fail_cleanup(c)
		return false
	if not c.is_enemy_swing_real():
		_fail_cleanup(c)
		return false
	c.tick_window(2.0)
	# Real swing + dodge: partial chip (20 × DODGE_REAL_SWING_FRACTION = 12).
	if not is_equal_approx(c.player_hp, 88.0):
		_fail_cleanup(c)
		return false
	if c.state != CombatManager.State.WAITING_PLAYER:
		_fail_cleanup(c)
		return false
	_fail_cleanup(c)
	return true


func parry_reduces_swing_damage(p_root: Window) -> bool:
	var c := _spawn_cm(p_root, null)
	await p_root.get_tree().process_frame
	var want_dur: float = PlayerMovesetModifiers.parry_window_ms() / 1000.0
	c.player_parry()
	if not is_equal_approx(c.window_duration, want_dur):
		_fail_cleanup(c)
		return false
	if not c.is_enemy_swing_real():
		_fail_cleanup(c)
		return false
	c.tick_window(2.0)
	# Real swing + parry: 20 × PARRY_INCOMING_FRACTION chip; riposte 0.3× on first window index.
	const RIPOSTE_MULT_FIRST := 0.3
	var want_enemy := c.enemy_max_hp - c.get_strike_damage_preview() * RIPOSTE_MULT_FIRST
	if not is_equal_approx(c.player_hp, 94.0):
		_fail_cleanup(c)
		return false
	if not is_equal_approx(c.enemy_hp, want_enemy):
		_fail_cleanup(c)
		return false
	_fail_cleanup(c)
	return true


func feint_chips_if_you_defend(p_root: Window) -> bool:
	var c := _spawn_cm(p_root, null)
	await p_root.get_tree().process_frame
	c.player_dodge()
	c.tick_window(2.0)
	if not is_equal_approx(c.player_hp, 88.0):
		_fail_cleanup(c)
		return false
	c.player_strike()
	if c.state != CombatManager.State.WAITING_PLAYER:
		_fail_cleanup(c)
		return false
	# No defensive reaction on instant enemy phase: full enemy_turn_damage (18).
	if not is_equal_approx(c.player_hp, 70.0):
		_fail_cleanup(c)
		return false
	c.player_dodge()
	if c.is_enemy_swing_real():
		_fail_cleanup(c)
		return false
	c.tick_window(2.0)
	# Feint + wasted dodge: 5 chip
	if not is_equal_approx(c.player_hp, 65.0):
		_fail_cleanup(c)
		return false
	_fail_cleanup(c)
	return true


func defeat_on_brutal_enemy_turn(p_root: Window) -> bool:
	var deadly := EncounterDef.new()
	deadly.encounter_id = "test_brutal"
	deadly.enemy_id = "false"
	deadly.enemy_max_hp = 80.0
	deadly.player_strike_damage = 15.0
	deadly.strike_stamina_cost = 15.0
	deadly.enemy_turn_damage = 200.0
	var c := _spawn_cm(p_root, deadly)
	await p_root.get_tree().process_frame
	c.player_strike()
	var ok: bool = c.state == CombatManager.State.ENDED and c.player_hp <= 0.0
	_fail_cleanup(c)
	return ok


func strike_ignored_during_defensive_window(p_root: Window) -> bool:
	var c := _spawn_cm(p_root, null)
	await p_root.get_tree().process_frame
	var hp_before_enemy := c.enemy_hp
	c.player_dodge()
	c.player_strike()
	if c.enemy_hp != hp_before_enemy:
		_fail_cleanup(c)
		return false
	c.tick_window(2.0)
	_fail_cleanup(c)
	return true


func misstep_inverts_first_defensive_window(p_root: Window) -> bool:
	var gs := _game_state(p_root.get_tree())
	gs.ethical_misstep_count = 1
	var c := _spawn_cm(p_root, null)
	await p_root.get_tree().process_frame
	c.player_dodge()
	var ok := c.state == CombatManager.State.REAL_TIME_WINDOW
	ok = ok and is_equal_approx(c.window_duration, 1.5)
	ok = ok and not c.is_enemy_swing_real()
	c.tick_window(2.0)
	# Feint + wasted dodge: 5 chip
	ok = ok and is_equal_approx(c.player_hp, 95.0)
	_fail_cleanup(c)
	gs.ethical_misstep_count = 0
	return ok


func strike_fails_when_stamina_too_low(p_root: Window) -> bool:
	var enc := EncounterDef.new()
	enc.encounter_id = "test_stam"
	enc.enemy_id = "false"
	enc.enemy_max_hp = 9999.0
	enc.player_strike_damage = 1.0
	# After a strike, instant enemy phase restores +15 stamina; cost must stay > post-regen stamina
	# for the second strike to no-op.
	enc.strike_stamina_cost = 58.0
	enc.enemy_turn_damage = 0.0
	var c := _spawn_cm(p_root, enc)
	await p_root.get_tree().process_frame
	c.player_strike()
	if not is_equal_approx(c.enemy_hp, 9998.0):
		_fail_cleanup(c)
		return false
	# Stamina after strike includes instant enemy-phase regen (+15).
	if not is_equal_approx(c.player_stamina, 57.0):
		_fail_cleanup(c)
		return false
	c.player_strike()
	if not is_equal_approx(c.enemy_hp, 9998.0):
		_fail_cleanup(c)
		return false
	_fail_cleanup(c)
	return true


func enemy_turn_reaction_dodge_partial(p_root: Window) -> bool:
	var c := CombatManager.new()
	c.use_instant_resolution_for_tests = true
	c.force_enemy_turn_reaction_window_in_tests = true
	p_root.add_child(c)
	await p_root.get_tree().process_frame
	if c.state != CombatManager.State.WAITING_PLAYER:
		_fail_cleanup(c)
		return false
	c.player_strike()
	if c.state != CombatManager.State.REAL_TIME_WINDOW:
		_fail_cleanup(c)
		return false
	if not c.is_enemy_swing_real():
		_fail_cleanup(c)
		return false
	c.player_dodge()
	c.tick_window(99.0)
	# enemy_turn_damage 18 × DODGE_REAL_SWING_FRACTION
	var want_hp := 100.0 - 18.0 * CombatManager.DODGE_REAL_SWING_FRACTION
	if not is_equal_approx(c.player_hp, want_hp):
		_fail_cleanup(c)
		return false
	if c.state != CombatManager.State.WAITING_PLAYER:
		_fail_cleanup(c)
		return false
	_fail_cleanup(c)
	return true


func enemy_turn_reaction_parry_partial_and_riposte(p_root: Window) -> bool:
	var c := CombatManager.new()
	c.use_instant_resolution_for_tests = true
	c.force_enemy_turn_reaction_window_in_tests = true
	p_root.add_child(c)
	await p_root.get_tree().process_frame
	c.player_strike()
	if not c.is_enemy_swing_real():
		_fail_cleanup(c)
		return false
	var enemy_after_strike := c.enemy_hp
	c.player_parry()
	c.tick_window(99.0)
	var riposte_mult := 0.3
	var want_player := 100.0 - 18.0 * CombatManager.PARRY_INCOMING_FRACTION
	var want_enemy := enemy_after_strike - c.get_strike_damage_preview() * riposte_mult
	if not is_equal_approx(c.player_hp, want_player):
		_fail_cleanup(c)
		return false
	if not is_equal_approx(c.enemy_hp, want_enemy):
		_fail_cleanup(c)
		return false
	_fail_cleanup(c)
	return true


func parry_window_ms_stays_in_band() -> bool:
	var tree := Engine.get_main_loop() as SceneTree
	if tree == null:
		return false
	var gs := _game_state(tree)
	gs.reset_from_new_game()
	for i in 80:
		gs.ethical_misstep_count = i
		gs.has_ever_had_hunger = (i % 3) != 0
		var ms: int = PlayerMovesetModifiers.parry_window_ms()
		if ms < 170 or ms > 230:
			gs.reset_from_new_game()
			return false
	gs.reset_from_new_game()
	return true


func _fail_cleanup(c: CombatManager) -> void:
	if is_instance_valid(c):
		c.queue_free()

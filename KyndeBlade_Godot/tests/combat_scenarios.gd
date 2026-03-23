extends RefCounted
## Deterministic **combat scenario** suite for headless CI.
## Mirrors `PLAYABLE_SLICE.md` / False encounter combat loop + `CombatManager` (strike, enemy turn, dodge/parry windows).
## Requires `CombatManager.use_instant_resolution_for_tests` so turns do not depend on wall-clock timers.


func run_all(p_root: Window) -> bool:
	GameState.reset_from_new_game()
	# One frame: CombatManager._ready + _start_combat after add_child
	await p_root.get_tree().process_frame
	var ok := true
	ok = ok and await victory_false_by_strikes_only(p_root)
	ok = ok and await dodge_mitigates_first_swing(p_root)
	ok = ok and await parry_reduces_swing_damage(p_root)
	ok = ok and parry_window_ms_stays_in_band()
	ok = ok and await feint_chips_if_you_defend(p_root)
	ok = ok and await defeat_on_brutal_enemy_turn(p_root)
	ok = ok and await strike_ignored_during_defensive_window(p_root)
	ok = ok and await strike_fails_when_stamina_too_low(p_root)
	ok = ok and await misstep_inverts_first_defensive_window(p_root)
	return ok


func _spawn_cm(p_root: Window, enc: EncounterDef = null) -> CombatManager:
	var c := CombatManager.new()
	c.use_instant_resolution_for_tests = true
	if enc != null:
		c.encounter = enc
	p_root.add_child(c)
	return c


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
	c.player_parry()
	if not is_equal_approx(c.window_duration, 1.0):
		_fail_cleanup(c)
		return false
	if not c.is_enemy_swing_real():
		_fail_cleanup(c)
		return false
	c.tick_window(2.0)
	# 20 * 0.3 = 6 mitigated parry
	if not is_equal_approx(c.player_hp, 94.0):
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
	if not is_equal_approx(c.player_hp, 70.0):
		_fail_cleanup(c)
		return false
	c.player_dodge()
	if c.is_enemy_swing_real():
		_fail_cleanup(c)
		return false
	c.tick_window(2.0)
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
	GameState.ethical_misstep_count = 1
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
	GameState.ethical_misstep_count = 0
	return ok


func strike_fails_when_stamina_too_low(p_root: Window) -> bool:
	var enc := EncounterDef.new()
	enc.encounter_id = "test_stam"
	enc.enemy_id = "false"
	enc.enemy_max_hp = 9999.0
	enc.player_strike_damage = 1.0
	enc.strike_stamina_cost = 51.0
	enc.enemy_turn_damage = 0.0
	var c := _spawn_cm(p_root, enc)
	await p_root.get_tree().process_frame
	c.player_strike()
	if not is_equal_approx(c.enemy_hp, 9998.0):
		_fail_cleanup(c)
		return false
	if not is_equal_approx(c.player_stamina, 49.0):
		_fail_cleanup(c)
		return false
	c.player_strike()
	if not is_equal_approx(c.enemy_hp, 9998.0):
		_fail_cleanup(c)
		return false
	_fail_cleanup(c)
	return true


func parry_window_ms_stays_in_band() -> bool:
	GameState.reset_from_new_game()
	for i in 80:
		GameState.ethical_misstep_count = i
		GameState.has_ever_had_hunger = (i % 3) != 0
		var ms: int = PlayerMovesetModifiers.parry_window_ms()
		if ms < 170 or ms > 230:
			GameState.reset_from_new_game()
			return false
	GameState.reset_from_new_game()
	return true


func _fail_cleanup(c: CombatManager) -> void:
	if is_instance_valid(c):
		c.queue_free()

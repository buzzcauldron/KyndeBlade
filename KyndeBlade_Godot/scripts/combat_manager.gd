class_name CombatManager
extends Node
## Turn-based combat + real-time dodge/parry window. Deterministic enemy telegraph (no rand).
## Parry window duration is **170–230 ms** (see `PlayerMovesetModifiers.parry_window_ms()`); dodge window stays longer for readability.

signal turn_changed
signal combat_ended(victory: bool)
signal stats_changed(player_hp: float, player_max: float, enemy_hp: float, enemy_max: float, stamina: float, stamina_max: float)
## Emitted when a dodge/parry defensive window opens; `is_real_swing` is false for a feint.
signal defensive_window_started(is_real_swing: bool)
## Dreamer (Wille) presentation: strike / dodge / parry committed after stamina gate passes.
signal dreamer_move_committed(move_kind: DreamerMoveKind)

enum State { WAITING_PLAYER, EXECUTING, REAL_TIME_WINDOW, ENEMY_TURN, ENDED }

enum DreamerMoveKind { STRIKE, DODGE, PARRY }

## Loaded from `data/encounter_fair_field.tres` if unset (parity: demo-fayre-felde-encounter-wired).
@export var encounter: EncounterDef

## When **true**, strike → enemy phases resolve immediately (no `create_timer`). Used by headless combat scenario tests only.
@export var use_instant_resolution_for_tests: bool = false

## Shifts the deterministic feint pattern (odd `GameState.ethical_misstep_count` inverts first read). Tests leave at 0.
@export var feint_pattern_offset: int = 0

var state: State = State.ENDED
var window_remaining: float = 0.0
## Set when a defensive window opens; Unity `RealTimeWindowDuration` equivalent (presentation / ParryDodge eye phase).
var window_duration: float = 0.0
var player_hp: float = 100.0
var player_max_hp: float = 100.0
var player_stamina: float = 100.0
var player_max_stamina: float = 100.0
var enemy_hp: float = 80.0
var enemy_max_hp: float = 80.0
var player_dodging: bool = false
var player_parrying: bool = false

## Next defensive window: if true, enemy committed a real swing (mitigate with dodge/parry).
var _enemy_will_hit: bool = true
var _defense_windows_resolved: int = 0


func is_enemy_swing_real() -> bool:
	return _enemy_will_hit


## `t` in [0,1] for phased UI: 0 = window start, 1 = window end (matches Unity `1 - remaining/duration`).
func window_phase_t() -> float:
	if state != State.REAL_TIME_WINDOW or window_duration <= 0.0001:
		return 0.0
	return clampf(1.0 - window_remaining / window_duration, 0.0, 1.0)


static func enemy_swing_is_hit_for_window_index(resolved_count: int, pattern_offset: int = 0) -> bool:
	return ((resolved_count + pattern_offset) % 2) == 0


## W6: Unity applies Piers hazards each turn; this build returns 0 until hazards are ported (see PARITY_GAPS.md).
func apply_piers_hazard_damage() -> float:
	return 0.0


func _ready() -> void:
	if encounter == null:
		encounter = load("res://data/encounter_fair_field.tres") as EncounterDef
	var feint_from_reads: int = PlayerMovesetModifiers.feint_pattern_offset_delta()
	feint_pattern_offset = posmod(int(GameState.ethical_misstep_count) + feint_from_reads, 2)
	if use_instant_resolution_for_tests:
		_start_combat()
	else:
		call_deferred("_deferred_start_after_book_intro")


func _deferred_start_after_book_intro() -> void:
	var intro: Node = get_parent().get_node_or_null("BookIntroLayer/BookIntroRoot")
	if intro != null and intro.has_signal("intro_finished"):
		intro.intro_finished.connect(_start_combat, CONNECT_ONE_SHOT)
	else:
		_start_combat()


func _start_combat() -> void:
	if encounter != null:
		enemy_max_hp = encounter.enemy_max_hp
	player_hp = player_max_hp
	player_stamina = player_max_stamina
	enemy_hp = enemy_max_hp
	_defense_windows_resolved = 0
	_enemy_will_hit = true
	state = State.WAITING_PLAYER
	_emit_stats()
	turn_changed.emit()


func _emit_stats() -> void:
	stats_changed.emit(player_hp, player_max_hp, enemy_hp, enemy_max_hp, player_stamina, player_max_stamina)


func _strike_stamina_cost() -> float:
	var base: float = encounter.strike_stamina_cost if encounter else 15.0
	return maxf(1.0, base + PlayerMovesetModifiers.strike_stamina_cost_delta())


func _strike_damage() -> float:
	var base: float = encounter.player_strike_damage if encounter else 15.0
	return maxf(1.0, base + PlayerMovesetModifiers.strike_damage_delta())


func _dodge_stamina_cost() -> float:
	return maxf(1.0, 20.0 + PlayerMovesetModifiers.dodge_stamina_flat_extra())


func get_strike_stamina_cost() -> float:
	return _strike_stamina_cost()


func get_strike_damage_preview() -> float:
	return _strike_damage()


func get_dodge_stamina_cost() -> float:
	return _dodge_stamina_cost()


func get_parry_stamina_cost() -> float:
	return 25.0


func player_strike() -> void:
	var st_cost: float = _strike_stamina_cost()
	var dmg: float = _strike_damage()
	if state != State.WAITING_PLAYER or player_stamina < st_cost:
		return
	player_stamina -= st_cost
	enemy_hp = maxf(0, enemy_hp - dmg)
	dreamer_move_committed.emit(DreamerMoveKind.STRIKE)
	_emit_stats()
	if enemy_hp <= 0:
		state = State.ENDED
		combat_ended.emit(true)
		return
	state = State.ENEMY_TURN
	turn_changed.emit()
	if use_instant_resolution_for_tests:
		_run_enemy_damage_phase()
	else:
		await get_tree().create_timer(1.0).timeout
		await get_tree().create_timer(0.5).timeout
		_run_enemy_damage_phase()


func player_dodge() -> void:
	var dodge_cost: float = _dodge_stamina_cost()
	if state != State.WAITING_PLAYER or player_stamina < dodge_cost:
		return
	player_stamina -= dodge_cost
	player_dodging = true
	player_parrying = false
	dreamer_move_committed.emit(DreamerMoveKind.DODGE)
	_begin_defensive_window(1.5)


func player_parry() -> void:
	if state != State.WAITING_PLAYER or player_stamina < 25:
		return
	player_stamina -= 25
	player_parrying = true
	player_dodging = false
	dreamer_move_committed.emit(DreamerMoveKind.PARRY)
	_begin_defensive_window(PlayerMovesetModifiers.parry_window_ms() / 1000.0)


func _begin_defensive_window(duration: float) -> void:
	# Deterministic: alternate real swing vs feint for QA reproducibility.
	_enemy_will_hit = enemy_swing_is_hit_for_window_index(_defense_windows_resolved, feint_pattern_offset)
	window_duration = duration
	window_remaining = duration
	state = State.REAL_TIME_WINDOW
	_emit_stats()
	turn_changed.emit()
	defensive_window_started.emit(_enemy_will_hit)


func tick_window(delta: float) -> void:
	if state != State.REAL_TIME_WINDOW:
		return
	window_remaining -= delta
	if window_remaining <= 0:
		_resolve_window()


func _resolve_window() -> void:
	window_duration = 0.0
	window_remaining = 0.0
	var was_dodging: bool = player_dodging
	var was_parrying: bool = player_parrying
	player_dodging = false
	player_parrying = false
	state = State.EXECUTING

	if _enemy_will_hit:
		var mitigated: bool = was_dodging or was_parrying
		var dmg: float = 12.0 if mitigated else 20.0
		if was_parrying and mitigated:
			dmg *= 0.3
		player_hp = maxf(0, player_hp - dmg)
	else:
		# Feint: wasting defensive stamina costs a little if you panicked.
		if was_dodging or was_parrying:
			player_hp = maxf(0, player_hp - 5.0)

	_defense_windows_resolved += 1
	_emit_stats()

	if player_hp <= 0:
		state = State.ENDED
		combat_ended.emit(false)
		return
	state = State.WAITING_PLAYER
	turn_changed.emit()


func _run_enemy_damage_phase() -> void:
	var dmg: float = encounter.enemy_turn_damage if encounter else 18.0
	var hazard: float = apply_piers_hazard_damage()
	player_hp = maxf(0, player_hp - dmg - hazard)
	_emit_stats()
	if player_hp <= 0:
		state = State.ENDED
		combat_ended.emit(false)
		return
	player_stamina = minf(player_max_stamina, player_stamina + 15)
	state = State.WAITING_PLAYER
	_emit_stats()
	turn_changed.emit()

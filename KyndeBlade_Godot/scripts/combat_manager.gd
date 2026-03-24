class_name CombatManager
extends Node
## Turn-based combat + real-time dodge/parry window. Deterministic enemy telegraph (no rand).
## **Dodge** on a real swing takes **partial** incoming damage ([constant DODGE_REAL_SWING_FRACTION] of full swing).
## **Parry** takes a **smaller partial** chip ([constant PARRY_INCOMING_FRACTION]) and deals a deterministic **riposte**
## (0.3–0.7× lowest equipped attack — [method _strike_damage], keyed by defensive window index).
## Parry window duration is **170–230 ms** (see `PlayerMovesetModifiers.parry_window_ms()`); dodge window stays longer for readability.

signal turn_changed
signal combat_ended(victory: bool)
signal stats_changed(player_hp: float, player_max: float, enemy_hp: float, enemy_max: float, stamina: float, stamina_max: float)
## Emitted when a dodge/parry defensive window opens; `is_real_swing` is false for a feint.
signal defensive_window_started(is_real_swing: bool)
## Fired when dodge/parry commits but the encounter delays the window (windup); same truth as the upcoming window.
signal defensive_windup_started(is_real_swing: bool)
## Dreamer (Wille) presentation: strike / dodge / parry committed after stamina gate passes.
signal dreamer_move_committed(move_kind: DreamerMoveKind)

enum State { WAITING_PLAYER, EXECUTING, REAL_TIME_WINDOW, ENEMY_TURN, DEFENSE_WINDUP, ENDED }

enum DreamerMoveKind { STRIKE, DODGE, PARRY }

## If `EncounterDef.defensive_windup_sec` is **0**, playable runs still use this gather so the eye / wind-up SFX read. **Headless** skips via [member use_instant_resolution_for_tests].
const MIN_GATHER_WHEN_WINDUP_ZERO_SEC := 0.22

## Full swing damage for **player-initiated** defensive windows (before encounter-specific enemy-turn scaling).
const PLAYER_PHASE_SWING_DAMAGE := 20.0
## Fraction of incoming damage applied when you **dodge** a real swing (rest is mitigated).
const DODGE_REAL_SWING_FRACTION := 0.6
## Fraction of incoming damage applied when you **parry** a real swing (smaller than dodge; you also riposte).
const PARRY_INCOMING_FRACTION := 0.3

## Loaded from `data/encounter_fair_field.tres` if unset (parity: demo-fayre-felde-encounter-wired).
@export var encounter: EncounterDef

## When **true**, strike → enemy phases resolve immediately (no `create_timer`). Used by headless combat scenario tests only.
@export var use_instant_resolution_for_tests: bool = false
## When **true** with [member use_instant_resolution_for_tests], enemy turn opens a reaction window instead of immediate damage (headless only).
@export var force_enemy_turn_reaction_window_in_tests: bool = false

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

## Next defensive window: if true, enemy committed a real swing (dodge/parry reduce damage; parry also ripostes).
var _enemy_will_hit: bool = true
var _defense_windows_resolved: int = 0
## When **true**, `REAL_TIME_WINDOW` is the enemy-turn reaction phase (dodge/parry before `enemy_turn_damage` applies).
var _enemy_turn_reaction_window: bool = false
var _windup_preview_real: bool = true
var _pending_window_duration: float = 1.5


func is_enemy_swing_real() -> bool:
	return _enemy_will_hit


func is_enemy_turn_reaction_window_active() -> bool:
	return _enemy_turn_reaction_window


## During [constant State.DEFENSE_WINDUP], the upcoming swing truth (matches next window).
func defensive_preview_is_real() -> bool:
	return _windup_preview_real


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
	var pend := GameState.pending_combat_encounter_path.strip_edges()
	if not pend.is_empty():
		GameState.pending_combat_encounter_path = ""
		if ResourceLoader.exists(pend):
			encounter = load(pend) as EncounterDef
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
	_enemy_turn_reaction_window = false
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


## Slice: only strike exists; use this for parry counter scaling when more attacks are equipped.
func _lowest_equipped_attack_damage() -> float:
	return _strike_damage()


## Deterministic multiplier in **[0.3, 0.7]** from the defensive window index (test-stable, no RNG).
func _parry_counterstrike_multiplier() -> float:
	var step := posmod(_defense_windows_resolved, 5)
	return 0.3 + 0.4 * (float(step) / 4.0)


func _dodge_stamina_cost() -> float:
	return maxf(1.0, 20.0 + PlayerMovesetModifiers.dodge_stamina_flat_extra())


func get_strike_stamina_cost() -> float:
	return _strike_stamina_cost()


func get_strike_damage_preview() -> float:
	return _strike_damage()


func get_dodge_stamina_cost() -> float:
	return _dodge_stamina_cost()


func get_parry_stamina_cost() -> float:
	return PlayerMovesetModifiers.parry_stamina_total()


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
	if state == State.REAL_TIME_WINDOW and _enemy_turn_reaction_window:
		var ecost: float = _dodge_stamina_cost()
		if player_stamina < ecost:
			return
		player_stamina -= ecost
		player_dodging = true
		player_parrying = false
		dreamer_move_committed.emit(DreamerMoveKind.DODGE)
		_emit_stats()
		return
	var dodge_cost: float = _dodge_stamina_cost()
	if state != State.WAITING_PLAYER or player_stamina < dodge_cost:
		return
	player_stamina -= dodge_cost
	player_dodging = true
	player_parrying = false
	dreamer_move_committed.emit(DreamerMoveKind.DODGE)
	_request_defensive_window(1.5)


func player_parry() -> void:
	if state == State.REAL_TIME_WINDOW and _enemy_turn_reaction_window:
		var pc: float = get_parry_stamina_cost()
		if player_stamina < pc:
			return
		player_stamina -= pc
		player_parrying = true
		player_dodging = false
		dreamer_move_committed.emit(DreamerMoveKind.PARRY)
		_emit_stats()
		return
	var pcost: float = get_parry_stamina_cost()
	if state != State.WAITING_PLAYER or player_stamina < pcost:
		return
	player_stamina -= pcost
	player_parrying = true
	player_dodging = false
	dreamer_move_committed.emit(DreamerMoveKind.PARRY)
	_request_defensive_window(PlayerMovesetModifiers.parry_window_ms() / 1000.0)


func _defensive_windup_sec() -> float:
	if encounter == null:
		return 0.0
	return maxf(0.0, encounter.defensive_windup_sec)


func _effective_defensive_windup_sec() -> float:
	if use_instant_resolution_for_tests:
		return _defensive_windup_sec()
	var w: float = _defensive_windup_sec()
	if w > 0.0001:
		return w
	return MIN_GATHER_WHEN_WINDUP_ZERO_SEC


func _request_defensive_window(duration: float) -> void:
	var wu := _effective_defensive_windup_sec()
	if wu > 0.0001 and not use_instant_resolution_for_tests:
		_pending_window_duration = duration
		_windup_preview_real = enemy_swing_is_hit_for_window_index(_defense_windows_resolved, feint_pattern_offset)
		state = State.DEFENSE_WINDUP
		_emit_stats()
		turn_changed.emit()
		defensive_windup_started.emit(_windup_preview_real)
		get_tree().create_timer(wu).timeout.connect(_finish_defense_windup, CONNECT_ONE_SHOT)
	else:
		_begin_defensive_window(duration)


func _finish_defense_windup() -> void:
	if state != State.DEFENSE_WINDUP:
		return
	_begin_defensive_window(_pending_window_duration)


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
	var was_enemy_reaction: bool = _enemy_turn_reaction_window
	if was_enemy_reaction:
		_enemy_turn_reaction_window = false
	var was_dodging: bool = player_dodging
	var was_parrying: bool = player_parrying
	player_dodging = false
	player_parrying = false
	state = State.EXECUTING

	if was_enemy_reaction:
		_resolve_enemy_turn_reaction_window(was_dodging, was_parrying)
		return

	if _enemy_will_hit:
		if was_dodging:
			player_hp = maxf(0, player_hp - PLAYER_PHASE_SWING_DAMAGE * DODGE_REAL_SWING_FRACTION)
		elif was_parrying:
			player_hp = maxf(0, player_hp - PLAYER_PHASE_SWING_DAMAGE * PARRY_INCOMING_FRACTION)
			var ctr: float = _lowest_equipped_attack_damage() * _parry_counterstrike_multiplier()
			enemy_hp = maxf(0.0, enemy_hp - ctr)
		else:
			player_hp = maxf(0, player_hp - PLAYER_PHASE_SWING_DAMAGE)
	else:
		# Feint: wasting defensive stamina costs a little if you panicked.
		if was_dodging or was_parrying:
			player_hp = maxf(0, player_hp - 5.0)

	_defense_windows_resolved += 1
	_emit_stats()

	if enemy_hp <= 0.0:
		state = State.ENDED
		combat_ended.emit(true)
		turn_changed.emit()
		return

	if player_hp <= 0:
		state = State.ENDED
		combat_ended.emit(false)
		turn_changed.emit()
		return
	state = State.WAITING_PLAYER
	turn_changed.emit()


func _resolve_enemy_turn_reaction_window(was_dodging: bool, was_parrying: bool) -> void:
	var base: float = encounter.enemy_turn_damage if encounter else 18.0
	var hazard: float = apply_piers_hazard_damage()
	if _enemy_will_hit:
		var full: float = base + hazard
		if was_dodging:
			player_hp = maxf(0, player_hp - full * DODGE_REAL_SWING_FRACTION)
		elif was_parrying:
			player_hp = maxf(0, player_hp - full * PARRY_INCOMING_FRACTION)
			var ctr2: float = _lowest_equipped_attack_damage() * _parry_counterstrike_multiplier()
			enemy_hp = maxf(0.0, enemy_hp - ctr2)
		else:
			player_hp = maxf(0, player_hp - full)
	else:
		if was_dodging or was_parrying:
			player_hp = maxf(0, player_hp - 5.0)

	_defense_windows_resolved += 1
	_emit_stats()

	if enemy_hp <= 0.0:
		state = State.ENDED
		combat_ended.emit(true)
		turn_changed.emit()
		return

	if player_hp <= 0:
		state = State.ENDED
		combat_ended.emit(false)
		turn_changed.emit()
		return
	player_stamina = minf(player_max_stamina, player_stamina + 15)
	state = State.WAITING_PLAYER
	_emit_stats()
	turn_changed.emit()


func _run_enemy_damage_phase() -> void:
	if not SaveService.load_enable_real_time_defense_on_enemy_turn():
		_apply_enemy_turn_damage_immediate()
		return
	if use_instant_resolution_for_tests and not force_enemy_turn_reaction_window_in_tests:
		_apply_enemy_turn_damage_immediate()
		return
	_begin_enemy_turn_reaction_window()


func _begin_enemy_turn_reaction_window() -> void:
	_enemy_turn_reaction_window = true
	player_dodging = false
	player_parrying = false
	_enemy_will_hit = enemy_swing_is_hit_for_window_index(_defense_windows_resolved, feint_pattern_offset)
	var dur: float = encounter.enemy_attack_reaction_window_seconds if encounter else 1.2
	window_duration = dur
	window_remaining = dur
	state = State.REAL_TIME_WINDOW
	_emit_stats()
	turn_changed.emit()
	defensive_window_started.emit(_enemy_will_hit)


func _apply_enemy_turn_damage_immediate() -> void:
	var dmg: float = encounter.enemy_turn_damage if encounter else 18.0
	var bleed: float = encounter.enemy_turn_bleed_damage if encounter else 0.0
	var hazard: float = apply_piers_hazard_damage()
	player_hp = maxf(0, player_hp - dmg - bleed - hazard)
	_emit_stats()
	if player_hp <= 0:
		state = State.ENDED
		combat_ended.emit(false)
		turn_changed.emit()
		return
	player_stamina = minf(player_max_stamina, player_stamina + 15)
	state = State.WAITING_PLAYER
	_emit_stats()
	turn_changed.emit()

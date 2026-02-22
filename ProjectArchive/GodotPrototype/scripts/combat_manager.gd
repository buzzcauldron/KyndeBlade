extends Node
## Turn-based combat with parry/dodge real-time window.
## Mirrors Kynde Blade Unity combat: Strike, Escapade (dodge), Ward (parry).

signal turn_changed
signal combat_ended(victory: bool)

enum State { WAITING_PLAYER, EXECUTING, REAL_TIME_WINDOW, ENEMY_TURN, ENDED }

var state: State = State.ENDED
var window_remaining: float = 0.0
var player_hp: float = 100.0
var player_max_hp: float = 100.0
var player_stamina: float = 100.0
var player_max_stamina: float = 100.0
var enemy_hp: float = 80.0
var enemy_max_hp: float = 80.0
var player_dodging: bool = false
var player_parrying: bool = false
var _window_duration: float = 1.5

func _ready() -> void:
	_start_combat()

func _start_combat() -> void:
	player_hp = player_max_hp
	player_stamina = player_max_stamina
	enemy_hp = enemy_max_hp
	state = State.WAITING_PLAYER
	turn_changed.emit()

func player_strike() -> void:
	if state != State.WAITING_PLAYER or player_stamina < 15:
		return
	player_stamina -= 15
	var dmg: float = 15.0
	enemy_hp = maxf(0, enemy_hp - dmg)
	if enemy_hp <= 0:
		state = State.ENDED
		combat_ended.emit(true)
		return
	state = State.ENEMY_TURN
	turn_changed.emit()
	await get_tree().create_timer(1.0).timeout
	_enemy_turn()

func player_dodge() -> void:
	if state != State.WAITING_PLAYER or player_stamina < 20:
		return
	player_stamina -= 20
	player_dodging = true
	player_parrying = false
	_start_real_time_window(1.5)

func player_parry() -> void:
	if state != State.WAITING_PLAYER or player_stamina < 25:
		return
	player_stamina -= 25
	player_parrying = true
	player_dodging = false
	_start_real_time_window(1.0)

func _start_real_time_window(duration: float) -> void:
	_window_duration = duration
	window_remaining = duration
	state = State.REAL_TIME_WINDOW
	turn_changed.emit()

func _update_window(delta: float) -> void:
	if state != State.REAL_TIME_WINDOW:
		return
	window_remaining -= delta
	if window_remaining <= 0:
		_resolve_window()

func _resolve_window() -> void:
	var was_dodging: bool = player_dodging
	var was_parrying: bool = player_parrying
	player_dodging = false
	player_parrying = false
	state = State.EXECUTING
	var hit: bool = randf() > 0.5
	var mitigated: bool = (was_dodging and hit) or (was_parrying and hit)
	var dmg: float = 12.0 if mitigated else 20.0
	if was_parrying and hit:
		dmg *= 0.3
	player_hp = maxf(0, player_hp - dmg)
	if player_hp <= 0:
		state = State.ENDED
		combat_ended.emit(false)
		return
	state = State.WAITING_PLAYER
	turn_changed.emit()

func _enemy_turn() -> void:
	state = State.ENEMY_TURN
	await get_tree().create_timer(0.5).timeout
	var dmg: float = 18.0
	player_hp = maxf(0, player_hp - dmg)
	if player_hp <= 0:
		state = State.ENDED
		combat_ended.emit(false)
		return
	player_stamina = minf(player_max_stamina, player_stamina + 15)
	state = State.WAITING_PLAYER
	turn_changed.emit()

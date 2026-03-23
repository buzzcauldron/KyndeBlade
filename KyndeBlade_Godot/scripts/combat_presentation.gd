extends Node2D
## Lane B side-view stage: **manuscript-illumination** silhouettes (dreamer / ploughman vs Langage fals) + motion (presentation only).

@export var show_foreground_hazard: bool = false

const HIT_FLASH_SEC := 0.22
const SHAKE_SEC := 0.14
const SHAKE_PX := 7.0
## ~3 frames at 60fps: frozen tableau when the defensive window opens (Salome “stillness” before motion).
const TELEGRAPH_HOLD_SEC := 0.05

var _combat: CombatManager
var _player_base: Vector2
var _enemy_base: Vector2
var _prev_enemy_hp: float = -1.0
var _prev_player_hp: float = -1.0
var _strike_punch_t: float = 0.0
var _hit_flash_t: float = 0.0
var _shake_t: float = 0.0
var _shake_dir: Vector2 = Vector2.RIGHT
var _prev_combat_state: CombatManager.State = CombatManager.State.ENDED
var _telegraph_hold_left: float = 0.0
var _telegraph_snap_bob: float = 0.0
var _telegraph_snap_ms: float = 0.0

## Main hull used for hit flash; whole trees bob for idle.
var player_body: Polygon2D
var enemy_body: Polygon2D
var player_staff: Polygon2D
var player_cowl: Polygon2D
var enemy_maw: Polygon2D
var enemy_tongue: Polygon2D
var enemy_scroll: Polygon2D
var enemy_stains: Polygon2D

@onready var player_root: Node2D = $PlayerRoot
@onready var enemy_root: Node2D = $EnemyRoot
@onready var hazard_strip: Polygon2D = $HazardStrip
@onready var camera: Camera2D = $Camera2D


func _ready() -> void:
	_build_manuscript_figures()
	_bind_figure_refs_from_scene()
	_player_base = player_root.position
	_enemy_base = enemy_root.position
	hazard_strip.visible = show_foreground_hazard
	var parent_n := get_parent()
	if parent_n:
		_combat = parent_n.get_node_or_null("CombatManager") as CombatManager
	if _combat:
		_combat.turn_changed.connect(_on_turn_changed)
		_combat.stats_changed.connect(_on_stats_changed)
		_prev_enemy_hp = _combat.enemy_hp
		_prev_player_hp = _combat.player_hp
		_on_turn_changed()


func _build_manuscript_figures() -> void:
	if player_root.get_child_count() > 0 or enemy_root.get_child_count() > 0:
		return
	# --- Dreamer / labourer (Wille): cowl, tunic, plough-staff (lapis) ---
	player_cowl = Polygon2D.new()
	player_cowl.z_index = 2
	player_cowl.color = Color(0.22, 0.2, 0.26, 1)
	player_cowl.polygon = PackedVector2Array([
		Vector2(-8, -58), Vector2(14, -62), Vector2(18, -38), Vector2(-4, -32)
	])
	player_root.add_child(player_cowl)

	player_body = Polygon2D.new()
	player_body.z_index = 1
	player_body.color = Color(0.36, 0.28, 0.2, 1)
	player_body.polygon = PackedVector2Array([
		Vector2(-22, -32), Vector2(24, -28), Vector2(28, 38), Vector2(-26, 42), Vector2(-30, 8)
	])
	player_root.add_child(player_body)

	player_staff = Polygon2D.new()
	player_staff.z_index = 0
	player_staff.color = KyndeBladeArtPalette.LAPIS.lerp(Color.BLACK, 0.12)
	player_staff.polygon = PackedVector2Array([
		Vector2(26, -8), Vector2(62, 4), Vector2(60, 10), Vector2(24, -2)
	])
	player_root.add_child(player_staff)

	# --- False: jagged “rubric” fiend, manuscript margin beast simplified ---
	enemy_body = Polygon2D.new()
	enemy_body.z_index = 1
	var eb: Color = KyndeBladeArtPalette.RUBRICATION.darkened(0.1)
	enemy_body.color = eb.lerp(KyndeBladeArtPalette.JEWEL_CRIMSON, KyndeBladeArtPalette.ENEMY_BODY_JEWEL_MIX)
	enemy_body.polygon = PackedVector2Array([
		Vector2(-32, -92), Vector2(8, -98), Vector2(44, -72), Vector2(38, -20),
		Vector2(48, 28), Vector2(12, 84), Vector2(-36, 76), Vector2(-44, 20), Vector2(-40, -40)
	])
	enemy_root.add_child(enemy_body)

	enemy_maw = Polygon2D.new()
	enemy_maw.z_index = 2
	enemy_maw.color = KyndeBladeArtPalette.BORDER_RED.lerp(Color.BLACK, 0.18)
	enemy_maw.polygon = PackedVector2Array([
		Vector2(-10, -30), Vector2(24, -34), Vector2(20, 6), Vector2(-14, 2)
	])
	enemy_root.add_child(enemy_maw)

	# False speaks crookedly: split tongue motif (langage fals).
	enemy_tongue = Polygon2D.new()
	enemy_tongue.z_index = 3
	enemy_tongue.color = KyndeBladeArtPalette.JEWEL_CRIMSON
	enemy_tongue.polygon = PackedVector2Array([
		Vector2(6, -10), Vector2(16, -6), Vector2(10, 22), Vector2(4, 26),
		Vector2(8, 10), Vector2(2, 26), Vector2(-4, 22), Vector2(0, -8)
	])
	enemy_root.add_child(enemy_tongue)

	# Hawkin's coat echo: parchment sash stained with rubric marks.
	enemy_scroll = Polygon2D.new()
	enemy_scroll.z_index = 0
	enemy_scroll.color = KyndeBladeArtPalette.PARCHMENT_AGED.lerp(KyndeBladeArtPalette.INK_LIGHT, 0.2)
	enemy_scroll.polygon = PackedVector2Array([
		Vector2(-38, 14), Vector2(34, 6), Vector2(38, 26), Vector2(-42, 34)
	])
	enemy_root.add_child(enemy_scroll)

	enemy_stains = Polygon2D.new()
	enemy_stains.z_index = 1
	enemy_stains.color = KyndeBladeArtPalette.RUBRICATION.lerp(KyndeBladeArtPalette.BORDER_RED, 0.25)
	enemy_stains.polygon = PackedVector2Array([
		Vector2(-28, 14), Vector2(-16, 12), Vector2(-10, 18), Vector2(-22, 20),
		Vector2(-4, 14), Vector2(10, 12), Vector2(16, 18), Vector2(2, 20),
		Vector2(20, 10), Vector2(30, 8), Vector2(34, 14), Vector2(24, 16)
	])
	enemy_root.add_child(enemy_stains)


## `combat.tscn` keeps **empty** `PlayerRoot` / `EnemyRoot` so `_build_manuscript_figures` runs; if you add scene children again, building is skipped — bind refs below so hit-flash paths stay valid.
func _bind_figure_refs_from_scene() -> void:
	if player_body == null:
		player_body = player_root.get_node_or_null("Body") as Polygon2D
	if enemy_body == null:
		enemy_body = enemy_root.get_node_or_null("Body") as Polygon2D
	if player_cowl == null:
		player_cowl = player_root.get_node_or_null("Cowl") as Polygon2D
	if player_staff == null:
		player_staff = player_root.get_node_or_null("Staff") as Polygon2D
	if enemy_maw == null:
		enemy_maw = enemy_root.get_node_or_null("Maw") as Polygon2D
	if enemy_tongue == null:
		enemy_tongue = enemy_root.get_node_or_null("Tongue") as Polygon2D
	if enemy_scroll == null:
		enemy_scroll = enemy_root.get_node_or_null("Scroll") as Polygon2D
	if enemy_stains == null:
		enemy_stains = enemy_root.get_node_or_null("Stains") as Polygon2D


func _on_stats_changed(p_hp: float, _pm: float, e_hp: float, _em: float, _s: float, _sm: float) -> void:
	if _combat == null:
		return
	if _prev_player_hp >= 0.0 and p_hp < _prev_player_hp - 0.01:
		var lost: float = _prev_player_hp - p_hp
		_hit_flash_t = HIT_FLASH_SEC
		if lost >= 8.0:
			_shake_t = SHAKE_SEC
			_shake_dir = Vector2(randf_range(-1.0, 1.0), randf_range(-1.0, 1.0))
			if _shake_dir.length_squared() < 0.01:
				_shake_dir = Vector2(1, 0.2)
			_shake_dir = _shake_dir.normalized()
	_prev_player_hp = p_hp

	if _prev_enemy_hp < 0.0:
		_prev_enemy_hp = e_hp
		return
	if e_hp < _prev_enemy_hp - 0.01 and _combat.state == _combat.State.WAITING_PLAYER:
		_strike_punch_t = 1.0
	_prev_enemy_hp = e_hp


func _on_turn_changed() -> void:
	if _combat == null:
		return
	if _combat.state == _combat.State.WAITING_PLAYER:
		_prev_enemy_hp = _combat.enemy_hp


func _modulate_player_figure(c: Color) -> void:
	if player_body:
		player_body.modulate = c
	if player_cowl:
		player_cowl.modulate = c
	if player_staff:
		player_staff.modulate = c


func _modulate_enemy_figure(c: Color) -> void:
	if enemy_body:
		enemy_body.modulate = c
	if enemy_maw:
		enemy_maw.modulate = c
	if enemy_tongue:
		enemy_tongue.modulate = c
	if enemy_scroll:
		enemy_scroll.modulate = c
	if enemy_stains:
		enemy_stains.modulate = c


func _process(delta: float) -> void:
	if _combat == null:
		return
	var st: CombatManager.State = _combat.state
	if st == CombatManager.State.REAL_TIME_WINDOW and _prev_combat_state != CombatManager.State.REAL_TIME_WINDOW:
		_telegraph_hold_left = TELEGRAPH_HOLD_SEC
		_telegraph_snap_ms = float(Time.get_ticks_msec())
		_telegraph_snap_bob = _telegraph_snap_ms * 0.003
	_prev_combat_state = st

	var bob_t := float(Time.get_ticks_msec()) * 0.003
	if _strike_punch_t > 0.0:
		_strike_punch_t = maxf(0.0, _strike_punch_t - delta * 3.5)
		var punch := sin(_strike_punch_t * PI) * 18.0
		player_root.position = _player_base + Vector2(punch, 0)
	else:
		match _combat.state:
			CombatManager.State.WAITING_PLAYER:
				player_root.position = _player_base + Vector2(0, sin(bob_t) * 3.0)
				enemy_root.position = _enemy_base + Vector2(0, sin(bob_t + 1.2) * 2.5)
				enemy_root.scale = Vector2.ONE
				_modulate_enemy_figure(Color.WHITE)
				_modulate_player_figure(Color.WHITE)
			CombatManager.State.REAL_TIME_WINDOW:
				var holding_telegraph := _telegraph_hold_left > 0.0
				if holding_telegraph:
					bob_t = _telegraph_snap_bob
				## Dramatic stillness: minimal bob; slow, small breathing scale (tableau before violence).
				player_root.position = _player_base + Vector2(0, sin(bob_t * 0.35) * 0.35)
				enemy_root.position = _enemy_base + Vector2(0, sin(bob_t * 0.28 + 0.5) * 0.28)
				var t_ms: float = _telegraph_snap_ms if holding_telegraph else float(Time.get_ticks_msec())
				if _combat.is_enemy_swing_real():
					var w := 1.0 + sin(t_ms * 0.011) * 0.028
					enemy_root.scale = Vector2(w, w)
					var env: Color = KyndeBladeArtPalette.JEWEL_CRIMSON.lerp(KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW, 0.38)
					_modulate_enemy_figure(env.lerp(Color.WHITE, 0.12))
				else:
					var w2 := 1.0 + sin(t_ms * 0.022) * 0.018
					enemy_root.scale = Vector2(w2, w2)
					var em: Color = KyndeBladeArtPalette.JEWEL_EMERALD
					_modulate_enemy_figure(Color(em.r * 1.05, em.g * 1.08, em.b * 1.12, 1.0).lerp(Color.WHITE, 0.45))
				_modulate_player_figure(Color(0.9, 0.92, 0.98, 1.0))
				if holding_telegraph:
					_telegraph_hold_left = maxf(0.0, _telegraph_hold_left - delta)
			_:
				player_root.position = _player_base
				enemy_root.position = _enemy_base
				enemy_root.scale = Vector2.ONE
				_modulate_enemy_figure(Color.WHITE)
				_modulate_player_figure(Color.WHITE)

	if _hit_flash_t > 0.0:
		_hit_flash_t = maxf(0.0, _hit_flash_t - delta)
		if player_body != null:
			var k := clampf(_hit_flash_t / HIT_FLASH_SEC, 0.0, 1.0)
			var rub := KyndeBladeArtPalette.RUBRICATION
			var flash := Color(rub.r, rub.g, rub.b, 1.0)
			var base := player_body.modulate
			_modulate_player_figure(base.lerp(flash, (1.0 - k) * 0.85))

	if _shake_t > 0.0:
		_shake_t = maxf(0.0, _shake_t - delta)
		var mag := SHAKE_PX * clampf(_shake_t / SHAKE_SEC, 0.0, 1.0)
		camera.offset = _shake_dir * mag
	else:
		camera.offset = Vector2.ZERO

extends Node2D
## Lane B side-view stage: manuscript silhouettes (dreamer vs Langage fals) with motion.

@export var show_foreground_hazard: bool = false

const HIT_FLASH_SEC := 0.22
const SHAKE_SEC := 0.14
const SHAKE_PX := 7.0
## ~3 frames at 60fps: frozen tableau at defensive window open.
const TELEGRAPH_HOLD_SEC := 0.05
const CAM_SWAY_X := 8.0
const CAM_SWAY_Y := 3.5
const CAM_PULL_X := 14.0
const CAM_PULL_Y := 6.0
const CAM_LERP := 6.0

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
var _camera_base_offset: Vector2 = Vector2.ZERO

## Main hull used for hit flash; whole trees bob for idle.
var player_body: Polygon2D
var enemy_body: Polygon2D
var player_staff: Polygon2D
var player_cowl: Polygon2D
var enemy_maw: Polygon2D
var enemy_tongue: Polygon2D
var enemy_scroll: Polygon2D
var enemy_stains: Polygon2D
var _back_mist: Polygon2D
var _mid_canopy: Polygon2D
var _front_grass: Polygon2D

@onready var player_root: Node2D = $PlayerRoot
@onready var enemy_root: Node2D = $EnemyRoot
@onready var hazard_strip: Polygon2D = $HazardStrip
@onready var camera: Camera2D = $Camera2D


func _ready() -> void:
	_build_manuscript_figures()
	_bind_figure_refs_from_scene()
	_ensure_depth_layers()
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


func _ensure_depth_layers() -> void:
	_back_mist = get_node_or_null("BackMist") as Polygon2D
	if _back_mist == null:
		_back_mist = Polygon2D.new()
		_back_mist.name = "BackMist"
		_back_mist.z_index = -8
		_back_mist.color = Color(0.68, 0.79, 0.84, 0.24)
		_back_mist.polygon = PackedVector2Array([
			Vector2(-560, -130), Vector2(560, -140), Vector2(560, 18), Vector2(-560, 8)
		])
		add_child(_back_mist)
		move_child(_back_mist, 0)

	_mid_canopy = get_node_or_null("MidCanopy") as Polygon2D
	if _mid_canopy == null:
		_mid_canopy = Polygon2D.new()
		_mid_canopy.name = "MidCanopy"
		_mid_canopy.z_index = -3
		_mid_canopy.color = Color(0.11, 0.2, 0.15, 0.42)
		_mid_canopy.polygon = PackedVector2Array([
			Vector2(-540, -60), Vector2(-380, -110), Vector2(-120, -70), Vector2(140, -115),
			Vector2(360, -78), Vector2(540, -104), Vector2(540, -8), Vector2(-540, 0)
		])
		add_child(_mid_canopy)
		move_child(_mid_canopy, 1)

	_front_grass = get_node_or_null("FrontGrass") as Polygon2D
	if _front_grass == null:
		_front_grass = Polygon2D.new()
		_front_grass.name = "FrontGrass"
		_front_grass.z_index = 4
		_front_grass.color = Color(0.18, 0.28, 0.14, 0.44)
		_front_grass.polygon = PackedVector2Array([
			Vector2(-560, 134), Vector2(-420, 112), Vector2(-260, 136), Vector2(-80, 110),
			Vector2(100, 132), Vector2(260, 114), Vector2(420, 140), Vector2(560, 118),
			Vector2(560, 234), Vector2(-560, 234)
		])
		add_child(_front_grass)


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
	enemy_body.color = eb.lerp(
		KyndeBladeArtPalette.JEWEL_CRIMSON,
		KyndeBladeArtPalette.ENEMY_BODY_JEWEL_MIX
	)
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
	enemy_scroll.color = KyndeBladeArtPalette.PARCHMENT_AGED.lerp(
		KyndeBladeArtPalette.INK_LIGHT,
		0.2
	)
	enemy_scroll.polygon = PackedVector2Array([
		Vector2(-38, 14), Vector2(34, 6), Vector2(38, 26), Vector2(-42, 34)
	])
	enemy_root.add_child(enemy_scroll)

	enemy_stains = Polygon2D.new()
	enemy_stains.z_index = 1
	enemy_stains.color = KyndeBladeArtPalette.RUBRICATION.lerp(
		KyndeBladeArtPalette.BORDER_RED,
		0.25
	)
	enemy_stains.polygon = PackedVector2Array([
		Vector2(-28, 14), Vector2(-16, 12), Vector2(-10, 18), Vector2(-22, 20),
		Vector2(-4, 14), Vector2(10, 12), Vector2(16, 18), Vector2(2, 20),
		Vector2(20, 10), Vector2(30, 8), Vector2(34, 14), Vector2(24, 16)
	])
	enemy_root.add_child(enemy_stains)


## `combat.tscn` keeps empty `PlayerRoot` / `EnemyRoot` so procedural build runs.
## If scene children are added later, this binds references for hit-flash compatibility.
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
	if (
		st == CombatManager.State.REAL_TIME_WINDOW
		and _prev_combat_state != CombatManager.State.REAL_TIME_WINDOW
	):
		_telegraph_hold_left = TELEGRAPH_HOLD_SEC
		_telegraph_snap_ms = float(Time.get_ticks_msec())
		_telegraph_snap_bob = _telegraph_snap_ms * 0.003
	_prev_combat_state = st

	var bob_t := float(Time.get_ticks_msec()) * 0.003
	var player_scale_mult := 1.0
	var enemy_scale_mult := 1.0
	if _strike_punch_t > 0.0:
		_strike_punch_t = maxf(0.0, _strike_punch_t - delta * 3.5)
		var punch := sin(_strike_punch_t * PI) * 18.0
		player_root.position = _player_base + Vector2(punch, 0)
		enemy_root.position = _enemy_base + Vector2(0, sin(bob_t + 1.1) * 1.8)
		enemy_scale_mult = 1.0 + sin(bob_t * 0.85) * 0.015
	else:
		match _combat.state:
			CombatManager.State.WAITING_PLAYER:
				player_root.position = _player_base + Vector2(0, sin(bob_t) * 3.0)
				enemy_root.position = _enemy_base + Vector2(0, sin(bob_t + 1.2) * 2.5)
				enemy_scale_mult = 1.0 + sin(bob_t * 0.6 + 0.8) * 0.01
				player_scale_mult = 1.0 + sin(bob_t * 0.7) * 0.012
				_modulate_enemy_figure(Color.WHITE)
				_modulate_player_figure(Color.WHITE)
			CombatManager.State.DEFENSE_WINDUP:
				var tw := float(Time.get_ticks_msec())
				player_root.position = _player_base + Vector2(0, sin(bob_t * 0.5) * 1.2)
				enemy_root.position = _enemy_base + Vector2(0, sin(bob_t * 0.45 + 0.4) * 1.0)
				if _combat.defensive_preview_is_real():
					var w := 1.0 + sin(tw * 0.010) * 0.085
					enemy_scale_mult = w
					var env: Color = KyndeBladeArtPalette.JEWEL_CRIMSON.lerp(
						KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW,
						0.42
					)
					_modulate_enemy_figure(env.lerp(Color.WHITE, 0.08))
				else:
					var w2 := 1.0 + sin(tw * 0.019) * 0.055
					enemy_scale_mult = w2
					var em: Color = KyndeBladeArtPalette.JEWEL_EMERALD
					var feint_col := Color(em.r * 1.08, em.g * 1.1, em.b * 1.14, 1.0)
					_modulate_enemy_figure(feint_col.lerp(Color.WHITE, 0.38))
				player_scale_mult = 0.985
				_modulate_player_figure(Color(0.88, 0.9, 0.96, 1.0))
			CombatManager.State.REAL_TIME_WINDOW:
				var holding_telegraph := _telegraph_hold_left > 0.0
				if holding_telegraph:
					bob_t = _telegraph_snap_bob
				## Dramatic stillness: minimal bob and shallow breathing scale.
				player_root.position = _player_base + Vector2(0, sin(bob_t * 0.35) * 0.35)
				enemy_root.position = _enemy_base + Vector2(0, sin(bob_t * 0.28 + 0.5) * 0.28)
				var t_ms: float = (
					_telegraph_snap_ms if holding_telegraph else float(Time.get_ticks_msec())
				)
				if _combat.is_enemy_swing_real():
					var w := 1.0 + sin(t_ms * 0.011) * 0.028
					enemy_scale_mult = w
					var env: Color = KyndeBladeArtPalette.JEWEL_CRIMSON.lerp(
						KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW,
						0.38
					)
					_modulate_enemy_figure(env.lerp(Color.WHITE, 0.12))
				else:
					var w2 := 1.0 + sin(t_ms * 0.022) * 0.018
					enemy_scale_mult = w2
					var em: Color = KyndeBladeArtPalette.JEWEL_EMERALD
					var feint_col := Color(em.r * 1.05, em.g * 1.08, em.b * 1.12, 1.0)
					_modulate_enemy_figure(feint_col.lerp(Color.WHITE, 0.45))
				player_scale_mult = 0.99
				_modulate_player_figure(Color(0.9, 0.92, 0.98, 1.0))
				if holding_telegraph:
					_telegraph_hold_left = maxf(0.0, _telegraph_hold_left - delta)
			_:
				player_root.position = _player_base
				enemy_root.position = _enemy_base
				enemy_scale_mult = 1.0
				player_scale_mult = 1.0
				_modulate_enemy_figure(Color.WHITE)
				_modulate_player_figure(Color.WHITE)

	_apply_fake_depth_scale(player_scale_mult, enemy_scale_mult)
	_update_stage_parallax(st, bob_t)
	_update_cinematic_camera(delta, st, bob_t)

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
		camera.offset = _camera_base_offset + _shake_dir * mag
	else:
		camera.offset = _camera_base_offset


func _apply_fake_depth_scale(player_state_scale: float, enemy_state_scale: float) -> void:
	var p_depth := clampf((player_root.position.y - _player_base.y) / 58.0, -1.0, 1.0)
	var e_depth := clampf((enemy_root.position.y - _enemy_base.y) / 58.0, -1.0, 1.0)
	var p_scale := (1.0 + p_depth * 0.035) * player_state_scale
	var e_scale := (1.0 + e_depth * 0.045) * enemy_state_scale
	player_root.scale = Vector2(p_scale, p_scale)
	enemy_root.scale = Vector2(e_scale, e_scale)


func _update_stage_parallax(st: CombatManager.State, bob_t: float) -> void:
	if _back_mist == null or _mid_canopy == null or _front_grass == null:
		return
	var rush := 0.0
	if st == CombatManager.State.DEFENSE_WINDUP:
		rush = 1.0
	elif st == CombatManager.State.REAL_TIME_WINDOW:
		rush = 1.4
	var drift := sin(bob_t * 0.6) * 5.0
	_back_mist.position = Vector2(drift * 0.35 + rush * -3.5, sin(bob_t * 0.31) * 2.0)
	_mid_canopy.position = Vector2(
		drift * 0.65 + rush * -6.0,
		sin(bob_t * 0.42 + 0.7) * 2.8
	)
	_front_grass.position = Vector2(
		drift + rush * -10.0,
		sin(bob_t * 0.95 + 0.3) * 3.6
	)
	_front_grass.color.a = 0.44 + rush * 0.05
	_mid_canopy.color.a = 0.42 + rush * 0.04


func _update_cinematic_camera(delta: float, st: CombatManager.State, bob_t: float) -> void:
	var mid := (_player_base + _enemy_base) * 0.5
	var pull := (enemy_root.position - player_root.position).normalized()
	if pull.length_squared() < 0.001:
		pull = Vector2.RIGHT
	var mood := 0.0
	if st == CombatManager.State.DEFENSE_WINDUP:
		mood = 0.65
	elif st == CombatManager.State.REAL_TIME_WINDOW:
		mood = 1.0
	var sway := Vector2(
		sin(bob_t * 0.52) * CAM_SWAY_X,
		cos(bob_t * 0.67) * CAM_SWAY_Y
	)
	var target := sway + pull * (CAM_PULL_X * mood) + Vector2(0.0, -CAM_PULL_Y * mood)
	_camera_base_offset = _camera_base_offset.lerp(target, clampf(delta * CAM_LERP, 0.0, 1.0))
	camera.position = camera.position.lerp(mid * 0.03, clampf(delta * 2.4, 0.0, 1.0))

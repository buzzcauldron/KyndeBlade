extends Node3D
## Graybox 3D combat arena: actor motion, mood camera, hit feedback. Logic stays in `CombatManager`.

const HIT_FLASH_SEC := 0.22
const SHAKE_SEC := 0.14
const SHAKE_MAG := 0.14
const TELEGRAPH_HOLD_SEC := 0.05
const CAM_SWAY_XZ := 0.22
const CAM_SWAY_Y := 0.14
const CAM_PULL_MOOD := 0.85
const CAM_LERP := 6.0
const STRIKE_LUNGE := 0.42
const STRIKE_Y_BUMP := 0.18

var _combat: CombatManager
var _player_base: Vector3
var _enemy_base: Vector3
var _prev_enemy_hp: float = -1.0
var _prev_player_hp: float = -1.0
var _strike_punch_t: float = 0.0
var _hit_flash_t: float = 0.0
var _shake_t: float = 0.0
var _shake_dir: Vector3 = Vector3.RIGHT
var _prev_combat_state: CombatManager.State = CombatManager.State.ENDED
var _telegraph_hold_left: float = 0.0
var _telegraph_snap_bob: float = 0.0
var _telegraph_snap_ms: float = 0.0
var _cam_offset_lerped: Vector3 = Vector3.ZERO

var _base_player_albedo: Color
var _base_enemy_albedo: Color
var _player_model_scale: Vector3 = Vector3.ONE
var _enemy_model_scale: Vector3 = Vector3.ONE

@onready var player_root: Node3D = $PlayerActor
@onready var enemy_root: Node3D = $EnemyActor
@onready var player_mesh: MeshInstance3D = $PlayerActor/PlayerMesh
@onready var enemy_mesh: MeshInstance3D = $EnemyActor/EnemyMesh
@onready var camera_3d: Camera3D = $CombatCamera
@onready var hit_flash_light: OmniLight3D = $HitFlashLight


func _ready() -> void:
	var root := get_parent()
	if root:
		_combat = root.get_node_or_null("CombatManager") as CombatManager
	_player_base = player_root.position
	_enemy_base = enemy_root.position
	_player_model_scale = player_root.scale
	_enemy_model_scale = enemy_root.scale
	var pm: StandardMaterial3D = player_mesh.get_active_material(0) as StandardMaterial3D
	var em: StandardMaterial3D = enemy_mesh.get_active_material(0) as StandardMaterial3D
	if pm:
		_base_player_albedo = pm.albedo_color
	else:
		_base_player_albedo = Color(0.4, 0.32, 0.24, 1)
	if em:
		_base_enemy_albedo = em.albedo_color
	else:
		_base_enemy_albedo = Color(0.5, 0.12, 0.1, 1)
	if _combat:
		_combat.turn_changed.connect(_on_turn_changed)
		_combat.stats_changed.connect(_on_stats_changed)
		_prev_enemy_hp = _combat.enemy_hp
		_prev_player_hp = _combat.player_hp
		_on_turn_changed()
	_snap_camera_initial()


func _snap_camera_initial() -> void:
	var bob0 := float(Time.get_ticks_msec()) * 0.003
	var mid := _midpoint()
	_cam_offset_lerped = _desired_camera_pos(0.0, CombatManager.State.WAITING_PLAYER, bob0)
	camera_3d.global_position = _cam_offset_lerped
	camera_3d.look_at(mid, Vector3.UP)


func _midpoint() -> Vector3:
	return (player_root.global_position + enemy_root.global_position) * 0.5 + Vector3(0, 1.35, 0)


func _axis_player_to_enemy() -> Vector3:
	var d: Vector3 = enemy_root.global_position - player_root.global_position
	d.y = 0.0
	if d.length_squared() < 0.0001:
		return Vector3(0, 0, -1)
	return d.normalized()


func _desired_camera_pos(mood: float, st: CombatManager.State, bob_t: float) -> Vector3:
	var mid := _midpoint()
	var toward_enemy: Vector3 = _axis_player_to_enemy()
	var back: Vector3 = -toward_enemy
	var base: Vector3 = mid + back * 5.8 + Vector3(0.55, 2.1, 0)
	var sway := Vector3(
			sin(bob_t * 0.52) * CAM_SWAY_XZ,
			cos(bob_t * 0.67) * CAM_SWAY_Y,
			sin(bob_t * 0.44) * CAM_SWAY_XZ * 0.65
	)
	var target: Vector3 = base + sway - toward_enemy * (CAM_PULL_MOOD * mood)
	if st == CombatManager.State.EXECUTING:
		target += back * 0.35
	return target


func _on_stats_changed(p_hp: float, _pm: float, e_hp: float, _em: float, _s: float, _sm: float) -> void:
	if _combat == null:
		return
	if _prev_player_hp >= 0.0 and p_hp < _prev_player_hp - 0.01:
		var lost: float = _prev_player_hp - p_hp
		_hit_flash_t = HIT_FLASH_SEC
		_pulse_hit_light(player_root.global_position + Vector3(0, 1.0, 0), Color(0.9, 0.25, 0.2, 1), 3.5)
		if lost >= 8.0:
			_shake_t = SHAKE_SEC
			_shake_dir = Vector3(randf_range(-1.0, 1.0), randf_range(-0.4, 0.9), randf_range(-1.0, 1.0))
			if _shake_dir.length_squared() < 0.01:
				_shake_dir = Vector3(1, 0.15, 0.2)
			_shake_dir = _shake_dir.normalized()
	_prev_player_hp = p_hp

	if _prev_enemy_hp < 0.0:
		_prev_enemy_hp = e_hp
		return
	if e_hp < _prev_enemy_hp - 0.01 and _combat.state == _combat.State.WAITING_PLAYER:
		_strike_punch_t = 1.0
		_pulse_hit_light(enemy_root.global_position + Vector3(0, 1.4, 0), Color(1.0, 0.92, 0.75, 1), 5.0)
	_prev_enemy_hp = e_hp


func _pulse_hit_light(at: Vector3, c: Color, peak: float) -> void:
	hit_flash_light.global_position = at
	hit_flash_light.light_color = c
	hit_flash_light.light_energy = peak
	hit_flash_light.visible = true


func _on_turn_changed() -> void:
	if _combat == null:
		return
	if _combat.state == _combat.State.WAITING_PLAYER:
		_prev_enemy_hp = _combat.enemy_hp


func _modulate_player_mesh(c: Color) -> void:
	var pm: StandardMaterial3D = player_mesh.get_active_material(0) as StandardMaterial3D
	if pm:
		pm.albedo_color = Color(
				_base_player_albedo.r * c.r,
				_base_player_albedo.g * c.g,
				_base_player_albedo.b * c.b,
				_base_player_albedo.a * c.a
		)


func _modulate_enemy_mesh(c: Color) -> void:
	var em: StandardMaterial3D = enemy_mesh.get_active_material(0) as StandardMaterial3D
	if em:
		em.albedo_color = Color(
				_base_enemy_albedo.r * c.r,
				_base_enemy_albedo.g * c.g,
				_base_enemy_albedo.b * c.b,
				_base_enemy_albedo.a * c.a
		)


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
	var toward_enemy: Vector3 = _axis_player_to_enemy()

	if _strike_punch_t > 0.0:
		_strike_punch_t = maxf(0.0, _strike_punch_t - delta * 3.5)
		var punch := sin(_strike_punch_t * PI)
		player_root.position = (
				_player_base
				+ toward_enemy * punch * STRIKE_LUNGE
				+ Vector3(0, punch * STRIKE_Y_BUMP, 0)
		)
		enemy_root.position = _enemy_base + Vector3(0, sin(bob_t + 1.1) * 0.06, 0)
		enemy_scale_mult = 1.0 + sin(bob_t * 0.85) * 0.015
	else:
		match _combat.state:
			CombatManager.State.WAITING_PLAYER:
				player_root.position = _player_base + Vector3(0, sin(bob_t) * 0.05, 0)
				enemy_root.position = _enemy_base + Vector3(0, sin(bob_t + 1.2) * 0.045, 0)
				enemy_scale_mult = 1.0 + sin(bob_t * 0.6 + 0.8) * 0.01
				player_scale_mult = 1.0 + sin(bob_t * 0.7) * 0.012
				_modulate_enemy_mesh(Color.WHITE)
				_modulate_player_mesh(Color.WHITE)
			CombatManager.State.DEFENSE_WINDUP:
				var tw := float(Time.get_ticks_msec())
				player_root.position = _player_base + Vector3(0, sin(bob_t * 0.5) * 0.025, 0)
				enemy_root.position = _enemy_base + Vector3(0, sin(bob_t * 0.45 + 0.4) * 0.02, 0)
				if _combat.defensive_preview_is_real():
					var w := 1.0 + sin(tw * 0.010) * 0.085
					enemy_scale_mult = w
					var env: Color = KyndeBladeArtPalette.JEWEL_CRIMSON.lerp(
							KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW,
							0.42
					)
					_modulate_enemy_mesh(env.lerp(Color.WHITE, 0.08))
				else:
					var w2 := 1.0 + sin(tw * 0.019) * 0.055
					enemy_scale_mult = w2
					var emc: Color = KyndeBladeArtPalette.JEWEL_EMERALD
					var feint_col := Color(emc.r * 1.08, emc.g * 1.1, emc.b * 1.14, 1.0)
					_modulate_enemy_mesh(feint_col.lerp(Color.WHITE, 0.38))
				player_scale_mult = 0.985
				_modulate_player_mesh(Color(0.88, 0.9, 0.96, 1.0))
			CombatManager.State.REAL_TIME_WINDOW:
				var holding_telegraph := _telegraph_hold_left > 0.0
				if holding_telegraph:
					bob_t = _telegraph_snap_bob
				player_root.position = _player_base + Vector3(0, sin(bob_t * 0.35) * 0.008, 0)
				enemy_root.position = _enemy_base + Vector3(0, sin(bob_t * 0.28 + 0.5) * 0.008, 0)
				var t_ms: float = (
						_telegraph_snap_ms if holding_telegraph else float(Time.get_ticks_msec())
				)
				if _combat.is_enemy_swing_real():
					var w := 1.0 + sin(t_ms * 0.011) * 0.028
					enemy_scale_mult = w
					var env2: Color = KyndeBladeArtPalette.JEWEL_CRIMSON.lerp(
							KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW,
							0.38
					)
					_modulate_enemy_mesh(env2.lerp(Color.WHITE, 0.12))
				else:
					var w2b := 1.0 + sin(t_ms * 0.022) * 0.018
					enemy_scale_mult = w2b
					var em2: Color = KyndeBladeArtPalette.JEWEL_EMERALD
					var feint2 := Color(em2.r * 1.05, em2.g * 1.08, em2.b * 1.12, 1.0)
					_modulate_enemy_mesh(feint2.lerp(Color.WHITE, 0.45))
				player_scale_mult = 0.99
				_modulate_player_mesh(Color(0.9, 0.92, 0.98, 1.0))
				if holding_telegraph:
					_telegraph_hold_left = maxf(0.0, _telegraph_hold_left - delta)
			_:
				player_root.position = _player_base
				enemy_root.position = _enemy_base
				enemy_scale_mult = 1.0
				player_scale_mult = 1.0
				_modulate_enemy_mesh(Color.WHITE)
				_modulate_player_mesh(Color.WHITE)

	_apply_actor_scales(player_scale_mult, enemy_scale_mult)
	_update_cinematic_camera(delta, st, bob_t)

	if _hit_flash_t > 0.0:
		_hit_flash_t = maxf(0.0, _hit_flash_t - delta)
		var k := clampf(_hit_flash_t / HIT_FLASH_SEC, 0.0, 1.0)
		var rub := KyndeBladeArtPalette.RUBRICATION
		var flash := Color(rub.r, rub.g, rub.b, 1.0)
		var base_col := Color.WHITE.lerp(flash, (1.0 - k) * 0.85)
		_modulate_player_mesh(base_col)

	if hit_flash_light.visible and hit_flash_light.light_energy > 0.05:
		hit_flash_light.light_energy = maxf(0.0, hit_flash_light.light_energy - delta * 12.0)
		if hit_flash_light.light_energy <= 0.05:
			hit_flash_light.light_energy = 0.0
			hit_flash_light.visible = false


func _apply_actor_scales(player_state_scale: float, enemy_state_scale: float) -> void:
	var p_depth := clampf((player_root.position.y - _player_base.y) / 0.35, -1.0, 1.0)
	var e_depth := clampf((enemy_root.position.y - _enemy_base.y) / 0.45, -1.0, 1.0)
	var p_scale := (1.0 + p_depth * 0.035) * player_state_scale
	var e_scale := (1.0 + e_depth * 0.045) * enemy_state_scale
	player_root.scale = _player_model_scale * p_scale
	enemy_root.scale = _enemy_model_scale * e_scale


func _update_cinematic_camera(delta: float, st: CombatManager.State, bob_t: float) -> void:
	if _shake_t > 0.0:
		_shake_t = maxf(0.0, _shake_t - delta)
	var mood := 0.0
	if st == CombatManager.State.DEFENSE_WINDUP:
		mood = 0.65
	elif st == CombatManager.State.REAL_TIME_WINDOW:
		mood = 1.0
	elif st == CombatManager.State.EXECUTING:
		mood = 0.35
	var desired := _desired_camera_pos(mood, st, bob_t)
	var mid := _midpoint()
	_cam_offset_lerped = _cam_offset_lerped.lerp(desired, clampf(delta * CAM_LERP, 0.0, 1.0))
	var shake := Vector3.ZERO
	if _shake_t > 0.0:
		var mag := SHAKE_MAG * clampf(_shake_t / SHAKE_SEC, 0.0, 1.0)
		shake = _shake_dir * mag
	camera_3d.global_position = _cam_offset_lerped + shake
	camera_3d.look_at(mid, Vector3.UP)

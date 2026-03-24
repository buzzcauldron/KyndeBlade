extends Node2D
## Wille / dreamer moveset presentation: **ink blobs** as `RigidBody2D` with high damping (viscous) + gravity.
## Listens to `CombatManager.dreamer_move_committed`. Presentation-only; does not affect combat math.

const INK_GROUP := &"dreamer_ink_blob"
const MAX_BLOBS := 48
const BLOB_LIFETIME_SEC := 3.2

@export var strike_blob_count: int = 16
@export var dodge_blob_count: int = 12
@export var parry_blob_count: int = 10
@export var strike_impulse: float = 220.0
@export var dodge_impulse: float = 180.0
@export var parry_impulse: float = 140.0

var _combat: CombatManager
@onready var _player_root: Node2D = $"../PlayerRoot"
@onready var _enemy_root: Node2D = $"../EnemyRoot"


func _ready() -> void:
	var stage := get_parent()
	if stage == null:
		return
	var root := stage.get_parent()
	if root:
		_combat = root.get_node_or_null("CombatManager") as CombatManager
	if _combat:
		_combat.dreamer_move_committed.connect(_on_dreamer_move)


func _on_dreamer_move(kind: CombatManager.DreamerMoveKind) -> void:
	if _player_root == null:
		return
	var ppos: Vector2 = _player_root.global_position
	var toward_enemy := Vector2.RIGHT
	if _enemy_root:
		toward_enemy = (_enemy_root.global_position - ppos).normalized()
		if toward_enemy.length_squared() < 0.01:
			toward_enemy = Vector2.RIGHT
	var up := Vector2(0, -1)
	match kind:
		CombatManager.DreamerMoveKind.STRIKE:
			_burst_strike(ppos, toward_enemy, up)
		CombatManager.DreamerMoveKind.DODGE:
			_burst_dodge(ppos, toward_enemy, up)
		CombatManager.DreamerMoveKind.PARRY:
			_burst_parry(ppos, toward_enemy, up)


func _trim_old_blobs() -> void:
	var mine: Array[RigidBody2D] = []
	for c in get_children():
		if c is RigidBody2D and c.is_in_group(INK_GROUP):
			mine.append(c)
	while mine.size() > MAX_BLOBS:
		var drop: RigidBody2D = mine.pop_front()
		if is_instance_valid(drop):
			drop.queue_free()


func _burst_strike(ppos: Vector2, toward: Vector2, up: Vector2) -> void:
	for i in strike_blob_count:
		var spread: float = deg_to_rad(randf_range(-35.0, 35.0))
		var dir: Vector2 = toward.rotated(spread) + up * randf_range(0.15, 0.55)
		dir = dir.normalized()
		var local_pos: Vector2 = to_local(ppos) + Vector2(randf_range(-10, 14), randf_range(-18, 10))
		_spawn_ink_blob(local_pos, dir * randf_range(strike_impulse * 0.45, strike_impulse), randf_range(0.85, 1.25))
	_trim_old_blobs()


func _burst_dodge(ppos: Vector2, toward: Vector2, up: Vector2) -> void:
	var back: Vector2 = -toward
	for i in dodge_blob_count:
		var dir: Vector2 = (back * randf_range(0.65, 1.0) + up * randf_range(0.35, 0.95)).normalized()
		var local_pos: Vector2 = to_local(ppos) + Vector2(randf_range(-12, 12), randf_range(-8, 16))
		_spawn_ink_blob(local_pos, dir * randf_range(dodge_impulse * 0.4, dodge_impulse), randf_range(0.7, 1.1))
	_trim_old_blobs()


func _burst_parry(ppos: Vector2, toward: Vector2, up: Vector2) -> void:
	for i in parry_blob_count:
		var dir: Vector2 = (toward * randf_range(0.2, 0.55) + up * randf_range(0.4, 0.85)).normalized()
		var local_pos: Vector2 = to_local(ppos) + Vector2(randf_range(4, 22), randf_range(-24, -4))
		_spawn_ink_blob(local_pos, dir * randf_range(parry_impulse * 0.35, parry_impulse), randf_range(0.65, 1.0))
	_trim_old_blobs()


func _ink_color() -> Color:
	var base := KyndeBladeArtPalette.INK_PRIMARY
	return Color(
			clampf(base.r + randf_range(-0.06, 0.06), 0.0, 1.0),
			clampf(base.g + randf_range(-0.06, 0.06), 0.0, 1.0),
			clampf(base.b + randf_range(-0.04, 0.08), 0.0, 1.0),
			randf_range(0.88, 1.0)
	)


func _spawn_ink_blob(local_pos: Vector2, linear_vel: Vector2, scale_mult: float) -> void:
	var body := RigidBody2D.new()
	body.collision_layer = 0
	body.collision_mask = 1
	body.gravity_scale = 1.05
	body.linear_damp = 5.8
	body.angular_damp = 4.5
	body.mass = clampf(randf_range(0.08, 0.16) * scale_mult, 0.05, 0.22)
	body.continuous_cd = RigidBody2D.CCD_MODE_DISABLED
	body.add_to_group(INK_GROUP)

	var col := CollisionShape2D.new()
	var circle := CircleShape2D.new()
	circle.radius = randf_range(2.8, 5.5) * scale_mult
	col.shape = circle
	body.add_child(col)

	var ink := Polygon2D.new()
	ink.polygon = _blob_polygon(circle.radius)
	ink.color = _ink_color()
	body.add_child(ink)

	body.position = local_pos
	body.linear_velocity = linear_vel
	body.angular_velocity = randf_range(-5.0, 5.0)
	add_child(body)

	get_tree().create_timer(BLOB_LIFETIME_SEC).timeout.connect(_fade_free_blob.bind(body))


func _fade_free_blob(body: RigidBody2D) -> void:
	if not is_instance_valid(body):
		return
	var tw := create_tween()
	tw.tween_property(body, "modulate:a", 0.0, 0.35)
	tw.finished.connect(body.queue_free)


func _blob_polygon(radius: float) -> PackedVector2Array:
	var pts := PackedVector2Array()
	var n := 7
	for i in n:
		var a := TAU * float(i) / float(n)
		pts.append(Vector2(cos(a), sin(a)) * radius)
	return pts

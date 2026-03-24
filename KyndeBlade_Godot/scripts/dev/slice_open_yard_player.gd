extends CharacterBody2D
## Prologue wanderer (Langland opening): soft pace on the lea; same physics as hi-bit slice.

const MOVE_SPEED := 46.0
const JUMP_VELOCITY := -220.0
const GRAVITY := 620.0

var spawn_position: Vector2
var _frozen: bool = false

@onready var _figure: Node2D = $Figure


func _ready() -> void:
	floor_snap_length = 6.0
	add_to_group("slice_open_yard_player")
	spawn_position = global_position


func freeze() -> void:
	_frozen = true
	velocity = Vector2.ZERO


func respawn() -> void:
	_frozen = false
	velocity = Vector2.ZERO
	global_position = spawn_position
	if is_instance_valid(_figure):
		_figure.rotation_degrees = 0.0
		_figure.position = Vector2.ZERO


func play_dream_lie_down() -> void:
	if not is_instance_valid(_figure):
		return
	var tw := create_tween()
	tw.set_parallel(true)
	tw.tween_property(_figure, "rotation_degrees", -92.0, 0.42).set_ease(Tween.EASE_OUT).set_trans(
			Tween.TRANS_QUAD
	)
	tw.tween_property(_figure, "position", Vector2(8, 11), 0.42).set_ease(Tween.EASE_OUT).set_trans(
			Tween.TRANS_QUAD
	)
	await tw.finished


func _physics_process(delta: float) -> void:
	if _frozen:
		return
	if not is_on_floor():
		velocity.y += GRAVITY * delta
	else:
		velocity.y = 0.0
		if Input.is_action_just_pressed("hi_bit_jump"):
			velocity.y = JUMP_VELOCITY
	var dir := Input.get_axis("hi_bit_move_left", "hi_bit_move_right")
	velocity.x = dir * MOVE_SPEED
	move_and_slide()

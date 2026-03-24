extends CharacterBody2D
## Quirky mini-knight for the high-bit bonus: run, jump, respawn.

const MOVE_SPEED := 88.0
const JUMP_VELOCITY := -248.0
const GRAVITY := 620.0

var spawn_position: Vector2
var _frozen: bool = false


func _ready() -> void:
	floor_snap_length = 6.0
	add_to_group("hi_bit_bonus_player")
	spawn_position = global_position


func freeze() -> void:
	_frozen = true
	velocity = Vector2.ZERO


func respawn() -> void:
	_frozen = false
	velocity = Vector2.ZERO
	global_position = spawn_position


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

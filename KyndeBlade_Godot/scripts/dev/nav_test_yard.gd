extends Node
## Greybox tactical yard: click floor (layer 1) to pathfind. Gate closed = two nav islands; open = merged plane.

const MAIN_MENU := "res://scenes/main_menu.tscn"
const MOVE_SPEED := 5.5
const FLOOR_COLLISION_LAYER := 1

@onready var _nav_region: NavigationRegion3D = %NavRegion
@onready var _player: CharacterBody3D = %Player
@onready var _agent: NavigationAgent3D = %NavAgent
@onready var _camera: Camera3D = %YardCamera
@onready var _gate_body: StaticBody3D = %GateBody
@onready var _gate_mesh: MeshInstance3D = %GateMesh
@onready var _gate_obstacle: NavigationObstacle3D = %GateObstacle
@onready var _hint: Label = %HintLabel
@onready var _dist: Label = %DistLabel
@onready var _gate_check: CheckButton = %GateCheck

var _gate_open: bool = false


func _ready() -> void:
	_agent.avoidance_enabled = true
	_agent.velocity_computed.connect(_on_velocity_computed)
	_gate_check.toggled.connect(_on_gate_toggled)
	%BackButton.pressed.connect(_on_back_pressed)
	_apply_gate_visual_and_collision()
	_rebuild_navigation_mesh()
	_hint.text = "Click the ground to move · Open the gate to merge the yard (pathfinding)"
	await get_tree().physics_frame
	await get_tree().physics_frame
	_agent.target_position = _player.global_position


func _rebuild_navigation_mesh() -> void:
	var nm := NavigationMesh.new()
	const y := 0.05
	const h := 12.0
	const gap := 0.35
	if _gate_open:
		nm.set_vertices(PackedVector3Array([
				Vector3(-h, y, -h),
				Vector3(h, y, -h),
				Vector3(h, y, h),
				Vector3(-h, y, h),
		]))
		nm.add_polygon(PackedInt32Array([0, 1, 2]))
		nm.add_polygon(PackedInt32Array([0, 2, 3]))
	else:
		nm.set_vertices(PackedVector3Array([
				Vector3(-h, y, -h),
				Vector3(-gap, y, -h),
				Vector3(-gap, y, h),
				Vector3(-h, y, h),
				Vector3(gap, y, -h),
				Vector3(h, y, -h),
				Vector3(h, y, h),
				Vector3(gap, y, h),
		]))
		nm.add_polygon(PackedInt32Array([0, 1, 2]))
		nm.add_polygon(PackedInt32Array([0, 2, 3]))
		nm.add_polygon(PackedInt32Array([4, 5, 6]))
		nm.add_polygon(PackedInt32Array([4, 6, 7]))
	_nav_region.navigation_mesh = nm


func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		var hit := _raycast_floor(event.position)
		if hit.has("position"):
			_agent.target_position = hit.position


func _raycast_floor(screen_pos: Vector2) -> Dictionary:
	var space := _player.get_world_3d().direct_space_state
	var from := _camera.project_ray_origin(screen_pos)
	var to := from + _camera.project_ray_normal(screen_pos) * 250.0
	var q := PhysicsRayQueryParameters3D.create(from, to)
	q.collision_mask = FLOOR_COLLISION_LAYER
	return space.intersect_ray(q)


func _physics_process(_delta: float) -> void:
	if _agent.is_navigation_finished():
		_player.velocity = Vector3.ZERO
		_player.move_and_slide()
		_dist.text = "Idle / arrived"
		return
	var next := _agent.get_next_path_position()
	var flat := Vector3(next.x - _player.global_position.x, 0.0, next.z - _player.global_position.z)
	if flat.length_squared() < 0.0025:
		var pts := _agent.get_current_navigation_path()
		if pts.size() >= 2:
			var p2: Vector3 = pts[1]
			flat = Vector3(p2.x - _player.global_position.x, 0.0, p2.z - _player.global_position.z)
	var vel := Vector3.ZERO
	if flat.length_squared() > 0.0001:
		vel = flat.normalized() * MOVE_SPEED
	_agent.set_velocity(vel)
	_dist.text = "To target: %.1f" % _player.global_position.distance_to(_agent.target_position)


func _on_velocity_computed(safe_velocity: Vector3) -> void:
	if _agent.is_navigation_finished():
		return
	_player.velocity = safe_velocity
	_player.move_and_slide()


func _on_back_pressed() -> void:
	await ManuscriptNav.turn_page_to(MAIN_MENU)


func _on_gate_toggled(opened: bool) -> void:
	_gate_open = opened
	_apply_gate_visual_and_collision()
	_rebuild_navigation_mesh()


func _apply_gate_visual_and_collision() -> void:
	if _gate_open:
		_gate_body.collision_layer = 0
		_gate_mesh.visible = false
		if _gate_obstacle:
			_gate_obstacle.avoidance_enabled = false
	else:
		_gate_body.collision_layer = 2
		_gate_mesh.visible = true
		if _gate_obstacle:
			_gate_obstacle.avoidance_enabled = true


## For tests: open gate so the nav mesh is one island (long paths east-west).
func set_gate_open_for_tests(opened: bool) -> void:
	_gate_open = opened
	_apply_gate_visual_and_collision()
	_rebuild_navigation_mesh()
	_gate_check.set_pressed_no_signal(opened)

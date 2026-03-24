extends Node3D
## Procedural voxel grid for the 3D arena; see docs/COMBAT_VOXEL_STAGE_FUTURE.md.
## Combat rules stay in `CombatManager`.

@export var enabled: bool = true
@export var voxel_size: float = 0.42
@export var grid_half: int = 12
@export var floor_thickness_ratio: float = 0.42
@export var wall_height_blocks: int = 4
@export var hide_classic_ground: bool = true


func _ready() -> void:
	if not enabled:
		return
	if hide_classic_ground:
		var g := get_parent().get_node_or_null("Ground") as MeshInstance3D
		if g:
			g.visible = false
	_build_floor()
	_build_walls()


func _make_mat(albedo: Color) -> StandardMaterial3D:
	var m := StandardMaterial3D.new()
	m.albedo_color = albedo
	m.roughness = 0.92
	m.metallic = 0.05
	return m


func _build_floor() -> void:
	var box := BoxMesh.new()
	box.size = Vector3(voxel_size * 0.97, voxel_size * floor_thickness_ratio, voxel_size * 0.97)
	var mm := MultiMesh.new()
	mm.transform_format = MultiMesh.TRANSFORM_3D
	mm.mesh = box
	var span := grid_half * 2 + 1
	mm.instance_count = span * span
	var i := 0
	var y0 := voxel_size * floor_thickness_ratio * -0.5 + 0.015
	for x in range(-grid_half, grid_half + 1):
		for z in range(-grid_half, grid_half + 1):
			var pos := Vector3(float(x) * voxel_size, y0, float(z) * voxel_size)
			mm.set_instance_transform(i, Transform3D(Basis.IDENTITY, pos))
			i += 1
	var mmi := MultiMeshInstance3D.new()
	mmi.multimesh = mm
	mmi.material_override = _make_mat(Color(0.13, 0.15, 0.21))
	add_child(mmi)


func _build_walls() -> void:
	var positions: PackedVector3Array = PackedVector3Array()
	for x in range(-grid_half, grid_half + 1):
		for z in range(-grid_half, grid_half + 1):
			var on_edge: bool = abs(x) == grid_half or abs(z) == grid_half
			if not on_edge:
				continue
			for h in range(wall_height_blocks):
				positions.append(Vector3(float(x), float(h), float(z)))
	if positions.is_empty():
		return
	var box := BoxMesh.new()
	box.size = Vector3(voxel_size * 0.94, voxel_size * 0.94, voxel_size * 0.94)
	var mm := MultiMesh.new()
	mm.transform_format = MultiMesh.TRANSFORM_3D
	mm.mesh = box
	mm.instance_count = positions.size()
	var floor_top_y: float = voxel_size * floor_thickness_ratio * 0.5 + (
			voxel_size * floor_thickness_ratio * -0.5 + 0.015
	)
	var s := voxel_size
	for j in range(positions.size()):
		var p: Vector3 = positions[j]
		var cy: float = floor_top_y + voxel_size * 0.5 + p.y * voxel_size
		var world := Vector3(p.x * s, cy, p.z * s)
		mm.set_instance_transform(j, Transform3D(Basis.IDENTITY, world))
	var mmi := MultiMeshInstance3D.new()
	mmi.multimesh = mm
	mmi.material_override = _make_mat(Color(0.09, 0.11, 0.17))
	add_child(mmi)

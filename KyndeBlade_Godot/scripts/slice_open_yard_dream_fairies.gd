extends Node2D
## Tiny luminous motes that wheel about the dreamer during the Malvern prologue sleep beat.

const FAIRY_COUNT := 9

var _running: bool = false
var _t: float = 0.0
var _anchor: Vector2 = Vector2.ZERO


func start_swarm(anchor_global: Vector2) -> void:
	stop_swarm()
	_anchor = anchor_global
	_running = true
	_t = 0.0
	z_index = 6
	for i in FAIRY_COUNT:
		var holder := Node2D.new()
		holder.name = "Fairy%d" % i
		var poly := Polygon2D.new()
		poly.z_index = 1
		poly.polygon = PackedVector2Array(
				[
					Vector2(-2, 0),
					Vector2(-1, -3),
					Vector2(2, -1),
					Vector2(1, 3),
					Vector2(-2, 2),
				]
		)
		match i % 3:
			0:
				var g := KyndeBladeArtPalette.GOLD
				poly.color = Color(g.r, g.g, g.b, 0.9)
			1:
				poly.color = Color(
						KyndeBladeArtPalette.JEWEL_EMERALD.r,
						KyndeBladeArtPalette.JEWEL_EMERALD.g,
						KyndeBladeArtPalette.JEWEL_EMERALD.b,
						0.82
				)
			_:
				poly.color = Color(0.72, 0.58, 0.92, 0.78)
		holder.add_child(poly)
		add_child(holder)


func stop_swarm() -> void:
	_running = false
	for c in get_children():
		c.queue_free()


func _process(dt: float) -> void:
	if not _running:
		return
	_t += dt
	var n := get_child_count()
	if n == 0:
		return
	var idx := 0
	for child in get_children():
		if not (child is Node2D):
			idx += 1
			continue
		var node2d := child as Node2D
		var phase := float(idx) * TAU / float(n)
		var orbit_r := 20.0 + sin(_t * 2.1 + phase * 2.0) * 12.0
		var orbit_a := _t * 1.35 + phase
		var bob := sin(_t * 3.4 + phase * 1.7) * 6.0
		node2d.global_position = (
				_anchor
				+ Vector2(cos(orbit_a) * orbit_r, sin(orbit_a) * orbit_r * 0.5 + bob)
		)
		node2d.rotation = orbit_a * 0.2 + sin(_t * 5.0 + phase) * 0.35
		idx += 1

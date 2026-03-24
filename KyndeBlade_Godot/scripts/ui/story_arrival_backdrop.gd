extends Control
## Lane A arrival: **twilight → mist** vertical ramp (ART_DIRECTION_GODOT — hub / tower vista).

func _ready() -> void:
	set_anchors_preset(PRESET_FULL_RECT)
	offset_left = 0.0
	offset_top = 0.0
	offset_right = 0.0
	offset_bottom = 0.0
	mouse_filter = MOUSE_FILTER_IGNORE
	z_index = -10
	resized.connect(queue_redraw)


func _draw() -> void:
	var r := Rect2(Vector2.ZERO, size)
	if r.size.x < 1.0 or r.size.y < 1.0:
		return
	var steps := 16
	var hh: float = r.size.y / float(steps)
	for i in steps:
		var t := float(i) / float(max(1, steps - 1))
		var c: Color = KyndeBladeArtPalette.HUB_TWILIGHT.lerp(KyndeBladeArtPalette.HUB_MIST, t)
		draw_rect(Rect2(r.position + Vector2(0, hh * i), Vector2(r.size.x, hh + 1.0)), c, true)
	# Low bruised horizon (coastal haze note): single sick violet whisper.
	var hz := r.position.y + r.size.y * 0.72
	draw_rect(
			Rect2(r.position.x, hz, r.size.x, r.size.y * 0.28 + 2.0),
			Color(
					KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW.r,
					KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW.g,
					KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW.b,
					0.06
			),
			true
	)

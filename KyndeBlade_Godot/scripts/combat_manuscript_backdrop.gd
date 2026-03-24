extends Control
## Lane B backdrop: **hi-bit ruin vista** + **crawl-style parallax** via [`CrawlParallax`](crawl_parallax.gd).
## Visual target: [`assets/hi_bit_ruin_vista/reference_style_target.png`](../assets/hi_bit_ruin_vista/reference_style_target.png).
## Optional **jewel wash** (`jewel_wash_strength`): Salome / Pre-Raphaelite contamination over sky—see `KyndeBladeArtPalette.JEWEL_*`.

const _HI_BIT_STYLE_REF := preload("res://assets/hi_bit_ruin_vista/reference_style_target.png")

@export var manuscript_void_blend: float = 0.12
@export var parallax_enabled: bool = true
@export var parallax_speed_scale: float = 1.0
## Draw the committed **hi-bit style bible** PNG under translucent procedural sky bands (ART_DIRECTION_GODOT — reference + procedural mix).
@export var hi_bit_reference_underlay: bool = true
## Opacity of procedural sky bands when [member hi_bit_reference_underlay] is on (0.35–0.75 typical).
@export_range(0.2, 1.0, 0.02) var procedural_sky_alpha: float = 0.56
## Optional full-frame mood plate (e.g. concept or export). Used when [member paint_mood_texture_only] is on.
@export var mood_texture: Texture2D
## **true** = draw only [member mood_texture] scaled to the control (no procedural parallax). **false** = procedural hi-bit vista (default).
@export var paint_mood_texture_only: bool = false
## Salome / Pre-Raphaelite contamination: 0 = off, ~0.12–0.22 = subtle crimson→violet wash over sky.
@export_range(0.0, 0.35, 0.01) var jewel_wash_strength: float = 0.14
## Skyward end of the wash gradient: lerp from violet toward ultramarine (0 = crimson→violet only).
@export_range(0.0, 1.0, 0.05) var jewel_wash_ultramarine_mix: float = 0.12

var _time_sec: float = 0.0


func _ready() -> void:
	set_anchors_preset(PRESET_FULL_RECT)
	offset_left = 0.0
	offset_top = 0.0
	offset_right = 0.0
	offset_bottom = 0.0
	mouse_filter = MOUSE_FILTER_IGNORE
	resized.connect(queue_redraw)


func _process(delta: float) -> void:
	if not parallax_enabled:
		return
	if get_tree().paused:
		return
	_time_sec += delta
	queue_redraw()


func _shift_poly(pts: PackedVector2Array, ox: float, oy: float) -> PackedVector2Array:
	var out: PackedVector2Array = PackedVector2Array()
	out.resize(pts.size())
	for i in pts.size():
		out[i] = pts[i] + Vector2(ox, oy)
	return out


func _draw() -> void:
	var w: float = size.x
	var h: float = size.y
	if w < 1.0 or h < 1.0:
		return
	var void_c: Color = KyndeBladeArtPalette.COMBAT_VOID
	var sc: float = parallax_speed_scale
	var t: float = _time_sec if parallax_enabled else 0.0

	if paint_mood_texture_only and mood_texture != null:
		draw_texture_rect(mood_texture, Rect2(0.0, 0.0, w, h), false)
		return

	var o_sky: Vector2 = CrawlParallax.offset(t, CrawlParallax.Layer.SKY, sc)
	var sky_a: float = procedural_sky_alpha if hi_bit_reference_underlay else 1.0

	if hi_bit_reference_underlay:
		draw_texture_rect(_HI_BIT_STYLE_REF, Rect2(0.0, 0.0, w, h), false)
		# Distant weather: cool corners (grey–teal haze note).
		var cc := KyndeBladeArtPalette.COMBAT_VOID_COOL
		draw_rect(Rect2(0.0, 0.0, w * 0.24, h * 0.42), Color(cc.r, cc.g, cc.b, 0.14), true)
		draw_rect(Rect2(w * 0.76, 0.0, w * 0.26, h * 0.45), Color(cc.r, cc.g, cc.b, 0.12), true)

	# --- Sky bands (subtle per-band phase) ---
	var bands: int = 10
	for i in bands:
		var t0: float = float(i) / float(bands)
		var t1: float = float(i + 1) / float(bands)
		var mid: float = (t0 + t1) * 0.5
		var c: Color
		if mid < 0.35:
			c = KyndeBladeArtPalette.HI_BIT_SKY_TEAL.lerp(KyndeBladeArtPalette.HI_BIT_SKY_MIST, mid / 0.35)
		elif mid < 0.65:
			var u: float = (mid - 0.35) / 0.3
			c = KyndeBladeArtPalette.HI_BIT_SKY_MIST.lerp(KyndeBladeArtPalette.HI_BIT_SKY_PEACH, u)
		else:
			var v: float = (mid - 0.65) / 0.35
			c = KyndeBladeArtPalette.HI_BIT_SKY_PEACH.lerp(KyndeBladeArtPalette.HI_BIT_SKY_GLOW, v)
		c = Color(c.r, c.g, c.b, c.a * sky_a)
		var band_ox: float = o_sky.x + sin(t * 0.2 + float(i) * 0.35) * 1.5
		draw_rect(Rect2(band_ox, h * t0, w + 8.0, h * (t1 - t0) + 1.0), c)

	# --- Jewel wash (crimson → violet, low alpha): “cool / wrong” layer over hi-bit sky ---
	if jewel_wash_strength > 0.001:
		var strips: int = 9
		var y0: float = h * 0.12
		var y1: float = h * 0.56
		for j in strips:
			var sj: float = float(j) / float(max(1, strips - 1))
			var y_a: float = lerpf(y0, y1, float(j) / float(strips))
			var y_b: float = lerpf(y0, y1, float(j + 1) / float(strips))
			var wash_end: Color = KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW.lerp(
					KyndeBladeArtPalette.JEWEL_ULTRAMARINE, clampf(jewel_wash_ultramarine_mix, 0.0, 1.0)
			)
			var wash_c: Color = KyndeBladeArtPalette.JEWEL_CRIMSON.lerp(wash_end, sj)
			var a: float = jewel_wash_strength * lerpf(0.04, 0.2, sj)
			draw_rect(Rect2(o_sky.x * 0.3, y_a, w + 12.0, y_b - y_a + 0.5), Color(wash_c.r, wash_c.g, wash_c.b, a))

	var o_far: Vector2 = CrawlParallax.offset(t, CrawlParallax.Layer.FAR_SILHOUETTE, sc)
	draw_colored_polygon(
			_shift_poly(
					PackedVector2Array([
						Vector2(0, h * 0.42),
						Vector2(w * 0.22, h * 0.38),
						Vector2(w * 0.45, h * 0.44),
						Vector2(w * 0.72, h * 0.36),
						Vector2(w, h * 0.4),
						Vector2(w, h * 0.58),
						Vector2(0, h * 0.58),
					]),
					o_far.x,
					o_far.y
			),
			Color(KyndeBladeArtPalette.HI_BIT_SILHOUETTE_FAR.r, KyndeBladeArtPalette.HI_BIT_SILHOUETTE_FAR.g, KyndeBladeArtPalette.HI_BIT_SILHOUETTE_FAR.b, 0.55)
	)

	var o_mid: Vector2 = CrawlParallax.offset(t, CrawlParallax.Layer.MID_SILHOUETTE, sc)
	draw_colored_polygon(
			_shift_poly(
					PackedVector2Array([
						Vector2(0, h * 0.48),
						Vector2(w * 0.18, h * 0.46),
						Vector2(w * 0.38, h * 0.52),
						Vector2(w * 0.62, h * 0.47),
						Vector2(w * 0.88, h * 0.5),
						Vector2(w, h * 0.49),
						Vector2(w, h * 0.66),
						Vector2(0, h * 0.66),
					]),
					o_mid.x,
					o_mid.y
			),
			Color(KyndeBladeArtPalette.HI_BIT_SILHOUETTE_MID.r, KyndeBladeArtPalette.HI_BIT_SILHOUETTE_MID.g, KyndeBladeArtPalette.HI_BIT_SILHOUETTE_MID.b, 0.45)
	)

	var o_ruin: Vector2 = CrawlParallax.offset(t, CrawlParallax.Layer.RUIN_MASS, sc)
	var rx: float = w * 0.02 + o_ruin.x
	var ry_off: float = o_ruin.y
	var rw: float = w * 0.38
	draw_rect(
			Rect2(rx, h * 0.28 + ry_off, rw * 0.12, h * 0.42),
			KyndeBladeArtPalette.HI_BIT_TERRACOTTA_DEEP
	)
	draw_rect(
			Rect2(rx + rw * 0.1, h * 0.32 + ry_off, rw * 0.85, h * 0.08),
			KyndeBladeArtPalette.HI_BIT_TERRACOTTA
	)
	draw_rect(
			Rect2(rx + rw * 0.22, h * 0.4 + ry_off, rw * 0.14, h * 0.32),
			KyndeBladeArtPalette.HI_BIT_TERRACOTTA.lerp(KyndeBladeArtPalette.HI_BIT_TEAL_SHADOW, 0.25)
	)
	draw_rect(
			Rect2(rx + rw * 0.4, h * 0.38 + ry_off, rw * 0.16, h * 0.36),
			KyndeBladeArtPalette.HI_BIT_TERRACOTTA
	)
	draw_rect(
			Rect2(rx + rw * 0.42, h * 0.62 + ry_off, rw * 0.12, h * 0.14),
			KyndeBladeArtPalette.HI_BIT_TEAL_SHADOW.lerp(Color.BLACK, 0.18)
	)

	# Wet stone sheen (cool grey–teal vs warm brick — art note).
	var sheen_c := Color(
			KyndeBladeArtPalette.HI_BIT_SKY_MIST.r,
			KyndeBladeArtPalette.HI_BIT_SKY_MIST.g,
			KyndeBladeArtPalette.HI_BIT_SKY_MIST.b,
			0.11
	)
	var sx0: float = rx + rw * 0.08
	for k in 5:
		var yy: float = h * (0.34 + float(k) * 0.048) + ry_off
		draw_line(Vector2(sx0, yy), Vector2(rx + rw * 0.92, yy + 0.5), sheen_c, 1.0)

	var o_mist: Vector2 = CrawlParallax.offset(t, CrawlParallax.Layer.MIST_WASH, sc)
	draw_rect(
			Rect2(o_mist.x * 0.5, h * 0.35 + o_mist.y * 0.3, w + 16.0, h * 0.35),
			Color(0.88, 0.9, 0.89, 0.14)
	)

	var o_ground: Vector2 = CrawlParallax.offset(t, CrawlParallax.Layer.GROUND, sc)
	var ground_top: float = h * 0.62 + o_ground.y * 0.25
	draw_rect(Rect2(o_ground.x * 0.35, ground_top, w + 20.0, h - ground_top + 4.0), KyndeBladeArtPalette.HI_BIT_SAGE_DEEP)
	var ground_line: Color = KyndeBladeArtPalette.HI_BIT_FOLIAGE.lerp(KyndeBladeArtPalette.HI_BIT_SAGE, 0.5)
	var ground_line_a := Color(ground_line.r, ground_line.g, ground_line.b, 0.28)
	for i in 6:
		var y: float = ground_top + float(i) * h * 0.055
		draw_line(
				Vector2(o_ground.x * 0.2, y),
				Vector2(w + o_ground.x * 0.2, y + 1),
				ground_line_a,
				1.0
		)
	var bot_blend: float = clampf(manuscript_void_blend, 0.0, 0.45)
	draw_rect(
			Rect2(0, h * (1.0 - bot_blend * 0.55), w, h * bot_blend * 0.55 + 2.0),
			Color(void_c.r, void_c.g, void_c.b, 0.55)
	)

	var o_tower: Vector2 = CrawlParallax.offset(t, CrawlParallax.Layer.TOWER, sc)
	var tw: float = w * 0.07
	var tx: float = w * 0.78 + o_tower.x
	var ty: float = o_tower.y
	draw_rect(Rect2(tx, h * 0.22 + ty, tw, h * 0.48), KyndeBladeArtPalette.HI_BIT_TEAL_SHADOW)
	draw_rect(Rect2(tx + tw * 0.12, h * 0.14 + ty, tw * 0.76, h * 0.12), KyndeBladeArtPalette.HI_BIT_SILHOUETTE_MID)
	draw_colored_polygon(
			PackedVector2Array([
				Vector2(tx + tw * 0.5, h * 0.07 + ty),
				Vector2(tx + tw * 0.92, h * 0.15 + ty),
				Vector2(tx + tw * 0.08, h * 0.15 + ty),
			]),
			KyndeBladeArtPalette.HI_BIT_TERRACOTTA_DEEP
	)

	var o_fg: Vector2 = CrawlParallax.offset(t, CrawlParallax.Layer.FOREGROUND, sc)
	var rng := RandomNumberGenerator.new()
	rng.seed = 2244
	var fol_base: Color = KyndeBladeArtPalette.HI_BIT_FOLIAGE
	var fol_dot_a := Color(fol_base.r, fol_base.g, fol_base.b, 0.22)
	var sage_base: Color = KyndeBladeArtPalette.HI_BIT_SAGE
	var sage_dot_a := Color(sage_base.r, sage_base.g, sage_base.b, 0.2)
	for i in 28:
		var fx: float = rng.randf_range(0, w * 0.35)
		var fy: float = rng.randf_range(h * 0.72, h * 0.98)
		draw_circle(
				Vector2(fx + o_fg.x, fy + o_fg.y),
				rng.randf_range(2.0, 9.0),
				fol_dot_a
		)
	for i in 22:
		var fx2: float = rng.randf_range(w * 0.65, w)
		var fy2: float = rng.randf_range(h * 0.74, h * 0.98)
		draw_circle(
				Vector2(fx2 + o_fg.x * 1.1, fy2 + o_fg.y * 1.1),
				rng.randf_range(2.0, 8.0),
				sage_dot_a
		)

	var o_dith: Vector2 = CrawlParallax.offset(t, CrawlParallax.Layer.DITHER, sc)
	rng.seed = 1337
	for i in 168:
		var sx: float = rng.randf_range(0, w)
		var sy: float = rng.randf_range(0, h * 0.5)
		draw_circle(Vector2(sx + o_dith.x, sy + o_dith.y), rng.randf_range(0.4, 1.1), KyndeBladeArtPalette.HI_BIT_DITHER)

	var o_folk: Vector2 = CrawlParallax.offset(t, CrawlParallax.Layer.FOLK_DUST, sc)
	rng.seed = 1338
	for i in 32:
		var px: float = rng.randf_range(w * 0.2, w * 0.95)
		var py: float = rng.randf_range(h * 0.64, h * 0.92)
		draw_circle(Vector2(px + o_folk.x, py + o_folk.y), rng.randf_range(0.5, 1.3), Color(0.55, 0.58, 0.52, 0.18))

class_name PlaceholderLocationBackdrop
extends Control
## Lane A / hi-bit **read** backdrop for location preview shell — driven by [`data/art/placeholder_registry.json`](../../data/art/placeholder_registry.json) `levels` + art bible bands in [`ART_DIRECTION_GODOT.md`](../../docs/ART_DIRECTION_GODOT.md).

var _location_id: String = ""
var _preset: String = "twilight_tower_mist"
var _jewel: float = 0.06


func _ready() -> void:
	mouse_filter = Control.MOUSE_FILTER_IGNORE
	set_anchors_preset(Control.PRESET_FULL_RECT)


func set_location_id(loc_id: String) -> void:
	_location_id = loc_id
	if loc_id.is_empty():
		_preset = "twilight_tower_mist"
		_jewel = 0.06
	else:
		_preset = PlaceholderArtRegistry.level_preset(loc_id)
		_jewel = PlaceholderArtRegistry.level_jewel_wash(loc_id)
	queue_redraw()


func _draw() -> void:
	var r := get_rect()
	if r.size.x < 1.0 or r.size.y < 1.0:
		return
	match _preset:
		"malvern_dawn_band":
			_draw_gradient_vertical(r, KyndeBladeArtPalette.HI_BIT_SKY_PEACH, KyndeBladeArtPalette.HI_BIT_SKY_TEAL)
			_draw_band(r, 0.55, 0.92, KyndeBladeArtPalette.HI_BIT_SILHOUETTE_FAR.darkened(0.2))
		"twilight_tower_mist":
			_draw_gradient_vertical(r, KyndeBladeArtPalette.HUB_MIST, KyndeBladeArtPalette.HUB_TWILIGHT)
			_draw_band(r, 0.35, 0.65, KyndeBladeArtPalette.VISTA_GOLD_TITLE.darkened(0.55))
		"fair_field_hi_bit":
			_draw_gradient_vertical(r, KyndeBladeArtPalette.HI_BIT_SKY_MIST, KyndeBladeArtPalette.HI_BIT_SKY_TEAL)
			_draw_band(r, 0.5, 0.88, KyndeBladeArtPalette.HI_BIT_SAGE.lerp(KyndeBladeArtPalette.HI_BIT_TEAL_SHADOW, 0.4))
			_draw_ground_strip(r, KyndeBladeArtPalette.HI_BIT_TERRACOTTA)
		"dungeon_cool_depth":
			_draw_solid(r, KyndeBladeArtPalette.COMBAT_VOID_COOL)
			_draw_band(r, 0.2, 0.45, KyndeBladeArtPalette.HI_BIT_TEAL_SHADOW.lightened(0.08))
		"labour_earth_stripes":
			_draw_gradient_vertical(r, KyndeBladeArtPalette.PARCHMENT_LIGHT, KyndeBladeArtPalette.HI_BIT_SKY_TEAL)
			_draw_band(r, 0.48, 0.85, KyndeBladeArtPalette.HI_BIT_TERRACOTTA)
			_draw_stripes(r, KyndeBladeArtPalette.HI_BIT_SAGE.darkened(0.15))
		"sin_gathering_red":
			_draw_solid(r, KyndeBladeArtPalette.COMBAT_VOID)
			_draw_radial_wash(r, KyndeBladeArtPalette.RUBRICATION, 0.35)
		"green_void_chapel":
			_draw_solid(r, KyndeBladeArtPalette.COMBAT_VOID)
			_draw_band(r, 0.15, 0.55, KyndeBladeArtPalette.BOSS_GREEN_KNIGHT.darkened(0.35))
		"orfeo_violet_tree":
			_draw_gradient_vertical(r, KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW, KyndeBladeArtPalette.HUB_TWILIGHT)
			_draw_band(r, 0.4, 0.75, KyndeBladeArtPalette.HI_BIT_SILHOUETTE_MID)
		"vision2_gold_search":
			_draw_gradient_vertical(r, KyndeBladeArtPalette.PARCHMENT, KyndeBladeArtPalette.GOLD_DARK.darkened(0.3))
			_draw_band(r, 0.3, 0.5, KyndeBladeArtPalette.GOLD.darkened(0.4))
		"depths_hunger_teal":
			_draw_solid(r, KyndeBladeArtPalette.COMBAT_VOID)
			_draw_gradient_vertical(r, KyndeBladeArtPalette.COMBAT_VOID_COOL, KyndeBladeArtPalette.BOSS_HUNGER.darkened(0.5))
		"time_dust_parchment":
			_draw_solid(r, KyndeBladeArtPalette.PARCHMENT_AGED)
			_draw_band(r, 0.25, 0.4, KyndeBladeArtPalette.INK_LIGHT.darkened(0.2))
		"grace_pale_terminal":
			_draw_gradient_vertical(r, Color(0.92, 0.9, 0.88, 1.0), KyndeBladeArtPalette.PARCHMENT_LIGHT)
			_draw_band(r, 0.6, 0.9, KyndeBladeArtPalette.PARCHMENT.darkened(0.05))
		"fairy_terminal_mist":
			_draw_gradient_vertical(r, KyndeBladeArtPalette.JEWEL_ULTRAMARINE.darkened(0.5), KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW)
			_draw_radial_wash(r, KyndeBladeArtPalette.SICKLY_HIGHLIGHT, 0.2)
		_:
			_draw_gradient_vertical(r, KyndeBladeArtPalette.HUB_MIST, KyndeBladeArtPalette.HUB_TWILIGHT)
	if _jewel > 0.0001:
		_draw_jewel_wash(r, _jewel)


func _draw_solid(rect: Rect2, c: Color) -> void:
	draw_rect(rect, c, true)


func _draw_gradient_vertical(rect: Rect2, top: Color, bottom: Color) -> void:
	var steps := 14
	var h := rect.size.y / float(steps)
	for i in steps:
		var t := float(i) / float(max(1, steps - 1))
		var c := top.lerp(bottom, t)
		draw_rect(Rect2(rect.position + Vector2(0, h * i), Vector2(rect.size.x, h + 1.0)), c, true)


func _draw_band(rect: Rect2, y0n: float, y1n: float, c: Color) -> void:
	var y0 := rect.position.y + rect.size.y * y0n
	var y1 := rect.position.y + rect.size.y * y1n
	draw_rect(Rect2(Vector2(rect.position.x, y0), Vector2(rect.size.x, y1 - y0)), c, true)


func _draw_ground_strip(rect: Rect2, c: Color) -> void:
	var g0 := rect.position.y + rect.size.y * 0.78
	draw_rect(Rect2(Vector2(rect.position.x, g0), Vector2(rect.size.x, rect.size.y * 0.22 + 2.0)), c, true)


func _draw_stripes(rect: Rect2, c: Color) -> void:
	var n := 9
	var w := rect.size.x / float(n)
	for i in n:
		var x := rect.position.x + w * float(i)
		var col := c if i % 2 == 0 else c.darkened(0.25)
		draw_rect(Rect2(Vector2(x, rect.position.y + rect.size.y * 0.5), Vector2(w + 1.0, rect.size.y * 0.45)), col, true)


func _draw_radial_wash(rect: Rect2, c: Color, strength: float) -> void:
	var ctr := rect.get_center()
	var rad := rect.size.length() * 0.55
	var col := c
	col.a = strength
	draw_circle(ctr, rad, col)


func _draw_jewel_wash(rect: Rect2, strength: float) -> void:
	var c := (
			KyndeBladeArtPalette.JEWEL_CRIMSON
			.lerp(KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW, 0.45)
			.lerp(KyndeBladeArtPalette.JEWEL_ULTRAMARINE, 0.1)
	)
	c.a = clampf(strength, 0.0, 0.35)
	draw_rect(rect, c, true)

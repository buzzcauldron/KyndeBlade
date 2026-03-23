extends Control
## **Crawl** hub backdrop: layered twilight / mist bands with [`CrawlParallax`](crawl_parallax.gd) drift
## (preview of future route-map parallax). Renders behind UI.

@export var parallax_speed_scale: float = 1.0
@export var parallax_enabled: bool = true

var _time_sec: float = 0.0


func _ready() -> void:
	set_anchors_preset(PRESET_FULL_RECT)
	offset_left = 0.0
	offset_top = 0.0
	offset_right = 0.0
	offset_bottom = 0.0
	mouse_filter = MOUSE_FILTER_IGNORE
	z_index = -4
	resized.connect(queue_redraw)


func _process(delta: float) -> void:
	if not parallax_enabled:
		return
	if get_tree().paused:
		return
	_time_sec += delta
	queue_redraw()


func _draw() -> void:
	var w: float = size.x
	var h: float = size.y
	if w < 1.0 or h < 1.0:
		return
	var sc: float = parallax_speed_scale

	# Base twilight + bruised lower air (Lane A coastal haze — ART_DIRECTION_GODOT).
	draw_rect(Rect2(0, 0, w, h), KyndeBladeArtPalette.HUB_TWILIGHT)
	var jv := KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW
	draw_rect(
			Rect2(0, h * 0.58, w, h * 0.42 + 2.0),
			Color(jv.r, jv.g, jv.b, 0.07)
	)

	var o0: Vector2 = CrawlParallax.hub_band_offset(_time_sec, 0, sc)
	draw_rect(
			Rect2(o0.x * 0.4, o0.y, w + 40.0, h * 0.58),
			KyndeBladeArtPalette.HUB_MIST.lerp(KyndeBladeArtPalette.HUB_TWILIGHT, 0.28)
	)

	var o1: Vector2 = CrawlParallax.hub_band_offset(_time_sec, 1, sc)
	draw_rect(
			Rect2(o1.x, h * 0.22 + o1.y * 0.5, w + 24.0, h * 0.48),
			Color(KyndeBladeArtPalette.HUB_MIST.r, KyndeBladeArtPalette.HUB_MIST.g, KyndeBladeArtPalette.HUB_MIST.b, 0.62)
	)

	var o2: Vector2 = CrawlParallax.hub_band_offset(_time_sec, 2, sc)
	draw_rect(
			Rect2(o2.x * 1.2, h * 0.48 + o2.y, w + 32.0, h * 0.38),
			Color(KyndeBladeArtPalette.VISTA_GOLD_TITLE.r, KyndeBladeArtPalette.VISTA_GOLD_TITLE.g, KyndeBladeArtPalette.VISTA_GOLD_TITLE.b, 0.11)
	)

	# Distant “wayes” specks (crawl path flavour)
	var rng := RandomNumberGenerator.new()
	rng.seed = 90210
	for i in 24:
		var bx: float = rng.randf_range(-20, w + 20)
		var by: float = rng.randf_range(h * 0.35, h * 0.72)
		var o3: Vector2 = CrawlParallax.hub_band_offset(_time_sec, 3, sc)
		draw_circle(Vector2(bx + o3.x * 0.6, by + o3.y * 0.4), rng.randf_range(0.8, 2.2), Color(0.55, 0.5, 0.45, 0.12))

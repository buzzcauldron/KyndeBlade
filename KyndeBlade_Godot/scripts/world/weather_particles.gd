extends RefCounted
class_name WeatherParticles
## Builds [CPUParticles2D] emitters from [`data/world/weather_presets.json`](../../data/world/weather_presets.json).

const PATH := "res://data/world/weather_presets.json"

static var _doc: Dictionary = {}


static func _load_doc() -> Dictionary:
	if not _doc.is_empty():
		return _doc
	if not FileAccess.file_exists(PATH):
		push_warning("WeatherParticles: missing %s" % PATH)
		return {}
	var f := FileAccess.open(PATH, FileAccess.READ)
	var raw: Variant = JSON.parse_string(f.get_as_text())
	if typeof(raw) != TYPE_DICTIONARY:
		push_warning("WeatherParticles: invalid JSON")
		return {}
	_doc = raw
	return _doc


static func _color_from_hex(s: String) -> Color:
	var h := s.strip_edges()
	if h.begins_with("#"):
		h = h.substr(1)
	if h.length() == 8:
		return Color("#" + h)
	if h.length() == 6:
		return Color("#" + h)
	return Color.WHITE


static func _direction_from_deg(deg: float) -> Vector2:
	var r := deg_to_rad(deg)
	return Vector2(cos(r), sin(r)).normalized()


static func _configure_emitter(
		p: CPUParticles2D,
		row: Dictionary,
		intensity: float
) -> void:
	var amt := int(round(float(row.get("amount", 40)) * intensity))
	amt = clampi(amt, 0, 400)
	p.amount = amt
	p.lifetime = maxf(0.05, float(row.get("lifetime", 2.0)))
	p.preprocess = float(row.get("preprocess", 0.0))
	p.explosiveness = clampf(float(row.get("explosiveness", 0.0)), 0.0, 1.0)
	p.randomness = clampf(float(row.get("randomness", 0.3)), 0.0, 1.0)
	p.direction = _direction_from_deg(float(row.get("direction_deg", 90.0)))
	p.spread = float(row.get("spread_deg", 15.0))
	p.gravity = Vector2(float(row.get("gravity_x", 0.0)), float(row.get("gravity_y", 0.0)))
	p.initial_velocity_min = float(row.get("velocity_min", 10.0))
	p.initial_velocity_max = float(row.get("velocity_max", 30.0))
	p.angular_velocity_min = float(row.get("angular_velocity_min", 0.0))
	p.angular_velocity_max = float(row.get("angular_velocity_max", 0.0))
	p.scale_amount_min = float(row.get("scale_min", 1.0))
	p.scale_amount_max = float(row.get("scale_max", 2.0))
	p.emission_shape = CPUParticles2D.EMISSION_SHAPE_RECTANGLE
	var hw := float(row.get("emission_half_w", 400.0))
	var hh := float(row.get("emission_half_h", 30.0))
	p.emission_rect_extents = Vector2(hw, hh)
	var oy := float(row.get("emission_offset_y", -280.0))
	p.position = Vector2(0.0, oy)
	var c0 := _color_from_hex(str(row.get("color_hex", "ffffffff")))
	var c1 := _color_from_hex(str(row.get("color_end_hex", "ffffff00")))
	var grad := Gradient.new()
	grad.set_color(0, c0)
	grad.set_color(1, c1)
	p.color_ramp = grad
	p.local_coords = false
	p.one_shot = false


## Clears prior [CPUParticles2D] children of `root` and adds new ones. `root` should sit at viewport center.
static func rebuild_under(
		root: Node2D,
		viewport_size: Vector2,
		preset_id: String,
		intensity: float,
		emitting: bool
) -> void:
	for c in root.get_children():
		if c is CPUParticles2D:
			c.queue_free()
	root.position = viewport_size * 0.5
	var doc := _load_doc()
	var presets: Variant = doc.get("presets", {})
	if typeof(presets) != TYPE_DICTIONARY:
		return
	var preset: Variant = presets.get(preset_id.strip_edges(), null)
	var used_fallback := false
	if preset == null or typeof(preset) != TYPE_DICTIONARY:
		used_fallback = true
		preset = presets.get("mist_calm", {})
	if typeof(preset) != TYPE_DICTIONARY:
		return
	var emitters: Variant = preset.get("emitters", [])
	if typeof(emitters) != TYPE_ARRAY or emitters.is_empty():
		return
	var inten := clampf(intensity, 0.0, 2.0)
	for item in emitters:
		if typeof(item) != TYPE_DICTIONARY:
			continue
		var cp := CPUParticles2D.new()
		root.add_child(cp)
		_configure_emitter(cp, item, inten)
		cp.emitting = emitting and inten > 0.001


static func preset_exists(preset_id: String) -> bool:
	var doc := _load_doc()
	var presets: Variant = doc.get("presets", {})
	if typeof(presets) != TYPE_DICTIONARY:
		return false
	return presets.has(preset_id.strip_edges())

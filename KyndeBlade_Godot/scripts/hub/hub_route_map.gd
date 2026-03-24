class_name HubRouteMap
extends Control
## Manuscript **route map**: click adjacent revealed stedes to travel (fog-of-war from [GameState.hub_revealed_node_ids]).

signal travel_requested(location_id: String)

var _node_buttons: Dictionary = {}


func _ready() -> void:
	mouse_filter = Control.MOUSE_FILTER_STOP
	resized.connect(_rebuild)
	_rebuild()


func refresh() -> void:
	_rebuild()


func _rebuild() -> void:
	for c in get_children():
		c.queue_free()
	_node_buttons.clear()
	var sz := size
	if sz.x < 8.0 or sz.y < 8.0:
		return
	# Edges
	for e in HubRouteRegistry.edges():
		if typeof(e) != TYPE_DICTIONARY:
			continue
		var a := str(e.get("from", ""))
		var b := str(e.get("to", ""))
		if not GameState.is_hub_node_revealed(a) or not GameState.is_hub_node_revealed(b):
			continue
		var pa := _node_pixel(a, sz)
		var pb := _node_pixel(b, sz)
		var ln := Line2D.new()
		ln.width = 2.0
		ln.default_color = Color(
				KyndeBladeArtPalette.BORDER_SEPIA.r,
				KyndeBladeArtPalette.BORDER_SEPIA.g,
				KyndeBladeArtPalette.BORDER_SEPIA.b,
				0.55
		)
		ln.points = PackedVector2Array([pa, pb])
		add_child(ln)
	for item in HubRouteRegistry.nodes():
		if typeof(item) != TYPE_DICTIONARY:
			continue
		var nid := str(item.get("id", ""))
		if nid.is_empty():
			continue
		var revealed := GameState.is_hub_node_revealed(nid)
		var pos := _node_pixel(nid, sz)
		var btn := Button.new()
		btn.text = str(item.get("title", nid)) if revealed else "· · ·"
		btn.custom_minimum_size = Vector2(108, 36)
		btn.position = pos - btn.custom_minimum_size * 0.5
		btn.focus_mode = Control.FOCUS_NONE
		var here := GameState.current_location_id == nid
		var neighbor := HubRouteRegistry.neighbor_ids(GameState.current_location_id).has(nid)
		btn.disabled = not revealed or (not neighbor and not here) or here
		if here:
			btn.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
		if revealed and neighbor and not here:
			btn.pressed.connect(_on_node_pressed.bind(nid))
		add_child(btn)
		_node_buttons[nid] = btn
		if not revealed:
			var fog := ColorRect.new()
			fog.color = Color(
					KyndeBladeArtPalette.HUB_MIST.r,
					KyndeBladeArtPalette.HUB_MIST.g,
					KyndeBladeArtPalette.HUB_MIST.b,
					0.72
			)
			fog.size = Vector2(120, 44)
			fog.position = pos - fog.size * 0.5
			fog.mouse_filter = Control.MOUSE_FILTER_IGNORE
			add_child(fog)


func _node_pixel(node_id: String, sz: Vector2) -> Vector2:
	var row := HubRouteRegistry.node_by_id(node_id)
	if row.is_empty():
		return sz * 0.5
	return Vector2(float(row.get("x", 0.5)) * sz.x, float(row.get("y", 0.5)) * sz.y)


func _on_node_pressed(loc_id: String) -> void:
	travel_requested.emit(loc_id)

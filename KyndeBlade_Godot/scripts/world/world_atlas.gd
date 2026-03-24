extends Control
## Lists every **location id** in the registry — authoring / navigation aid for the full Unity-planned world.

@onready var list_container: VBoxContainer = %LocationList


func _ready() -> void:
	theme = KyndeBladeManuscriptTheme.build_theme()
	_build_list()


func _build_list() -> void:
	for c in list_container.get_children():
		c.queue_free()

	for loc_id in LocationRegistry.list_location_ids_sorted():
		var row := LocationRegistry.get_location(loc_id)
		var hb := HBoxContainer.new()
		var btn := Button.new()
		var name: String = str(row.get("display_name", loc_id))
		btn.text = "%s — %s" % [loc_id, name]
		btn.pressed.connect(_on_pick.bind(loc_id))
		hb.add_child(btn)
		var tag := Label.new()
		tag.text = "  [%s]" % LocationRegistry.get_implementation_status(loc_id)
		tag.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_BODY)
		hb.add_child(tag)
		list_container.add_child(hb)


func _on_pick(loc_id: String) -> void:
	await WorldNav.open_location_preview(loc_id)


func _on_hub_pressed() -> void:
	await WorldNav.go_to_hub()

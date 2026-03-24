extends Control
## Preview shell for one **location** from [`locations_registry.json`](../../data/world/locations_registry.json). Use World Atlas or `WorldNav.open_location_preview`.

const HUB := "res://scenes/hub_map.tscn"

@onready var backdrop: PlaceholderLocationBackdrop = $Backdrop
@onready var character_preview: Control = $CharacterPreview
@onready var preview_actor: Node2D = $CharacterPreview/CenterActor/Actor

@onready var title_label: Label = %TitleLabel
@onready var meta_label: Label = %MetaLabel
@onready var body_label: Label = %BodyLabel
@onready var notes_label: Label = %NotesLabel
@onready var next_container: VBoxContainer = %NextDestinations


func _ready() -> void:
	theme = KyndeBladeManuscriptTheme.build_theme()
	var loc_id := WorldNav.pending_location_id.strip_edges()
	WorldNav.clear_pending()
	if loc_id.is_empty():
		loc_id = GameState.current_location_id
	_populate(loc_id)


func _populate(loc_id: String) -> void:
	var row := LocationRegistry.get_location(loc_id)
	if row.is_empty():
		title_label.text = "Unknown stede"
		body_label.text = "No row for `%s` in locations_registry.json." % loc_id
		meta_label.text = ""
		notes_label.text = ""
		_clear_next()
		if backdrop:
			backdrop.set_location_id("")
		if character_preview:
			character_preview.visible = false
		return

	if backdrop:
		backdrop.set_location_id(loc_id)
	var pc := PlaceholderArtRegistry.level_preview_character(loc_id)
	if character_preview and preview_actor:
		if pc.is_empty():
			character_preview.visible = false
		else:
			character_preview.visible = true
			var pa := preview_actor as PlaceholderActor2D
			if pa:
				pa.character_id = pc
				pa.apply_character()

	title_label.text = LocationRegistry.get_display_name(loc_id)
	var folder: String = str(row.get("unity_data_folder", ""))
	var st: String = LocationRegistry.get_implementation_status(loc_id)
	meta_label.text = "%s · Vision %s · %s · [%s]" % [
		loc_id,
		str(row.get("vision_index", "?")),
		str(row.get("passus_title", "")),
		st,
	]
	if not folder.is_empty():
		meta_label.text += "\nUnity: Resources/Data/%s/%s.asset" % [folder, str(row.get("unity_asset", ""))]

	body_label.text = LocationRegistry.get_description(loc_id)
	var wn := LocationRegistry.get_writer_notes(loc_id)
	notes_label.visible = not wn.is_empty()
	notes_label.text = "Writer: %s" % wn if not wn.is_empty() else ""

	_build_next_buttons(loc_id)


func _clear_next() -> void:
	for c in next_container.get_children():
		c.queue_free()


func _build_next_buttons(loc_id: String) -> void:
	_clear_next()
	var hdr := Label.new()
	hdr.text = "Weyes leading hence (graph)"
	hdr.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	next_container.add_child(hdr)

	for nid in LocationRegistry.get_next_ids(loc_id):
		var hb := HBoxContainer.new()
		var btn := Button.new()
		btn.text = "%s — %s" % [nid, LocationRegistry.get_display_name(nid)]
		btn.disabled = not _is_skeleton_travel_allowed(nid)
		btn.pressed.connect(_on_next_pressed.bind(nid))
		hb.add_child(btn)
		var stat := Label.new()
		stat.text = "  (%s)" % LocationRegistry.get_implementation_status(nid)
		stat.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_BODY)
		hb.add_child(stat)
		next_container.add_child(hb)


func _is_skeleton_travel_allowed(target_id: String) -> bool:
	## Slice: only hub-safe destinations are interactive; everything else is preview-only from atlas.
	if target_id == GameWorldIds.LOCATION_FAYRE_FELDE:
		return true
	if target_id == GameWorldIds.LOCATION_TOUR:
		return true
	return false


func _on_next_pressed(target_id: String) -> void:
	if not _is_skeleton_travel_allowed(target_id):
		WorldNav.open_location_preview(target_id)
		return
	if target_id == GameWorldIds.LOCATION_FAYRE_FELDE:
		GameState.record_location_visit("fayre_felde")
		GameState.current_location_id = "fayre_felde"
		GameState.sync_to_save()
		get_tree().change_scene_to_file(HUB)
		return
	if target_id == GameWorldIds.LOCATION_TOUR:
		GameState.record_location_visit("tour")
		GameState.current_location_id = "tour"
		GameState.sync_to_save()
		get_tree().change_scene_to_file(HUB)


func _on_back_hub_pressed() -> void:
	get_tree().change_scene_to_file(HUB)


func _on_back_atlas_pressed() -> void:
	WorldNav.open_world_atlas()

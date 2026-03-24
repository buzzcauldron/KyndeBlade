extends Control
## Hub stub: tour / Fair Field travel (mirrors slice names + Unity export when present).

const COMBAT := "res://scenes/combat.tscn"
const MENU := "res://scenes/main_menu.tscn"

enum CounselChoice { NONE, TREWTHE, MEDE, HUNGER }

@onready var location_label: RichTextLabel = %LocationLabel
@onready var flavor_panel: PanelContainer = %FlavorPanel
@onready var fair_button: Button = %FairFieldButton
@onready var flavor_text: Label = %FlavorText
@onready var locked_button: Button = %LockedButton
@onready var hint_label: Label = %HintLabel
@onready var proceed_combat_button: Button = %EnterCombat
@onready var counsel_trewthe: Button = %CounselTrewthe
@onready var counsel_mede: Button = %CounselMede
@onready var counsel_hunger: Button = %CounselHunger

var _unity_export: Dictionary = {}
var _pending_counsel: CounselChoice = CounselChoice.NONE


func _ready() -> void:
	theme = KyndeBladeManuscriptTheme.build_theme()
	# Twilight / mist backdrop: `CrawlParallaxBackdrop` (parallax crawl feature)
	$Margin/VBox/Title.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_GOLD_TITLE)
	$Margin/VBox/HintLabel.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_BODY)
	_unity_export = UnityExportData.load_export()
	_validate_slice_locations_json()
	_validate_unity_export()
	_update_ui()


func _validate_slice_locations_json() -> void:
	var path := "res://data/slice_locations.json"
	if not FileAccess.file_exists(path):
		push_warning("HubMap: missing %s" % path)
		return
	var f := FileAccess.open(path, FileAccess.READ)
	var data = JSON.parse_string(f.get_as_text())
	if typeof(data) != TYPE_DICTIONARY:
		push_warning("HubMap: invalid slice_locations.json")
		return
	var tour: Variant = data.get("tour", {})
	var nxt: Variant = tour.get("next", []) if typeof(tour) == TYPE_DICTIONARY else []
	if typeof(nxt) != TYPE_ARRAY or not nxt.has("fayre_felde"):
		push_warning("HubMap: tour.next should include fayre_felde for slice parity")


func _validate_unity_export() -> void:
	if _unity_export.is_empty():
		push_warning("HubMap: optional data/exported_from_unity.json missing — run Unity KyndeBlade → Export Slice Data for Godot")
		return
	var tour: Dictionary = UnityExportData.location_by_id(_unity_export, "tour")
	var nxt: Variant = tour.get("next_location_ids", [])
	if typeof(nxt) != TYPE_ARRAY or not nxt.has("fayre_felde"):
		push_warning("HubMap: Unity export tour.next_location_ids should include fayre_felde")
	var fayre: Dictionary = UnityExportData.location_by_id(_unity_export, "fayre_felde")
	var enc_name: String = str(fayre.get("encounter_asset_name", ""))
	if enc_name.is_empty():
		push_warning("HubMap: Unity export fayre_felde missing encounter_asset_name")
		return
	var enc: Dictionary = UnityExportData.encounter_for_asset_name(_unity_export, enc_name)
	var types: Variant = enc.get("enemy_character_types", [])
	if typeof(types) != TYPE_ARRAY or types.is_empty():
		push_warning("HubMap: Unity export encounter missing enemy_character_types")


func _bb_escape(s: String) -> String:
	return s.replace("[", "[lb]")


func _world_state_prefix() -> String:
	var parts: PackedStringArray = []
	if GameState.ethical_misstep_count > 0:
		parts.append(
				"Thou hast heeded Mede’s whisper %d time(s); False feinteth the read of battle."
				% GameState.ethical_misstep_count
		)
	if GameState.has_ever_had_hunger:
		parts.append(
				"Hunger hath been named; the feeld remembereth thy want — "
				+ "the next shelde-fray taketh a slightly other tithe each tyme."
		)
	if parts.is_empty():
		return ""
	return "\n\n".join(parts) + "\n\n"


func _update_ui() -> void:
	var loc := GameState.current_location_id
	var display_name := "Tower on the Toft" if loc == "tour" else "Fair Field" if loc == "fayre_felde" else loc
	if not _unity_export.is_empty():
		var row: Dictionary = UnityExportData.location_by_id(_unity_export, loc)
		if not row.is_empty():
			display_name = str(row.get("display_name", display_name))
	var ink := KyndeBladeArtPalette.INK_PRIMARY.to_html(false)
	var gold := KyndeBladeArtPalette.GOLD.to_html(false)
	location_label.text = (
			"[color=#%s]Current:[/color] [color=#%s]%s[/color] [color=#%s](%s)[/color]"
			% [ink, gold, _bb_escape(display_name), ink, _bb_escape(loc)]
	)
	fair_button.disabled = false
	fair_button.text = "Fayr Feeld — abide the fight again" if GameState.fair_field_cleared else "Travel to the Fayr Feeld"
	if hint_label:
		var base: String
		if not _unity_export.is_empty():
			base = "Vision I writ is bound—the weyes and the feeld are true to the export."
		else:
			base = "Chuse whither thou goest. The Fayr Feeld lieth open in this slice. The World atlas listeth every stede planned with Unity."
		var extra := ""
		if GameState.ethical_misstep_count > 0:
			extra += " Mede’s count: %d." % GameState.ethical_misstep_count
		if GameState.has_ever_had_hunger:
			extra += " Hunger is named; punishments shift like weather — odd, sharp, not quite the same twice."
		hint_label.text = base + extra
	if locked_button:
		locked_button.text = "Dongeoun — loke, not yet in this build"


func _reset_counsel_ui() -> void:
	_pending_counsel = CounselChoice.NONE
	if proceed_combat_button:
		proceed_combat_button.disabled = true


func _on_fair_field_pressed() -> void:
	_reset_counsel_ui()
	# Reading the Fair Field arrival flavor counts as ingesting that beat’s text grant.
	GameState.mark_medieval_text_read("FayreFelde")
	var body := "You step toward Fair Field. False waits in the encounter ahead."
	if not _unity_export.is_empty():
		var fayre: Dictionary = UnityExportData.location_by_id(_unity_export, "fayre_felde")
		if not fayre.is_empty():
			var beat: String = str(fayre.get("arrival_beat_text", ""))
			if not beat.is_empty():
				body = beat + "\n\n" + body
			else:
				var desc: String = str(fayre.get("description", ""))
				if not desc.is_empty():
					body = desc + "\n\n" + body
	else:
		body = "A fair feeld ful of folke… (slice flavor)\n\n" + body
	body = _world_state_prefix() + body
	if flavor_text:
		flavor_text.text = body
	flavor_panel.visible = true


func _on_counsel_trewthe_pressed() -> void:
	_pending_counsel = CounselChoice.TREWTHE
	if proceed_combat_button:
		proceed_combat_button.disabled = false


func _on_counsel_mede_pressed() -> void:
	_pending_counsel = CounselChoice.MEDE
	if proceed_combat_button:
		proceed_combat_button.disabled = false


func _on_counsel_hunger_pressed() -> void:
	_pending_counsel = CounselChoice.HUNGER
	if proceed_combat_button:
		proceed_combat_button.disabled = false


func _on_flavor_continue_pressed() -> void:
	if _pending_counsel == CounselChoice.NONE:
		return
	match _pending_counsel:
		CounselChoice.TREWTHE:
			pass
		CounselChoice.MEDE:
			GameState.ethical_misstep_count += 1
		CounselChoice.HUNGER:
			GameState.has_ever_had_hunger = true
	GameState.record_location_visit("fayre_felde")
	GameState.sync_to_save()
	flavor_panel.visible = false
	_reset_counsel_ui()
	_update_ui()
	get_tree().change_scene_to_file(COMBAT)


func _on_cancel_flavor_pressed() -> void:
	flavor_panel.visible = false
	_reset_counsel_ui()


func _on_back_menu_pressed() -> void:
	get_tree().change_scene_to_file(MENU)


func _on_locked_pressed() -> void:
	pass


func _on_world_atlas_pressed() -> void:
	WorldNav.open_world_atlas()

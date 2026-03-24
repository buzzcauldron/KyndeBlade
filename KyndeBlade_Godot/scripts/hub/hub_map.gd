extends Control
## Hub: **route map** travel + Fair Field counsel flow. Atlas is secondary (writers’ index).

const _NarrativeBeats := preload("res://scripts/world/narrative_beats.gd")
const COMBAT := "res://scenes/combat.tscn"
const MENU := "res://scenes/main_menu.tscn"

enum CounselChoice { NONE, TREWTHE, MEDE, HUNGER }

@onready var route_map: HubRouteMap = %HubRouteMap
@onready var wales_basemap: TextureRect = $RouteColumns/HubMapStack/WalesBasemap
@onready var weather_root: Node2D = $WeatherFX/WeatherRoot
@onready var counsel_row: HBoxContainer = %CounselRow
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
## When true, Enter Combat skips counsel and loads the dongeoun gate encounter.
var _pending_combat_from_dongeoun: bool = false


func _ready() -> void:
	theme = KyndeBladeManuscriptTheme.build_theme()
	$RouteColumns/LeftWrap/VBox/Title.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_GOLD_TITLE)
	$RouteColumns/LeftWrap/VBox/HintLabel.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_BODY)
	_unity_export = UnityExportData.load_export()
	_validate_slice_locations_json()
	_validate_unity_export()
	GameState.hub_on_map_opened()
	AtmosphereProfile.apply_to_hub_crawl($CrawlParallaxBackdrop, GameState.current_location_id)
	_apply_hub_basemap()
	route_map.travel_requested.connect(_on_route_travel_requested)
	_update_ui()
	route_map.refresh()
	_apply_hub_weather()


func _apply_hub_basemap() -> void:
	var bm := HubRouteRegistry.basemap_config()
	var tex_path := str(bm.get("texture", "")).strip_edges() if not bm.is_empty() else ""
	if tex_path.is_empty():
		tex_path = "res://assets/world/basemap_wales.png"
	if not ResourceLoader.exists(tex_path):
		push_warning("HubMap: basemap texture missing: %s" % tex_path)
		wales_basemap.visible = false
		return
	var loaded: Variant = load(tex_path)
	if loaded is Texture2D:
		wales_basemap.texture = loaded as Texture2D
		wales_basemap.visible = true
	else:
		wales_basemap.visible = false


func _apply_hub_weather() -> void:
	var sz: Vector2 = get_viewport_rect().size
	var w: Dictionary = AtmosphereProfile.weather_preset_for_location(GameState.current_location_id)
	WeatherParticles.rebuild_under(
			weather_root,
			sz,
			str(w.get("preset_id", "mist_calm")),
			float(w.get("intensity", 1.0)),
			true
	)


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
	if GameState.fair_field_cleared and not GameState.dongeoun_gate_cleared:
		parts.append(
				"The vale dongeoun openeth upon the map — a second fray awaiteth at the gate."
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
		var base := (
				"**Tread the map** at right: each lit stede is a leaf thou mayst turn. "
				+ "The writers’ index below listeth every place in the larger book, but the weyes thou goest here."
		)
		if not _unity_export.is_empty():
			base = (
					"Tread the **map** — the codex and the export agree on tour and feeld. "
					+ "Writers’ index: all skeleton stedes."
			)
		var extra := ""
		if GameState.ethical_misstep_count > 0:
			extra += " Mede’s count: %d." % GameState.ethical_misstep_count
		if GameState.has_ever_had_hunger:
			extra += " Hunger is named; punishments shift like weather — odd, sharp, not quite the same twice."
		hint_label.text = base.replace("**", "") + extra
	if locked_button:
		locked_button.text = (
				"Dongeoun — tread the map when the Fayr Feeld is won"
				if not GameState.fair_field_cleared
				else "Dongeoun — gate-ward shelde-fray (after the feeld)"
		)


func _reset_counsel_ui() -> void:
	_pending_counsel = CounselChoice.NONE
	_pending_combat_from_dongeoun = false
	if proceed_combat_button:
		proceed_combat_button.disabled = true
	if counsel_row:
		counsel_row.visible = true


func _on_route_travel_requested(loc_id: String) -> void:
	match loc_id:
		"fayre_felde":
			_offer_travel_to_fair_field()
		"tour":
			_travel_to_tour()
		"dongeoun":
			_on_dongeoun_travel()
		_:
			pass


func _travel_to_tour() -> void:
	GameState.current_location_id = "tour"
	GameState.record_location_visit("tour")
	GameState.hub_on_map_opened()
	GameState.sync_to_save()
	_update_ui()
	route_map.refresh()
	_apply_hub_weather()


func _on_dongeoun_travel() -> void:
	if not GameState.fair_field_cleared:
		_show_dongeoun_locked_stub()
		return
	if GameState.dongeoun_gate_cleared:
		_show_dongeoun_cleared_flavor()
		return
	_offer_dongeoun_combat()


func _show_dongeoun_locked_stub() -> void:
	_reset_counsel_ui()
	flavor_text.text = (
			"The dongeoun yawneth in the vale — yet the way is sealed until thou hast "
			+ "faced False upon the Fayr Feeld."
	)
	if counsel_row:
		counsel_row.visible = false
	if proceed_combat_button:
		proceed_combat_button.visible = false
	flavor_panel.visible = true


func _show_dongeoun_cleared_flavor() -> void:
	_reset_counsel_ui()
	flavor_text.text = "The outer ward is stille. Thy lettre of the gate is writ; turne againe to the map."
	if counsel_row:
		counsel_row.visible = false
	if proceed_combat_button:
		proceed_combat_button.visible = false
	flavor_panel.visible = true


func _offer_dongeoun_combat() -> void:
	_reset_counsel_ui()
	_pending_combat_from_dongeoun = true
	GameState.mark_medieval_text_read("MalvernPrologue")
	GameState.current_location_id = "dongeoun"
	GameState.record_location_visit("dongeoun")
	GameState.hub_on_map_opened()
	GameState.sync_to_save()
	var base := (
			"The dongeoun mouth gapes; a warden keepeth the writ-mount of the gate. "
			+ "Somer seson liȝ in memorie — lettres redde here spare a little stamyn ere the shelde-fray.\n\n"
			+ "Enter combat when thou art ready."
	)
	var prefix := ""
	if not _unity_export.is_empty():
		var dun: Dictionary = UnityExportData.location_by_id(_unity_export, "dongeoun")
		if not dun.is_empty():
			var dbeat: String = str(dun.get("arrival_beat_text", ""))
			if not dbeat.is_empty():
				prefix = dbeat
		if prefix.is_empty():
			var mal: Dictionary = UnityExportData.location_by_id(_unity_export, "malvern")
			if mal.is_empty():
				mal = UnityExportData.location_by_id(_unity_export, "tour")
			if not mal.is_empty():
				var beat: String = str(mal.get("arrival_beat_text", ""))
				if not beat.is_empty():
					prefix = beat
	if prefix.is_empty():
		prefix = _NarrativeBeats.player_facing_arrival_line("dongeoun")
	var body := base if prefix.is_empty() else prefix + "\n\n" + base
	body = _world_state_prefix() + body
	flavor_text.text = body
	if counsel_row:
		counsel_row.visible = false
	flavor_panel.visible = true
	if proceed_combat_button:
		proceed_combat_button.visible = true
		proceed_combat_button.disabled = false
	_update_ui()
	route_map.refresh()
	_apply_hub_weather()


func _offer_travel_to_fair_field() -> void:
	_reset_counsel_ui()
	_pending_combat_from_dongeoun = false
	if proceed_combat_button:
		proceed_combat_button.visible = true
	GameState.mark_medieval_text_read("FayreFelde")
	var base := "You step toward Fair Field. False waits in the encounter ahead."
	var prefix := ""
	if not _unity_export.is_empty():
		var fayre: Dictionary = UnityExportData.location_by_id(_unity_export, "fayre_felde")
		if not fayre.is_empty():
			var beat: String = str(fayre.get("arrival_beat_text", ""))
			if not beat.is_empty():
				prefix = beat
			else:
				var desc: String = str(fayre.get("description", ""))
				if not desc.is_empty():
					prefix = desc
	if prefix.is_empty():
		prefix = _NarrativeBeats.player_facing_arrival_line("fayre_felde")
	if prefix.is_empty() and _unity_export.is_empty():
		prefix = "A fair feeld ful of folke… (slice flavor)"
	var body := base if prefix.is_empty() else prefix + "\n\n" + base
	body = _world_state_prefix() + body
	if flavor_text:
		flavor_text.text = body
	flavor_panel.visible = true


## Headless / tests: same as choosing Fair Field on the map.
func travel_to_fair_field_for_tests() -> void:
	_offer_travel_to_fair_field()


func _on_fair_field_pressed() -> void:
	_offer_travel_to_fair_field()


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
	if _pending_combat_from_dongeoun:
		GameState.pending_combat_encounter_path = "res://data/encounter_dongeoun_gate.tres"
		flavor_panel.visible = false
		_reset_counsel_ui()
		if proceed_combat_button:
			proceed_combat_button.visible = true
		await ManuscriptNav.turn_page_to(COMBAT, true)
		return
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
	GameState.hub_on_map_opened()
	GameState.sync_to_save()
	flavor_panel.visible = false
	_reset_counsel_ui()
	if proceed_combat_button:
		proceed_combat_button.visible = true
	_update_ui()
	route_map.refresh()
	await ManuscriptNav.turn_page_to(COMBAT, true)


func _on_cancel_flavor_pressed() -> void:
	flavor_panel.visible = false
	_reset_counsel_ui()
	if proceed_combat_button:
		proceed_combat_button.visible = true


func _on_back_menu_pressed() -> void:
	await ManuscriptNav.turn_page_to(MENU)


func _on_locked_pressed() -> void:
	_show_dongeoun_locked_stub()


func _on_world_atlas_pressed() -> void:
	await WorldNav.open_world_atlas()

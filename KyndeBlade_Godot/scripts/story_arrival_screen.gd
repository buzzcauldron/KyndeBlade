extends Control
## Lane A–style arrival beat from Unity export (`StoryBeatOnArrival`). Default: **tour** tower vista before hub.

const HUB := "res://scenes/hub_map.tscn"

## Which `locations[]` entry supplies `arrival_beat_*` fields.
@export var source_location_id: String = "tour"

@onready var title_label: Label = %TitleLabel
@onready var speaker_label: Label = %SpeakerLabel
@onready var body_label: RichTextLabel = %BodyLabel

const _FALLBACK_TOWER := "A tour on a toft, trieliche ymaked—narwe, lovelich and selcouth. Fro this elritch heght thou overlookest the fayr feeld ful of folke. Hier the drem is stille; no triaile stireth. The Tour of Trewthe and the world bynethe abyden."


func _ready() -> void:
	theme = KyndeBladeManuscriptTheme.build_theme()
	title_label.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	speaker_label.add_theme_color_override("font_color", KyndeBladeArtPalette.LAPIS)
	body_label.add_theme_color_override("default_color", KyndeBladeArtPalette.INK_PRIMARY)
	var ex: Dictionary = UnityExportData.load_export()
	var loc: Dictionary = UnityExportData.location_by_id(ex, source_location_id)
	var beat_text: String = str(loc.get("arrival_beat_text", ""))
	var speaker: String = str(loc.get("arrival_beat_speaker", ""))
	var display: String = str(loc.get("display_name", source_location_id))

	if beat_text.is_empty() and source_location_id == "tour":
		beat_text = _FALLBACK_TOWER
		if speaker.is_empty():
			speaker = "Narrator"

	title_label.text = display
	speaker_label.text = speaker if not speaker.is_empty() else "—"
	body_label.text = "[center]%s[/center]" % _escape_bbcode(beat_text)


func _escape_bbcode(s: String) -> String:
	return s.replace("[", "[lb]")


func _on_continue_pressed() -> void:
	# Medieval-text grant aligned with Unity `tower_vista` arrival beat (`medieval_text_unlocks.json`).
	GameState.record_location_visit("tour")
	GameState.mark_medieval_text_read("tower_vista")
	get_tree().change_scene_to_file(HUB)

extends Control
## Main menu: New / Continue / Settings / Quit

const TOWER_INTRO := "res://scenes/tower_intro.tscn"
const HUB := "res://scenes/hub_map.tscn"
const BEGINNER_LOOP := "res://scenes/beginner_loop.tscn"
const HI_BIT_BONUS := "res://scenes/hi_bit_bonus_level.tscn"

@onready var continue_btn: Button = %ContinueButton
@onready var settings_panel: ColorRect = %SettingsPanel
@onready var volume_slider: HSlider = %VolumeSlider
@onready var fullscreen_toggle: CheckButton = %Fullscreen


func _ready() -> void:
	_apply_manuscript_lane_a()
	SaveService.save_changed.connect(_refresh_continue)
	SaveService.apply_stored_settings()
	volume_slider.value = SaveService.load_master_volume()
	fullscreen_toggle.button_pressed = SaveService.load_fullscreen()
	_refresh_continue()


func _apply_manuscript_lane_a() -> void:
	theme = KyndeBladeManuscriptTheme.build_theme()
	$Center/VBox/Title.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	$Center/VBox/Subtitle.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_BODY)
	$VignetteOverlay.color = Color(KyndeBladeArtPalette.HUB_TWILIGHT.r, KyndeBladeArtPalette.HUB_TWILIGHT.g, KyndeBladeArtPalette.HUB_TWILIGHT.b, 0.48)
	$SettingsPanel.color = Color(
		KyndeBladeArtPalette.PARCHMENT_AGED.r,
		KyndeBladeArtPalette.PARCHMENT_AGED.g,
		KyndeBladeArtPalette.PARCHMENT_AGED.b,
		0.94
	)
	$SettingsPanel/SettingsCenter/SettingsVBox/SettingsTitle.add_theme_color_override("font_color", KyndeBladeArtPalette.RUBRICATION)


func _refresh_continue() -> void:
	continue_btn.disabled = not SaveService.has_save()


func _on_new_game_pressed() -> void:
	SaveService.write_new_game()
	GameState.reset_from_new_game()
	# PLAYABLE_SLICE: tower arrival beat before map (Continue skips straight to hub).
	get_tree().change_scene_to_file(TOWER_INTRO)


func _on_continue_pressed() -> void:
	if not SaveService.has_save():
		return
	GameState.apply_loaded(SaveService.load_save())
	get_tree().change_scene_to_file(HUB)


func _on_settings_pressed() -> void:
	volume_slider.value = SaveService.load_master_volume()
	fullscreen_toggle.button_pressed = SaveService.load_fullscreen()
	settings_panel.visible = true


func _on_tiny_loop_pressed() -> void:
	get_tree().change_scene_to_file(BEGINNER_LOOP)


func _on_hi_bit_bonus_pressed() -> void:
	get_tree().change_scene_to_file(HI_BIT_BONUS)


func _on_quit_pressed() -> void:
	get_tree().quit()


func _on_settings_close_pressed() -> void:
	settings_panel.visible = false


func _on_volume_changed(value: float) -> void:
	SaveService.save_master_volume(value)


func _on_fullscreen_toggled(pressed: bool) -> void:
	SaveService.save_fullscreen(pressed)

extends Control
## Main menu — manuscript page layout: parchment field, framed panel, rubric + motif (`main_menu.tscn`).

const HUB := "res://scenes/hub_map.tscn"
const BEGINNER_LOOP := "res://scenes/beginner_loop.tscn"
const HI_BIT_BONUS := "res://scenes/hi_bit_bonus_level.tscn"
const COMBAT := "res://scenes/combat.tscn"
const NAV_TEST_YARD := "res://scenes/nav_test_yard.tscn"

@onready var continue_btn: Button = %ContinueButton
@onready var demo_gauntlet_btn: Button = %DemoGauntletButton
@onready var combat_drill_btn: Button = %CombatDrillButton
@onready var nav_test_btn: Button = %NavTestButton
@onready var settings_panel: ColorRect = %SettingsPanel
@onready var volume_slider: HSlider = %VolumeSlider
@onready var fullscreen_toggle: CheckButton = %Fullscreen
@onready var enemy_defense_toggle: CheckButton = %EnemyDefenseWindows


func _ready() -> void:
	_apply_manuscript_page()
	SaveService.save_changed.connect(_refresh_continue)
	SaveService.apply_stored_settings()
	volume_slider.value = SaveService.load_master_volume()
	fullscreen_toggle.button_pressed = SaveService.load_fullscreen()
	enemy_defense_toggle.button_pressed = SaveService.load_enable_real_time_defense_on_enemy_turn()
	_refresh_continue()
	combat_drill_btn.visible = OS.is_debug_build() or Engine.is_editor_hint()
	if nav_test_btn:
		nav_test_btn.visible = OS.is_debug_build() or Engine.is_editor_hint()


func _apply_manuscript_page() -> void:
	theme = KyndeBladeManuscriptTheme.build_theme()
	KyndeBladeManuscriptTheme.style_menu_volume_slider(volume_slider)
	KyndeBladeManuscriptTheme.style_menu_checkbutton(fullscreen_toggle)
	KyndeBladeManuscriptTheme.style_menu_checkbutton(enemy_defense_toggle)

	var ink_faint := KyndeBladeArtPalette.INK_PRIMARY
	ink_faint.a = 0.11
	$ParchmentBase.color = KyndeBladeArtPalette.PARCHMENT_LIGHT
	$ParchmentWash.color = Color(
			KyndeBladeArtPalette.PARCHMENT.r,
			KyndeBladeArtPalette.PARCHMENT.g,
			KyndeBladeArtPalette.PARCHMENT.b,
			0.14
	)
	$EdgeAging.color = ink_faint

	var vb: VBoxContainer = $SheetMargin/ManuscriptPanel/PageInner/Center/VBox
	vb.get_node("Title").add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	vb.get_node("Subtitle").add_theme_color_override("font_color", KyndeBladeArtPalette.INK_SECONDARY)
	vb.get_node("RubricRule").color = KyndeBladeArtPalette.RUBRICATION.lerp(KyndeBladeArtPalette.GOLD_DARK, 0.35)

	$SettingsPanel.color = KyndeBladeArtPalette.COMBAT_UI_SCRIM
	var st: Label = $SettingsPanel/SettingsCenter/SettingsFrame/SettingsInner/SettingsVBox/SettingsTitle
	st.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	$SettingsPanel/SettingsCenter/SettingsFrame/SettingsInner/SettingsVBox/VolLabel.add_theme_color_override(
			"font_color", KyndeBladeArtPalette.INK_PRIMARY
	)
	$SettingsPanel/SettingsCenter/SettingsFrame/SettingsInner/SettingsVBox/SettingsRule.color = (
			KyndeBladeArtPalette.RUBRICATION
	)


func _refresh_continue() -> void:
	continue_btn.disabled = not SaveService.has_save()


func _on_new_game_pressed() -> void:
	SaveService.write_new_game()
	GameState.reset_from_new_game()
	# Opens directly on the hub route map (`slice_open_yard` / `tower_intro` remain as scenes for tests / future menu hooks).
	await ManuscriptNav.turn_page_to(HUB)


func _on_continue_pressed() -> void:
	if not SaveService.has_save():
		return
	GameState.apply_loaded(SaveService.load_save())
	get_tree().change_scene_to_file(HUB)


func _on_settings_pressed() -> void:
	volume_slider.value = SaveService.load_master_volume()
	fullscreen_toggle.button_pressed = SaveService.load_fullscreen()
	enemy_defense_toggle.button_pressed = SaveService.load_enable_real_time_defense_on_enemy_turn()
	settings_panel.visible = true


func _on_tiny_loop_pressed() -> void:
	await ManuscriptNav.turn_page_to(BEGINNER_LOOP)


func _on_hi_bit_bonus_pressed() -> void:
	await ManuscriptNav.turn_page_to(HI_BIT_BONUS)


func _on_demo_gauntlet_pressed() -> void:
	GameState.begin_demo_gauntlet()
	await ManuscriptNav.turn_page_to(COMBAT, true)


func _on_combat_drill_pressed() -> void:
	SaveService.write_new_game()
	GameState.reset_from_new_game()
	await ManuscriptNav.turn_page_to(COMBAT, true)


func _on_nav_test_pressed() -> void:
	await ManuscriptNav.turn_page_to(NAV_TEST_YARD)


func _on_quit_pressed() -> void:
	get_tree().quit()


func _on_settings_close_pressed() -> void:
	settings_panel.visible = false


func _on_volume_changed(value: float) -> void:
	SaveService.save_master_volume(value)


func _on_fullscreen_toggled(pressed: bool) -> void:
	SaveService.save_fullscreen(pressed)


func _on_enemy_defense_windows_toggled(pressed: bool) -> void:
	SaveService.save_enable_real_time_defense_on_enemy_turn(pressed)

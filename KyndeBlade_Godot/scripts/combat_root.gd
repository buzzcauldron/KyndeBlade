extends Node2D
## Combat scene: UI, pause (freezes tree + window), outcome → hub.

const HUB := "res://scenes/hub_map.tscn"

@onready var combat: Node = $CombatManager
@onready var hud_encounter_title: Label = %EncounterTitle
@onready var turn_label: Label = %TurnLabel
@onready var telegraph_label: Label = %TelegraphLabel
@onready var field_subtitle: Label = %FieldSubtitle
@onready var moveset_rubric: Label = %MovesetRubric
@onready var player_hp_bar: ProgressBar = %PlayerHP
@onready var enemy_hp_bar: ProgressBar = %EnemyHP
@onready var stamina_bar: ProgressBar = %Stamina
@onready var pause_layer: CanvasLayer = $PauseLayer
@onready var outcome_layer: CanvasLayer = $OutcomeLayer
@onready var outcome_title: Label = %OutcomeTitle
@onready var outcome_body: Label = %OutcomeBody
@onready var enemy_hp_label: Label = %EHLabel
@onready var ph_bar_caption: Label = %PHLabel
@onready var st_bar_caption: Label = %StLabel
@onready var parry_dodge_eye: Node = %ParryDodgeEye
@onready var window_sfx_player: AudioStreamPlayer = $WindowSfx
@onready var kill_sfx_player: AudioStreamPlayer = $KillSfx
@onready var kill_fx_layer: CanvasLayer = $KillFxLayer
@onready var kill_desat_rect: ColorRect = $KillFxLayer/DesatRect
@onready var controls_hint: Label = %ControlsHint
@onready var strike_btn: Button = %StrikeCombatButton
@onready var dodge_btn: Button = %DodgeCombatButton
@onready var parry_btn: Button = %ParryCombatButton

var _kill_sfx_stream: AudioStreamWAV


func _ready() -> void:
	_apply_art_direction_theme()
	pause_layer.hide()
	outcome_layer.hide()
	if combat.encounter != null:
		var enc: EncounterDef = combat.encounter
		var place: String = str(enc.encounter_id).replace("_", " ")
		hud_encounter_title.text = "%s · %s" % [place.capitalize(), enc.enemy_display_name]
		enemy_hp_label.text = "%s (HP)" % PiersCombatVoice.enemy_epithet(enc.enemy_display_name)
	else:
		hud_encounter_title.text = "Shelde-fray"
	if field_subtitle:
		field_subtitle.text = PiersCombatVoice.field_subtitle()
	if moveset_rubric:
		moveset_rubric.text = PiersCombatVoice.granted_kennings_block()
	combat.turn_changed.connect(_on_turn_changed)
	combat.combat_ended.connect(_on_combat_ended)
	combat.stats_changed.connect(_on_stats_changed)
	combat.defensive_window_started.connect(_on_defensive_window_started)
	_kill_sfx_stream = CombatWindowTone.make_enemy_kill_impact()
	window_sfx_player.bus = "SFX"
	kill_sfx_player.bus = "SFX"
	if parry_dodge_eye.has_method("setup"):
		parry_dodge_eye.call("setup", combat)
	strike_btn.pressed.connect(_on_strike_button_pressed)
	dodge_btn.pressed.connect(_on_dodge_button_pressed)
	parry_btn.pressed.connect(_on_parry_button_pressed)
	_on_turn_changed()
	_on_stats_changed(combat.player_hp, combat.player_max_hp, combat.enemy_hp, combat.enemy_max_hp, combat.player_stamina, combat.player_max_stamina)
	_refresh_combat_action_buttons()


func _apply_art_direction_theme() -> void:
	# Lane B void + manuscript UI per Unity ART_DIRECTION + UI_MANUSCRIPT_THEME.
	var t: Theme = KyndeBladeManuscriptTheme.build_theme()
	var hud_panel: PanelContainer = $UI/HudMargin/HudColumn/HudRow/HudPanel
	hud_panel.theme = t
	$UI/ActionBar/ActionCenter/ActionPanel.theme = t
	$PauseLayer/PauseCenter/PauseVBox.theme = t
	$OutcomeLayer/OutcomeCenter/OutcomeVBox.theme = t
	KyndeBladeManuscriptTheme.style_progress_bar(player_hp_bar, KyndeBladeArtPalette.GOLD, KyndeBladeArtPalette.GOLD_DARK)
	KyndeBladeManuscriptTheme.style_progress_bar(enemy_hp_bar, KyndeBladeArtPalette.RUBRICATION, KyndeBladeArtPalette.BORDER_RED)
	KyndeBladeManuscriptTheme.style_progress_bar(stamina_bar, KyndeBladeArtPalette.LAPIS, KyndeBladeArtPalette.BORDER_BLUE)
	const hud_wrap := 404.0
	for lbl: Label in [turn_label, telegraph_label, field_subtitle, moveset_rubric]:
		if lbl:
			lbl.custom_minimum_size.x = hud_wrap
	hud_encounter_title.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	turn_label.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	telegraph_label.add_theme_color_override("font_color", KyndeBladeArtPalette.RUBRICATION)
	if field_subtitle:
		field_subtitle.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_BODY)
	if moveset_rubric:
		moveset_rubric.add_theme_color_override("font_color", KyndeBladeArtPalette.INK_LIGHT)
	for cap: Label in [ph_bar_caption, enemy_hp_label, st_bar_caption]:
		if cap:
			cap.add_theme_color_override("font_color", KyndeBladeArtPalette.INK_SECONDARY)
	if controls_hint:
		controls_hint.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_BODY)
		controls_hint.custom_minimum_size.x = 392
	hud_encounter_title.custom_minimum_size.x = hud_wrap
	outcome_title.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	outcome_body.add_theme_color_override("font_color", KyndeBladeArtPalette.INK_PRIMARY)
	$PauseLayer/PauseCenter/PauseVBox/PausedTitle.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	$PauseLayer/PauseBg.color = KyndeBladeArtPalette.COMBAT_UI_SCRIM
	$OutcomeLayer/OutcomeBg.color = Color(
		KyndeBladeArtPalette.INK_PRIMARY.r,
		KyndeBladeArtPalette.INK_PRIMARY.g,
		KyndeBladeArtPalette.INK_PRIMARY.b,
		0.9
	)


func _process(delta: float) -> void:
	if get_tree().paused:
		return
	if combat:
		combat.tick_window(delta)


func _input(event: InputEvent) -> void:
	if outcome_layer.visible:
		return
	if event.is_action_pressed("pause"):
		if pause_layer.visible:
			_on_resume_pressed()
		else:
			_toggle_pause()
		return
	if get_tree().paused:
		return
	if combat == null:
		return
	if combat.state == combat.State.WAITING_PLAYER:
		if event.is_action_pressed("strike"):
			combat.player_strike()
	elif combat.state == combat.State.REAL_TIME_WINDOW:
		if event.is_action_pressed("dodge"):
			combat.player_dodge()
		elif event.is_action_pressed("parry"):
			combat.player_parry()


func _toggle_pause() -> void:
	var p := not get_tree().paused
	get_tree().paused = p
	pause_layer.visible = p
	_refresh_combat_action_buttons()


func _on_resume_pressed() -> void:
	get_tree().paused = false
	pause_layer.hide()
	_refresh_combat_action_buttons()


func _on_pause_main_menu_pressed() -> void:
	get_tree().paused = false
	if SaveService.has_save():
		GameState.sync_to_save()
	get_tree().change_scene_to_file("res://scenes/main_menu.tscn")


func _refresh_combat_action_buttons() -> void:
	if strike_btn == null or dodge_btn == null or parry_btn == null:
		return
	var blocked := (
			outcome_layer.visible
			or pause_layer.visible
			or get_tree().paused
			or combat == null
	)
	if blocked:
		strike_btn.disabled = true
		dodge_btn.disabled = true
		parry_btn.disabled = true
		return
	match combat.state:
		combat.State.WAITING_PLAYER:
			strike_btn.disabled = false
			dodge_btn.disabled = true
			parry_btn.disabled = true
		combat.State.REAL_TIME_WINDOW:
			strike_btn.disabled = true
			dodge_btn.disabled = false
			parry_btn.disabled = false
		_:
			strike_btn.disabled = true
			dodge_btn.disabled = true
			parry_btn.disabled = true


func _on_strike_button_pressed() -> void:
	if outcome_layer.visible or pause_layer.visible or get_tree().paused:
		return
	if combat and combat.state == combat.State.WAITING_PLAYER:
		combat.player_strike()
	_refresh_combat_action_buttons()


func _on_dodge_button_pressed() -> void:
	if outcome_layer.visible or pause_layer.visible or get_tree().paused:
		return
	if combat and combat.state == combat.State.REAL_TIME_WINDOW:
		combat.player_dodge()
	_refresh_combat_action_buttons()


func _on_parry_button_pressed() -> void:
	if outcome_layer.visible or pause_layer.visible or get_tree().paused:
		return
	if combat and combat.state == combat.State.REAL_TIME_WINDOW:
		combat.player_parry()
	_refresh_combat_action_buttons()


func _on_turn_changed() -> void:
	if not combat or not turn_label:
		return
	_refresh_combat_action_buttons()
	match combat.state:
		combat.State.WAITING_PLAYER:
			turn_label.text = PiersCombatVoice.player_turn_rubric(
					combat.get_strike_stamina_cost(),
					combat.get_strike_damage_preview(),
					combat.get_dodge_stamina_cost(),
					combat.get_parry_stamina_cost(),
			)
			telegraph_label.text = ""
		combat.State.REAL_TIME_WINDOW:
			var ms_left: int = int(round(combat.window_remaining * 1000.0))
			turn_label.text = "Window: %d ms — withdrawe o shelde!" % ms_left
			telegraph_label.text = PiersCombatVoice.defensive_telegraph(combat.is_enemy_swing_real())
		combat.State.ENEMY_TURN:
			turn_label.text = "False lasteth his tale in the feeld…"
			telegraph_label.text = ""
		combat.State.ENDED:
			turn_label.text = "The fecht is doon."
			telegraph_label.text = ""
		_:
			turn_label.text = "…"
			telegraph_label.text = ""


func _on_defensive_window_started(is_real_swing: bool) -> void:
	# Keep cue shorter than the parry band (170–230 ms) so the ear still reads the window edge.
	var dur := 0.038 if combat.window_duration < 0.28 else 0.065
	var dur_feint := 0.032 if combat.window_duration < 0.28 else 0.055
	if is_real_swing:
		window_sfx_player.stream = CombatWindowTone.make_tone_hz(620.0, dur, 0.2)
	else:
		window_sfx_player.stream = CombatWindowTone.make_tone_hz(260.0, dur_feint, 0.17)
	window_sfx_player.play()


func _on_stats_changed(p_hp: float, p_max: float, e_hp: float, e_max: float, st: float, st_max: float) -> void:
	player_hp_bar.max_value = p_max
	player_hp_bar.value = p_hp
	enemy_hp_bar.max_value = e_max
	enemy_hp_bar.value = e_hp
	stamina_bar.max_value = st_max
	stamina_bar.value = st


func _on_combat_ended(victory: bool) -> void:
	if victory and combat and not combat.use_instant_resolution_for_tests:
		await _play_victory_kill_presentation()
	get_tree().paused = false
	outcome_layer.visible = true
	outcome_layer.process_mode = Node.PROCESS_MODE_ALWAYS
	if victory:
		outcome_title.text = "Victory"
		outcome_body.text = PiersCombatVoice.outcome_victory_line()
		GameState.on_victory_fair_field()
	else:
		outcome_title.text = "Defeat"
		outcome_body.text = PiersCombatVoice.outcome_defeat_line()
		GameState.has_ever_had_hunger = true
		GameState.sync_to_save()
	_refresh_combat_action_buttons()


## Slow-motion + desaturate + deep wound SFX before the victory panel (skipped in headless tests).
func _play_victory_kill_presentation() -> void:
	var mat: ShaderMaterial = kill_desat_rect.material as ShaderMaterial
	kill_fx_layer.visible = true
	var prev_scale: float = Engine.time_scale
	Engine.time_scale = 0.2
	kill_sfx_player.stream = _kill_sfx_stream
	kill_sfx_player.play()
	const RT := true
	const IGNORE_TS := true
	var up_steps := 10
	for i in up_steps:
		var u := float(i + 1) / float(up_steps)
		if mat:
			mat.set_shader_parameter("desat_strength", u * 0.92)
		await get_tree().create_timer(0.028, RT, IGNORE_TS).timeout
	await get_tree().create_timer(0.38, RT, IGNORE_TS).timeout
	for j in up_steps:
		var v := 1.0 - float(j + 1) / float(up_steps)
		if mat:
			mat.set_shader_parameter("desat_strength", v * 0.92)
		await get_tree().create_timer(0.022, RT, IGNORE_TS).timeout
	Engine.time_scale = prev_scale
	if mat:
		mat.set_shader_parameter("desat_strength", 0.0)
	kill_fx_layer.visible = false


func _on_outcome_hub_pressed() -> void:
	outcome_layer.hide()
	get_tree().paused = false
	get_tree().change_scene_to_file(HUB)


func _on_outcome_retry_pressed() -> void:
	outcome_layer.hide()
	get_tree().paused = false
	get_tree().reload_current_scene()

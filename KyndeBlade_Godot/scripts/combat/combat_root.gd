extends Node
## Combat scene: UI, pause (freezes tree + window), outcome → hub.
## Framed as immersion **in the writ** — `ManuscriptNav` page-turns elsewhere; `PiersCombatVoice.field_subtitle` carries the folio line.

const HUB := "res://scenes/hub_map.tscn"
const MAIN_MENU := "res://scenes/main_menu.tscn"
const COMBAT := "res://scenes/combat.tscn"
const _CONTROLS_HINT_BASE := "Esc — Pause"
const _DEFENSE_README_LINE := "\nFeint tincture ≠ true edge: watch the wind-up and ear the pitch ere thou React."

@onready var combat: Node = $CombatManager
@onready var turn_label: Label = %TurnLabel
@onready var telegraph_label: Label = %TelegraphLabel
@onready var field_subtitle: Label = %FieldSubtitle
@onready var moveset_rubric: Label = %MovesetRubric
@onready var help_title: Label = %HelpTitle
@onready var help_body: Label = %HelpBody
@onready var player_hp_bar: ProgressBar = %PlayerHP
@onready var stamina_bar: ProgressBar = %Stamina
@onready var party_hp_nums: Label = %PartyHpNumsA
@onready var footer_hints: Label = %FooterHints
@onready var roster_slot_a: PanelContainer = $UI/ReferenceLeftRoster/RosterVBox/RosterSlotA
@onready var roster_slot_b: PanelContainer = $UI/ReferenceLeftRoster/RosterVBox/RosterSlotB
@onready var roster_slot_c: PanelContainer = $UI/ReferenceLeftRoster/RosterVBox/RosterSlotC
@onready var roster_slot_d: PanelContainer = $UI/ReferenceLeftRoster/RosterVBox/RosterSlotD
@onready var pause_layer: CanvasLayer = $PauseLayer
@onready var outcome_layer: CanvasLayer = $OutcomeLayer
@onready var outcome_title: Label = %OutcomeTitle
@onready var outcome_body: Label = %OutcomeBody
@onready var parry_dodge_eye: Node = %ParryDodgeEye
@onready var window_sfx_player: AudioStreamPlayer = $WindowSfx
@onready var kill_sfx_player: AudioStreamPlayer = $KillSfx
@onready var kill_fx_layer: CanvasLayer = $KillFxLayer
@onready var kill_desat_rect: ColorRect = $KillFxLayer/DesatRect
@onready var controls_hint: Label = %ControlsHint
@onready var strike_btn: Button = $UI/ActionBar/ActionCenter/ActionPanel/ActionInner/ActionSkew/ActionCommandCol/StrikeRow/StrikeCombatButton
@onready var dodge_btn: Button = $UI/ActionBar/ActionCenter/ActionPanel/ActionInner/ActionSkew/ActionCommandCol/DodgeRow/DodgeCombatButton
@onready var parry_btn: Button = $UI/ActionBar/ActionCenter/ActionPanel/ActionInner/ActionSkew/ActionCommandCol/ParryRow/ParryCombatButton
@onready var outcome_hub_btn: Button = $OutcomeLayer/OutcomeCenter/OutcomeVBox/ToHubButton
@onready var outcome_retry_btn: Button = $OutcomeLayer/OutcomeCenter/OutcomeVBox/RetryButton
@onready var top_boss_name: Label = $UI/ReferenceTopBossBar/BossVBox/BossName
@onready var top_boss_hp: ProgressBar = $UI/ReferenceTopBossBar/BossVBox/BossHP

var _kill_sfx_stream: AudioStreamWAV


func _ready() -> void:
	_apply_atmosphere_from_world()
	_apply_art_direction_theme()
	pause_layer.hide()
	outcome_layer.hide()
	if combat.encounter != null:
		var enc: EncounterDef = combat.encounter
		var place: String = str(enc.encounter_id).replace("_", " ")
		top_boss_name.text = "%s · %s" % [place.capitalize(), enc.enemy_display_name]
	else:
		top_boss_name.text = "Shelde-fray"
	if field_subtitle:
		field_subtitle.text = PiersCombatVoice.field_subtitle()
	if moveset_rubric:
		moveset_rubric.text = PiersCombatVoice.granted_kennings_block()
	combat.turn_changed.connect(_on_turn_changed)
	combat.combat_ended.connect(_on_combat_ended)
	combat.stats_changed.connect(_on_stats_changed)
	combat.defensive_window_started.connect(_on_defensive_window_started)
	combat.defensive_windup_started.connect(_on_defensive_windup_started)
	_kill_sfx_stream = CombatWindowTone.make_enemy_kill_impact()
	window_sfx_player.bus = "SFX"
	kill_sfx_player.bus = "SFX"
	if is_instance_valid(parry_dodge_eye) and parry_dodge_eye.has_method("setup"):
		parry_dodge_eye.call("setup", combat)
	if not (is_instance_valid(strike_btn) and is_instance_valid(dodge_btn) and is_instance_valid(parry_btn)):
		push_error(
				"Combat: action bar buttons missing. Expected UI/ActionBar/.../ActionSkew/ActionCommandCol/*Row/*CombatButton — check combat.tscn."
		)
	else:
		strike_btn.pressed.connect(_on_strike_button_pressed)
		dodge_btn.pressed.connect(_on_dodge_button_pressed)
		parry_btn.pressed.connect(_on_parry_button_pressed)
	_on_turn_changed()
	_on_stats_changed(combat.player_hp, combat.player_max_hp, combat.enemy_hp, combat.enemy_max_hp, combat.player_stamina, combat.player_max_stamina)
	_refresh_combat_action_buttons()


func _apply_atmosphere_from_world() -> void:
	var backdrop: Control = $BackdropLayer/ManuscriptBackdrop
	AtmosphereProfile.apply_to_combat_backdrop(backdrop, GameState.current_location_id)
	var w: Dictionary = AtmosphereProfile.weather_preset_for_location(GameState.current_location_id)
	var wx_on := not _suppress_combat_weather()
	WeatherParticles.rebuild_under(
			$BackdropLayer/CombatWeatherRoot,
			get_viewport().get_visible_rect().size,
			str(w.get("preset_id", "mist_calm")),
			float(w.get("intensity", 1.0)),
			wx_on
	)


func _suppress_combat_weather() -> bool:
	return combat != null and combat.use_instant_resolution_for_tests


func _apply_art_direction_theme() -> void:
	# Lane B void + manuscript UI per Unity ART_DIRECTION + UI_MANUSCRIPT_THEME.
	var t: Theme = KyndeBladeManuscriptTheme.build_theme()
	$UI/ActionBar/ActionCenter/ActionPanel.theme = t
	$UI/ReferenceTopBossBar.theme = t
	$UI/ReferenceLeftRoster.theme = t
	$UI/ReferenceRightHelp.theme = t
	$UI/ReferenceBottomParty.theme = t
	$UI/ReferenceTopBossBar.modulate = Color(1.0, 1.0, 1.0, 0.95)
	$UI/ReferenceLeftRoster.modulate = Color(0.96, 0.98, 1.0, 0.86)
	$UI/ReferenceRightHelp.modulate = Color(0.96, 0.98, 1.0, 0.9)
	$UI/ReferenceBottomParty.modulate = Color(0.96, 0.98, 1.0, 0.9)
	if help_title:
		help_title.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	if help_body:
		help_body.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_BODY)
	var stains_icon: Label = $UI/ReferenceRightHelp/HelpVBox/StainsWireIcon as Label
	if stains_icon:
		stains_icon.add_theme_color_override("font_color", Color(0.85, 0.65, 0.2, 0.78))
	$PauseLayer/PauseCenter/PauseVBox.theme = t
	$OutcomeLayer/OutcomeCenter/OutcomeVBox.theme = t
	KyndeBladeManuscriptTheme.style_progress_bar(player_hp_bar, KyndeBladeArtPalette.GOLD, KyndeBladeArtPalette.GOLD_DARK)
	KyndeBladeManuscriptTheme.style_progress_bar(stamina_bar, KyndeBladeArtPalette.LAPIS, KyndeBladeArtPalette.BORDER_BLUE)
	var party_b_pb: ProgressBar = $UI/ReferenceBottomParty/BottomPartyVBox/PartyRowB/PartyColB/PartyHPB
	var party_c_pb: ProgressBar = $UI/ReferenceBottomParty/BottomPartyVBox/PartyRowC/PartyColC/PartyHPC
	KyndeBladeManuscriptTheme.style_progress_bar(party_b_pb, KyndeBladeArtPalette.LAPIS, KyndeBladeArtPalette.BORDER_BLUE)
	KyndeBladeManuscriptTheme.style_progress_bar(party_c_pb, KyndeBladeArtPalette.LAPIS, KyndeBladeArtPalette.BORDER_BLUE)
	const hud_wrap := 252.0
	for lbl: Label in [turn_label, telegraph_label, field_subtitle, moveset_rubric, help_title, help_body]:
		if lbl:
			lbl.custom_minimum_size.x = hud_wrap
	if turn_label:
		turn_label.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	if telegraph_label:
		telegraph_label.add_theme_color_override("font_color", KyndeBladeArtPalette.RUBRICATION)
	if field_subtitle:
		field_subtitle.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_BODY)
	if moveset_rubric:
		moveset_rubric.add_theme_color_override("font_color", KyndeBladeArtPalette.INK_LIGHT)
	if party_hp_nums:
		party_hp_nums.add_theme_color_override("font_color", KyndeBladeArtPalette.INK_SECONDARY)
	if footer_hints:
		footer_hints.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_BODY)
	if controls_hint:
		controls_hint.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_BODY)
		controls_hint.custom_minimum_size.x = 320
		_refresh_controls_hint_line()
	if top_boss_name:
		top_boss_name.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	if top_boss_hp:
		KyndeBladeManuscriptTheme.style_progress_bar(
			top_boss_hp,
			KyndeBladeArtPalette.RUBRICATION,
			KyndeBladeArtPalette.BORDER_RED
		)
	for pl: Label in [
			$UI/ReferenceBottomParty/BottomPartyVBox/PartyRowA/PartyColA/PartyNameA,
			$UI/ReferenceBottomParty/BottomPartyVBox/PartyRowB/PartyColB/PartyNameB,
			$UI/ReferenceBottomParty/BottomPartyVBox/PartyRowC/PartyColC/PartyNameC,
			$UI/ReferenceBottomParty/BottomPartyVBox/PartyRowB/PartyColB/PartyHpNumsB,
			$UI/ReferenceBottomParty/BottomPartyVBox/PartyRowC/PartyColC/PartyHpNumsC,
	]:
		if pl:
			pl.add_theme_color_override("font_color", KyndeBladeArtPalette.INK_SECONDARY)
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
	pause_layer.hide()
	if GameState.demo_gauntlet_active:
		GameState.end_demo_gauntlet_from_defeat()
	if SaveService.has_save():
		GameState.sync_to_save()
	await ManuscriptNav.turn_page_to(MAIN_MENU)


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
		combat.State.DEFENSE_WINDUP:
			strike_btn.disabled = true
			dodge_btn.disabled = true
			parry_btn.disabled = true
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
		combat.State.DEFENSE_WINDUP:
			turn_label.text = PiersCombatVoice.defensive_windup_rubric(combat.defensive_preview_is_real())
			telegraph_label.text = PiersCombatVoice.defensive_telegraph(combat.defensive_preview_is_real())
		combat.State.REAL_TIME_WINDOW:
			var ms_left: int = int(round(combat.window_remaining * 1000.0))
			if combat.is_enemy_turn_reaction_window_active():
				turn_label.text = "React! %.1fs — withdrawe o shelde!" % combat.window_remaining
			else:
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
	_refresh_right_help_card()
	_update_roster_highlight()


func _refresh_right_help_card() -> void:
	if combat == null or help_title == null or help_body == null:
		return
	match combat.state:
		combat.State.WAITING_PLAYER:
			help_title.text = "Commands"
			help_body.text = "Choose a stroke while the shelde-fray waits on thee."
		combat.State.DEFENSE_WINDUP:
			help_title.text = "Gather"
			help_body.text = "Read the wind-up: feint tincture is not the true edge."
		combat.State.REAL_TIME_WINDOW:
			help_title.text = "React"
			help_body.text = "Dodge or parry within the window — withdrawe o shelde."
		combat.State.ENEMY_TURN:
			help_title.text = "False's advance"
			help_body.text = "The enemy spends their beat; hold thy ground."
		combat.State.EXECUTING:
			help_title.text = "Stroke"
			help_body.text = "The cut is loosed; mark what follows."
		combat.State.ENDED:
			help_title.text = "Still"
			help_body.text = "The fecht is measured — await the verdict."
		_:
			help_title.text = "Field"
			help_body.text = "—"


func _roster_slot_style(bright_border: bool, dim: bool) -> StyleBoxFlat:
	var sb := StyleBoxFlat.new()
	sb.bg_color = Color(0, 0, 0, 0)
	sb.set_border_width_all(2)
	var g := 0.42 if dim else 0.52
	sb.border_color = KyndeBladeArtPalette.GOLD if bright_border else Color(g, g * 0.95, g * 0.9, 1.0)
	return sb


func _update_roster_highlight() -> void:
	if combat == null:
		return
	var ended: bool = combat.state == combat.State.ENDED
	var player_active: bool = (
			not ended
			and combat.state != combat.State.ENEMY_TURN
	)
	if roster_slot_a:
		roster_slot_a.add_theme_stylebox_override(
				"panel",
				_roster_slot_style(player_active, false)
		)
		roster_slot_a.self_modulate = Color(1.05, 1.02, 1.0) if player_active else Color(0.78, 0.78, 0.8)
	for slot: PanelContainer in [roster_slot_b, roster_slot_c, roster_slot_d]:
		if slot:
			slot.add_theme_stylebox_override("panel", _roster_slot_style(false, true))
			slot.self_modulate = Color(0.88, 0.88, 0.9) if player_active else Color(0.68, 0.68, 0.7)


func _refresh_controls_hint_line() -> void:
	if not controls_hint:
		return
	var t := _CONTROLS_HINT_BASE
	if not GameState.combat_defense_tip_ack:
		t += _DEFENSE_README_LINE
	controls_hint.text = t


func _on_defensive_windup_started(is_real_swing: bool) -> void:
	if not GameState.combat_defense_tip_ack:
		GameState.combat_defense_tip_ack = true
		GameState.sync_to_save()
		_refresh_controls_hint_line()
	window_sfx_player.stream = CombatWindowTone.make_defensive_windup_chirp(is_real_swing)
	window_sfx_player.play()


func _on_defensive_window_started(is_real_swing: bool) -> void:
	var short_w: bool = combat.window_duration < 0.28
	window_sfx_player.stream = CombatWindowTone.make_defensive_window_open_tone(is_real_swing, short_w)
	window_sfx_player.play()


func _on_stats_changed(p_hp: float, p_max: float, e_hp: float, e_max: float, st: float, st_max: float) -> void:
	player_hp_bar.max_value = p_max
	player_hp_bar.value = p_hp
	if party_hp_nums:
		party_hp_nums.text = "%d / %d" % [int(round(p_hp)), int(round(p_max))]
	if top_boss_hp:
		top_boss_hp.max_value = e_max
		top_boss_hp.value = e_hp
	stamina_bar.max_value = st_max
	stamina_bar.value = st


func _on_combat_ended(victory: bool) -> void:
	if victory and combat and not combat.use_instant_resolution_for_tests:
		await _play_victory_kill_presentation()
	get_tree().paused = false
	outcome_layer.visible = true
	outcome_layer.process_mode = Node.PROCESS_MODE_ALWAYS
	outcome_hub_btn.text = "Continue to hub"
	outcome_retry_btn.visible = true
	if victory:
		outcome_title.text = "Victory"
		if GameState.demo_gauntlet_active:
			var just_finished: int = GameState.demo_gauntlet_next_index
			GameState.demo_gauntlet_after_victory()
			if GameState.demo_gauntlet_exit_to_menu:
				outcome_title.text = "Gauntlet complete"
				outcome_body.text = (
						"Greybox chain done: Fair Field ×2, then Dongeoun gate. "
						+ "Campaign progress was not advanced."
				)
				outcome_hub_btn.text = "Back to main menu"
			else:
				outcome_body.text = "Skirmish %d of %d won — press Next when ready." % [
						just_finished + 1,
						GameState.DEMO_GAUNTLET_ENCOUNTERS.size(),
				]
				outcome_hub_btn.text = "Next skirmish"
		else:
			outcome_body.text = PiersCombatVoice.outcome_victory_line()
			var enc: EncounterDef = combat.encounter as EncounterDef
			if enc != null and enc.encounter_id == "dongeoun_gate":
				GameState.on_victory_dongeoun_gate()
			else:
				GameState.on_victory_fair_field()
	else:
		outcome_title.text = "Defeat"
		outcome_body.text = PiersCombatVoice.outcome_defeat_line()
		if GameState.demo_gauntlet_active:
			GameState.end_demo_gauntlet_from_defeat()
			outcome_body.text = "Gauntlet broken (greybox) — no save progress from these fights."
			outcome_hub_btn.text = "Back to main menu"
			outcome_retry_btn.visible = false
		else:
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
	if GameState.demo_gauntlet_exit_to_menu:
		GameState.finalize_demo_gauntlet_menu_return()
		await ManuscriptNav.turn_page_to(MAIN_MENU)
		return
	if GameState.demo_gauntlet_active:
		await ManuscriptNav.turn_page_to(COMBAT, true)
		return
	await ManuscriptNav.turn_page_to(HUB)


func _on_outcome_retry_pressed() -> void:
	outcome_layer.hide()
	get_tree().paused = false
	if GameState.demo_gauntlet_active:
		GameState.demo_gauntlet_refresh_pending_for_current_round()
	await ManuscriptNav.turn_page_reopen_current()

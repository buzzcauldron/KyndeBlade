extends Control
## Greybox **B** slice: multi-round reaction loop (win 3 pulses before 2 misses) + manuscript chrome.
## Optional art: `assets/ng_open_access/backdrop.png` (NGA Open Access).

const MENU_SCENE := "res://scenes/main_menu.tscn"
const OPTIONAL_ART_CANDIDATES: Array[String] = [
	"res://assets/ng_open_access/backdrop.png",
	"res://assets/ng_open_access/backdrop.jpg",
	"res://assets/ng_open_access/backdrop.jpeg",
]
const RUN_WINS_NEED := 3
const RUN_LOSSES_MAX := 2

@onready var status_label: Label = %StatusLabel
@onready var result_label: Label = %ResultLabel
@onready var art_texture: TextureRect = %ArtTexture
@onready var glow_rect: ColorRect = %GlowRect
@onready var play_again_btn: Button = %PlayAgainButton
@onready var art_hint: Label = %ArtHint
@onready var run_stat_label: Label = %RunStatLabel
@onready var wire_panel: Panel = %WireframePanel

var _awaiting_click: bool = false
var _glow_tween: Tween
var _run_wins: int = 0
var _run_losses: int = 0
var _run_active: bool = false
## Bumped to cancel in-flight timers when leaving or restarting.
var _phase_token: int = 0


func _kill_glow_tween() -> void:
	if is_instance_valid(_glow_tween):
		_glow_tween.kill()
	_glow_tween = null


func _ready() -> void:
	theme = KyndeBladeManuscriptTheme.build_theme()
	$Margin/VBox/TitleLabel.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	$Margin/VBox/CreditLabel.add_theme_color_override("font_color", KyndeBladeArtPalette.LAPIS)
	$Margin/VBox/GreyboxTag.add_theme_color_override("font_color", KyndeBladeArtPalette.INK_LIGHT)
	run_stat_label.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_BODY)
	$Bg.color = KyndeBladeArtPalette.HUB_TWILIGHT
	_style_wireframe_panel()
	_load_optional_art()
	play_again_btn.visible = false
	result_label.text = ""
	_begin_new_run()


func _style_wireframe_panel() -> void:
	var sb := StyleBoxFlat.new()
	sb.set_border_width_all(2)
	sb.border_color = KyndeBladeArtPalette.LAPIS
	sb.bg_color = Color(0.0, 0.0, 0.0, 0.0)
	wire_panel.add_theme_stylebox_override("panel", sb)


func _load_optional_art() -> void:
	for path in OPTIONAL_ART_CANDIDATES:
		if not ResourceLoader.exists(path):
			continue
		var tex: Resource = ResourceLoader.load(path)
		if tex is Texture2D:
			art_texture.texture = tex as Texture2D
			art_hint.visible = false
			art_texture.modulate = Color.WHITE
			return
	art_hint.visible = true
	art_texture.modulate = Color(0.32, 0.28, 0.26)


func _begin_new_run() -> void:
	_phase_token += 1
	_run_wins = 0
	_run_losses = 0
	_run_active = true
	_refresh_run_ui()
	_start_round()


func _refresh_run_ui() -> void:
	run_stat_label.text = "Wireframe run: %d / %d wins · %d / %d losses (then stop)" % [
			_run_wins,
			RUN_WINS_NEED,
			_run_losses,
			RUN_LOSSES_MAX,
	]


func _start_round() -> void:
	if not _run_active:
		return
	_phase_token += 1
	var phase := _phase_token
	_awaiting_click = false
	result_label.text = ""
	play_again_btn.visible = false
	glow_rect.color = Color(0.78, 0.62, 0.35, 0.0)
	status_label.text = "Wait for the gold pulse (wireframe cue)…"
	var delay: float = randf_range(1.2, 2.8)
	await get_tree().create_timer(delay).timeout
	if phase != _phase_token or not is_inside_tree() or not _run_active:
		return
	await _open_reaction_window(phase)


func _open_reaction_window(phase: int) -> void:
	if phase != _phase_token or not _run_active:
		return
	_awaiting_click = true
	status_label.text = "Now — click, Space, or Enter"
	_kill_glow_tween()
	_glow_tween = create_tween()
	_glow_tween.tween_property(glow_rect, "color:a", 0.55, 0.12)
	await get_tree().create_timer(0.75).timeout
	if phase != _phase_token or not is_inside_tree():
		return
	if _awaiting_click:
		_awaiting_click = false
		_kill_glow_tween()
		glow_rect.color = Color(0.78, 0.62, 0.35, 0.0)
		_show_outcome(false)


func _unhandled_input(event: InputEvent) -> void:
	if not _awaiting_click:
		return
	if event.is_action_pressed("dodge") or event.is_action_pressed("ui_confirm"):
		get_viewport().set_input_as_handled()
		_resolve_success()
	elif event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		get_viewport().set_input_as_handled()
		_resolve_success()


func _resolve_success() -> void:
	if not _awaiting_click:
		return
	_awaiting_click = false
	_kill_glow_tween()
	glow_rect.color = Color(0.45, 0.75, 0.5, 0.45)
	_show_outcome(true)


func _show_outcome(won: bool) -> void:
	status_label.text = "Pulse resolved."
	if won:
		_run_wins += 1
		result_label.text = "Hit — thou caught the window."
	else:
		_run_losses += 1
		result_label.text = "Miss — the glow fled."
	_refresh_run_ui()
	if _run_wins >= RUN_WINS_NEED:
		_run_active = false
		status_label.text = "Run complete (greybox)."
		result_label.text = "Three true beats — wireframe exercise passed."
		play_again_btn.visible = true
	elif _run_losses >= RUN_LOSSES_MAX:
		_run_active = false
		status_label.text = "Run ended (greybox)."
		result_label.text = "Two misses — begin the run anew when thou wilt."
		play_again_btn.visible = true
	else:
		_schedule_next_round()


func _schedule_next_round() -> void:
	var phase := _phase_token
	await get_tree().create_timer(0.48).timeout
	if phase != _phase_token or not is_inside_tree() or not _run_active:
		return
	_start_round()


func _on_play_again_pressed() -> void:
	play_again_btn.disabled = true
	_begin_new_run()
	play_again_btn.disabled = false


func _on_back_to_menu_pressed() -> void:
	_phase_token += 1
	_run_active = false
	await ManuscriptNav.turn_page_to(MENU_SCENE)

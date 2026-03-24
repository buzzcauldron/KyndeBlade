extends Control
## Minimal game loop: wait → react (click / Space / Enter) → win or lose → play again.
## Optional art: place `assets/ng_open_access/backdrop.png` (see [NGA Open Access](https://www.nga.gov/artworks/free-images-and-open-access)).

const MENU_SCENE := "res://scenes/main_menu.tscn"
const OPTIONAL_ART_CANDIDATES: Array[String] = [
	"res://assets/ng_open_access/backdrop.png",
	"res://assets/ng_open_access/backdrop.jpg",
	"res://assets/ng_open_access/backdrop.jpeg",
]

@onready var status_label: Label = %StatusLabel
@onready var result_label: Label = %ResultLabel
@onready var art_texture: TextureRect = %ArtTexture
@onready var glow_rect: ColorRect = %GlowRect
@onready var play_again_btn: Button = %PlayAgainButton
@onready var art_hint: Label = %ArtHint

var _awaiting_click: bool = false
var _glow_tween: Tween


func _kill_glow_tween() -> void:
	if is_instance_valid(_glow_tween):
		_glow_tween.kill()
	_glow_tween = null


func _ready() -> void:
	theme = KyndeBladeManuscriptTheme.build_theme()
	$Margin/VBox/TitleLabel.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	$Margin/VBox/CreditLabel.add_theme_color_override("font_color", KyndeBladeArtPalette.LAPIS)
	$Bg.color = KyndeBladeArtPalette.HUB_TWILIGHT
	_load_optional_art()
	play_again_btn.visible = false
	result_label.text = ""
	_start_round()


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


func _start_round() -> void:
	_awaiting_click = false
	result_label.text = ""
	play_again_btn.visible = false
	glow_rect.color = Color(0.78, 0.62, 0.35, 0.0)
	status_label.text = "Wait for the gold glow on the frame…"
	# Random quiet moment (museum hush), then a short reaction window.
	var delay: float = randf_range(1.4, 3.2)
	await get_tree().create_timer(delay).timeout
	if not is_inside_tree():
		return
	await _open_reaction_window()


func _open_reaction_window() -> void:
	_awaiting_click = true
	status_label.text = "Now! Click, Space, or Enter"
	_kill_glow_tween()
	_glow_tween = create_tween()
	_glow_tween.tween_property(glow_rect, "color:a", 0.55, 0.12)
	await get_tree().create_timer(0.75).timeout
	if not is_inside_tree():
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
	status_label.text = "Round complete."
	if won:
		result_label.text = "You caught the moment — win."
	else:
		result_label.text = "Missed the beat — try again."
	play_again_btn.visible = true
	play_again_btn.disabled = false


func _on_play_again_pressed() -> void:
	play_again_btn.disabled = true
	_start_round()


func _on_back_to_menu_pressed() -> void:
	get_tree().change_scene_to_file(MENU_SCENE)

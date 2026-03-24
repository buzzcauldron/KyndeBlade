extends Control
## New Game opening: Malvern prologue crawl (grass, streme, right bank — door-style enter) in 320×180.
## Does not grant `tower_vista`; that stays on tower_intro Continue.

const TOWER_INTRO := "res://scenes/tower_intro.tscn"
const MENU_SCENE := "res://scenes/main_menu.tscn"

@onready var _sub: SubViewport = (
		$SheetMargin/ManuscriptPanel/PageInner/VBox/SubViewportContainer/SubViewport
)
@onready var _win_label: Label = %WinLabel
@onready var _flavor: Label = %FlavorLabel
@onready var _title: Label = %YardTitle
@onready var _subtitle: Label = %YardSubtitle

const DREAM_FAIRY_DURATION_SEC := 2.55

var _won: bool = false
var _advancing: bool = false
var _dream_playing: bool = false


func _ready() -> void:
	theme = KyndeBladeManuscriptTheme.build_theme()
	_apply_manuscript_chrome()
	var world: Node2D = _sub.get_node("World")
	world.goal_reached.connect(_on_goal_reached)
	world.fell_in_moat.connect(_on_fell_in_moat)
	_win_label.visible = false
	_flavor.text = (
			"Walk the lea in a somer seson. When thou reachest the streme, "
			+ "step upon the right-hand bank — there thou liest down, "
			+ "and small fayery lights gather ere the dreme taketh thee thence."
	)


func _apply_manuscript_chrome() -> void:
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
	_title.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)
	_subtitle.add_theme_color_override("font_color", KyndeBladeArtPalette.INK_SECONDARY)
	_flavor.add_theme_color_override("font_color", KyndeBladeArtPalette.VISTA_BODY)
	_win_label.add_theme_color_override("font_color", KyndeBladeArtPalette.GOLD)


func _on_goal_reached() -> void:
	if _won or _advancing or _dream_playing:
		return
	_dream_playing = true
	var world: Node2D = _sub.get_node("World")
	var p: CharacterBody2D = world.get_node("Player")
	if p.has_method("freeze"):
		p.freeze()
	if p.has_method("play_dream_lie_down"):
		await p.play_dream_lie_down()
	var swarm: Node = world.get_node_or_null("DreamFairySwarm")
	var dream_center: Vector2 = p.global_position + Vector2(2, -8)
	if swarm != null and swarm.has_method("start_swarm"):
		swarm.start_swarm(dream_center)
	_win_label.visible = true
	await get_tree().create_timer(DREAM_FAIRY_DURATION_SEC).timeout
	if swarm != null and swarm.has_method("stop_swarm"):
		swarm.stop_swarm()
	_dream_playing = false
	if _won or _advancing:
		return
	_won = true
	_advancing = true
	GameState.record_location_visit("tour")
	GameState.sync_to_save()
	await ManuscriptNav.turn_page_to(TOWER_INTRO)


func _on_fell_in_moat() -> void:
	_flavor.text = (
			"The streme is colde upon thy shoon — a ford, not a bath; "
			+ "the soft bank to thy right hand is where thou liest down."
	)


func _on_back_pressed() -> void:
	await ManuscriptNav.turn_page_to(MENU_SCENE)

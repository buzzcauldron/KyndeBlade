extends Control
## High-bit medieval bonus: HUD + wires SubViewport world to win / menu.

const MENU_SCENE := "res://scenes/main_menu.tscn"

@onready var _sub: SubViewport = $SubViewportContainer/SubViewport
@onready var _win_label: Label = %WinLabel
@onready var _flavor: Label = %FlavorLabel

var _won: bool = false


func _ready() -> void:
	var world: Node2D = _sub.get_node("World")
	world.goal_reached.connect(_on_goal_reached)
	world.fell_in_moat.connect(_on_fell_in_moat)
	_win_label.visible = false
	_flavor.text = "The duck-board is sponsored by nobody. Mind the moat."


func _on_goal_reached() -> void:
	if _won:
		return
	_won = true
	_win_label.visible = true
	var p: CharacterBody2D = _sub.get_node("World/Player")
	if p.has_method("freeze"):
		p.freeze()


func _on_fell_in_moat() -> void:
	_flavor.text = "Splash! Thou art damp. The moat says hi."


func _on_back_pressed() -> void:
	get_tree().change_scene_to_file(MENU_SCENE)

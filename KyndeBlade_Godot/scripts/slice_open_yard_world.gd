extends Node2D
## Malvern-hills prologue crawl: grass, streme, then the right bank (like hi_bit **Goal** — enter to rest).
## Camera scrolls; stepping into **StreamRest** freezes the crawl and hands off to the dream beat.

signal goal_reached
## Stream ford — one quip when thou first wettest thy shoon.
signal fell_in_moat

const WORLD_WIDTH_PX := 512

@onready var player: CharacterBody2D = $Player
@onready var stream_rest: Area2D = $StreamRest
@onready var stream_ford: Area2D = $StreamFord

var _camera: Camera2D
var _stream_quip_shown: bool = false
var _stream_rest_done: bool = false


func _ready() -> void:
	_camera = get_parent().get_node_or_null("Camera2D") as Camera2D
	if _camera != null:
		_camera.limit_right = WORLD_WIDTH_PX
	stream_rest.body_entered.connect(_on_stream_rest_entered)
	stream_ford.body_entered.connect(_on_stream_entered)


func _on_stream_rest_entered(body: Node) -> void:
	if _stream_rest_done or not _is_wanderer(body):
		return
	_stream_rest_done = true
	goal_reached.emit()


func _on_stream_entered(body: Node) -> void:
	if not _is_wanderer(body) or _stream_quip_shown:
		return
	_stream_quip_shown = true
	fell_in_moat.emit()


func _is_wanderer(body: Node) -> bool:
	return body.is_in_group("slice_open_yard_player")


func _physics_process(_delta: float) -> void:
	if _camera != null and is_instance_valid(player):
		var cx: float = clampf(player.global_position.x, 160.0, float(WORLD_WIDTH_PX) - 160.0)
		_camera.global_position = Vector2(cx, 90.0)

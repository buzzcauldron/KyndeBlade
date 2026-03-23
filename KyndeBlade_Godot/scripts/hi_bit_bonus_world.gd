extends Node2D
## Connects goal + moat to the HUD root.

signal goal_reached
signal fell_in_moat

@onready var player: CharacterBody2D = $Player


func _ready() -> void:
	$Goal.body_entered.connect(_on_goal_entered)
	$Moat.body_entered.connect(_on_moat_entered)


func _on_goal_entered(body: Node) -> void:
	if body.is_in_group("hi_bit_bonus_player"):
		goal_reached.emit()


func _on_moat_entered(body: Node) -> void:
	if body.is_in_group("hi_bit_bonus_player"):
		fell_in_moat.emit()
		if player.has_method("respawn"):
			player.respawn()

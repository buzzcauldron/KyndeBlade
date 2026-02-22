extends Node2D
## Kynde Blade Combat Prototype - Main scene
## Turn-based + parry/dodge window combat

@onready var combat: Node = $CombatManager
@onready var turn_label: Label = $UI/TurnLabel

func _ready() -> void:
	if combat:
		combat.turn_changed.connect(_on_turn_changed)
		combat.combat_ended.connect(_on_combat_ended)
	_update_ui()

func _input(event: InputEvent) -> void:
	if not combat or combat.state != combat.State.WAITING_PLAYER:
		return
	if event.is_action_pressed("strike") or (event is InputEventKey and event.pressed and event.keycode == KEY_X):
		combat.player_strike()
	elif event.is_action_pressed("dodge") or (event is InputEventKey and event.pressed and event.keycode == KEY_SPACE):
		combat.player_dodge()
	elif event.is_action_pressed("parry") or (event is InputEventKey and event.pressed and event.keycode == KEY_SHIFT):
		combat.player_parry()

func _process(_delta: float) -> void:
	if combat and combat.state == combat.State.REAL_TIME_WINDOW:
		combat._update_window(_delta)

func _on_turn_changed() -> void:
	_update_ui()

func _on_combat_ended(victory: bool) -> void:
	turn_label.text = "Victory!" if victory else "Defeat!"

func _update_ui() -> void:
	if not turn_label or not combat:
		return
	match combat.state:
		combat.State.WAITING_PLAYER:
			turn_label.text = "Your turn | Strike (X) / Dodge (Space) / Parry (Shift)"
		combat.State.REAL_TIME_WINDOW:
			turn_label.text = "Dodge/Parry! %.1fs" % combat.window_remaining
		combat.State.ENEMY_TURN:
			turn_label.text = "Enemy turn..."
		_:
			turn_label.text = "Combat"

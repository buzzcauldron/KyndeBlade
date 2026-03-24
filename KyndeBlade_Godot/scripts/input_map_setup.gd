extends Node
## Registers default keyboard (and basic gamepad) bindings if actions have no events.
## Runs before other autoloads via order in project.godot.

func _enter_tree() -> void:
	_add_key("strike", KEY_X)
	_add_key_if_absent("strike", KEY_1)
	_add_key_if_absent("strike", KEY_KP_1)
	_add_key("dodge", KEY_SPACE)
	_add_key("parry", KEY_SHIFT)
	_add_key("pause", KEY_ESCAPE)
	_add_key("ui_confirm", KEY_ENTER)
	_add_key("ui_confirm", KEY_KP_ENTER)
	_add_key("ui_cancel", KEY_ESCAPE)

	# High-bit bonus level (platformer slice)
	_add_key("hi_bit_move_left", KEY_A)
	_add_key("hi_bit_move_left", KEY_LEFT)
	_add_key("hi_bit_move_right", KEY_D)
	_add_key("hi_bit_move_right", KEY_RIGHT)
	_add_key("hi_bit_jump", KEY_SPACE)
	_add_key("hi_bit_jump", KEY_W)
	_add_key("hi_bit_jump", KEY_UP)

	# Optional: gamepad (first connected style)
	_add_joy_button("strike", JOY_BUTTON_X)
	_add_joy_button("dodge", JOY_BUTTON_A)
	_add_joy_button("parry", JOY_BUTTON_B)
	_add_joy_button("pause", JOY_BUTTON_START)


func _add_key(action: String, keycode: Key) -> void:
	if not InputMap.has_action(action):
		InputMap.add_action(action, 0.5)
	var has_key := false
	for ev in InputMap.action_get_events(action):
		if ev is InputEventKey:
			has_key = true
			break
	if has_key:
		return
	var e := InputEventKey.new()
	e.keycode = keycode
	InputMap.action_add_event(action, e)


func _add_key_if_absent(action: String, keycode: Key) -> void:
	if not InputMap.has_action(action):
		InputMap.add_action(action, 0.5)
	for ev in InputMap.action_get_events(action):
		if ev is InputEventKey and (ev as InputEventKey).keycode == keycode:
			return
	var e := InputEventKey.new()
	e.keycode = keycode
	InputMap.action_add_event(action, e)


func _add_joy_button(action: String, button: JoyButton) -> void:
	if not InputMap.has_action(action):
		InputMap.add_action(action, 0.5)
	for ev in InputMap.action_get_events(action):
		if ev is InputEventJoypadButton and (ev as InputEventJoypadButton).button_index == button:
			return
	var j := InputEventJoypadButton.new()
	j.button_index = button
	InputMap.action_add_event(action, j)

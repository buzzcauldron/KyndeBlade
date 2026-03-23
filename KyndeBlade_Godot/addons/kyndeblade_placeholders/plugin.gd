@tool
extends EditorPlugin


func _enter_tree() -> void:
	add_tool_menu_item("KyndeBlade: Validate art placeholders", _on_validate_placeholders)


func _exit_tree() -> void:
	remove_tool_menu_item("KyndeBlade: Validate art placeholders")


func _on_validate_placeholders() -> void:
	PlaceholderArtRegistry.clear_cache()
	LocationRegistry.clear_cache()
	var ok1 := PlaceholderArtRegistry.validate_coverage()
	var ok2 := PlaceholderArtRegistry.validate_characters_known()
	if ok1 and ok2:
		print_editor("KyndeBlade placeholders: OK (all locations + core characters defined).")
	else:
		push_warning("KyndeBlade placeholders: validation failed — see Errors / Output.")


func print_editor(msg: String) -> void:
	print(msg)

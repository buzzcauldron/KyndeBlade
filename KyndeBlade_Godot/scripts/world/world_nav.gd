extends Node
## Autoload: navigate the **world skeleton** (preview shells + atlas). See [`docs/GAME_SKELETON.md`](../../docs/GAME_SKELETON.md).

const LOCATION_SHELL := "res://scenes/world/location_shell.tscn"
const WORLD_ATLAS := "res://scenes/world/world_atlas.tscn"
const HUB := "res://scenes/hub_map.tscn"

var pending_location_id: String = ""


func open_location_preview(location_id: String) -> void:
	pending_location_id = location_id.strip_edges()
	if pending_location_id.is_empty():
		return
	get_tree().change_scene_to_file(LOCATION_SHELL)


func open_world_atlas() -> void:
	get_tree().change_scene_to_file(WORLD_ATLAS)


func go_to_hub() -> void:
	pending_location_id = ""
	get_tree().change_scene_to_file(HUB)


func clear_pending() -> void:
	pending_location_id = ""

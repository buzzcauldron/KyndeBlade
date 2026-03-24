extends Node
## Autoload: navigate the **world skeleton** (preview shells + atlas).
## See [`docs/GAME_SKELETON.md`](../../docs/GAME_SKELETON.md).
## Scene changes await [ManuscriptNav] **page turns** — call sites should `await`
## [method open_location_preview], [method open_world_atlas], [method go_to_hub].

const LOCATION_SHELL := "res://scenes/world/location_shell.tscn"
const WORLD_ATLAS := "res://scenes/world/world_atlas.tscn"
const HUB := "res://scenes/hub_map.tscn"
const NAV_TEST_YARD := "res://scenes/nav_test_yard.tscn"

var pending_location_id: String = ""


func open_location_preview(location_id: String) -> void:
	pending_location_id = location_id.strip_edges()
	if pending_location_id.is_empty():
		return
	await ManuscriptNav.turn_page_to(LOCATION_SHELL)


func open_world_atlas() -> void:
	await ManuscriptNav.turn_page_to(WORLD_ATLAS)


func go_to_hub() -> void:
	pending_location_id = ""
	await ManuscriptNav.turn_page_to(HUB)


func open_nav_test_yard() -> void:
	await ManuscriptNav.turn_page_to(NAV_TEST_YARD)


func clear_pending() -> void:
	pending_location_id = ""

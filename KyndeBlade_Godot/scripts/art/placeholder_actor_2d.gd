class_name PlaceholderActor2D
extends Node2D
## Art-bible placeholder figure: set `character_id` to a key in [`data/art/placeholder_registry.json`](../../data/art/placeholder_registry.json) `characters`.

@export var character_id: String = "will_dreamer"


func _ready() -> void:
	apply_character()


func apply_character() -> void:
	PlaceholderSilhouetteLibrary.clear_and_build(character_id, self)

extends Node2D
## Fake-voxel crawl **shell** (phase C0). Combat resolves in `combat.tscn` only.
## Design: [`docs/DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md`](DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md).

@onready var _shell_label: Label = %CrawlShellLabel


func _ready() -> void:
	if _shell_label:
		_shell_label.text = "Crawl overworld (stub — TileMap pass in C1)"

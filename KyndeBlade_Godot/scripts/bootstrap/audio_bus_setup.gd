extends Node
## Ensures **Music** and **SFX** buses exist (W7 audio shell). Master volume still drives `Master` via SaveService.


func _ready() -> void:
	_ensure_named_bus("Music")
	_ensure_named_bus("SFX")


func _ensure_named_bus(bus_name: String) -> void:
	if AudioServer.get_bus_index(bus_name) >= 0:
		return
	AudioServer.add_bus(-1)
	var idx: int = AudioServer.get_bus_count() - 1
	AudioServer.set_bus_name(idx, bus_name)

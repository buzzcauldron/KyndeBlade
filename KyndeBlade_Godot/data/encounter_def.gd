class_name EncounterDef
extends Resource
## Data-driven encounter for slice parity (Godot). Unity oracle: Loc_fayre_felde / False.

@export var encounter_id: String = "fayre_felde"
@export var enemy_id: String = "false"
@export var enemy_display_name: String = "False"
@export var enemy_max_hp: float = 80.0
@export var player_strike_damage: float = 15.0
@export var strike_stamina_cost: float = 15.0
@export var enemy_turn_damage: float = 18.0
## Duration of dodge/parry reaction window when enemy damage is gated (parity: Unity offensive `SuccessWindow`).
@export var enemy_attack_reaction_window_seconds: float = 1.2
## `auto` picks from `enemy_id` (false → langage_fals; green → green_chapel). See [`CombatBookIntro`](../../scripts/combat_book_intro.gd).
@export var book_intro_theme: String = "auto"


static func resolve_book_intro_theme(enc: EncounterDef) -> String:
	if enc == null:
		return "default"
	var t: String = enc.book_intro_theme.strip_edges().to_lower()
	if t != "auto" and not t.is_empty():
		return t
	var eid: String = enc.enemy_id.to_lower()
	if eid.contains("green") or eid.contains("grene") or eid.contains("chapel"):
		return "green_chapel"
	if eid.contains("false") or eid.contains("fals"):
		return "langage_fals"
	return "default"

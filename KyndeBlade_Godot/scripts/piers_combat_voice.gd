extends RefCounted
class_name PiersCombatVoice
## Middle English / archaic-modern **combat copy** rooted in *Piers Plowman* (fair field, Tower, labour, Langage fals).
## Pairs with [`PlayerMovesetModifiers`](player_moveset_modifiers.gd) for numbers; flavor lines map [`medieval_text_unlocks.json`](../data/medieval_text_unlocks.json) codes.


const CODE_KENNING := {
	"dreamer_ledger_stride": "The ledgere of thy dreme lengtheth the stroke.",
	"hunger_strike_surge": "Hunger whetteth the yren—yet it eteth thy breeth.",
	"crowd_surge": "The multitude crieth; Medes echo maketh thyn arme hevy.",
	"ward_warden": "The Grene chapels covenant biddeth thee kepe thy ward—costly withdrawyng.",
	"gk_pressure_trace": "A grene trace folewith—more encountres on the weye (world hook).",
	"fae_drift_step": "Fayerie cadence lighteth thy foot—othir londes drawe nerre.",
	"drowse_guard": "Somer seson lulleth the herte; thy blowe is softe but spares stamyn.",
}


static func field_subtitle() -> String:
	return "On the fayr feeld ful of folke — Langage fals abydeth."


static func strike_action_name() -> String:
	return "Plouȝ-trewe stroke"


static func dodge_action_name() -> String:
	return "Withdrawyng fro the fals feyntinge"


static func parry_action_name() -> String:
	return "Shelde of Conscience"


static func enemy_epithet(display_name: String) -> String:
	if display_name.to_lower().contains("false"):
		return "%s — he that speketh trewe til it be fals" % display_name
	return display_name


static func player_turn_rubric(strike_stam: float, strike_dmg: float, dodge_stam: float) -> String:
	return "%s (%.0f stam / %.0f smyting)  ·  %s (%.0f stam)  ·  %s (25 stam)" % [
		strike_action_name(),
		strike_stam,
		strike_dmg,
		dodge_action_name(),
		dodge_stam,
		parry_action_name(),
	]


static func defensive_telegraph(is_real_swing: bool) -> String:
	return "A soth swyng!" if is_real_swing else "Fals feynteth — loke wel!"


static func granted_kennings_block() -> String:
	var codes := MedievalTextCatalog.list_granted_codes_in_order(GameState.read_medieval_text_ids)
	if codes.is_empty():
		return "No lettres yet bounde to thy labour in this dreme."
	var lines: PackedStringArray = PackedStringArray()
	for c in codes:
		var line: String = str(CODE_KENNING.get(c, "— %s (unwrit kennynge)" % c))
		lines.append("• " + line)
	return "Lettres redde:\n" + "\n".join(lines)


static func outcome_victory_line() -> String:
	return "Fals boweth in the feeld. Thy save is writ — tour and feeld rememberen."


static func outcome_defeat_line() -> String:
	return "The dreme fadeth; Hunger gnaweth the name thou lef test. Autosave hath kept thy lettres."

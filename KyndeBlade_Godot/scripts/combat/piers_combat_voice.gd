extends RefCounted
class_name PiersCombatVoice
## Middle English / archaic-modern **combat copy** rooted in *Piers Plowman* (fair field, Tower, labour, Langage fals).
## Pairs with [`PlayerMovesetModifiers`](player_moveset_modifiers.gd) for numbers; flavor lines map [`medieval_text_unlocks.json`](../../data/medieval_text_unlocks.json) codes.


const CODE_KENNING := {
	"dreamer_ledger_stride": "The ledgere of thy dreme lengtheth the stroke — a dear bargayn.",
	"hunger_strike_surge": "Hunger whetteth the yren, yet it gnaweth thyn shelde-hand never the same wyse twice.",
	"crowd_surge": "The multitude crieth; Medes echo maketh thyn arme hevy and thy rede of fals untrewe.",
	"ward_warden": "The Grene chapels covenant biddeth kepen of ward — withdrawyng groweth costly and oȝe.",
	"gk_pressure_trace": "A grene trace folewith; the next covenant feleth nerre, odd and kepe.",
	"fae_drift_step": "Fayerie cadence lighteth the foot — the other world leyeth a finger on thy feint-rede.",
	"drowse_guard": "Somer seson lulleth the herte; softe blowe, spared stamyn, a small mercy in a cruel feeld.",
}


static func field_subtitle() -> String:
	return "Thou art sunk into the writ itself — the fayr feeld ful of folke; Langage fals abydeth."


static func strike_action_name() -> String:
	return "Plouȝ-trewe stroke"


static func dodge_action_name() -> String:
	return "Withdrawyng fro the fals feyntinge"


static func parry_action_name() -> String:
	return "Shelde of Conscience"


static func enemy_epithet(display_name: String) -> String:
	var low := display_name.to_lower()
	if low.contains("false"):
		return "%s — he that speketh trewe til it be fals" % display_name
	if low.contains("warden") or low.contains("gate"):
		return "%s — kepere of the writ-mount" % display_name
	return display_name


static func player_turn_rubric(strike_stam: float, strike_dmg: float, dodge_stam: float, parry_stam: float) -> String:
	return "%s (%.0f stam / %.0f smyting)  ·  %s (%.0f stam)  ·  %s (%.0f stam)" % [
		strike_action_name(),
		strike_stam,
		strike_dmg,
		dodge_action_name(),
		dodge_stam,
		parry_action_name(),
		parry_stam,
	]


static func defensive_telegraph(is_real_swing: bool) -> String:
	return "A soth swyng!" if is_real_swing else "Fals feynteth — loke wel!"


static func defensive_windup_rubric(is_real_swing: bool) -> String:
	if is_real_swing:
		return "The warden draweth true stel — withdrawe ere the edge falleth!"
	return "A fals wind riseth — rede the feynt, spend not thy shelde on naught."


static func granted_kennings_block() -> String:
	var codes := MedievalTextCatalog.list_granted_codes_in_order(GameState.read_medieval_text_ids)
	if codes.is_empty():
		return "No lettres yet bounde to thy labour in this dreme."
	var lines: PackedStringArray = PackedStringArray()
	for c in codes:
		var line: String = str(CODE_KENNING.get(c, "— %s (unwrit kennynge)" % c))
		lines.append("• " + line)
	var tail := ""
	if GameState.has_ever_had_hunger:
		tail = "\n\nHunger’s countenance shifteth — shelde and tyme pay uneven tribute."
	if PlayerMovesetModifiers.fae_chance_delta() > 0.0001:
		tail += "\nThe fae-weight leaneth on the field — odd spawns when the crawl knows that lettre."
	if PlayerMovesetModifiers.green_knight_weight_delta() > 0.0001:
		tail += "\nGrene pressure traceth thee — the chapel’s math groweth stranger."
	return "Lettres redde:\n" + "\n".join(lines) + tail


static func outcome_victory_line() -> String:
	return "Fals boweth in the feeld. Thy save is writ — tour and feeld rememberen."


static func outcome_defeat_line() -> String:
	return "The dreme fadeth; Hunger gnaweth the name thou lef test. Autosave hath kept thy lettres."

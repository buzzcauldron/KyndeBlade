extends RefCounted
class_name PlayerMovesetModifiers
## Aggregates combat deltas from `GameState.read_medieval_text_ids` via [`MedievalTextCatalog`](medieval_text_catalog.gd).
## **Design bar:** odd, **punishing-but-rewarding** — Mede/hunger/fae/GK hooks stack pressure while some lettres sharpen damage or tempo at a hidden cost.


static func aggregate_totals() -> Dictionary:
	return MedievalTextCatalog.aggregate_totals(GameState.read_medieval_text_ids)


static func strike_stamina_cost_delta() -> float:
	return float(aggregate_totals().get("strike_stamina_cost_delta", 0.0))


static func strike_damage_delta() -> float:
	return float(aggregate_totals().get("strike_damage_delta", 0.0))


static func feint_pattern_offset_delta() -> int:
	return int(aggregate_totals().get("feint_pattern_offset_delta", 0))


static func dodge_stamina_flat_extra() -> float:
	return float(aggregate_totals().get("dodge_stamina_flat_extra", 0.0))


static func fae_chance_delta() -> float:
	return float(aggregate_totals().get("fae_chance_delta", 0.0))


static func green_knight_weight_delta() -> float:
	return float(aggregate_totals().get("green_knight_random_weight_delta", 0.0))


## Deterministic “whimsy” so Hunger’s bite **wobbles slightly** (same save state → same numbers; new defeats / travel → new mix).
static func _meta_whimsy_seed() -> int:
	var parts: PackedStringArray = PackedStringArray(
			[
				GameState.current_location_id,
				str(GameState.ethical_misstep_count),
				str(GameState.dream_iteration),
				str(GameState.fair_field_return_count),
				str(int(GameState.has_ever_had_hunger)),
			]
	)
	return hash("|".join(parts))


## Extra parry-window shrink when `has_ever_had_hunger` (replaces a flat +25 ms).
static func hunger_parry_ms_penalty() -> int:
	if not GameState.has_ever_had_hunger:
		return 0
	var s := _meta_whimsy_seed()
	return 20 + (abs(s) % 11)


## Extra stamina tax on parry while Hunger is named — small band so the body never quite trusts the timing.
static func parry_stamina_total() -> float:
	var base := 25.0
	if GameState.has_ever_had_hunger:
		base += 2.0 + float(abs(_meta_whimsy_seed()) % 7)
	return base


## Parry reaction window in milliseconds (Lane B read). Clamped **170–230**; shrinks with **varying** hunger, missteps, and data `parry_window_ms_penalty` grants.
static func parry_window_ms() -> int:
	const MS_MAX := 230
	const MS_MIN := 170
	var penalty := int(aggregate_totals().get("parry_window_ms_penalty", 0))
	penalty += hunger_parry_ms_penalty()
	penalty += mini(int(GameState.ethical_misstep_count) * 10, 30)
	return clampi(MS_MAX - penalty, MS_MIN, MS_MAX)

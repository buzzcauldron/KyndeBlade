extends RefCounted
class_name PlayerMovesetModifiers
## Aggregates combat deltas from `GameState.read_medieval_text_ids` via [`MedievalTextCatalog`](medieval_text_catalog.gd).


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


## Parry reaction window in milliseconds (Lane B read). Clamped **170–230**; shrinks with hunger, missteps, and data `parry_window_ms_penalty` grants.
static func parry_window_ms() -> int:
	const MS_MAX := 230
	const MS_MIN := 170
	var penalty := int(aggregate_totals().get("parry_window_ms_penalty", 0))
	if GameState.has_ever_had_hunger:
		penalty += 25
	penalty += mini(int(GameState.ethical_misstep_count) * 10, 30)
	return clampi(MS_MAX - penalty, MS_MIN, MS_MAX)

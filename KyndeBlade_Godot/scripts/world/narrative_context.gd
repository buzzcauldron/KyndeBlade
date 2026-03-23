extends RefCounted
class_name NarrativeContext
## Stub helpers for future dialogue JSON — variant keys from `GameState` visit counts. See [`docs/PIERS_SOURCE_VERSIONING.md`](../../docs/PIERS_SOURCE_VERSIONING.md).


## Returns `first` when the player has at most one recorded visit; otherwise `return_<n>` where n is the visit count.
static func arrival_variant_key(location_id: String, game_state: Node = null) -> String:
	var gs: Node = game_state if game_state != null else GameState
	var lid := location_id.strip_edges()
	if lid.is_empty() or gs == null:
		return "first"
	var counts: Variant = gs.get("location_visit_counts")
	if typeof(counts) != TYPE_DICTIONARY:
		return "first"
	var n: int = int((counts as Dictionary).get(lid, 0))
	if n <= 1:
		return "first"
	return "return_%d" % n

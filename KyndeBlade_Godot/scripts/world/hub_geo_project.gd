extends RefCounted
class_name HubGeoProject
## Equirectangular **Wales bbox** → normalized hub `x`/`y` (0–1). See [`docs/NAV_MAP_BASEMAP.md`](../../docs/NAV_MAP_BASEMAP.md).

const DEFAULT_WEST := -5.55
const DEFAULT_EAST := -2.65
const DEFAULT_SOUTH := 51.35
const DEFAULT_NORTH := 53.55


static func lon_lat_to_normalized(lon: float, lat: float, bounds: Dictionary) -> Vector2:
	var w := float(bounds.get("west", DEFAULT_WEST))
	var e := float(bounds.get("east", DEFAULT_EAST))
	var s := float(bounds.get("south", DEFAULT_SOUTH))
	var n := float(bounds.get("north", DEFAULT_NORTH))
	if is_equal_approx(e, w) or is_equal_approx(n, s):
		return Vector2(0.5, 0.5)
	var x := (lon - w) / (e - w)
	# Screen Y grows downward; latitude grows northward.
	var y := (n - lat) / (n - s)
	return Vector2(clampf(x, 0.0, 1.0), clampf(y, 0.0, 1.0))


static func basemap_bounds_from_registry() -> Dictionary:
	var bm := HubRouteRegistry.basemap_config()
	var b: Variant = bm.get("bounds", {}) if not bm.is_empty() else {}
	return b if typeof(b) == TYPE_DICTIONARY else {}


## Authoring helper: **`lon`/`lat`** → normalized; otherwise returns **`x`/`y`** from the row (runtime map uses the latter).
static func normalized_from_node_row(row: Dictionary, bounds: Dictionary = {}) -> Vector2:
	var b := bounds
	if b.is_empty():
		b = basemap_bounds_from_registry()
	if row.has("lon") and row.has("lat"):
		return lon_lat_to_normalized(float(row["lon"]), float(row["lat"]), b)
	return Vector2(float(row.get("x", 0.5)), float(row.get("y", 0.5)))

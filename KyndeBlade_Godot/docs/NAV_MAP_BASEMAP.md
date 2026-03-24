# Hub navigation basemap (Wales)

## Purpose

The **hub route map** ([`hub_route_map.json`](../data/world/hub_route_map.json) + [`HubRouteMap`](../scripts/hub/hub_route_map.gd)) stays a **graph**: nodes, edges, fog-of-war. The **basemap** is a **read** layer only — silhouette and mood, not GPS routing or crawl collision.

## Wales crop and projection

The committed raster uses an **equirectangular-style** mapping against a **Wales-focused WGS84 bounding box** stored in JSON (`basemap.bounds`):

| Key | Value (approx.) | Notes |
|-----|------------------|--------|
| `west` | −5.55 | Irish Sea / St George’s Channel side |
| `east` | −2.65 | English border |
| `south` | 51.35 | Bristol Channel south |
| `north` | 53.55 | North coast / Liverpool Bay |

**Normalized hub coordinates** (`x`, `y` in `0–1`) map with **x = easting**, **y = northing inverted** (screen-down), via [`HubGeoProject`](../scripts/world/hub_geo_project.gd):

- `x = (lon − west) / (east − west)`
- `y = (north − lat) / (north − south)`

Optional per-node **`lon` / `lat`** in `hub_route_map.json` are **WGS84** hints tied to `basemap.bounds` (same equirectangular mapping as **`x` / `y`**). CI asserts they match normalized pins within tolerance; runtime travel still uses **`x` / `y`** plus the graph’s **edges** and **fog** rules.

## Fiction vs geography

Slice **titles** (Malvern, tour, Fayr Feeld, Dongeoun) remain **literary**; **`lon` / `lat`** follow the **pin positions** on the basemap (not real-world toponym survey). Players should not read the map as ordnance-grade truth.

## Art source

The checked-in **`basemap_wales.png`** is **rasterized from Natural Earth** admin-1 polygons (Wales) inside the JSON `basemap.bounds`, then toned for manuscript read. Source pack, terms, and regeneration: [`assets/third_party/natural_earth_10m_admin1/README.md`](../assets/third_party/natural_earth_10m_admin1/README.md). Inventory row: [`docs/ASSET_LICENSES.md`](../../docs/ASSET_LICENSES.md).

## Manual validation (hub scene)

After changing the basemap asset, bounds, or `hub_map.tscn` stack, run through this once in the editor (Play → main menu → **New Game** or **Continue** → hub):

- **Basemap:** Wales silhouette/terrain reads clearly behind the route panel (`HubMapStack` / `WalesBasemap`).
- **Input:** Route **buttons** and **edges** remain clickable; the `TextureRect` does not steal focus (`mouse_filter` = ignore).
- **Fog:** Hidden nodes still show mist overlay; revealed nodes match `GameState` / `hub_route_map.json` fog rules.
- **Stretch:** If the panel aspect (e.g. min size 400×280) distorts the map, adjust `stretch_mode` / `expand_mode` on `WalesBasemap` or rebalance art resolution — note it in the validation log below.

### Validation log (in-repo)

| Date | Godot | Result / notes |
|------|-------|----------------|
| *(fill when you QA)* | | |

## Related

- [`GAME_SKELETON.md`](GAME_SKELETON.md) — world data index  
- [`hub_map.tscn`](../scenes/hub_map.tscn) — `WalesBasemap` `TextureRect` behind `%HubRouteMap`

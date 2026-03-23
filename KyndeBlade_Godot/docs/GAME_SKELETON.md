# Game skeleton (Unity plan → Godot scaffold)

This is the **authoring spine** for the full Kynde Blade campaign: same **location ids**, **graph edges**, and **Unity file paths** as `KyndeBlade_Unity/Assets/Resources/Data/`, so you can write text, drop encounters, and wire scenes without reverse‑engineering the C# project.

## Quick map

| Layer | Godot | Unity analogue |
|--------|--------|----------------|
| Canonical ids | `scripts/world/world_constants.gd` (`GameWorldIds`) | `GameWorldConstants.cs` |
| All locations + graph + writer notes | `data/world/locations_registry.json` | `LocationNode` assets (`Loc_*.asset`) |
| Act / vision ordering (for writers) | `data/world/campaign_spine.json` | `WorldMapManager` + folder layout |
| Beat placeholders | `data/world/narrative_beats_skeleton.json` | `StoryBeat` assets on `LocationNode` |
| Encounter checklist | `data/encounters/encounter_index.json` | `EncounterConfig` referenced by locations |
| Preview UI | `scenes/world/world_atlas.tscn` → `location_shell.tscn` | N/A (authoring aid) |
| Navigation autoload | `WorldNav` | `WorldMapManager.TransitionTo` (future parity) |

## Player-facing flow today

- **Hub** `hub_map.tscn` — slice: Tower / Fair Field → counsel → combat (unchanged).
- **World atlas** button on hub — opens **every** registry location for reading / preview.
- **Location shell** — shows description, Unity asset path, **next** edges; only **tour** ↔ **fayre_felde** travel buttons are live (returns to hub). Other edges open another shell preview (skeleton).

## How to add a level

1. **Confirm or add** a row in `locations_registry.json` (copy an existing block). Match **Unity** `LocationId` and `NextLocationIds`.
2. **Encounter** (if any): create `data/encounter_<slug>.tres` (`EncounterDef`), point `encounter.godot_resource` at it, and add a line to `data/encounters/encounter_index.json`.
3. **Text**: add beats to `narrative_beats_skeleton.json` and/or `medieval_text_unlocks.json`; keep beat ids consistent with Unity export when you regenerate `exported_from_unity.json`.
4. **Scene** (optional): either keep using `location_shell.tscn` or set a dedicated scene path later (add `godot_scene` field to registry when you introduce it).
5. **Wire travel**: extend `LocationShell._is_skeleton_travel_allowed`, `hub_map.gd`, or a future `WorldMap` scene to enable the new edge.

## Unity source of truth (until M6)

- Locations: `KyndeBlade_Unity/Assets/Resources/Data/Vision1|Vision2|GreenChapel|OrfeoOtherworld/Loc_*.asset`
- Constants: `KyndeBlade_Unity/Assets/KyndeBlade/Code/Core/Game/GameWorldConstants.cs`
- Map logic: `KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/Map/WorldMapManager.cs`

If Unity data changes, update **`locations_registry.json`** and **`campaign_spine.json`** in the same PR (or automate export later).

## Related docs

- Planning index: [`docs/KYNDEBLADE_CAREFUL_CANON.md`](../../docs/KYNDEBLADE_CAREFUL_CANON.md)
- Module map: [`docs/UNITY_GODOT_MODULE_MAP.md`](../../docs/UNITY_GODOT_MODULE_MAP.md)
- Story digest: [`docs/UNITY_STORY_AND_SPAWN_DIGEST.md`](../../docs/UNITY_STORY_AND_SPAWN_DIGEST.md)
- Slice export: `data/exported_from_unity.json` + `scripts/unity_export_data.gd`

# Unity → Godot module map (starter)

High-level map of [`KyndeBlade_Unity/Assets/KyndeBlade/Code/`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/) assemblies to Godot folders under [`KyndeBlade_Godot/`](../KyndeBlade_Godot/). Extend as waves advance; asmdefs: `KyndeBlade.Core`, `KyndeBlade.Combat`, `KyndeBlade.Editor` (export tooling only), `EditModeTests`, `PlayModeTests` (see `.asmdef` files under `Code/` and `Assets/Tests/`).

| Unity area | Typical paths | Godot target (suggested) |
|------------|---------------|---------------------------|
| Core game / save | `Code/Core/Game/`, `Code/Core/Save/` | `scripts/save_service.gd`, `scripts/game_state.gd` |
| Input | `Code/Core/Input/` | `scripts/input_map_setup.gd`, `project.godot` input map |
| Narrative | `Code/Core/Narrative/`, `StoryBeat` assets | `scripts/story_arrival_screen.gd`, `exported_from_unity.json` `arrival_beat_*` per location |
| Map / locations | `Code/Core/Map/`, `Code/Combat/Map/`, `WorldMapManager`, `LocationNode` | `scripts/hub_map.gd`, `scripts/unity_export_data.gd`, `data/slice_locations.json`, `data/exported_from_unity.json`, **`data/world/locations_registry.json`**, **`scripts/world/`**, **`WorldNav` autoload** — see [`KyndeBlade_Godot/docs/GAME_SKELETON.md`](../KyndeBlade_Godot/docs/GAME_SKELETON.md) |
| Combat loop / UI | `Code/Combat/*.cs`, `Code/Combat/UI/` | `scripts/combat_manager.gd`, `scripts/combat_root.gd`, `scenes/combat.tscn` |
| Characters / enemies | `Code/Combat/Characters/` | future `data/` + combat scripts |
| Visual / shaders | `Code/Core/Visual/`, `Shaders/` | Godot materials / CanvasItem; track in [`PARITY_GAPS.md`](../KyndeBlade_Godot/PARITY_GAPS.md) |
| Editor export tooling | `Assets/Editor/ExportGodotSliceData.cs` | Unity menu **KyndeBlade → Export Slice Data for Godot** → `KyndeBlade_Godot/data/exported_from_unity.json` |
| Audio buses (shell) | Unity mixers (future parity) | `scripts/audio_bus_setup.gd` autoload (`Music`, `SFX`) |

**Data:** Unity `Resources` / ScriptableObjects → JSON or `.tres` + loaders (`data/encounter_def.gd`, `class_name UnityExportData`).

| Unity data (examples) | Godot consumer |
|-----------------------|----------------|
| `Assets/Resources/Data/**/Loc_*.asset` (all visions) | `data/world/locations_registry.json` + `LocationRegistry`; slice still uses `exported_from_unity.json` + `UnityExportData` |
| `FayreFeldeEncounter.asset` | Same export + `encounter_fair_field.tres` (hand-tuned combat numbers) |

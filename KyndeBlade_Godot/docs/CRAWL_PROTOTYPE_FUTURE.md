# Future — overworld crawl prototype (fake voxels)

**Status:** **C0 stub** — [`scenes/crawl_overworld.tscn`](../scenes/crawl_overworld.tscn) + headless `crawl_overworld_scene_smoke`; hub + Fair Field remain the playable overworld until C1+. **Note:** [`combat_voxel_arena.gd`](../scripts/combat/combat_voxel_arena.gd) is **combat** only.

## Goals

1. **Structured overworld** — routes, dungeon floors, or a **node map** (Pokémon-like clarity of path).
2. **Look** — **fake voxels:** `TileMap`, layered `Sprite2D`, oblique “block” shading; **no** 3D requirement on the crawl layer.
3. **Encounters** — triggers from steps, tiles, or interactables; **no combat resolution** on crawl — only `change_scene_to_file("res://scenes/combat.tscn")` (or packed equivalent).
4. **Return** — after combat outcome, return to crawl with **loot / flags** via existing `GameState` + `SaveService` patterns.
5. **Mood** — optional 2D particles / shaders in the **Noita-influenced** lane (inspiration only); see [`VISION_CRAWL_NOITA_E33.md`](VISION_CRAWL_NOITA_E33.md).

## Headless / CI

- **Combat logic** remains **raster-agnostic** (`CombatManager`, save tests).
- **Transition smoke:** headless suite mounts `hub_map.tscn` and `combat.tscn` for one frame each to prove **scene paths and instantiation** (`tests/run_headless_tests.gd` → `_test_scene_transition_smoke`). This does **not** play a full crawl loop; when a dedicated crawl scene exists, extend the test to load that scene and assert the same combat path constant.

## Related

- **Unified design (crawl + voxels + shaders + GPU CI + M6):** [`DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md`](DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md)  
- [`GODOT_TARGET_EXPERIENCE.md`](GODOT_TARGET_EXPERIENCE.md)  
- [`COMBAT_VOXEL_STAGE_FUTURE.md`](COMBAT_VOXEL_STAGE_FUTURE.md)  
- [`docs/CI_GODOT_TESTS.md`](../../docs/CI_GODOT_TESTS.md)

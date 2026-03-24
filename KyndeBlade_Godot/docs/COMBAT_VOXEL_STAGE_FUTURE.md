# Future — full-screen 3D voxel combat stage

**Status:** **partial** — combat ships a **procedural voxel grid** (floor + perimeter columns) built at runtime by [`combat_voxel_arena.gd`](../scripts/combat_voxel_arena.gd) under `CombatArena3D` in [`combat.tscn`](../scenes/combat.tscn). The classic single `Ground` mesh is hidden when the voxel arena is enabled. **Imported** MagicaVoxel / GLTF sets, rim/outline polish, and ortho tuning remain future work.

## Shipped prototype (this slice)

| Piece | Role |
|--------|------|
| **`VoxelArena`** | `Node3D` + `MultiMeshInstance3D` floor + wall ring; palette-aligned albedo, low spec |
| **Actors** | Existing `PlayerActor` / `EnemyActor` capsules + box; unchanged |
| **Camera** | Existing `CombatCamera` in [`combat_presentation_3d.gd`](../scripts/combat_presentation_3d.gd) |

Toggle or resize via `VoxelArena` **Inspector** exports (`enabled`, `grid_half`, `voxel_size`, `wall_height_blocks`, `hide_classic_ground`).

## Scene architecture (target)

| Piece | Role |
|--------|------|
| **World root** | `Node3D` — fills the **viewport**; voxel environment + actors as `MeshInstance3D` (or imported GLTF from MagicaVoxel, etc.). |
| **Camera** | `Camera3D` — ortho or mild perspective; **rim / outline** so reads survive particle noise; scale tuned for **feint vs swing** timing (E33-style windows stay authoritative). |
| **HUD** | `CanvasLayer`(s) — manuscript bars, labels, **ParryDodgeEye**, pause / outcome UI. **Not** a full-screen `SubViewport` texture on a quad as the primary HUD (avoid the “tiny 3D inset” pattern). |
| **`CombatManager`** | Stays **logic-only** — autoload or sibling node as today; **no** embedding inside a `SubViewport` for rules. Presentation reads signals / polls typed state as now. |

## Headless / CI / GPU

- **Required CI:** keep **logic** coverage headless (`CombatManager`, save, data) — no GPU dependency.
- **Optional CI:** visual smoke (screenshot compare, 3D boot) may need **GPU** runners (e.g. software GL, or labeled `godot-3d-smoke` job). Document any new job in [`docs/CI_GODOT_TESTS.md`](../../docs/CI_GODOT_TESTS.md) and [`PARITY_GAPS.md`](../PARITY_GAPS.md) if Unity vs Godot visual parity is deferred.

## Related

- [`VISION_CRAWL_NOITA_E33.md`](VISION_CRAWL_NOITA_E33.md)  
- [`TDAD_COMBAT_PRESENTATION_PLAN.md`](TDAD_COMBAT_PRESENTATION_PLAN.md) — current 2D presentation segments  
- [`docs/GODOT_PLANS_INDEX.md`](GODOT_PLANS_INDEX.md)  
- [`docs/CI_GODOT_TESTS.md`](../../docs/CI_GODOT_TESTS.md)

# Vision — crawl, combat, Noita mood, E33 rules (Godot)

Short **at-a-glance** target for the Godot slice. Detail and file pointers live in [`GODOT_TARGET_EXPERIENCE.md`](GODOT_TARGET_EXPERIENCE.md).

## Loop

| Layer | Target |
|--------|--------|
| **Crawl / overworld** | Pokémon-like routes, dungeon floors, or a node map. **Encounters** fire from movement or tiles. **No combat math** on the crawl layer — resolution only in the battle scene. **Art:** **fake voxels** (2D tiles, stacked sprites, one oblique light read). |
| **Pop-out** | Full scene swap into [`combat.tscn`](../scenes/combat.tscn) (today). Later: iris / ink / manuscript transition framing. |
| **Combat** | **Expedition 33–style:** turn flow + **real-time defensive windows** (`CombatManager`, feint pattern, eye / React!). **Art target:** **full-screen 3D** — `Node3D` world, `Camera3D`, real voxel meshes; **manuscript HUD** and `ParryDodgeEye` on **`CanvasLayer`** above the world (not a `SubViewport` quad). *Current slice:* 2D stage until [`COMBAT_VOXEL_STAGE_FUTURE.md`](COMBAT_VOXEL_STAGE_FUTURE.md) lands. |
| **Mood** | **Noita-influenced** lane: particles, shaders, material-like noise, readable silhouettes. **No cloned assets** — inspiration only; cite in [`docs/ASSET_LICENSES.md`](../../docs/ASSET_LICENSES.md) if player-facing. |

## Parity / oracle

Until **M6 archive** policy changes, the **Unity Tier A slice** (`PLAYABLE_SLICE`, `DEMO_RUN`) remains the **behaviour oracle** for the demo vertical slice. Godot may **lead** on documented Godot-first features; see [`PARITY_GAPS.md`](../PARITY_GAPS.md) and [`AUTHORIAL_OVERRIDE.md`](AUTHORIAL_OVERRIDE.md).

## Future phases (tracked in-repo)

- **Overworld crawl prototype:** [`CRAWL_PROTOTYPE_FUTURE.md`](CRAWL_PROTOTYPE_FUTURE.md)
- **Full-screen 3D combat stage:** [`COMBAT_VOXEL_STAGE_FUTURE.md`](COMBAT_VOXEL_STAGE_FUTURE.md)
- **Manuscript → pixel pipeline:** [`assets/ui_manuscript/README.md`](../assets/ui_manuscript/README.md) + [`assets/reference_manuscript/README.md`](../assets/reference_manuscript/README.md)

## Related

- [`STEAM_BUILD.md`](../STEAM_BUILD.md) — shipping slice and QA  
- [`docs/KYNDEBLADE_CAREFUL_CANON.md`](../../docs/KYNDEBLADE_CAREFUL_CANON.md) — repo planning index

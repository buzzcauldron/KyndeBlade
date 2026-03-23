# Godot target experience (north star)

This doc captures the **intended** Godot-facing game shape beyond the current vertical slice. The **shipping Godot slice** (Steam / desktop build) and tests remain authoritative for what is implemented today — see [`STEAM_BUILD.md`](../STEAM_BUILD.md), [`PARITY_GAPS.md`](../PARITY_GAPS.md), and Unity [`PLAYABLE_SLICE.md`](../../KyndeBlade_Unity/Assets/KyndeBlade/Docs/PLAYABLE_SLICE.md) / [`DEMO_RUN.md`](../../KyndeBlade_Unity/Assets/KyndeBlade/Docs/DEMO_RUN.md) as oracle until M6 archive policy changes.

## Loop

| Layer | Intent |
|-------|--------|
| **Crawl** | Pokémon-like **routes / dungeon / node map**; encounters from steps or tiles; **no combat resolution** on the crawl scene. **Parallax:** shared [`CrawlParallax`](../scripts/crawl_parallax.gd) drives hub [`hub_crawl_parallax.gd`](../scripts/hub_crawl_parallax.gd) and combat [`combat_manuscript_backdrop.gd`](../scripts/combat_manuscript_backdrop.gd) layer drift. |
| **Pop-out** | Transition into a **dedicated combat scene** (today: `combat.tscn`) with clear framing (iris, ink, manuscript — future). |
| **Combat rules** | **Expedition 33–inspired**: turn-based actions + **real-time defensive windows** (`CombatManager`, feint pattern, eye/React!). |
| **Art — map** | **Fake voxels**: 2D tiles / stacked sprites / oblique “block” read; cheap iteration. |
| **Art — combat** | **Full-screen 3D** with **real voxel** meshes (`Camera3D`, world fills viewport); **2D manuscript HUD** on `CanvasLayer`. *Future migration from current 2D Lane B stage.* |
| **Mood** | **Noita-influenced** particles/shaders/material noise — inspiration only, no cloned assets. |

## Manuscript reference → pixels

Historical **illumination** plates may be **downscaled** (nearest-neighbor + palette toward `KyndeBladeArtPalette`) for UI ornaments and parchment panels. **Record provenance** in [`docs/ASSET_LICENSES.md`](../../docs/ASSET_LICENSES.md). **Modern book covers** are **not** for redistribution — reference only.

Staging paths outside the repo (e.g. editor asset drops) should be copied into [`assets/reference_manuscript/`](../assets/reference_manuscript/README.md) once cleared.

## Parity

Godot may **lead** Unity on **counsel → meta flags → feint offset** and **Godot-only** presentation until the Unity slice matches. Track in [`PARITY_GAPS.md`](../PARITY_GAPS.md) and [`AUTHORIAL_OVERRIDE.md`](AUTHORIAL_OVERRIDE.md).

## Related

- [`CULT_SHIP_BAR.md`](CULT_SHIP_BAR.md) — shippable voice and checklist.  
- [`docs/KYNDEBLADE_CAREFUL_CANON.md`](../../docs/KYNDEBLADE_CAREFUL_CANON.md) — repo-wide planning index.

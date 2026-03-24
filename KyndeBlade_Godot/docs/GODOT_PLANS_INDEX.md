# Godot plans index

Single map of **planning docs** vs **implementation status**. “Done” here means a baseline exists in-tree; polish and parity rows may still be open in [`PARITY_GAPS.md`](../PARITY_GAPS.md).

| Plan | Scope | Status |
|------|--------|--------|
| [`DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md`](DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md) | Crawl, voxels, rim, GPU CI, M6 — unified design + **§8 status** | **Partial** — C0 crawl, rim mats, `assets/voxel/`, GitHub Actions |
| [`TDAD_COMBAT_PRESENTATION_PLAN.md`](TDAD_COMBAT_PRESENTATION_PLAN.md) | CP-01–CP-06 Lane B + HUD + audio hooks | **Baseline done**; manual QA ongoing |
| [`COMBAT_VOXEL_STAGE_FUTURE.md`](COMBAT_VOXEL_STAGE_FUTURE.md) | Full-screen 3D voxel arena + HUD overlay | **Partial** — [`combat_voxel_arena.gd`](../scripts/combat_voxel_arena.gd) procedural grid; imported voxel art TBD |
| [`COMBAT_REVIEW_WIREFRAME_E33.md`](COMBAT_REVIEW_WIREFRAME_E33.md) | E33-style read ledger | **Living** — tracks partial vs met |
| [`CRAWL_PROTOTYPE_FUTURE.md`](CRAWL_PROTOTYPE_FUTURE.md) | Fake-voxel hub crawl → combat pop-out | **Not started** — hub + Fair Field stand in |
| [`VISION_CRAWL_NOITA_E33.md`](VISION_CRAWL_NOITA_E33.md) | Vision capsule | **Design** |
| [`GODOT_TARGET_EXPERIENCE.md`](GODOT_TARGET_EXPERIENCE.md) | Target feel | **Design** |
| [`PORT_WAVE.md`](../PORT_WAVE.md) | W* port waves | **Tracking** |
| [`STEAM_BUILD.md`](../STEAM_BUILD.md) | Manual QA / ship bar | **Active** |
| [`M6_CUTOVER_CHECKLIST.md`](../M6_CUTOVER_CHECKLIST.md) | Godot canonical / Unity archive gate | **Checklist** (human) |
| **TDAD** `.tdad/workflows/godot-*` | Subsystem mirrors + parity slice | **Graph + proofs** — see [`.tdad/README.md`](../../.tdad/README.md) |

**Unity archive policy:** [`ProjectArchive/UnityKyndeBlade/UNITY_REFERENCE_ARCHIVE.md`](../../ProjectArchive/UnityKyndeBlade/UNITY_REFERENCE_ARCHIVE.md).

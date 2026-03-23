# Plan a Godot port milestone (TDAD: godot-full-port)

You are expanding **one milestone node** from [`.tdad/workflows/godot-full-port/godot-full-port.workflow.json`](../workflows/godot-full-port/godot-full-port.workflow.json) into an executable task list.

**TDAD layout:** [`.tdad/README.md`](../README.md) — root graph, tiers, BDD. **Godot shipping slice** (paths, autosave, CI, export): [`godot-steam-build`](../workflows/godot-steam-build/godot-steam-build.workflow.json) + [`STEAM_BUILD.md`](../../KyndeBlade_Godot/STEAM_BUILD.md).

## Rules

1. **Oracle**: Until M6 archive, read Unity behavior from [`.tdad/workflows/demo-vertical-slice/`](../workflows/demo-vertical-slice/demo-vertical-slice.workflow.json), [`PLAYABLE_SLICE.md`](../../KyndeBlade_Unity/Assets/KyndeBlade/Docs/PLAYABLE_SLICE.md), and relevant C# under `KyndeBlade_Unity/`. Do not delete or move Unity in this prompt unless the user explicitly requested **M6**.
2. **Godot code** lives under [`KyndeBlade_Godot/`](../../KyndeBlade_Godot/).
3. **Output** for the chosen milestone:
   - **Tasks** (ordered, small)
   - **Unity file references** (paths to read)
   - **Godot files to create/change**
   - **Parity tests**: automated (`tests/run_headless_tests.gd`, future GUT) + **manual** rows from [`STEAM_BUILD.md`](../../KyndeBlade_Godot/STEAM_BUILD.md)
   - **Risks / cuts** if scope is too large

## Milestone map (summary)

| Id | Focus |
|----|--------|
| godot-m1-foundation | Project/scenes/autoloads |
| godot-m2-combat-parity | Encounter resource, deterministic combat |
| godot-m3-save-ui-parity | Save/menu/pause/settings vs Unity |
| godot-m4-map-narrative | Locations + beats + victory→hub |
| godot-m5-systems-depth | Kynde, break, hazards, AI slices |
| godot-m6-cutover-archive | Sign-off + run [`godot-archive-unity-snapshot.md`](./godot-archive-unity-snapshot.md) |

## Input

{{#if milestoneId}}
**Milestone node id:** `{{milestoneId}}`
{{/if}}

{{#if extraContext}}
### Extra context
{{extraContext}}
{{/if}}

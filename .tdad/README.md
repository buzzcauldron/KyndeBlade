# TDAD — Test-Driven Agile Development (repo index)

This folder holds **workflows** (feature graphs), **BDD** (Gherkin), and **prompts** for Kynde Blade. Use this file as the **single map** before editing slice or shipping plans.

## Tiers (release paths)

| Tier | Role | Entry artifact |
|------|------|----------------|
| **A** | Playable vertical slice (Unity oracle path) | [`workflows/demo-vertical-slice/demo-vertical-slice.workflow.json`](workflows/demo-vertical-slice/demo-vertical-slice.workflow.json) + [`bdd/demo-vertical-slice.feature`](bdd/demo-vertical-slice.feature) |
| **B** | Steam / EA **planning** (milestones, store, depots) | [`workflows/steam-early-access/steam-early-access.workflow.json`](workflows/steam-early-access/steam-early-access.workflow.json) + [`prompts/plan-steam-milestones.md`](prompts/plan-steam-milestones.md) |
| **Godot — parity** | Same **behaviours** as Tier A, Godot proofs | [`workflows/godot-parity-slice/godot-parity-slice.workflow.json`](workflows/godot-parity-slice/godot-parity-slice.workflow.json) + [`bdd/godot-parity-slice.feature`](bdd/godot-parity-slice.feature) |
| **Godot — demo components** | Granular slice subsystems → headless / BDD / manual | [`workflows/godot-demo-components/godot-demo-components.workflow.json`](workflows/godot-demo-components/godot-demo-components.workflow.json) + traceability in [`docs/CI_GODOT_TESTS.md`](../docs/CI_GODOT_TESTS.md) |
| **Godot — Steam build** | **Shipping** filenames, autosave, migration, CI, export | [`workflows/godot-steam-build/godot-steam-build.workflow.json`](workflows/godot-steam-build/godot-steam-build.workflow.json) + [`KyndeBlade_Godot/STEAM_BUILD.md`](../KyndeBlade_Godot/STEAM_BUILD.md) |
| **Godot — full port** | M1–M6 migration + archive gate | [`workflows/godot-full-port/godot-full-port.workflow.json`](workflows/godot-full-port/godot-full-port.workflow.json) |

Full narrative for demos vs Steam vs Godot: **[`docs/TDAD_RELEASE_PATHS.md`](../docs/TDAD_RELEASE_PATHS.md)**.

## Root dependency graph

**[`workflows/root.workflow.json`](workflows/root.workflow.json)** — all subsystem folders (combat-*, save-system, …) plus:

- `demo-vertical-slice` → depends on ui-shell, map-progression, narrative, save-system, scenes, combat-core  
- `steam-early-access` → depends on `demo-vertical-slice`  
- `godot-full-port` → depends on `demo-vertical-slice`  
- `godot-parity-slice` → depends on `demo-vertical-slice`, `godot-full-port`  
- **`godot-demo-components`** → depends on **`godot-parity-slice`** (granular test hooks; optional planning graph)  
- **`godot-steam-build`** → depends on **`godot-parity-slice`** (shipping layer on top of parity proofs)

## Subsystem workflows (`workflows/<id>/`)

| Folder | Topic |
|--------|--------|
| `combat-core`, `combat-actions`, `combat-damage`, `combat-defense`, `combat-break`, `combat-status`, `combat-hazards`, `combat-kynde` | Combat stack |
| `characters`, `enemies`, `game-state`, `map-progression`, `narrative` | World + story |
| **`save-system`** | Unity save/load (+ **Godot SaveService** trace node) |
| `meta-progression`, `input`, `audio`, `ui-shell`, `scenes`, `perf`, `a11y-l10n` | Shell + quality |

## BDD

| File | Workflow |
|------|----------|
| [`bdd/demo-vertical-slice.feature`](bdd/demo-vertical-slice.feature) | `demo-vertical-slice` |
| [`bdd/godot-parity-slice.feature`](bdd/godot-parity-slice.feature) | `godot-parity-slice` (tags `@gdc-*` → `godot-demo-components` nodes) |

## Prompts (`prompts/`)

| Prompt | Use |
|--------|-----|
| [`generate-bdd.md`](prompts/generate-bdd.md), [`generate-tests.md`](prompts/generate-tests.md) | New Tier A / parity scenarios |
| [`plan-godot-port-milestone.md`](prompts/plan-godot-port-milestone.md) | Expand `godot-full-port` milestones |
| [`plan-steam-milestones.md`](prompts/plan-steam-milestones.md) | Expand `steam-early-access` |
| [`godot-archive-unity-snapshot.md`](prompts/godot-archive-unity-snapshot.md) | M6 human gate |

## Conventions

- **workflowId** on each node matches the parent JSON’s logical slice (`save-system`, `godot-steam-build`, …).  
- **Proof** lines in descriptions should name a test file, BDD scenario, or doc section (e.g. `STEAM_BUILD.md`).  
- **Oracle** until M6 archive: Unity `PLAYABLE_SLICE.md` / `DEMO_RUN.md` + `demo-vertical-slice`; Godot parity tests prove equivalence for the Godot build.

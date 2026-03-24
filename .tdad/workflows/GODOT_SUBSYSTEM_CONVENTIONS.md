# Godot subsystem TDAD mirrors — conventions

Thin **parallel** workflows under `.tdad/workflows/godot-<name>/` trace the same subsystem boundaries as Unity TDAD folders (`combat-core`, `save-system`, …) but cite **Godot** proofs and files. Release-slice truth stays in `godot-parity-slice`, `godot-demo-components`, and `godot-steam-build`.

## Node rules

- **`workflowId`** on every feature node = the folder name (e.g. `godot-save-system`), matching the JSON filename stem.
- **`testLayers`:** prefer `godot_headless`, `bdd`, `godot_manual` (subsets OK). Use `api` only when documenting a shared contract with automated API tests.
- **Descriptions** should name a proof: `KyndeBlade_Godot/tests/run_headless_tests.gd` / `combat_scenarios.gd` function, `.tdad/bdd/godot-parity-slice.feature` (`@gdc-*`), `KyndeBlade_Godot/STEAM_BUILD.md`, or a doc path.
- **Stubs:** use explicit **TBD** / **PARITY_GAPS** wording where Unity has systems Godot does not ship yet.

## Root graph

Each mirror folder appears in `root.workflow.json` as `nodeType: "folder"` with:

- `folderPath`: `godot-<subsystem>` (matches directory name).
- `dependencies`: include `godot-parity-slice` unless a narrower dependency is documented.
- `children`: feature **ids** that exist in `godot-<subsystem>.workflow.json` (same contract as Unity `combat-core` → `combat-core.workflow.json`).

## Cross-links

- Granular test nodes: `godot-demo-components.workflow.json` (`gdc-*`).
- Combat presentation / E33 wireframe ledger: `KyndeBlade_Godot/docs/COMBAT_REVIEW_WIREFRAME_E33.md`.
- CI: `docs/CI_GODOT_TESTS.md`.

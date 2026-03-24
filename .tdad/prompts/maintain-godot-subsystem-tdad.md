# Prompt — maintain Godot subsystem TDAD mirrors

When you add or change Godot behavior that maps to a Unity TDAD subsystem:

1. Identify the mirror folder (`godot-combat-defense`, `godot-save-system`, …).
2. Add or update a **feature** node in `.tdad/workflows/<folder>/<folder>.workflow.json` with `workflowId` = folder name and proof lines in `description`.
3. If the node id is new, append it to the matching `children` array on that folder in `root.workflow.json`.
4. Prefer linking existing `gdc-*` / `gparity-*` proofs in `godot-demo-components` / `godot-parity-slice` instead of duplicating scenarios.
5. Read `.tdad/workflows/GODOT_SUBSYSTEM_CONVENTIONS.md` for `testLayers` and stub policy.

Combat presentation changes: also skim `KyndeBlade_Godot/docs/COMBAT_REVIEW_WIREFRAME_E33.md` and `PARITY_GAPS.md` combat row.

# Encounters (Godot)

Each **combat encounter** should become a `EncounterDef` Resource (`.tres`) + optional flavor in `data/world/locations_registry.json`.

## Index

See [`encounter_index.json`](encounter_index.json) for Unity `EncounterConfig` names → target `res://data/*.tres` paths.

## Authoring steps

1. Duplicate `encounter_fair_field.tres` → rename (`encounter_green_chapel.tres`, etc.).
2. Point `locations_registry.json` → `encounter.godot_resource` at the new `.tres`.
3. Wire hub / `LocationShell` / custom scene to load combat with that resource on `CombatManager.encounter`.

Unity source assets: `ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/Resources/Data/**/*.asset` (encounter objects referenced by `LocationNode`).

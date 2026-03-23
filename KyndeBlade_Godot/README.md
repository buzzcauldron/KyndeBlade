# Kynde Blade — Godot 4 (Steam / retail build)

This folder is the **Godot 4 shipping target** for the current vertical slice: same player-facing bar as the Unity Tier A slice, packaged for **Steam and desktop** (saves, autosave, settings, headless CI).

**Unity** authoring and oracle docs live under `KyndeBlade_Unity/` until M6 archive policy changes.

**Primary doc:** **[STEAM_BUILD.md](STEAM_BUILD.md)** — scope, `user://` save paths (including migration from legacy `kyndeblade_demo_*` files), QA checklist, export / depot notes, Steam Cloud caveats.

**Full campaign skeleton (levels + text scaffolding):** **[docs/GAME_SKELETON.md](docs/GAME_SKELETON.md)** — mirrors Unity `LocationNode` / `GameWorldConstants`; hub → **World atlas**.

**Redirect:** [GODOT_DEMO.md](GODOT_DEMO.md) → points here for older links.

## Quick start

1. Open this folder in **Godot 4.2+** (project pins 4.6 features in `project.godot`; adjust if your editor warns).
2. **Run** the main scene (`scenes/main_menu.tscn`).
3. **New Game** → tower intro → hub → Fair Field → combat.

## Tests (headless)

From repo root:

```bash
godot4 --path KyndeBlade_Godot --headless --script res://tests/run_headless_tests.gd
```

See [docs/CI_GODOT_TESTS.md](../docs/CI_GODOT_TESTS.md).

**Godot ecosystem (showcase):** [RPG in a Box — Justin Arnold on Godot](https://godotengine.org/article/godot-showcase-justin-arnold-rpg-in-a-box/) (voxel/grid RPG tooling + editor UI in Godot).

## Controls

Defaults are applied by autoload `InputMapSetup` if actions are empty: strike (**X**), dodge (**Space**), parry (**Shift**), pause (**Escape**). Gamepad: **A / B / X** and **Start** when bound.

Full table: [STEAM_BUILD.md](STEAM_BUILD.md).

## Structure (high level)

| Path | Role |
|------|------|
| `scenes/` | Main menu, hub, combat, tower intro, extras |
| `scripts/` | Autoloads, combat, save, UI theme, ink presentation |
| `data/` | Encounters, slice JSON, Unity export, medieval text catalog, **`data/world/`** full location registry + campaign spine |
| `scenes/world/` | `world_atlas.tscn`, `location_shell.tscn` — preview entire Unity-planned map |
| `tests/` | Headless smoke + combat scenarios |
| `docs/GODOT_AUDIO_DESIGN.md` | Procedural SFX policy, parry-band vs cue length, CC0 guidance |
| `assets/reference_preraphaelite/` | Optional cleared Salome / Lee mood-board PNGs ([README](assets/reference_preraphaelite/README.md)) |

## License

Game code and assets in this tree follow the **repository root** license unless a subdirectory specifies otherwise (see `docs/ASSET_LICENSES.md` for third-party bits).

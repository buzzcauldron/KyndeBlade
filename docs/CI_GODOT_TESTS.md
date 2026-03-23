# Godot — CI and headless tests

Run automated checks for [`KyndeBlade_Godot/`](../KyndeBlade_Godot/). **TDAD:** [`.tdad/README.md`](../.tdad/README.md); headless save/autosave/migration proofs trace to **`godot-steam-build`** + **`godot-parity-slice`** (see [`godot-steam-build.workflow.json`](../.tdad/workflows/godot-steam-build/godot-steam-build.workflow.json)).

## Prerequisites

- **Godot 4.6.1** (or **4.6.x**) binary on `PATH` (e.g. `godot4`, or rename the editor binary).
- Project path: repo root + `KyndeBlade_Godot`.

### PATH on macOS (this repo + your shell)

- **Cursor / VS Code:** [`.vscode/settings.json`](../.vscode/settings.json) prepends `Godot.app` / `Godot 4.app` `Contents/MacOS` to `PATH` in the **integrated terminal** (new terminal tab after saving).
- **System zsh:** if you use the snippet in `~/.zshrc` (Godot paths + optional `alias godot4=Godot`), open a **new** terminal or run `source ~/.zshrc`. Install Godot from [godotengine.org](https://godotengine.org/download) or `brew install --cask godot` (adjust paths in `.zshrc` if the cask uses a different app name).

## Headless smoke (no addon)

The project ships [`tests/run_headless_tests.gd`](../KyndeBlade_Godot/tests/run_headless_tests.gd). It runs save/settings/export checks, scene smokes (**main menu Continue**, **hub counsel gate**, **combat pause vs window tick**, **world atlas**, **location shell**), **`_test_scene_transition_smoke`** (loads `hub_map.tscn` and `combat.tscn`, mounts each for one frame — future crawl pop-out path), then **[`tests/combat_scenarios.gd`](../KyndeBlade_Godot/tests/combat_scenarios.gd)** (strike loop, dodge/parry windows, feint chip, stamina gate, defeat) via `CombatManager.use_instant_resolution_for_tests`, then exits.

## TDAD `godot-demo-components` traceability

Each row maps a node in [`.tdad/workflows/godot-demo-components/godot-demo-components.workflow.json`](../.tdad/workflows/godot-demo-components/godot-demo-components.workflow.json) to BDD ([`.tdad/bdd/godot-parity-slice.feature`](../.tdad/bdd/godot-parity-slice.feature)), headless proof in `run_headless_tests.gd` / `combat_scenarios.gd`, and manual QA in [`STEAM_BUILD.md`](../KyndeBlade_Godot/STEAM_BUILD.md) (checklist numbering).

| TDAD node | BDD scenario (title) | Headless / automated proof | Manual (`STEAM_BUILD.md`) |
|-----------|----------------------|----------------------------|---------------------------|
| `gdc-main-menu` | Continue is available when save exists | `_test_main_menu_continue_gated_smoke` | § Manual QA #1 (Continue disabled cold run) |
| `gdc-tower-intro` | Hub shows tour after New Game; Unity export JSON is present for pipeline parity | `_test_unity_export_json` (arrival beat fields) | § Player journey; Manual #2 |
| `gdc-hub-slice-locations` | Hub offers travel to Fair Field; Unity export JSON… | `_test_slice_locations_json` | § Parity table (tour → Fair Field) |
| `gdc-hub-counsel-gate` | Fair Field travel requires counsel before combat | `_test_hub_counsel_gate_smoke` | § Player journey; Manual #2 |
| `gdc-world-atlas` | — | `_test_world_atlas_scene_smoke` | § World skeleton (World atlas) |
| `gdc-location-shell` | — | `_test_location_shell_scene_smoke` | — (authoring shell) |
| `gdc-combat-encounter-load` | Fair Field combat uses False encounter data; Unity export JSON… | `_test_encounter_resource` | § Parity table |
| `gdc-combat-scenarios` | Headless combat scenario suite passes | `combat_scenarios.gd` via runner | — |
| `gdc-combat-misstep-feint` | Ethical misstep inverts first defensive read in combat | `misstep_inverts_first_defensive_window` in `combat_scenarios.gd` | — |
| `gdc-combat-pause` | Pause exists in combat | `_test_combat_pause_freezes_window_tick_smoke` | § Manual QA #3 |
| `gdc-combat-victory-save` | Victory returns to hub with updated progress | `_test_victory_fair_field_updates_gamestate` | § Manual QA #4 |
| `gdc-settings-fullscreen` | — | `_test_settings_fullscreen_roundtrip` | § Manual QA #5 (fullscreen) |
| `gdc-medieval-catalog` | — | `_test_medieval_catalog_aggregate`, `_test_read_medieval_text_ids_roundtrip`, `_test_medieval_list_granted_codes` | — |
| `gdc-piers-symbols` | — | `_test_piers_symbol_catalog_fayre_felde`, `_test_piers_state_save_roundtrip` | — |
| `gdc-placeholder-art-registry` | — | `_test_placeholder_art_registry` | — |
| `gdc-scene-transition` | — | `_test_scene_transition_smoke` | — |

**Future full-screen 3D combat** may need optional GPU-backed visual smoke; logic tests stay headless — see [`KyndeBlade_Godot/docs/COMBAT_VOXEL_STAGE_FUTURE.md`](../KyndeBlade_Godot/docs/COMBAT_VOXEL_STAGE_FUTURE.md).

```bash
cd /path/to/KyndeBlade
godot4 --path KyndeBlade_Godot --headless --script res://tests/run_headless_tests.gd
```

- Exit code **0** = all assertions passed.
- Exit code **1** = failure (see stderr).

**Note:** Autoloads from `project.godot` load with `--path`; the script extends `SceneTree` and runs after the engine initializes.

## Local editor

- Open the Godot project folder and use **Run** for manual QA per [`STEAM_BUILD.md`](../KyndeBlade_Godot/STEAM_BUILD.md).

## GUT (optional)

For richer suites, add the [GUT](https://github.com/bitwes/Gut) addon under `addons/gut/`, then:

```bash
godot4 --path KyndeBlade_Godot --headless -s addons/gut/gut_cmdln.gd -gdir=res://tests -gexit
```

Document your GUT version in `KyndeBlade_Godot/README.md` when enabled.

## CI wiring (GitHub Actions example)

```yaml
jobs:
  godot-headless:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup Godot
        uses: firebelley/godot-export@v5   # or install godot4 from releases
        with:
          godot_executable_download_url: 'https://github.com/godotengine/godot/releases/download/4.6.1-stable/Godot_v4.6.1-stable_linux.x86_64.zip'
      - name: Run headless tests
        run: godot --path KyndeBlade_Godot --headless --script res://tests/run_headless_tests.gd
```

Adjust the download URL to your pinned Godot version. If the action is overkill, use a plain `curl` + `unzip` step.

## Unity vs Godot CI

- Unity batchmode remains documented in [`CI_UNITY_TESTS.md`](CI_UNITY_TESTS.md).
- After **M6 archive**, disable Unity jobs or scope them to `ProjectArchive/UnityKyndeBlade/` only.

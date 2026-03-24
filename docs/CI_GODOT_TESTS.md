# Godot — CI and headless tests

Run automated checks for [`KyndeBlade_Godot/`](../KyndeBlade_Godot/). **TDAD:** [`.tdad/README.md`](../.tdad/README.md); headless save/autosave/migration proofs trace to **`godot-steam-build`** + **`godot-parity-slice`** (see [`godot-steam-build.workflow.json`](../.tdad/workflows/godot-steam-build/godot-steam-build.workflow.json)).

## Prerequisites

- **Godot 4.6.1** (or **4.6.x**): the editor binary inside the macOS `.app` is named **`Godot`** (capital G). This repo ships a launcher that works in **non-interactive** shells too: [`scripts/godot4`](../scripts/godot4) (executable; forwards to `Godot` after fixing `PATH`).
- Project path: repo root + `KyndeBlade_Godot`.

**Fresh clone / CI:** If `--headless` fails with parse errors like `Identifier "HubRouteRegistry" not declared` (or other `class_name` types), the project has not been imported yet. Run **once** (generates `KyndeBlade_Godot/.godot/`, usually gitignored):

```bash
godot4 --path KyndeBlade_Godot --import
```

Then re-run the headless command below.

### PATH on macOS (this repo + your shell)

- **Cursor / VS Code:** [`.vscode/settings.json`](../.vscode/settings.json) prepends `Godot.app` / `Godot 4.app` `Contents/MacOS` (and optional `${workspaceFolder}/.cache/Godot.app/...`) to `PATH` in the **integrated terminal** (new terminal tab after saving).
- **System zsh:** `source scripts/path_godot4.zsh` from this repo (see file header) or prepend `scripts/` to `PATH` so `godot4` resolves to the launcher. Install Godot from [godotengine.org](https://godotengine.org/download) or `brew install --cask godot`.

## Headless smoke (no addon)

The project ships [`tests/run_headless_tests.gd`](../KyndeBlade_Godot/tests/run_headless_tests.gd). It runs save/settings/export checks (incl. **dongeoun gate** + **`combat_defense_tip_ack`** roundtrips, **`narrative_beats_skeleton_lines`**, and **`replay_moveset_matrix`** — parry/feint/hunger/misstep/read-text + `NarrativeContext` return visit), scene smokes (**main menu Continue**, **hub counsel gate**, **combat pause vs window tick**, **world atlas**, **location shell**, **`slice_open_yard_scene_smoke`**), **`_test_scene_transition_smoke`** (loads `hub_map.tscn` and `combat.tscn`, mounts each for one frame — future crawl pop-out path), **`hub_route_map_pin_geo_consistency`** (`lon`/`lat` vs normalized `x`/`y` under `basemap.bounds`), then **[`tests/combat_scenarios.gd`](../KyndeBlade_Godot/tests/combat_scenarios.gd)** (strike loop, dodge/parry **partial** + riposte, feint chip, **enemy-turn reaction** dodge/parry via `force_enemy_turn_reaction_window_in_tests`, stamina gate, defeat) via `CombatManager.use_instant_resolution_for_tests`, then exits.

**Wireframe combat:** deterministic rows ↔ tests are mapped in [`KyndeBlade_Godot/docs/WIREFRAME_COMBAT_CHECKLIST.md`](../KyndeBlade_Godot/docs/WIREFRAME_COMBAT_CHECKLIST.md) §2.

## TDAD `godot-parity-slice` presentation nodes

| Node | Proof |
|------|--------|
| `gparity-combat-stage` | `combat.tscn` + presentation scripts; `_test_scene_transition_smoke` mounts combat; manual STEAM_BUILD |
| `gparity-parry-dodge-eye` | `parry_dodge_eye.tscn` + `CombatManager.presentation_tick`; manual / BDD `@manual` wireframe rows |
| *(combat voxel prototype)* | `combat_voxel_arena.gd` in `combat.tscn` — no extra headless assert; GPU/visual smoke optional per [`COMBAT_VOXEL_STAGE_FUTURE.md`](../KyndeBlade_Godot/docs/COMBAT_VOXEL_STAGE_FUTURE.md) |

## TDAD `godot-demo-components` traceability

Each row maps a node in [`.tdad/workflows/godot-demo-components/godot-demo-components.workflow.json`](../.tdad/workflows/godot-demo-components/godot-demo-components.workflow.json) to BDD ([`.tdad/bdd/godot-parity-slice.feature`](../.tdad/bdd/godot-parity-slice.feature)), headless proof in `run_headless_tests.gd` / `combat_scenarios.gd`, and manual QA in [`STEAM_BUILD.md`](../KyndeBlade_Godot/STEAM_BUILD.md) (checklist numbering).

| TDAD node | BDD scenario (title) | Headless / automated proof | Manual (`STEAM_BUILD.md`) |
|-----------|----------------------|----------------------------|---------------------------|
| `gdc-main-menu` | Continue is available when save exists | `_test_main_menu_continue_gated_smoke` | § Manual QA #1 (Continue disabled cold run) |
| `gdc-tower-intro` | Hub shows tour after New Game (direct from menu); Unity export JSON is present for pipeline parity | `_test_unity_export_json` (arrival beat fields) | § Player journey; Manual #2 |
| `gdc-hub-slice-locations` | Hub offers travel to Fair Field; Unity export JSON… | `_test_slice_locations_json` | § Parity table (tour → Fair Field) |
| `gdc-hub-counsel-gate` | Fair Field travel requires counsel before combat | `_test_hub_counsel_gate_smoke` | § Player journey; Manual #2 |
| `gdc-world-atlas` | — | `_test_world_atlas_scene_smoke` | § World skeleton (World atlas) |
| `gdc-location-shell` | — | `_test_location_shell_scene_smoke` | — (authoring shell) |
| `gdc-combat-encounter-load` | Fair Field combat uses False encounter data; Unity export JSON… | `_test_encounter_resource` | § Parity table |
| `gdc-combat-scenarios` | Headless combat scenario suite passes | `combat_scenarios.gd` via runner (incl. `enemy_turn_reaction_*`) | [`WIREFRAME_COMBAT_CHECKLIST.md`](../KyndeBlade_Godot/docs/WIREFRAME_COMBAT_CHECKLIST.md) §2 |
| `gdc-combat-misstep-feint` | Ethical misstep inverts first defensive read in combat | `misstep_inverts_first_defensive_window` in `combat_scenarios.gd` | — |
| `gdc-combat-pause` | Pause exists in combat | `_test_combat_pause_freezes_window_tick_smoke` | § Manual QA #3 |
| `gdc-combat-victory-save` | Victory returns to hub with updated progress | `_test_victory_fair_field_updates_gamestate` | § Manual QA #4 |
| `gdc-settings-fullscreen` | — | `_test_settings_fullscreen_roundtrip` | § Manual QA #5 (fullscreen) |
| `gdc-medieval-catalog` | — | `_test_medieval_catalog_aggregate`, `_test_read_medieval_text_ids_roundtrip`, `_test_medieval_list_granted_codes` | — |
| `gdc-piers-symbols` | — | `_test_piers_symbol_catalog_fayre_felde`, `_test_piers_state_save_roundtrip` | — |
| `gdc-placeholder-art-registry` | — | `_test_placeholder_art_registry` | — |
| `gdc-scene-transition` | — | `_test_scene_transition_smoke` | — |
| *(hub basemap pins)* | — | `hub_route_map_pin_geo_consistency` | [`NAV_MAP_BASEMAP.md`](../KyndeBlade_Godot/docs/NAV_MAP_BASEMAP.md) validation log |

**Future full-screen 3D combat** may need optional GPU-backed visual smoke; logic tests stay headless — see [`KyndeBlade_Godot/docs/COMBAT_VOXEL_STAGE_FUTURE.md`](../KyndeBlade_Godot/docs/COMBAT_VOXEL_STAGE_FUTURE.md).

```bash
cd /path/to/KyndeBlade
godot4 --path KyndeBlade_Godot --headless res://tests/headless_main.tscn
```

- Exit code **0** = all assertions passed.
- Exit code **1** = failure (see stderr).

**Note:** Entry scene `tests/headless_main.tscn` mounts [`run_headless_tests.gd`](../KyndeBlade_Godot/tests/run_headless_tests.gd) on a `Node` so autoload singletons (`SaveService`, `GameState`, …) exist before the runner script compiles. Running with `--script` on a `SceneTree` script was unreliable on Godot **4.6.x** (singletons not in scope at compile time).

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
      - name: Import project (global class cache)
        run: godot --path KyndeBlade_Godot --import
      - name: Run headless tests
        run: godot --path KyndeBlade_Godot --headless res://tests/headless_main.tscn
```

Adjust the download URL to your pinned Godot version. If the action is overkill, use a plain `curl` + `unzip` step.

## Unity vs Godot CI

- Unity batchmode remains documented in [`CI_UNITY_TESTS.md`](CI_UNITY_TESTS.md).
- After **M6 archive**, disable Unity jobs or scope them to `ProjectArchive/UnityKyndeBlade/` only.

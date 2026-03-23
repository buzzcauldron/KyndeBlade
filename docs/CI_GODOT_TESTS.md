# Godot — CI and headless tests

Run automated checks for [`KyndeBlade_Godot/`](../KyndeBlade_Godot/). **TDAD:** [`.tdad/README.md`](../.tdad/README.md); headless save/autosave/migration proofs trace to **`godot-steam-build`** + **`godot-parity-slice`** (see [`godot-steam-build.workflow.json`](../.tdad/workflows/godot-steam-build/godot-steam-build.workflow.json)).

## Prerequisites

- **Godot 4.2+** binary on `PATH` (e.g. `godot4`, or rename the editor binary).
- Project path: repo root + `KyndeBlade_Godot`.

## Headless smoke (no addon)

The project ships [`tests/run_headless_tests.gd`](../KyndeBlade_Godot/tests/run_headless_tests.gd). It runs save/settings/export checks, then **[`tests/combat_scenarios.gd`](../KyndeBlade_Godot/tests/combat_scenarios.gd)** (strike loop, dodge/parry windows, feint chip, stamina gate, defeat) via `CombatManager.use_instant_resolution_for_tests`, then exits.

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
          godot_executable_download_url: 'https://github.com/godotengine/godot/releases/download/4.2.2-stable/Godot_v4.2.2-stable_linux.x86_64.zip'
      - name: Run headless tests
        run: godot --path KyndeBlade_Godot --headless --script res://tests/run_headless_tests.gd
```

Adjust the download URL to your pinned Godot version. If the action is overkill, use a plain `curl` + `unzip` step.

## Unity vs Godot CI

- Unity batchmode remains documented in [`CI_UNITY_TESTS.md`](CI_UNITY_TESTS.md).
- After **M6 archive**, disable Unity jobs or scope them to `ProjectArchive/UnityKyndeBlade/` only.

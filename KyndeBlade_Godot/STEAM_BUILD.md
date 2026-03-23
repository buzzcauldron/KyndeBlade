# Kynde Blade — Godot Steam / retail build

## TDAD traceability

| TDAD artifact | Role |
|---------------|------|
| [`.tdad/README.md`](../.tdad/README.md) | **Index** of all workflows, BDD, prompts, tier map |
| [`.tdad/workflows/root.workflow.json`](../.tdad/workflows/root.workflow.json) | **Root graph** — node `godot-steam-build` depends on `godot-parity-slice` |
| [`.tdad/workflows/godot-steam-build/godot-steam-build.workflow.json`](../.tdad/workflows/godot-steam-build/godot-steam-build.workflow.json) | **This build’s plan**: retail paths, autosave, migration, headless CI, export |
| [`.tdad/workflows/godot-parity-slice/godot-parity-slice.workflow.json`](../.tdad/workflows/godot-parity-slice/godot-parity-slice.workflow.json) | Behaviour parity vs Tier A demo slice |
| [`.tdad/bdd/godot-parity-slice.feature`](../.tdad/bdd/godot-parity-slice.feature) | Gherkin scenarios (includes Steam-path checks) |
| [`.tdad/workflows/save-system/save-system.workflow.json`](../.tdad/workflows/save-system/save-system.workflow.json) | Unity save model; node **Godot SaveService (Steam retail paths)** cross-links here |
| [Release paths](../docs/TDAD_RELEASE_PATHS.md) | Tier A / B / Godot narrative |

This Godot 4 project is treated as the **shipping Steam (and desktop) build** for the slice: same scope as the Unity Tier A vertical slice, packaged for players — **not** an internal “demo-only” artifact. Unity remains the authoring oracle until M6 archive policy changes; see parity table below.

**Player journey (current slice):** **main menu → tower arrival beat → hub → Fair Field flavor (export story text) → counsel (Trewthe / Mede / Name hunger) → combat → outcome → hub**, with **save / continue**, **autosave mirror**, **settings** (volume + fullscreen), and **pause** (freezes the real-time dodge/parry window).

**World skeleton (authoring):** From hub, **World atlas — all stedes** opens the full Unity-planned location list (`data/world/locations_registry.json`). See [`docs/GAME_SKELETON.md`](docs/GAME_SKELETON.md).

**Target experience (longer-term Godot direction):** [`docs/GODOT_TARGET_EXPERIENCE.md`](docs/GODOT_TARGET_EXPERIENCE.md) — fake-voxel map crawl, full-screen 3D voxel combat, E33-style windows, Noita mood, manuscript pixel refs (provenance required). **One-page vision:** [`docs/VISION_CRAWL_NOITA_E33.md`](docs/VISION_CRAWL_NOITA_E33.md). **Future phases:** [`docs/CRAWL_PROTOTYPE_FUTURE.md`](docs/CRAWL_PROTOTYPE_FUTURE.md), [`docs/COMBAT_VOXEL_STAGE_FUTURE.md`](docs/COMBAT_VOXEL_STAGE_FUTURE.md).

**Extra modes (menu):** **Tiny practice loop** → `scenes/beginner_loop.tscn`. **High-bit medieval bonus** → `scenes/hi_bit_bonus_level.tscn` — **320×180** faux-pixel view (integer scaled), procedural art only.

This is **not** a line-by-line port of Unity. It mirrors **names and flow** from [`PLAYABLE_SLICE.md`](../KyndeBlade_Unity/Assets/KyndeBlade/Docs/PLAYABLE_SLICE.md). TDAD parity: [`.tdad/workflows/godot-parity-slice/godot-parity-slice.workflow.json`](../.tdad/workflows/godot-parity-slice/godot-parity-slice.workflow.json).

**Engine / language:** GDScript (Godot 4).

**Automated smoke:** [`docs/CI_GODOT_TESTS.md`](../docs/CI_GODOT_TESTS.md) — run `tests/run_headless_tests.gd` headless.

**Data export:** Unity **KyndeBlade → Export Slice Data for Godot** → [`data/exported_from_unity.json`](data/exported_from_unity.json) (committed for CI).

**Port tracking:** [`PARITY_GAPS.md`](PARITY_GAPS.md), [`PORT_WAVE.md`](PORT_WAVE.md). **Unity ↔ Godot map:** [`docs/UNITY_GODOT_MODULE_MAP.md`](../docs/UNITY_GODOT_MODULE_MAP.md). **Licenses:** [`docs/ASSET_LICENSES.md`](../docs/ASSET_LICENSES.md). **Art → Godot theme:** [`docs/ART_DIRECTION_GODOT.md`](docs/ART_DIRECTION_GODOT.md).

**TDAD — combat presentation:** [`docs/TDAD_COMBAT_PRESENTATION_PLAN.md`](docs/TDAD_COMBAT_PRESENTATION_PLAN.md).

**Cult-ship & coauthorship:** [`docs/CULT_SHIP_BAR.md`](docs/CULT_SHIP_BAR.md), [`docs/CULT_AND_COAUTHORSHIP.md`](../docs/CULT_AND_COAUTHORSHIP.md).

## Steam / desktop shipping notes

| Topic | Notes |
|-------|--------|
| **Depots** | Use Godot export presets per OS; one depot per OS is typical for Steam. |
| **steam_appid.txt** | For local Steam API testing, place `steam_appid.txt` with your AppID next to the executable (not committed here). |
| **Cloud** | Saves live under Godot `user://` (`kyndeblade_save.cfg`, etc.). Enable **Steam Cloud** only after you map these paths in Steamworks partner settings. |
| **Build ID** | Bump `SaveService.SAVE_VERSION` when you migrate save schema; document in release notes. **v2** adds Piers metadata: `piers_text_edition`, `narrative_phase_id`, `location_visit_counts` (pipe-separated `loc:count` pairs), `fair_field_return_count`, `dream_iteration` — older saves default these on load. |

## Unity ↔ Godot parity (TDAD demo-vertical-slice titles)

| Unity TDAD node (demo-vertical-slice) | Godot proof |
|---------------------------------------|-------------|
| Main scene map bootstraps at tour | **New Game** → `tower_intro.tscn` → hub; `GameState.current_location_id == tour`; manual #2 |
| Tour location lists Fair Field as next | `data/slice_locations.json` + hub **Travel to Fair Field**; headless `_test_slice_locations_json` |
| Fair Field encounter wired to False | `data/encounter_fair_field.tres` (`enemy_id: false`); `CombatManager` loads it |
| Save checkpoint updates current location | `SaveService.save_progress`; headless `_test_save_roundtrip_service` |
| Game progress JSON roundtrip | **Unity:** JSON. **Godot:** `ConfigFile` — same *behaviour*; headless save test |
| Pause overlay present after bootstrap | `combat.tscn` `PauseLayer`; manual #3 |
| Settings master volume persists | `kyndeblade_settings.cfg`; headless `_test_settings_volume` |
| Continue when save exists | Main menu **Continue**; headless + manual #1 |
| Combat win → hub (manual) | Victory → **Continue to hub**; `GameState.on_victory_fair_field()`; manual #4 |

**Oracle:** Until M6 archive, if behaviour disagrees, **Unity + DEMO_RUN** wins unless you **update both sides** and BDD ([`demo-vertical-slice.feature`](../.tdad/bdd/demo-vertical-slice.feature) / [`godot-parity-slice.feature`](../.tdad/bdd/godot-parity-slice.feature)).

## Controls

| Action | Keyboard | Notes |
|--------|----------|--------|
| Strike | **X** | Stamina; damage to False |
| Dodge | **Space** | Timing window vs swing/feint |
| Parry | **Shift** | Shorter window |
| Pause | **Escape** | Combat pause / resume |
| Menu | **Mouse** | Buttons |

Gamepad: **A / B / X** and **Start** if actions had no keys (`scripts/input_map_setup.gd`).

## Save and settings files (vs Unity)

| Concern | Unity | Godot (Steam build) |
|---------|-------|---------------------|
| Save slot | `PlayerPrefs` `KyndeBlade_Save` (JSON) | `user://kyndeblade_save.cfg` + **autosave** `user://kyndeblade_autosave.cfg` |
| Settings | `KyndeBlade_Settings_*` | `user://kyndeblade_settings.cfg` |
| Continue gating | `SaveManager.HasSavedGame` | `SaveService.has_save()` |

**Legacy migration:** If a player still has **`kyndeblade_demo_save.cfg`** from an older Godot package, the first `load_save()` **imports** it into the new paths (see `save_service.gd`). **`kyndeblade_demo_settings.cfg`** is also read once and copied to `kyndeblade_settings.cfg` when loading volume/fullscreen.

**Autosave:** Mirrors every `save_progress`, flush on **focus loss**, **every 90s** while a save exists, and pause → **Main menu** from combat. **New Game** clears current slots and removes legacy demo save files.

## Combat (deterministic)

See parity: `feint_pattern_offset`, `PlayerMovesetModifiers`, `MedievalTextCatalog`, *Piers* voice + ink (`piers_combat_voice.gd`, `dreamer_ink_physics.gd`). Lane B backdrop: **hi-bit ruin vista** procedural read (`combat_manuscript_backdrop.gd`, `KyndeBladeArtPalette.HI_BIT_*`, reference PNG [`assets/hi_bit_ruin_vista/`](assets/hi_bit_ruin_vista/README.md)). Headless [`tests/combat_scenarios.gd`](tests/combat_scenarios.gd).

## Manual QA checklist

1. **Cold run**: F5 → main menu; **Continue** disabled without save.
2. **New Game** → tower vista → hub (**tower_vista** read) → **Fair Field** (**FayreFelde** read) → counsel → combat.
3. **Combat**: Lane B art, window SFX, eye UI, pause freezes window.
4. **Victory** → hub; save state; **Continue** restores.
5. **Settings**: volume + fullscreen.
6. **Defeat** → hunger flag + autosave.

## Headless regression

```bash
godot4 --path KyndeBlade_Godot --headless --script res://tests/run_headless_tests.gd
```

See [`docs/CI_GODOT_TESTS.md`](../docs/CI_GODOT_TESTS.md).

## Export (Steam / desktop)

1. Project → **Export** → Windows / macOS / Linux presets.
2. Example binary: `builds/KyndeBlade.exe` (or `.app` / Linux binary).
3. Zip export + PCK if separate; upload to Steam depots per [TDAD_RELEASE_PATHS.md](../docs/TDAD_RELEASE_PATHS.md).

## Roadmap (content depth)

- Kynde / break / hazards, full map graph, narrative parity with Unity.
- More `EncounterDef` resources; expanded automated tests.

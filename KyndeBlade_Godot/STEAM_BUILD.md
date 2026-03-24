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

**Player journey (current slice):** **main menu → hub route map** ([`scenes/hub_map.tscn`](scenes/hub_map.tscn)) **→ Fair Field flavor (export story text) → counsel (Trewthe / Mede / Name hunger) → combat → outcome → hub**, then optionally **Dongeoun** on the map (unlocks after Fair Field victory; gate warden encounter with defensive wind-up telegraph), with **save / continue**, **autosave mirror**, **settings** (volume + fullscreen), and **pause** (freezes the real-time dodge/parry window). **Malvern prologue yard** ([`scenes/slice_open_yard.tscn`](scenes/slice_open_yard.tscn)) and **tower arrival beat** ([`scenes/tower_intro.tscn`](scenes/tower_intro.tscn)) remain in the project for headless smokes and a future optional prologue path; they are **not** the default New Game opening. **Continue** loads the hub from save as before.

**Pacing (mini-act):** Target **~12–18 minutes** for a first cold run through tower → Fair Field combat → Dongeoun gate combat (reading speed and counsel choice variance). [`data/world/narrative_beats_skeleton.json`](data/world/narrative_beats_skeleton.json) supplies arrival fallbacks when the Unity export row has no beat text.

**World skeleton (authoring):** From hub, **World atlas — all stedes** opens the full Unity-planned location list (`data/world/locations_registry.json`). See [`docs/GAME_SKELETON.md`](docs/GAME_SKELETON.md).

**Target experience (longer-term Godot direction):** [`docs/GODOT_TARGET_EXPERIENCE.md`](docs/GODOT_TARGET_EXPERIENCE.md) — fake-voxel map crawl, full-screen 3D voxel combat, E33-style windows, Noita mood, manuscript pixel refs (provenance required). **One-page vision:** [`docs/VISION_CRAWL_NOITA_E33.md`](docs/VISION_CRAWL_NOITA_E33.md). **Future phases:** [`docs/CRAWL_PROTOTYPE_FUTURE.md`](docs/CRAWL_PROTOTYPE_FUTURE.md), [`docs/COMBAT_VOXEL_STAGE_FUTURE.md`](docs/COMBAT_VOXEL_STAGE_FUTURE.md).

**Extra modes (menu):** **Tiny practice loop** → `scenes/beginner_loop.tscn`. **High-bit medieval bonus** → `scenes/hi_bit_bonus_level.tscn` — **320×180** faux-pixel view (integer scaled), procedural art only.

This is **not** a line-by-line port of Unity. It mirrors **names and flow** from [`PLAYABLE_SLICE.md`](../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/PLAYABLE_SLICE.md). TDAD parity: [`.tdad/workflows/godot-parity-slice/godot-parity-slice.workflow.json`](../.tdad/workflows/godot-parity-slice/godot-parity-slice.workflow.json).

**Engine / language:** GDScript (**Godot 4.6.1** recommended; **4.6.x** compatible — see `project.godot` `config/features` and [`docs/CI_GODOT_TESTS.md`](../docs/CI_GODOT_TESTS.md)).

**Automated smoke:** [`docs/CI_GODOT_TESTS.md`](../docs/CI_GODOT_TESTS.md) — run `tests/run_headless_tests.gd` headless.

**Data export:** Unity **KyndeBlade → Export Slice Data for Godot** → [`data/exported_from_unity.json`](data/exported_from_unity.json) (committed for CI).

**Port tracking:** [`PARITY_GAPS.md`](PARITY_GAPS.md), [`PORT_WAVE.md`](PORT_WAVE.md). **Unity ↔ Godot map:** [`docs/UNITY_GODOT_MODULE_MAP.md`](../docs/UNITY_GODOT_MODULE_MAP.md). **Licenses:** [`docs/ASSET_LICENSES.md`](../docs/ASSET_LICENSES.md). **Art → Godot theme:** [`docs/ART_DIRECTION_GODOT.md`](docs/ART_DIRECTION_GODOT.md).

**TDAD — combat presentation:** [`docs/TDAD_COMBAT_PRESENTATION_PLAN.md`](docs/TDAD_COMBAT_PRESENTATION_PLAN.md). **`CombatManager.presentation_tick`** — fires on window open and each `tick_window` frame during `REAL_TIME_WINDOW` (ParryDodgeEye uses it; wind-up still updates in `_process`).

**Cult-ship & coauthorship:** [`docs/CULT_SHIP_BAR.md`](docs/CULT_SHIP_BAR.md), [`docs/CULT_AND_COAUTHORSHIP.md`](../docs/CULT_AND_COAUTHORSHIP.md).

## Steam / desktop shipping notes

| Topic | Notes |
|-------|--------|
| **Depots** | Use Godot export presets per OS; one depot per OS is typical for Steam. |
| **steam_appid.txt** | For local Steam API testing, place `steam_appid.txt` with your AppID next to the executable (not committed here). |
| **Cloud** | Saves live under Godot `user://` (`kyndeblade_save.cfg`, etc.). Enable **Steam Cloud** only after you map these paths in Steamworks partner settings. |
| **Build ID** | Bump `SaveService.SAVE_VERSION` when you migrate save schema; document in release notes. **v2** adds Piers metadata: `piers_text_edition`, `narrative_phase_id`, `location_visit_counts` (pipe-separated `loc:count` pairs), `fair_field_return_count`, `dream_iteration` — older saves default these on load. **v3** adds `dongeoun_gate_cleared` (second hub combat slice; defaults false on load). **`combat_defense_tip_ack`** (one-time feint-read HUD line; defaults false) is written alongside v3 saves. |

## Unity ↔ Godot parity (TDAD demo-vertical-slice titles)

| Unity TDAD node (demo-vertical-slice) | Godot proof |
|---------------------------------------|-------------|
| Main scene map bootstraps at tour | **New Game** → `hub_map.tscn`; `GameState.current_location_id == tour`; manual #2 |
| Tour location lists Fair Field as next | `data/slice_locations.json` + hub **Travel to Fair Field**; headless `_test_slice_locations_json` |
| Fair Field encounter wired to False | `data/encounter_fair_field.tres` (`enemy_id: false`); `CombatManager` loads it |
| Dongeoun gate encounter (post–Fair Field) | `data/encounter_dongeoun_gate.tres`; hub travel + `GameState.pending_combat_encounter_path`; headless `encounter_resource` + `dongeoun_gate_save_roundtrip` |
| Save checkpoint updates current location | `SaveService.save_progress`; headless `_test_save_roundtrip_service` |
| Game progress JSON roundtrip | **Unity:** JSON. **Godot:** `ConfigFile` — same *behaviour*; headless save test |
| Pause overlay present after bootstrap | `combat.tscn` `PauseLayer`; manual #3 |
| Settings master volume persists | `kyndeblade_settings.cfg`; headless `_test_settings_volume` |
| Continue when save exists | Main menu **Continue**; headless + manual #1 |
| Combat win → hub (manual) | Victory → **Continue to hub**; `GameState.on_victory_fair_field()` or `on_victory_dongeoun_gate()`; manual #4–5 |

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

See parity: `feint_pattern_offset`, `PlayerMovesetModifiers`, `MedievalTextCatalog`, *Piers* voice + ink (`piers_combat_voice.gd`, `dreamer_ink_physics.gd`). Lane B backdrop: **hi-bit ruin vista** procedural read (`combat_manuscript_backdrop.gd`, `KyndeBladeArtPalette.HI_BIT_*`, reference PNG [`assets/hi_bit_ruin_vista/`](assets/hi_bit_ruin_vista/README.md)). Headless [`tests/combat_scenarios.gd`](tests/combat_scenarios.gd). **Rules matrix + automation map:** [`docs/WIREFRAME_COMBAT_CHECKLIST.md`](docs/WIREFRAME_COMBAT_CHECKLIST.md).

## Manual QA checklist

1. **Cold run**: F5 → main menu; **Continue** disabled without save.
2. **New Game** → **hub** (route map at **tour**) → **Fair Field** (**FayreFelde** read) → counsel → combat. *(Optional later: re-chain Malvern yard → tower intro before hub.)*
3. **Combat**: Lane B art, window SFX, eye UI (wind-up gather → open window phases), pause freezes window; **first defensive wind-up** clears the extra feint-vs-true line under the control hint (persisted as `combat_defense_tip_ack`). **Wireframe pass** (ordered greybox rules + feel): see [§ Wireframe combat pass](#wireframe-combat-pass-greybox) below.
4. **Victory (Fair Field)** → hub; save state; **Continue** restores; **Dongeoun** should appear on the map with a hint in flavor copy.
5. **Dongeoun**: travel → flavor → **Enter combat** → gate fight (wind-up phase before dodge/parry window; bleed ticks on enemy turns); **Victory** → cleared flavor on revisit.
6. **Settings**: volume + fullscreen.
7. **Defeat** → hunger flag + autosave.

### Wireframe combat pass (greybox)

Use [`docs/WIREFRAME_COMBAT_CHECKLIST.md`](docs/WIREFRAME_COMBAT_CHECKLIST.md) for the full matrix. Short ordered pass:

1. Enter Fair Field combat (or gauntlet); confirm **first** defensive window matches feint/read expectations (misstep can invert).
2. On a **real** swing: **Dodge** — HP should drop by **12** (partial of 20), not full 20.
3. On the next **real** swing: **Parry** — HP chip should be **smaller** than dodge (**6** if 20 base), and **enemy** HP should drop from **riposte** (audible/visual feedback).
4. On a **feint** with wasted defend: **5** chip only.
5. **Pause** during an open defensive window; resume; timer should not have advanced while paused.
6. **Settings:** toggle **real-time defense on enemy turn** off → strike → confirm **no** reaction window; toggle on → strike → **dodge** or **parry** during reaction; chip should follow **enemy_turn_damage** fractions (see checklist).
7. Skim **2D + 3D** stage motion and **SFX** pitch (feint vs real) per checklist §4.

## Headless regression

```bash
godot4 --path KyndeBlade_Godot --headless res://tests/headless_main.tscn
```

See [`docs/CI_GODOT_TESTS.md`](../docs/CI_GODOT_TESTS.md).

## Boss demo week — macOS greybox (two-day focus)

**Today:** Run headless smoke; play **Reaction loop (greybox · B)** from the main menu (3 wins before 2 losses); play **Combat gauntlet (greybox · 3 fights)** (Fair Field ×2 → Dongeoun gate); confirm **Back to main menu** returns cleanly and your **Continue** save was not overwritten by gauntlet wins.

**Tomorrow:** **Project → Export** with a **macOS** preset (Godot 4.6.x export templates installed); export to a fresh folder; zip the `.app` bundle; on another account or machine, **right-click → Open** the first time to satisfy Gatekeeper; note one paragraph of “known rough edges” for your walkthrough.

## Export (Steam / desktop)

1. Project → **Export** → Windows / macOS / Linux presets.
2. Example binary: `builds/KyndeBlade.exe` (or `.app` / Linux binary).
3. Zip export + PCK if separate; upload to Steam depots per [TDAD_RELEASE_PATHS.md](../docs/TDAD_RELEASE_PATHS.md).

## Roadmap (content depth)

- Kynde / break / hazards, full map graph, narrative parity with Unity.
- More `EncounterDef` resources; expanded automated tests.

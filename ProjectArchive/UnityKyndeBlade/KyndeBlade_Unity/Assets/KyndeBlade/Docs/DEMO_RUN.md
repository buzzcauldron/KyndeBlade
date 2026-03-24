# Demo Run — Testable Demo Path

This document describes the intended demo path and how to run and verify it in the editor.

**Single-loop vertical slice (hub → combat → hub):** see [PLAYABLE_SLICE.md](PLAYABLE_SLICE.md).

**TDAD (repo root):** Tier A workflow [`.tdad/workflows/demo-vertical-slice/demo-vertical-slice.workflow.json`](../../../../.tdad/workflows/demo-vertical-slice/demo-vertical-slice.workflow.json), Gherkin [`.tdad/bdd/demo-vertical-slice.feature`](../../../../.tdad/bdd/demo-vertical-slice.feature). Tier B planning: [`.tdad/workflows/steam-early-access/`](../../../../.tdad/workflows/steam-early-access/) — see also [TDAD_RELEASE_PATHS.md](../../../../docs/TDAD_RELEASE_PATHS.md) at repo root.

## Definition of done (Tier A — mirrors TDAD `demo-vertical-slice`)

Automated (EditMode / PlayMode): run the Unity Test Runner on `KyndeBlade.Tests`.

| TDAD node (summary) | What passes |
|---------------------|-------------|
| Main scene map bootstraps at tour | `DemoVerticalSlicePlayModeTests.MainScene_SkipMenu_CurrentLocationId_IsTour` |
| Tour lists Fair Field as next | `PlayableSliceEditModeTests.PlayableSlice_Tour_ListsFayreFeldeAsNext` |
| Fair Field encounter wired | `PlayableSliceEditModeTests` (Fayre Felde + constants) |
| Save checkpoint updates location | `MapSaveTest.SaveCheckpoint_UpdatesLocationAndUnlocks` |
| Game progress JSON roundtrip | `MapSaveTest.GameProgress_ToJsonFromJson_Roundtrips` |
| Pause overlay present | `DemoVerticalSlicePlayModeTests.MainScene_SkipMenu_PauseRoot_ExistsAndStartsHidden` |
| Settings volume persists | `UiShellEditModeTests.KyndeBladeSettingsStore_MasterVolume_Persists` |
| Continue when save exists | `UiShellEditModeTests.SaveManager_HasSavedGame_TrueWhenPrefsContainSave` |

**Manual (required once per release candidate):** combat win → **Continue** → map at Fair Field with expected next locations — see [What to verify](#what-to-verify) below (TDAD node: *Combat win Continue returns to map (manual)*).

## Build / zip (friends can run)

1. **Build Settings:** `Main` (or your bootstrap scene) must be **first** in the list.
2. **Targets:** ship at least one desktop target you can support (e.g. Windows, macOS); note known gaps in your store page or README.
3. **Zip contents:** player executable + `*_Data` (and dependencies your Unity version requires); no Editor-only assets.
4. **Player-facing blurb:** one short paragraph + one screenshot (map or combat) matching the path in this doc.

## Demo path (intended)

1. **Start** — **Main menu** (New Game / Continue if a save exists). After you start, the game proceeds to the Tower on the Toft (first view): story beat (vista), then map.
2. **Navigate** — Open map; select next location (e.g. Fair Field / fayre_felde).
3. **Narrative** — Story beat and/or dialogue may play on arrival or before combat.
4. **Combat** — One encounter: turn-based + real-time parry/dodge window; clear success or fail.
5. **Outcome** — Victory or defeat panel; then return to map or restart. Save checkpoint and permanent flags stay consistent.

**Stretch:** One dialogue choice with a permanent effect (e.g. ethical misstep or Green Knight flag) that is discernable later (e.g. in combat or a later line). One “hidden but discernable” effect (e.g. status or environmental cue).

---

## How to run in the editor

1. Open the main scene (e.g. `Main.unity` or the scene that contains `KyndeBladeGameManager`).
2. Ensure **Build Settings** list this scene first so cold start is correct.
3. Press Play. You should see the map (or story beat then map) with the current location set from save or from **Start Location** on `WorldMapManager`.

### DemoTestHelper (optional)

Add a **DemoTestHelper** component to the scene (e.g. on the same GameObject as `KyndeBladeGameManager` or a dedicated debug object).

- **Force Start Location Id** — e.g. `tour` or `tower_on_toft`. When set, the first time the map initializes it uses this location instead of the saved location or `WorldMapManager.StartLocation`. Use for testing “start at Tower” without changing save data.
- **Skip Tower Intro Story** — if enabled, skips the Tower on the Toft arrival story beat so the map appears immediately (faster slice QA).
- **Skip To Location Id** — e.g. `fayre_felde`. Use the context menu **Skip to Location** (right-click component in Inspector) to jump directly to that location for quick combat testing.
- **Log World State** — Context menu **Log World State** prints to the console: current location, `EthicalMisstepCount`, `GreenKnightWillAppearRandomly`, `HasEverHadHunger`.

No UI is required; inspector and context menus are enough.

---

## What to verify

| Step | Check |
|------|--------|
| Start | New game: Tower vista story beat, then map with Tower (tour) as current location. No console errors. |
| Map | Current location = Tower on the Toft. Next locations include Fair Field (fayre_felde). |
| Select Fair Field | Transition runs; story beat “A fair feeld ful of folke…” (or hunger reflection if HasEverHadHunger); then combat. |
| Combat | Encounter starts; parry/dodge ~2 s, eye phases and imminent sound; victory or defeat panel. |
| After victory | Continue → map; current location = Fair Field; next locations (tour, dongeoun, green_chapel, boundary_tree) selectable. |
| After defeat | Green Chapel → restart at Malvern; other defeat → illumination then defeat message; state consistent. |
| DemoTestHelper | Force start and Skip to Location work; Log state shows Location, EthicalMisstep, GreenKnightRandom, HasEverHadHunger. |

---

## Demo test script (repeatable)

1. **Start game (new game).** Clear save if needed so you start fresh.
2. **You should see** the Tower vista story beat (“A tour on a toft… confined, lovely and strange…”), then the map with **Tower on the Toft** as current location.
3. **Select Fair Field** (Fayre Felde). You should see the arrival story beat (“A fair feeld ful of folke…”), then combat.
4. **Win or lose** the encounter. You should see the victory or defeat panel; then **Continue** (victory) returns to the map with Fair Field as current location and next locations selectable, or **Restart** / flow (defeat) per GameStateManager.
5. **If you made a dialogue choice** that set a permanent flag (e.g. ethical misstep or Green Knight), use **DemoTestHelper → Log World State** to verify, or see the effect in a later combat (e.g. more damage taken) or in a narrative branch (e.g. hunger reflection at Fair Field if HasEverHadHunger).

**Known limitations:** No 3D vista yet (vista is a story beat only). Build must have the main scene first in Build Settings; map and combat pipelines are created by GameManager so the demo runs in a build.

---

## Demo path (current)

**Start at Tower on the Toft** → vista story beat → map → choose **Fair Field** (fayre_felde) → story beat → combat → victory or defeat → map or restart.

# Playable vertical slice (single loop)

One **complete loop** you can ship to a playtester without editor knowledge: **hub → narrative beat → combat → victory → hub**, with save/checkpoint behaviour intact.

## Canonical path (default new game)

| Step | What happens | Data |
|------|----------------|------|
| 1 | **Play** `Main` (first in Build Settings). **Main menu**: **New Game** (or **Continue** if you have a save). | `GameFlowController` + `SaveManager.HasSavedGame` |
| 2 | **Tower on the Toft** — arrival story beat (vista), then **map**. | `Loc_tour` → `StoryBeatOnArrival` |
| 3 | On the map, choose **Fayre Felde** (Fair Field). | `NextLocationIds` from tour include `fayre_felde` |
| 4 | **Arrival beat** then **combat**: one enemy (**False**) via `FayreFeldeEncounter`. | `Loc_fayre_felde` |
| 5 | **Win** → **Continue** on the victory panel → **back to map** at Fair Field. | `GameStateManager` → `ReturnToMap` |
| 6 | (Optional) Pick **Dongeoun** or return to **Tour** for another loop. | `NextLocationIds` on `fayre_felde` |
| — | **Escape** during play: **pause** (resume / settings / main menu). | `GameFlowController` |

**Code reference:** `GameWorldConstants.PlayableSliceFirstCombatLocationId` (`fayre_felde`).

## Faster QA (editor)

Add **DemoTestHelper** to the scene:

- **Skip Tower Intro Story** — skips the tower vista beat; map appears immediately on load.
- **Skip To Location** (context menu) with **Skip To Location Id** = `fayre_felde` — jump straight into Fair Field’s arrival flow and combat.

See also [DEMO_RUN.md](DEMO_RUN.md).

## Definition of done (slice)

**TDAD Tier A** tracks the same bar under [`.tdad/workflows/demo-vertical-slice/`](../../../../.tdad/workflows/demo-vertical-slice/demo-vertical-slice.workflow.json); automated checks are listed in [DEMO_RUN.md](DEMO_RUN.md) (*Definition of done (Tier A)*). Narrative checklist:

- [ ] Cold play from `Main`: no blocking errors; player reaches combat without console spam.
- [ ] Combat is winnable with default moves (Strike / skills as wired).
- [ ] Victory **Continue** returns to map; current location matches design (Fair Field after that fight).
- [ ] Save/checkpoint after travel still loads a sensible location (see `SaveManager`).

## Optional stretch (same build)

- Second fight: **Dongeoun** (`Loc_dongeoun`) after unlocking from Fair Field.
- **Malvern** prologue sequence (real-life beats + `TutorialEncounter`) — longer, not required for the minimal slice above.

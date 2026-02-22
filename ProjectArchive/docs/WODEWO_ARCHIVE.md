# Wode-Wo Line — Archived

**Date archived:** 2025-02  
**Reason:** Feature cut from main line. Preserved here for possible future use or reference.

---

## What Was Removed

The **Wode-Wo** narrative thread: a companion character whose death could be triggered by (1) a wrong dialogue choice, or (2) player defeat to Hunger or the Green Knight. When dead, Wode-Wo stayed dead for the install (persistent save). Locations could show an alternate arrival beat when Wode-Wo was dead (`StoryBeatOnArrivalWhenWodeWoDead`).

The implementation was later gated behind a **secret code** (W O D E keys in sequence). The entire line has now been removed from the codebase.

---

## Design Summary

- **Wrong choice** (e.g. Green Chapel / sin-aligned choice): trigger Wode-Wo death cutscene (WodeWoDeath story beat), set install-permanent `WodeWoDead`, then continue flow (combat or transition).
- **Defeat to Hunger or Green Knight:** set `WodeWoDead`, show WodeWoDeath story beat, then show defeat panel.
- **Arrival:** If `WodeWoDead` and location had `StoryBeatOnArrivalWhenWodeWoDead`, show that beat instead of `StoryBeatOnArrival` (or sequence).

Assets under `Assets/Resources/Data/Vision1/` included: `WodeWoBaby`, `WodeWoCare`, `WodeWoGrown`, `WodeWoDeath`, `WodeWoRemains`. These were not deleted; only the code that referenced them was removed.

---

## Code That Was Removed

### GameProgress / SaveManager

- **GameProgress** (and `GameProgressData`): field `WodeWoDead` (bool), serialized in ToJson/FromJson.
- **SaveManager:** `SetWodeWoDead()`, `IsWodeWoDead` property.

### WorldMapManager

- On arrival: branch that showed `StoryBeatOnArrivalWhenWodeWoDead` when `WodeWoFeatureEnabled && IsWodeWoDead`.
- In `OnChoiceProceed`: when wrong choice, call `SetWodeWoDead()` and show `Resources.Load<StoryBeat>("Data/Vision1/WodeWoDeath")` before continuing; all gated by `GameRuntime.WodeWoFeatureEnabled`.

### GameStateManager

- In `Defeat()`: block that checked `LastEncounterHadHungerOrGreenKnight`, called `SetWodeWoDead()`, showed WodeWoDeath story beat, then showed defeat panel (only when `WodeWoFeatureEnabled`).

### KyndeBladeGameManager

- Field `_lastEncounterHadHunger`; property `LastEncounterHadHungerOrGreenKnight`.
- In `StartEncounterFromConfig`: set `_lastEncounterHadHunger` from config (BossCharacterType / Enemies) and from spawned prefabs (HungerCharacter).
- In `StartSinMinibossEncounter`: set `_lastEncounterHadHunger = (sin == SinType.Gluttony)`.
- Creation of `SecretCodeListener` in `EnsureMapPipeline()`.

### GameRuntime

- Static property `WodeWoFeatureEnabled`.

### LocationNode

- Field `StoryBeatOnArrivalWhenWodeWoDead` and its tooltip.  
  **Note:** Any LocationNode assets that had this field assigned will have that reference dropped when Unity re-serializes (slot becomes empty).

### SecretCodeListener (deleted file)

- Component that listened for key sequence **W, O, D, E** (within 3s) and set `GameRuntime.WodeWoFeatureEnabled = true`.
- Lived in `Assets/KyndeBlade/Code/Core/Game/SecretCodeListener.cs`.

### DialogueTreeDefinition

- Comment only: `BoolParam` example changed from "e.g. WodeWo dead" to "e.g. flag for conditional branch".

---

## How to Restore (Outline)

1. Re-add `WodeWoDead` to `GameProgress` / `GameProgressData` and ToJson/FromJson.
2. Re-add `SetWodeWoDead()` and `IsWodeWoDead` to `SaveManager`.
3. Re-add `StoryBeatOnArrivalWhenWodeWoDead` to `LocationNode` and the arrival branch in `WorldMapManager`.
4. Re-add wrong-choice flow in `WorldMapManager`: set WodeWo dead and show WodeWoDeath beat (optionally gate with a new flag or secret code).
5. Re-add `_lastEncounterHadHunger` and `LastEncounterHadHungerOrGreenKnight` in `KyndeBladeGameManager`, and Hunger-tracking in `StartEncounterFromConfig` / `StartSinMinibossEncounter`.
6. Re-add defeat block in `GameStateManager`: on defeat when encounter was Hunger/Green Knight, set WodeWo dead and show WodeWoDeath then defeat panel.
7. If using secret code again: re-add `WodeWoFeatureEnabled` to `GameRuntime` and restore `SecretCodeListener.cs` (and its creation in `EnsureMapPipeline()`).

Story beat assets (`WodeWoBaby`, `WodeWoCare`, `WodeWoGrown`, `WodeWoDeath`, `WodeWoRemains`) in `Assets/Resources/Data/Vision1/` were left in place.

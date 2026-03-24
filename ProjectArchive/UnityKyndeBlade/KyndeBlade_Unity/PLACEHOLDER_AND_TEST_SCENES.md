# Placeholder Components and Test Scenes

Placeholder bootstraps let you run a **visual test** and a **short gameplay test** with minimal setup.

## Visual Test

**Purpose:** Initialize a scene that shows the manuscript overlay and 16-bit pipeline (no combat or map).

1. **Open:** `KyndeBlade > Load Visual Test` (opens or creates `Assets/Scenes/VisualTest.unity`).
2. **Ensure bootstrap:** If the scene has no bootstrap, run `KyndeBlade > Add Visual Test Bootstrap to Scene`.
3. **Play:** The bootstrap ensures at runtime:
   - Main Camera with `ManuscriptOverlayEffect` (and optional `SixteenBitPipeline`)
   - Directional Light
   - A test quad with manuscript/toon-style material

**Component:** `VisualTestBootstrap` (in `Code/Core/Visual/`). Add to any GameObject; options: `EnsureTestQuad`, `UseSixteenBitPipeline`.

## Gameplay Test

**Purpose:** Initialize all major game systems so combat and map run without NRE (short playable test).

1. **Option A – New scene:** `KyndeBlade > Create Gameplay Test Scene` (creates `Assets/Scenes/GameplayTest.unity` with bootstrap + GameManager).
2. **Option B – Main scene:** `KyndeBlade > Setup Project (Create Scene + Build)` or `Create Main Scene` now adds `GameplayTestBootstrap` + `KyndeBladeGameManager`.
3. **Play:** Bootstrap creates (if missing):
   - SaveManager, NarrativeManager, DialogueSystem, MusicManager  
   - WorldMapManager, CombatUI (minimal Canvas), GameStateManager  
   - IlluminationManager, AgingManager  

Then `KyndeBladeGameManager` runs (TurnManager, spawn, combat/map). Use **Start With Map** = true for level select, or false + **Auto Spawn Test Characters** = true for immediate combat.

**Component:** `GameplayTestBootstrap` (in `Code/Combat/`). Execution order -1100 (before GameManager -1000). Toggles per system: `EnsureCombatUI`, `EnsureSaveManager`, `EnsureNarrative`, etc.

## Major Systems (placeholders created by GameplayTestBootstrap)

| System            | Real component used | Notes                                      |
|-------------------|---------------------|--------------------------------------------|
| Save              | SaveManager         | In-memory progress; no persistence required |
| Narrative         | NarrativeManager    | Story beats; DialogueSystem builds UI      |
| Dialogue          | DialogueSystem      | Requires Canvas; bootstrap creates one     |
| Music             | MusicManager        | No clips = silent                          |
| Map               | WorldMapManager     | Empty locations until level data added     |
| Combat UI         | CombatUI            | Minimal Canvas + CombatUI                  |
| Game State        | GameStateManager    | Victory/defeat panels                      |
| Illumination      | IlluminationManager| Optional victory/defeat flash              |
| Aging             | AgingManager        | Optional for short test                    |

All are the real types (no stub classes), so `FindFirstObjectByType<T>()` and `GameRuntime` see them. Replace or configure them in the scene as you add full content.

# GameDev.tv Unity 2D RPG Combat — Learning & Implementation Plan

## Course Overview

**Source:** [GameDev.tv Unity 2D RPG: Complete Combat System](https://www.gamedev.tv/courses/unity-2d-rpg-combat)  
**Repo:** [GitLab — unity-2d-rpg-combat-course](https://gitlab.com/GameDevTV/unity-2d-rpg-combat/unity-2d-rpg-combat-course)

The course builds a **2D top-down** RPG with **real-time** combat (sword swinging, knockback, dash). KyndeBlade is **side-view turn-based** with parry/dodge zones. This doc maps course content to KyndeBlade and identifies what to learn vs. implement.

---

## Course Structure vs. KyndeBlade

| Course Section | Course Content | KyndeBlade Equivalent | Implement? |
|----------------|----------------|------------------------|------------|
| **Player Movement** | Input, sprite, animation, physics | Map navigation, turn-based selection | ❌ Different model |
| **Combat** | Sword swing, collision, knockback, dash | TurnManager, CombatAction, parry/dodge | ⚠️ Concepts only |
| **Enemy State** | State machine, health, death | MedievalCharacter, CharacterStats | ✅ Study patterns |
| **Damage/Death FX** | Flash, death effects | CombatFeedback, IlluminationManager | ✅ Already aligned |
| **Environment** | Cinemachine, tilemap, parallax | Orthographic camera, sprites | ⚠️ Parallax possible |
| **Scene Management** | Portals, fade transitions | MapLevelSelectUI, scene loading | ✅ Fade transitions |
| **Scriptable Objects** | Weapons, items | CombatAction, EncounterConfig | ✅ Already used |

---

## Learning Path

### Phase 1: Clone & Study (No KyndeBlade Changes)

1. **Course repo** (already at `docs/reference/gamedevtv-2d-rpg-combat/`):
   ```bash
   # Or clone fresh:
   git clone https://gitlab.com/GameDevTV/unity-2d-rpg-combat/unity-2d-rpg-combat-course.git
   ```

2. **Key scripts to study** (in `Assets/Scripts/`):
   - `Scene Management/UIFade.cs` — fade to black/clear (adapted as `FadeTransition` in KyndeBlade)
   - `Scene Management/SceneManagement.cs` — singleton for scene state
   - `Scene Management/AreaExit.cs`, `AreaEntrance.cs` — portal/transition flow
   - `Enemies/EnemyAI.cs` — enemy state machine
   - `Misc/Flash.cs` — damage flash (material swap; KyndeBlade uses color tint)
   - `Misc/SpriteFade.cs` — sprite alpha fade
   - `Weapons/` — collision, damage (real-time; not applicable to turn-based)

3. **Watch the course** on GameDev.tv for context (paid course; repo is reference).

### Phase 2: Transferable Concepts (KyndeBlade-Aligned)

| Concept | Course Approach | KyndeBlade Implementation |
|---------|-----------------|---------------------------|
| **Fade transitions** | SceneManager + CanvasGroup fade | Add `FadeTransition` for map→combat, combat→map |
| **Singleton pattern** | Persistent scene manager | KyndeBladeGameManager, SaveManager already central |
| **Enemy state machine** | Chase, Attack, Dead states | AI scripts (SimpleEnemyAI, boss AIs) — refine state flow |
| **Scriptable Object design** | Weapons, stats as SOs | Extend CombatAction, StatusEffect patterns |

### Phase 3: Implementation Priorities (Original Vision)

Keeping manuscript + Pre-Raphaelite + 16-bit vision:

1. **Fade transitions** — Manuscript-style fade (parchment darken) between map and combat.
2. **Polish existing feedback** — CombatFeedback, IlluminationManager already match vision; no action-game FX.
3. **Scene flow** — Cleaner map→combat→map transitions using course-style fade.

---

## What NOT to Implement

- Real-time sword swinging / collision detection
- Knockback physics
- Dash ability
- Top-down movement
- Cinemachine (KyndeBlade uses fixed orthographic)
- Ragdolls or 3D death effects

---

## Implemented: FadeTransition

A manuscript-style fade overlay (`Assets/Scripts/UI/FadeTransition.cs`) has been added, adapted from the course's `UIFade.cs`:

- Uses `ManuscriptUITheme.InkPrimary` for fade-to-dark (manuscript ink)
- `FadeToDark(onComplete)` / `FadeToClear(onComplete)` for transitions
- Add to a persistent GameObject (e.g. KyndeBladeGameManager) and call from `WorldMapManager.TransitionTo` or combat start/end

**Wiring example** (in WorldMapManager or GameStateManager):
```csharp
var fade = FindObjectOfType<FadeTransition>();
if (fade == null) { var go = new GameObject("FadeTransition"); fade = go.AddComponent<FadeTransition>(); }
fade.FadeToDark(() => { /* start combat */ fade.FadeToClear(); });
```

---

## Next Steps

1. Course repo is at `docs/reference/gamedevtv-2d-rpg-combat/`.
2. Open it in Unity and run through the project.
3. Read the key scripts listed in Phase 1.
4. Wire `FadeTransition` into map↔combat flow when ready.
5. Optionally refine AI state patterns based on course enemy logic.

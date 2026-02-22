# Data-Driven Combat Actions (Phase 4)

Combat actions are already **data-driven** via ScriptableObjects. No hardcoded action logic in TurnManager or character code for defining new moves.

---

## Current Architecture

- **CombatAction** (`KyndeBlade.Combat`) is a **ScriptableObject** with **CombatActionData**:
  - **ActionType** — Strike, RangedStrike, Heal, Rest, Escapade, Ward, Counter, Special
  - **Damage** — Base damage (or heal amount)
  - **StaminaCost**, **KyndeCost**, **KyndeGenerated**
  - **BreakDamage**, **SuccessWindow**, **CastTime**, **ExecutionTime**
  - **ActionName** — Shown in UI

- **MedievalCharacter** holds a **list of CombatAction** (`AvailableActions`). Assign assets in the Inspector or via movesets (e.g. Expedition33Moveset, DefaultCombatActions.AddDefaultsTo).

- **TurnManager** and **CombatUI** iterate over `CurrentCharacter.AvailableActions`; they do not hardcode which actions exist. New moves = new ScriptableObject assets (Create → KyndeBlade → Combat Action).

---

## Tests vs Runtime

- **CombatMechanicsTest** and other Play Mode tests use **DefaultCombatActions.CreateStrike()** (and similar) to build **runtime** instances of CombatAction so tests don’t depend on asset files. That’s test-only.
- **Runtime game** uses **ScriptableObject assets** assigned to prefabs or added by movesets. No C# changes needed to add a new move—create an asset and assign it.

---

## Adding a New Move (No New C# Code)

1. In Project: **Right‑click → Create → KyndeBlade → Combat Action**.
2. Set **ActionType**, **Damage**, **StaminaCost**, **ActionName**, etc. in the Inspector.
3. Assign the asset to a character prefab’s **Available Actions** list, or use a moveset that adds it in code (e.g. Expedition33Moveset).

For custom behaviour (e.g. “hit all enemies”), create a **subclass** of CombatAction, override **ExecuteAction**, and create a ScriptableObject from that class. See **COMBAT_ACTION_SCRIPTABLEOBJECT.md**.

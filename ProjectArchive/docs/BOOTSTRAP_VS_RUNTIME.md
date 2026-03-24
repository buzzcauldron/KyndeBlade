# Bootstrap vs Runtime — Tests Don’t Run the Game

The game **does not** rely on `TestGameBootstrap` to run. That class exists only for **Play Mode tests** (NUnit). The real game uses **KyndeBladeGameManager** and normal Unity scenes.

---

## 1. Where the “bootstrap” really lives (runtime)

| Purpose | Used by | Location |
|--------|--------|----------|
| **Real game bootstrap** | Playing the game in Editor or build | `KyndeBladeGameManager` |
| **Test-only setup** | Play Mode tests only | `TestGameBootstrap` |

- **KyndeBladeGameManager** (`Assets/_Project/Code/Combat/KyndeBladeGameManager.cs`): In `Start()` it finds or creates `TurnManager`, runs `EnsureCombatPipeline()` and `EnsureMapPipeline()`, then either starts the map or auto-spawns a test party. When you press Play in a scene that has this component, the game runs without any test code.
- **TestGameBootstrap** (`Assets/Tests/PlayMode/TestGameBootstrap.cs`): Creates `TurnManager`, test characters, `SaveManager`, etc. **only** so that `CombatMechanicsTest`, `AgingPovertyTest`, `MapSaveTest` can run in the Test Runner. If you never open the Test Runner or run those tests, this code is never executed.

So: **the Editor “load and play” path does not use TestGameBootstrap.** It uses the scene (e.g. a scene with `KyndeBladeGameManager` and optional map/combat setup).

---

## 2. Where the “sword swing” actually happens (runtime)

The flow is:

1. **TurnManager** — Decides whose turn it is and calls `ExecuteAction(action, target)`.
2. **TurnManager** (inside `ProcessActionCast()`) — Calls `CurrentCharacter.ExecuteCombatAction(_currentAction, _currentTarget)`.
3. **MedievalCharacter.ExecuteCombatAction** — Calls `action.ExecuteAction(this, target)`.
4. **CombatAction.ExecuteAction** (ScriptableObject) — Applies damage/heal/break (e.g. `target.ApplyCustomDamage(...)`).
5. **MedievalCharacter.ApplyCustomDamage** — Reduces health, fires events, handles defeat.

So the “sword swing” is: **TurnManager → MedievalCharacter.ExecuteCombatAction → CombatAction.ExecuteAction → MedievalCharacter.ApplyCustomDamage.**

### File locations (after _Project refactor)

- **TurnManager**: `Assets/_Project/Code/Combat/TurnManager.cs`
- **MedievalCharacter**: `Assets/_Project/Code/Combat/Characters/MedievalCharacter.cs`
- **CombatAction** (Strike, Rest, etc.): `Assets/_Project/Code/Combat/CombatAction.cs`

---

## 3. Strike: tests vs real game

- **In tests**: `DefaultCombatActions.CreateStrike()` creates a **runtime** `CombatAction` instance (ScriptableObject.CreateInstance) so tests don’t need assets. That’s only for the test assembly.
- **In the real game**: Strike (and every other action) should be a **ScriptableObject asset** (Create → KyndeBlade → Combat Action), assigned to characters’ **Available Actions** in the Inspector or by movesets (e.g. Expedition33Moveset). No test code involved.

So: **Strike is already a ScriptableObject.** The tests use a helper that creates one in code; the game uses assets.

---

## 4. Making a scene that “loads and plays” without tests

1. Create a scene (e.g. `CombatTest`).
2. Add an empty GameObject, add **KyndeBladeGameManager**.
3. Optionally add **TurnManager** on another GameObject (or let GameManager create it).
4. Assign your **player and enemy prefabs** on the GameManager (WillePrefab, FalsePrefab, etc.).
5. Press Play — GameManager will spawn characters and start combat; no TestGameBootstrap involved.

If the Editor “won’t load and play,” the cause is usually missing scene setup or missing prefab references, not the test bootstrap. Check that a scene has `KyndeBladeGameManager` and that required references are assigned.

---

## 5. Don’t delete the tests

Keep `TestGameBootstrap` and the Play Mode tests. Use them once the game is loading and you want to lock in behaviour. For “get the Editor to work,” focus on the runtime scripts above and scene/prefab setup.

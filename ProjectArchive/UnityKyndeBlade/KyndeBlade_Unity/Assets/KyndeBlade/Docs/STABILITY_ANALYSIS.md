# Stability analysis: why major issues keep appearing

Review of project history and code patterns that lead to breakages (compiler errors, runtime exceptions, and fragile behavior). Intended to guide fixes and prevent recurrence.

---

## 1. Root causes (summary)

| Cause | What happens | Recent examples |
|-------|----------------|------------------|
| **Scattered resolution** | Many scripts use `FindFirstObjectByType<T>()` or `GameRuntime.X` to get managers. Order and scene setup are implicit; one missing object or wrong execution order → null ref or wrong ref. | CombatUI, ParryDodgeInputHandler, GameStateManager, etc. resolving TurnManager; GameRuntime filled in Awake and used everywhere. |
| **Single bootstrap doing everything** | `KyndeBladeGameManager` creates TurnManager, combat pipeline (EventSystem, CombatUI, CombatFeedback, …), map pipeline (SaveManager, MusicManager, WorldMapManager, …), and registers all in `GameRuntime`. Order is fragile; adding/changing systems is risky. | Changing EventSystem to InputSystemUIInputModule broke UI; reverting required project-setting change. |
| **Half-migrations** | Changing one part of the stack (e.g. “Input System only”) without updating every consumer (e.g. EventSystem still using legacy Input) causes runtime errors. | `InvalidOperationException`: Input System only + StandaloneInputModule → `Input.get_mousePosition` fails. |
| **Static global state** | `GameRuntime` holds static refs to managers. Scene reloads or multi-scene flows can leave stale or null refs; nothing clears the registry on unload. | Any script using `GameRuntime.CombatUI` or similar depends on one Awake having run and never being unloaded. |
| **Duplicate / copy-paste logic** | Same behavior implemented in more than one place (e.g. two `Update()` methods) leads to compiler errors or subtle bugs when only one path is updated. | `CombatUI` had two `Update()` → CS0111; merge fixed it. |
| **Package / engine coupling** | Assembly names or project settings interact badly with Unity or package tooling. | Burst couldn’t resolve `KyndeBlade.Editor` → renamed to `KyndeBlade.EditorTools`. |
| **Missing or narrow checks** | Changes are made without running full compile, tests, or play-through in the right configuration. | Input System–only + UI wasn’t exercised before commit; EventSystem broke in play. |

---

## 2. Project history (relevant to stability)

- **Unreal → Unity**: Repo merged from Unreal 5.7; Unity project lives in `KyndeBlade_Unity/`. Some patterns (e.g. “one game manager”) may come from that transition.
- **Big feature commits**: “Combat UI/input, enemy AI, Archer/Rogue, Guard, XP, game flow” and “Unity layout, visual pipeline, 16-bit” introduced a lot of systems at once; bootstrap and dependencies grew without a single clear contract.
- **Recent fixes (this conversation)**:
  - Duplicate `Update()` in `CombatUI` (CS0111).
  - Input: ParryDodgeInputHandler migrated to new Input System; then project set to “Input System only” → EventSystem broke → reverted to “Both” and reverted EventSystem to StandaloneInputModule.
  - `GamepadButton` not found → added `UnityEngine.InputSystem.LowLevel` (correct namespace).
  - Burst resolution failure for `KyndeBlade.Editor` → assembly renamed to `KyndeBlade.EditorTools`.
  - Stray `using UnityEngine.InputSystem.UI` in GameManager after reverting Input System UI module.

So instability is partly **incremental change** (Input, assemblies, bootstrap) without **full validation** and without **centralizing** who creates what and who resolves what.

---

## 3. Fragile patterns in code

### 3.1 Bootstrap and resolution

- **KyndeBladeGameManager.Awake** (DefaultExecutionOrder -1000):
  - Resolves or creates TurnManager.
  - Calls `EnsureCombatPipeline()` (EventSystem, CombatUI, CombatFeedback, GameStateManager, IlluminationManager, …).
  - Calls `EnsureMapPipeline()` (SaveManager, MusicManager, AgingManager, PovertyManager, NarrativeManager, DialogueSystem, WorldMapManager, MapLevelSelectUI).
  - Calls `RegisterGameRuntime()` which does `FindFirstObjectByType<>` for all of the above and assigns to `GameRuntime`.

So: **creation order is fixed in one place, but “who is allowed to use what and when” is not**. Any script that runs before or without relying on this Awake can see nulls. CombatUI and others also do their own `FindFirstObjectByType<TurnManager>()` in Start/Update, which duplicates the resolution contract and makes it easy for timing to diverge.

### 3.2 GameRuntime

- **GameRuntime** is a static registry set once in Awake. It is never cleared. If you load another scene or reload the play scene, old references can remain. Nothing enforces “only one active set of managers.”

### 3.3 Input

- Project is on **Both** (legacy + new Input System). ParryDodgeInputHandler uses **new** API; EventSystem uses **legacy** (StandaloneInputModule). So we depend on a specific project setting (`activeInputHandler: 2`). Moving to “Input System only” again would require migrating EventSystem (and any other legacy Input usage) and testing all UI and combat input.

### 3.4 EventSystem

- EventSystem is created in code with `StandaloneInputModule`. That ties stability to:
  - Project setting = Both.
  - No other EventSystem in the scene using a different module in a way that conflicts.

---

## 4. Recommendations (in order of impact)

1. **Document and enforce bootstrap order**
   - In ARCHITECTURE.md (or STABILITY_ANALYSIS.md), write: “KyndeBladeGameManager.Awake (order -1000) creates/wires X, Y, Z in this order; all other scripts must assume TurnManager/CombatUI/… are available from Start() onward and resolve via GameRuntime or inspector, not FindObjectByType in Update.”
   - Prefer **injecting** refs (inspector or GameRuntime set in Awake) over repeated Find in Start/Update.

2. **Reduce FindFirstObjectByType in hot paths**
   - Resolve once (e.g. in Start or from GameRuntime) and cache. Avoid resolving TurnManager or other managers in Update() every frame.

3. **Stabilize Input in one of two ways**
   - **Option A (current)**: Keep “Both”; document that UI uses legacy Input and combat uses new Input System; add a one-line note in ProjectSettings or README so no one flips to “Input System only” without migrating EventSystem.
   - **Option B**: Migrate EventSystem to InputSystemUIInputModule, then switch to “Input System only” and run full UI + combat test pass.

4. **Lifecycle for GameRuntime**
   - On scene unload (or when shutting down combat), clear or null `GameRuntime` refs so the next scene doesn’t see stale managers. Optionally make GameRuntime re-resolve on first access after clear.

5. **Pre-commit checks**
   - Enforce stability-checks.mdc: after any change, run linter on touched files and, for Unity, a clean compile and a short play in the main scene (with “Both” and the usual scene setup). If the project moves to “Input System only,” add a check that the main menu and combat UI both receive input.

6. **Split bootstrap if it keeps growing**
   - If more systems are added, consider separate “CombatPipeline” and “MapPipeline” components or a small bootstrap pipeline (list of “ensure X, then Y”) so order is explicit and testable, instead of one large Awake.

---

## 5. Small fix applied with this report

- **KyndeBladeGameManager.cs**: Removed unused `using UnityEngine.InputSystem.UI;` (leftover from the reverted InputSystemUIInputModule change). This avoids unnecessary dependency and confusion.

---

## References

- **ARCHITECTURE.md**: Object lifecycle, performance, organizing architecture, KISS.
- **.cursor/rules/stability-checks.mdc**: Require linter, Unity compile, and tests after changes.
- **.cursor/rules/unity-*.mdc**: KISS, assembly definitions, organizing architecture.

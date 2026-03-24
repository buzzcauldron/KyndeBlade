# Combat input (dodge / parry / counter)

## Current setup

- **Runtime**: [`ParryDodgeInputHandler`](../Code/Combat/UI/ParryDodgeInputHandler.cs) polls the **Unity Input System** (`Keyboard` / `Gamepad`) each frame when `TurnManager` is in a real-time defense window or counter window.
- **Contract**: [`ICombatWindowInput`](../Code/Core/Input/ICombatWindowInput.cs) defines the three boolean polls: dodge, parry, counter (same frame semantics as `wasPressedThisFrame`).

## Tests and automation

- Assign **`ParryDodgeInputHandler.InputSourceOverride`** to any implementation of `ICombatWindowInput` (e.g. a small scripted double in Play Mode tests). When non-null, hardware polling is skipped.
- See [`GameCompletenessPlayModeTests`](../../Tests/PlayMode/GameCompletenessPlayModeTests.cs) under `Assets/Tests/PlayMode/` for an example.

## Future: `.inputactions` asset

1. Add a **Input Actions** asset (`Create > Input Actions`) with maps for **Combat** (Dodge, Parry, Counter).
2. Generate the C# wrapper class (optional) or use `InputAction` references on a small `MonoBehaviour`.
3. Implement `ICombatWindowInput` on that behaviour (read `WasPressedThisFrame()` from the bound actions).
4. At runtime, assign that instance to `ParryDodgeInputHandler.InputSourceOverride`, or merge the polling into the handler behind a single code path.

## Rebinding (future)

- Persist user overrides with `PlayerPrefs` or save data; reload bindings when the Input Actions asset is enabled.
- Add Edit Mode or Play Mode tests that serialize/deserialize binding overrides and assert the correct `ICombatWindowInput` behaviour.

## Project settings

- Player **Active Input Handling** must remain compatible with UI (see [ARCHITECTURE.md](../ARCHITECTURE.md) → Input). Do not switch to **Input System package only** until EventSystem uses `InputSystemUIInputModule`.

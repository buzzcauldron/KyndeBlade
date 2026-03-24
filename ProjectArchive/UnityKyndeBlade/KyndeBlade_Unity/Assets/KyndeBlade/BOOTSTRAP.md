# Bootstrap and scene requirements

- **Main play scene** must contain one **KyndeBladeGameManager** (with or without prefab references; TurnManager and pipelines are created in code).
- The main scene (e.g. `Assets/Scenes/Main.unity`) must be **first in Build Settings** so the game manager is present from frame zero.
- Optionally include a Main Camera; otherwise `EnsureCombatCamera()` creates one at runtime.

Do not rely on multiple scenes or additive load for bootstrap unless documented; use a single main scene that runs `KyndeBladeGameManager.Awake()` first.

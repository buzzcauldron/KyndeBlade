# Code architecture (Unity guidance)

This project follows [Unity’s advanced programming and code architecture](https://unity.com/how-to/advanced-programming-and-code-architecture) and [how to architect code as your project scales](https://unity.com/how-to/how-architect-code-your-project-scales) where applicable. Cursor rules: **unity-kiss** (keep it simple; use patterns when they fit) and **unity-assembly-definitions** (modular asmdefs, one-way dependencies).

## Object lifecycle

- **Destroy at runtime**: Use `Destroy()` for runtime cleanup (e.g. materials, temporary objects). Use `DestroyImmediate()` only in Editor code paths (e.g. `if (!Application.isPlaying)`).
- **Unity null checks**: Use `if (obj == null)` for `UnityEngine.Object` so destroyed objects are treated as null; avoid `ReferenceEquals` for Unity objects.
- **Static refs**: Avoid strong static references to large or scene-specific assets to prevent leaks across scene loads. Static refs to small built-in resources (e.g. `Resources.GetBuiltinResource<Font>`) are acceptable when documented.

## Performance and memory

- Avoid reflection in hot paths.
- Do not add C# finalizers in runtime code (GC overhead, non-deterministic).
- Minimize allocations in Update/fixed paths; reuse buffers or pools where it helps.

## Organizing architecture

Principles from [Organizing architecture for games on Unity](https://dev.to/devsdaddy/organizing-architecture-for-games-on-unity-laying-out-the-important-things-that-matter-4d4p):

- **Separation of concerns**: Each component does one clear task; reduces dependencies and eases testing.
- **Modularity**: Parts are independent and replaceable so the system can adapt to changing requirements.
- **Readability**: Clear names, logical blocks, and light documentation so the code is easy to understand and change.
- **Don’t over-complicate**: Prefer the simplest approach that works; add structure when duplication or coupling becomes a real problem.

Unity is component-oriented; adapt patterns (MVC/MVVM, Observer, Command, DI, Pub/Sub) to the engine rather than forcing a single architecture. Use **Observer** or **Pub/Sub** to decouple producers and consumers; use **Command** when you need undo/redo or queued actions; consider **Dependency Injection** to make dependencies explicit and testable. Cursor rule: **unity-architecture-organizing**.

## Bootstrap

- **KyndeBladeGameManager** runs first (`DefaultExecutionOrder(-1000)`). In **Awake** it:
  1. Resolves or creates **TurnManager**
  2. **EnsureCombatPipeline()**: EventSystem (StandaloneInputModule), CombatUI, CombatFeedback, CombatFeedbackManager, GameStateManager, IlluminationManager
  3. **EnsureMapPipeline()**: SaveManager, MusicManager, AgingManager, PovertyManager, NarrativeManager, DialogueSystem, WorldMapManager, MapLevelSelectUI
  4. **RegisterGameRuntime()**: assigns all of the above to the static **GameRuntime** registry
- **Rule for other scripts**: Rely on **inspector refs** or **GameRuntime** for managers. Resolve once (e.g. in Start or first use), not every frame in Update. Prefer `GameRuntime.TurnManager` (or similar) over `FindFirstObjectByType<T>()` when the bootstrap has already run.
- **Lifecycle**: GameRuntime is cleared when KyndeBladeGameManager is destroyed (e.g. scene unload) so the next scene does not see stale refs.

## Input

- **Project setting**: **Active Input** must stay **Both** (legacy + new Input System) until the EventSystem is migrated. The built-in UI (EventSystem, StandaloneInputModule) uses the legacy `UnityEngine.Input` API; switching to "Input System package only" without replacing StandaloneInputModule with InputSystemUIInputModule causes runtime errors. Combat input (e.g. ParryDodgeInputHandler) uses the new Input System; UI continues to use legacy for now.

## Design

- Prefer **KISS**: use patterns (singleton, observer, factory) only when they fit the problem.
- Bootstrap and manager wiring are centralized in `KyndeBladeGameManager`; **GameRuntime** is the fallback registry after Awake.

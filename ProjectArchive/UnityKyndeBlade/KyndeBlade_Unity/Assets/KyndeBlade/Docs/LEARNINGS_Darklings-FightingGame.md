# Learnings from Darklings-FightingGame

Review of [kidagine/Darklings-FightingGame](https://github.com/kidagine/Darklings-FightingGame) (development branch) — a 2D fighting game with rollback netcode, deterministic simulation, and accessible design. Summary of patterns we can reuse or adapt in KyndeBlade.

---

## Game structure (overview)

How the game is organized: layers, bootstrap, and data flow.

### Folder layout (`Assets/_Project/Scripts/`)

| Folder | Role |
|--------|------|
| **NetworkScripts** | Game manager, runners, GGPO. `GameManager` (abstract), `SimulationManager` (concrete), `LocalRunner` / `GGPORunner` (`IGameRunner`), `GameInterface`, `ConnectionWidget`, `Interfaces.cs` (IGame, IGameRunner, IGameView). |
| **SimulationScripts** | Authoritative sim and view bridge. `GameSimulation` (struct, `IGame`), `GameSimulationView` (MonoBehaviour, `IGameView`), `StateSimulation`, `State` + all state classes, `ProjectilesSimulation`, input structs. |
| **ManagerScripts** | Scene-level singletons. `GameplayManager` (match setup, UI, hitstop, round flow), `InputManager`, `ReplayManager`, `ObjectPoolingManager`, `SoundManager`, `Singleton` base. |
| **PlayerScripts** | Per-player view and control. `Player`, `PlayerSimulation`, `PlayerController` / `CpuController` / `BrainController`, `PlayerAnimator`, `PlayerMovement`, `PlayerUI`, `Assist`. |
| **ScriptableObjectScripts** | Data assets. `AttackSO`, `ArcanaSO`, `PlayerStatsSO`, `StageSO`, `DialogueSO`, etc. |
| **CustomScripts** | Deterministic world. `DemonicsWorld` (frame counter, `WaitFramesOnce`), `DemonicsPhysics`, `DemonicsCollider`, `DemonicsAnimator`, etc. |
| **EnumScripts** | Enums (attack types, input, controller, etc.). |
| **UIScripts** | Menus, prompts, intro, training UI, replay cards, etc. |

Scenes: `0. UIScene.unity` (menus), `1. GameScene.unity` (match). Data: `SciptableObjects/` (character, attack, stage, assist, etc.).

### Three layers: Network runner → Simulation → View

1. **Runner (who ticks the sim)**  
   - **GameManager** (MonoBehaviour, singleton): holds `IGameRunner Runner`. In `Update()` it calls `Runner.RunFrame()` at 60 FPS (wall-clock throttled via `next` and `Utils.TimeGetTime()`). Fires `OnStateChanged` after each RunFrame (not heavily used; view uses its own Update).  
   - **SimulationManager** extends `GameManager`: `StartLocalGame()` creates `new GameSimulation(...)` and `new LocalRunner(game)`, then `StartGame(runner)`. `StartGGPOGame(...)` does the same with `GGPORunner`. So the “game” is created at match start and injected into the runner.  
   - **LocalRunner** / **GGPORunner** implement `IGameRunner`: they own `IGame Game` and each frame call `Game.ReadInputs(controllerId)` per player, then `Game.Update(inputs, disconnectFlags)`. So the only thing that advances the simulation is `Game.Update` called from the runner inside `GameManager.Update`.

2. **Simulation (authoritative state)**  
   - **GameSimulation** is a **struct** implementing **IGame**. It holds `Frames`, `_players` (PlayerNetwork[]), static `Hitstop`, `Timer`, `Run`, etc.  
   - **GameSimulation.Update(long[] inputs, int disconnectFlags)**: applies inputs to both players, then for each player calls `StateSimulation.SetState(player)` and `player.CurrentState.UpdateLogic(player)`, runs physics, hitboxes, projectiles, and decrements `Hitstop`. No MonoBehaviours; pure data and logic.  
   - **PlayerNetwork** holds position, velocity, state name, current State instance, hitbox/hurtbox, input buffer, etc. **GameSimulation._players[i].player** is a reference to the **view** `Player` (MonoBehaviour), set later when the view is created.

3. **View (read-only reflection of sim)**  
   - **GameSimulationView** (MonoBehaviour, **IGameView**): in **Update()**, if `gameManager.IsRunning`, calls **UpdateGameView(gameManager.Runner)**. That casts `runner.Game` to `GameSimulation`, reads `GameSimulation._players`, and for each player calls **playerViews[i].PlayerSimulation.Simulate(playersGss[i], gameInfo.players[i])**. So the view runs in Unity’s Update loop and only **reads** sim state; it never writes back.  
   - **PlayerSimulation.Simulate(PlayerNetwork, PlayerConnectionInfo)**: pushes network state into the view: position, animation, sounds, UI (arcana, combo), input buffer display, hit/hurt/push box visualizers. So **Player** (MonoBehaviour) is the visible character; **PlayerNetwork** is the sim state; the link is `GameSimulation._players[i].player = PlayerOne/PlayerTwo` set in **GameplayManager.SetupGame()**.

### Bootstrap and match start

1. **GameScene** loads with **GameplayManager** (singleton), **SimulationManager** (extends GameManager), **GameSimulationView**, **DemonicsWorld**, UI, etc.  
2. **GameplayManager.Awake** / init: sets **SceneSettings** from inspector or from menu (characters, stage, controllers, training mode, etc.).  
3. **Starting a match** (local): e.g. **ConnectionWidget** or **ConnectionPanel** calls `GameManager.Instance.StartLocalGame()`. **SimulationManager.StartLocalGame()** builds `new GameSimulation(GameplayManager.Instance.GetPlayerStats(), GetAssistStats())` and `new LocalRunner(game)`, then `StartGame(runner)`. So **GameplayManager** provides data (SOs); **SimulationManager** creates the sim and runner and assigns the runner to **GameManager**.  
4. **GameManager.Update** (every frame): `Runner.RunFrame()` → **LocalRunner** gets inputs via `Game.ReadInputs(controllerId)` (which reads from **PlayerController** / input buffer), then **Game.Update(inputs, 0)** → **GameSimulation.Update** advances one tick.  
5. **GameSimulationView.Update** (same or next frame): reads `runner.Game` (the same **GameSimulation**), calls **UpdateGameView** → **PlayerSimulation.Simulate** for each player, so the on-screen **Player** and UI reflect the sim.  
6. **First-time setup**: When **UpdateGameView** sees `GameSimulation.Start == true`, it calls **GameplayManager.Instance.SetupGame()**, which assigns **PlayerOne** / **PlayerTwo** to **GameSimulation._players[0/1].player**, starts intro or training round, and sets **GameSimulation.Run = true**. So the link from sim to view is established in **SetupGame**; before that, **GameSimulationView** may instantiate **Player** prefabs and call **GameplayManager.InitializePlayers**.

### Who owns what

- **GameManager** (SimulationManager): owns **Runner**; drives **RunFrame()** every frame. Does not hold match settings or UI.  
- **GameplayManager**: owns match setup (spawn positions, stage, characters, training flags), UI references, round flow (intro, round over, training), **hitstop list** and **RunHitStop()**, and wires **GameSimulation._players[i].player** to **PlayerOne** / **PlayerTwo**. Does **not** run the sim tick.  
- **GameSimulationView**: owns **playerViews** (Player instances); each frame pulls from **GameSimulation** and pushes to **PlayerSimulation**.  
- **Input**: Controllers (BrainController, etc.) feed into input buffers; **Game.ReadInputs(controllerId)** reads from **GameSimulation._players**’ input lists / buffers and returns a packed **long** for the runner to pass into **Game.Update**.

### Takeaway for KyndeBlade

- **Clear split**: “Runner” (what calls the sim tick) → “Simulation” (authoritative state, no MonoBehaviour) → “View” (MonoBehaviour that only reads sim and updates scene/UI). We already have a similar split (TurnManager / combat state vs CombatUI / feedback).  
- **Single entry to start a match**: One method (`StartLocalGame` / `StartGGPOGame`) creates the sim and runner and assigns the runner; after that, the runner is the only thing that calls the sim. Our **KyndeBladeGameManager** bootstrap is analogous: one place that wires services and starts the combat flow.  
- **GameplayManager as scene host**: Holds settings, UI, and match flow; does not own the sim tick but owns “when round starts/ends” and “wire sim players to view objects.” We can think of our **KyndeBladeGameManager** (or a dedicated CombatSceneController) as the same role: scene-level host and wiring, with TurnManager/combat logic as the “sim” and CombatUI as the view.

---

## 1. Deterministic simulation & 60 FPS logic tick

- **Fixed timestep**: Game logic runs on a **frame counter** (`Frames`, `FramesStatic`, `DemonicsWorld.Frame`) at 60 FPS (`Time.fixedDeltaTime = 0.01667f`, `Application.targetFrameRate = 60`). This is required for rollback and replay.
- **Simulation vs view**: `GameSimulation` holds the authoritative state (positions, velocities, states); the view (`GameSimulationView`) reads it and updates transforms/animations. Our project is turn-based, but the split between “simulation tick” and “presentation” is still a useful mental model for any future real-time or replay features.
- **Serialization**: `GameSimulation` implements `Serialize`/`Deserialize` (and `ToBytes`/`FromBytes`) so full game state can be saved for rollback and replay.

**Takeaway**: For any future deterministic or replay-sensitive logic, drive it from a fixed frame counter and keep a clear simulation/view boundary.

---

## 2. State machine (state-per-class, string name, factory)

- **State base**: `State` is a plain C# class (not MonoBehaviour) with:
  - `UpdateLogic(PlayerNetwork player)` — called every sim tick
  - `Exit(PlayerNetwork player)` — when leaving the state
  - `ToHurtState` / `ToBlockState` — overridable collision responses
  - `EnterState(player, stateName)` — sets `player.state` (string) and uses `StateSimulation.SetState(player)` to resolve to the correct state instance.
- **State factory**: `StateSimulation.SetState(player)` is a large switch/if chain mapping `player.state` (e.g. `"Idle"`, `"Attack"`, `"Hurt"`) to concrete state types (`IdleState`, `AttackState`, …). No dictionary; just string → type.
- **Concrete states**: e.g. `IdleState` extends `GroundParentState`, overrides `UpdateLogic`, and calls `EnterState(player, "Walk")` etc. when transitions are needed. Shared logic (e.g. blocking, flip) lives in base `State` or parent states like `GroundParentState`, `HurtParentState`.

**Takeaway**: A single “current state” object plus a central resolver (string or enum → type) keeps the state graph explicit and easy to debug. Our turn/combat flow could adopt a similar “state object + name” pattern if it grows.

---

## 3. Hitstop: frame-based, per-entity interface, list in manager

- **Global frame hitstop**: Simulation uses `GameSimulation.Hitstop` (int frames). When hit is detected, `GameSimulation.Hitstop = attack.hitstop`; each tick it’s decremented; when `Hitstop <= 0`, entities’ `hitstop` flags are cleared so movement/animations resume.
- **View hitstop**: A separate “view” hitstop runs in `GameplayManager`: `_hitstop` (frames) and `_hitstopList` (list of `IHitstop`). Each frame, `RunHitStop()` calls `DemonicsWorld.WaitFramesOnce(ref _hitstop)` (decrement; when 0, exit). When it reaches 0, every registered `IHitstop` gets `ExitHitstop()` and the list is cleared.
- **IHitstop**:
  - `EnterHitstop()` — freeze this entity (e.g. stop movement/anim)
  - `ExitHitstop()` — resume
  - `IsInHitstop()` — query
- **Registration**: Players (and any other freezable object) implement `IHitstop` and register with `GameplayManager.AddHitstop(this)`. When `HitStop(frames)` is called, all registered objects get `EnterHitstop()`; when the frame count hits 0, all get `ExitHitstop()`.

**Takeaway**: For frame-locked or deterministic games, hitstop as **integer frames** plus a **list of IHitstop** lets you freeze only the right entities (players, projectiles) and keep simulation and view in sync. Our current `HitStop` uses real time and `Time.timeScale`; that’s fine for single-player feel. If we ever add frame-based combat or replay, an `IHitstop`-style interface and a central “hitstop in frames” driver would align with Darklings.

---

## 4. Frame-based “wait once” helper

- **WaitFramesOnce(ref int frames)**:
  - Decrements `frames` by 1.
  - Returns `true` only when `frames` reaches **exactly 0** (one-shot).
  - Used for hitstop, intro timing, dialogue, round-over sequences, etc.
- **WaitFrames** (not “once”): same decrement but returns true when `frames <= 0` (repeatable).

**Takeaway**: A small static helper `WaitFramesOnce(ref int)` is a clean way to do “wait N frames then do something once” in a fixed-timestep loop. We could add something like this if we introduce a 60 FPS combat tick or scripted sequences in frames.

---

## 5. Input buffer with expiry

- **InputBufferItem**: Holds `InputEnum`, `timestamp` (frame), `priority`, and optional `Execute`. `CheckIfValid()` returns true if `timestamp + _timeBeforeActionsExpire >= DemonicsWorld.Frame` (e.g. 20-frame window).
- Input is stored with the current frame so that rollback can re-apply the same inputs; expiry prevents old buffered actions from firing forever.

**Takeaway**: Buffering input with a frame timestamp and an expiry window is standard for fighters and rollback. Our real-time reaction windows (parry/dodge) are different, but the idea of “input + frame number + validity window” is reusable for any future input-sensitive or combo system.

---

## 6. ScriptableObjects for data

- **AttackSO**, **ArcanaSO**, **PlayerStatsSO**, **DialogueSO**, **StageSO**, **CameraShakerSO**, etc. hold design data (damage, hitstop, sounds, framedata). The simulation uses network-friendly structs (e.g. `AttackNetwork`) built from these SOs when entering an attack state.
- **Hitstop on attack**: Stored per attack in `AttackSO` (`hitstop` field) and copied to `AttackNetwork`; on hit, `GameSimulation.Hitstop = player.attackHurtNetwork.hitstop`.

**Takeaway**: We already use ScriptableObjects for data; Darklings reinforces “SO for design data, simple structs for runtime/simulation” and “hitstop per attack” as a data-driven value.

---

## 7. Collision and hit logic in state

- **IsColliding(player)** in `State` checks: projectiles vs hurtbox, other player’s hitbox vs hurtbox; sets `player.attackHurtNetwork`, `GameSimulation.Hitstop`, `player.hitstop`, `player.gotHit`, and `hurtPosition`. Then states (e.g. Hurt, Block) use that data.
- **HitstopFully(player)** sets `player.hitstop` and propagates to shadow/projectiles so everything that should freeze does.

**Takeaway**: Centralizing “did we get hit this frame?” and “apply hitstop to this entity and its projectiles” in one place (base State) keeps state classes simpler.

---

## 8. What we’re already doing well (and don’t need to copy)

- **Time-based hitstop**: Our `HitStop` uses real seconds and `Time.timeScale`; good for single-player feel and independent of frame rate. Darklings’ frame-based hitstop is for determinism and rollback.
- **Turn-based flow**: We have turn/phase logic and real-time reaction windows; we don’t need a full 60 FPS state machine unless we add rollback or replay.
- **Assembly layout**: We already use asmdefs and ScriptableObjects; no need to restructure.

---

## 9. Optional small additions we could adopt

| Idea | Where | Benefit |
|------|--------|--------|
| **IHitstop interface** | Combat/UI or Core | Let specific objects (e.g. characters, VFX) register for hitstop so only they freeze; keeps our time-based HitStop but allows per-entity control later. |
| **WaitFramesOnce(ref int)** | If we add frame-based tick | Simple “wait N frames, then run once” for scripted sequences or future deterministic logic. |
| **State name + resolver** | If combat/turn flow gets complex | Explicit state graph (e.g. “ChoosingAction” → “RealTimeWindow” → “Resolving”) with one class per state and a central resolver. |

---

## References

- Repo: [github.com/kidagine/Darklings-FightingGame](https://github.com/kidagine/Darklings-FightingGame) (branch: development)
- Play: [GameJolt – Darklings](https://gamejolt.com/games/darklings/640842)
- Related: [Demonics library](https://github.com/kidagine/Demonics-Base-UnityLibrary) (deterministic math/physics), ParrelSync for local net testing, GGPO for rollback.

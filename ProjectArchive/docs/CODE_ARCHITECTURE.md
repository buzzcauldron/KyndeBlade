# Code Architecture — One Script, One Job

Guidelines for keeping Kynde Blade maintainable and debuggable. Aim for **separation of concerns** and **single responsibility**.

---

## The "God Object" Problem

**KyndeBladeGameManager** currently does many jobs:

- **Bootstrap** — Creates/finds TurnManager, SaveManager, MusicManager, AgingManager, PovertyManager, NarrativeManager, DialogueSystem, WorldMapManager, MapLevelSelectUI
- **Combat pipeline** — Camera, background, CombatUI, CombatFeedback, GameStateManager, IlluminationManager
- **Spawning** — Party (Wille, Piers, Conscience), enemies by prefab or type, boss movesets and AI
- **Encounter flow** — Start from EncounterConfig, Green Knight random logic, hazards, sin miniboss
- **Visuals** — Character appearance (PiersAppearanceData), combat background, canvas scaler

If one system breaks, it’s harder to debug when everything lives in one script.

---

## Target: One Script, One Job

| Responsibility | Current home | Better (refactor target) |
|----------------|--------------|---------------------------|
| **UI (buttons, panels)** | GameManager + CombatUI + MapLevelSelectUI | UIManager or keep per-canvas (CombatUI, MapUI) |
| **Damage / combat math** | MedievalCharacter.ApplyCustomDamage, CombatAction | CombatEngine or keep on character + actions |
| **Spawning party/enemies** | KyndeBladeGameManager | SpawnService or EncounterSpawner |
| **Ensuring managers exist** | KyndeBladeGameManager.Ensure* | Bootstrap or ServiceLocator (optional) |
| **Save/load** | SaveManager | Keep as-is (already one job) |
| **Turn order / combat flow** | TurnManager | Keep as-is (already one job) |

**Rule of thumb:** When you add a method, ask: *“What is this script’s one job?”* If the answer is “spawn and play music and save,” split it.

---

## Refactor Plan (Pro path)

1. **Extract spawning**  
   Move `SpawnPartyInPoemOrder`, `SpawnEnemy`, `SpawnEnemyFromPrefab`, `GetEnemyPrefabByType`, `GetEnemyPrefabBySin` into an **EncounterSpawner** (or **PartySpawner** + **EnemySpawner**). GameManager keeps only “start encounter” and “start combat” and calls the spawner.

2. **Extract bootstrap**  
   Move `EnsureMapPipeline`, `EnsureCombatPipeline`, `EnsureCombatCamera`, `EnsureCombatBackground`, `EnsureCharacterVisual`, `ConfigureCanvasScaler` into a **GameBootstrap** or keep as static helpers used only at startup. GameManager.Start() then just calls Bootstrap.Setup() and starts map or test spawn.

3. **Keep GameManager thin**  
   After refactor, KyndeBladeGameManager’s job is: *orchestrate startup and entry points* (map vs test spawn, StartEncounterFromConfig, StartSinMinibossEncounter, RestartAtCheckpoint). No direct spawning or pipeline creation.

4. **ScriptableObjects for data**  
   Use ScriptableObjects for enemy/encounter “data cards” (stats, prefab references, move sets) instead of hard-coding type names and prefab lookups in one manager. Lets designers add Slime/Boss without new code.

---

## Documentation habit

- Use **`/// <summary>`** above public (and important private) methods in C#. Unity shows these as tooltips when you hover over the method.
- If you leave the project for two weeks, the summary should answer: *“What does this do?”* (e.g. “Adds Kynde after applying hunger/age/poverty modifiers.”)

---

## Git Flow (thin repo)

- **.gitignore** already excludes Unity-generated folders: `Library/`, `Temp/`, `Logs/`, `UserSettings/`, and `*.csproj`, `*.sln`. Don’t force-add them.
- Use **`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/`** as the Unity project root so Hub and the editor see a clean project; non-Unity work lives under **ProjectArchive/** (and Godot under **`KyndeBlade_Godot/`** at repo root).

See also: README (project structure), PHASE3_PLAN.md, NEXT_MOVES.md.

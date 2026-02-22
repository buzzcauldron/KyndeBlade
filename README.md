# Kynde Blade 16 Bit

A **16-bit style** turn-based RPG with real-time combat mechanics, built in **Unity**. Merged from KyndeBlade + kyndeblade-5.7 into a single Unity-first project. Named after "Kynde" (Nature) from the medieval poem Piers Plowman.

## Quick Start

1. **Open in Unity** — Unity 6.3 LTS (6000.3) or Unity 2022.3 LTS
   - File → Open Project → Select the **KyndeBlade_Unity** folder (the Unity project root, inside this repo)
2. **Create a scene** — File → New Scene
3. **Add GameManager** — Create empty GameObject, add `KyndeBladeGameManager` component
4. **Press Play** — Characters auto-spawn, combat starts, UI and feedback auto-create

**Book-aligned features** (Hodent + Beginner's Guide): CombatUI, TutorialManager, CombatFeedback, GameStateManager, GameSettings (timing accessibility), SimpleEnemyAI.

## Project Structure

```
Kynde Blade 16 Bit/             # Repo root
├── KyndeBlade_Unity/          # Unity project root (open this in Unity Hub)
│   ├── Assets/
│   │   ├── _Project/          # Feature-based layout
│   │   │   ├── Code/          # Core, Combat, UI assemblies
│   │   │   ├── Art/           # Sprites, animations
│   │   │   └── Data/          # ScriptableObjects (enemy stats, etc.)
│   │   └── Scenes/
│   ├── ProjectSettings/
│   └── Packages/
├── ProjectArchive/            # Docs + non-Unity (keeps root clean for Unity Hub)
│   ├── docs/                  # Design documents
│   ├── legacy/                # Unreal Engine 5.7
│   └── GodotPrototype/        # Godot prototype
└── README.md
```

## Game Overview

- **Turn-based + real-time** — Dodge, parry, counter with timing
- **Kynde system** — Generate from melee, spend on skills (Expedition 33–inspired)
- **Break system** — Deplete gauge to stun enemies
- **Status effects** — Hunger (worst), Frost, Burning, Poison
- **Piers Plowman** — Enemies: False, Lady Mede, Wrath; themes of work, poverty, spiritual seeking

## Design Docs

Design docs live in **ProjectArchive/docs/** so the repo root stays clean (Unity Hub sees only KyndeBlade_Unity).

| Doc | Description |
|-----|-------------|
| `ProjectArchive/docs/VISUAL_DESIGN_ALAN_LEE.md` | 16-bit art direction, palettes, sprites |
| `ProjectArchive/docs/GAMEPLAY_DESIGN.md` | Combat mechanics, Haunting Actions |
| `ProjectArchive/docs/CODE_ARCHITECTURE.md` | One-script-one-job, refactor plan |

## ProjectArchive (Unreal, Godot, Docs)

Non-Unity work and docs live in **ProjectArchive/** so Unity Hub recognizes the project from **KyndeBlade_Unity/** only.

- **ProjectArchive/legacy/** — Unreal Engine 5.7 C++. To build: `cd ProjectArchive/legacy && ./build_5.7.sh [path/to/UnrealEngine]`
- **ProjectArchive/GodotPrototype/** — Godot prototype
- **ProjectArchive/docs/** — Design and architecture docs

## License

Educational and development purposes.

## Credits

Inspired by Clair Obscur: Expedition 33 (Sandfall Interactive). Named after "Kynde" from William Langland's Piers Plowman.

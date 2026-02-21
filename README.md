# Kynde Blade 16 Bit

A **16-bit style** turn-based RPG with real-time combat mechanics, built in **Unity**. Merged from KyndeBlade + kyndeblade-5.7 into a single Unity-first project. Named after "Kynde" (Nature) from the medieval poem Piers Plowman.

## Quick Start

1. **Open in Unity** — Unity 2022.3 LTS or Unity 6
   - File → Open Project → Select this folder (the repo root)
2. **Create a scene** — File → New Scene
3. **Add GameManager** — Create empty GameObject, add `KyndeBladeGameManager` component
4. **Press Play** — Characters auto-spawn, combat starts, UI and feedback auto-create

**Book-aligned features** (Hodent + Beginner's Guide): CombatUI, TutorialManager, CombatFeedback, GameStateManager, GameSettings (timing accessibility), SimpleEnemyAI.

## Project Structure

```
Kynde Blade 16 Bit/
├── Assets/                    # Unity project (primary)
│   ├── Scripts/
│   │   ├── Characters/        # MedievalCharacter, enemies
│   │   ├── Combat/            # TurnManager, CombatAction, StatusEffect
│   │   ├── Game/              # KyndeBladeGameManager
│   │   └── Editor/
│   └── Scenes/
├── ProjectSettings/
├── packages/
├── docs/                      # Design documents
│   ├── VISUAL_DESIGN_ALAN_LEE.md
│   ├── GAMEPLAY_DESIGN.md
│   ├── CAMPAIGN_LEVEL_DESIGN.md
│   ├── UX_AND_GAME_DESIGN_BASICS.md
│   └── ...
├── legacy/                    # Unreal Engine 5.7 (archived)
│   ├── Source/
│   ├── Config/
│   └── KyndeBlade.uproject
└── README.md
```

## Game Overview

- **Turn-based + real-time** — Dodge, parry, counter with timing
- **Kynde system** — Generate from melee, spend on skills (Expedition 33–inspired)
- **Break system** — Deplete gauge to stun enemies
- **Status effects** — Hunger (worst), Frost, Burning, Poison
- **Piers Plowman** — Enemies: False, Lady Mede, Wrath; themes of work, poverty, spiritual seeking

## Design Docs

| Doc | Description |
|-----|-------------|
| `docs/VISUAL_DESIGN_ALAN_LEE.md` | 16-bit art direction, palettes, sprites |
| `docs/GAMEPLAY_DESIGN.md` | Combat mechanics, Haunting Actions |
| `docs/CAMPAIGN_LEVEL_DESIGN.md` | Piers Plowman campaign structure |
| `docs/UX_AND_GAME_DESIGN_BASICS.md` | Hodent + beginner guide principles |

## Legacy (Unreal Engine)

The original Unreal Engine 5.7 C++ project is in `legacy/`. To build:

```bash
cd legacy
./build_5.7.sh [path/to/UnrealEngine]
```

## License

Educational and development purposes.

## Credits

Inspired by Clair Obscur: Expedition 33 (Sandfall Interactive). Named after "Kynde" from William Langland's Piers Plowman.

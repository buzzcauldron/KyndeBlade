# Kynde Blade 16 Bit

A **16-bit style** turn-based RPG with real-time combat mechanics. The **shipping slice** is **Godot 4** under `KyndeBlade_Godot/`; the **Unity** project remains an oracle / export source under `ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/`. Named after "Kynde" (Nature) from the medieval poem Piers Plowman.

## Quick Start

### Godot (primary)

1. Open **`KyndeBlade_Godot/`** in **Godot 4.6.x** — see [`KyndeBlade_Godot/README.md`](KyndeBlade_Godot/README.md).

### Unity (oracle / export)

1. **Unity** — Unity 6.3 LTS (6000.3) or Unity 2022.3 LTS
   - File → Open Project → Select **`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/`** (Unity project root)
2. **Create a scene** — File → New Scene
3. **Add GameManager** — Create empty GameObject, add `KyndeBladeGameManager` component
4. **Press Play** — Characters auto-spawn, combat starts, UI and feedback auto-create

**Book-aligned features** (Hodent + Beginner's Guide): CombatUI, TutorialManager, CombatFeedback, GameStateManager, GameSettings (timing accessibility), SimpleEnemyAI.

## Project Structure

```
Kynde Blade 16 Bit/             # Repo root
├── KyndeBlade_Godot/           # Godot 4 project (Steam / retail slice)
├── ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/          # Unity oracle (open in Unity Hub)
│   ├── Assets/
│   │   ├── KyndeBlade/        # Code, Art, Resources
│   │   └── Scenes/
│   ├── ProjectSettings/      # Canonical Unity settings (root ProjectSettings merged here)
│   └── Packages/
├── Content/                   # Unreal-style (empty); use ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets for content
├── ProjectSettings/           # Legacy at root; merged into ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/ProjectSettings
├── ProjectArchive/            # Docs + non-Unity (if present)
└── README.md
```

**Merge note:** Root-level `ProjectSettings` (e.g. EditorBuildSettings, app-ui config) have been merged into `ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/ProjectSettings`. The Unity project is the single source of truth.

## Game Overview

- **Turn-based + real-time** — Dodge, parry, counter with timing
- **Kynde system** — Generate from melee, spend on skills (Expedition 33–inspired)
- **Break system** — Deplete gauge to stun enemies
- **Status effects** — Hunger (worst), Frost, Burning, Poison
- **Piers Plowman** — Enemies: False, Lady Mede, Wrath; themes of work, poverty, spiritual seeking

## Design Docs

Design docs live in **ProjectArchive/docs/** so the repo root stays clean; open Unity from **`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/`** only.

| Doc | Description |
|-----|-------------|
| `ProjectArchive/docs/VISUAL_DESIGN_ALAN_LEE.md` | 16-bit art direction, palettes, sprites |
| `ProjectArchive/docs/GAMEPLAY_DESIGN.md` | Combat mechanics, Haunting Actions |
| `ProjectArchive/docs/CODE_ARCHITECTURE.md` | One-script-one-job, refactor plan |

## ProjectArchive (Unreal, Godot, Docs)

Non-Unity work and docs live in **ProjectArchive/** so Unity Hub recognizes the project from **ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/** only.

- **ProjectArchive/legacy/** — Unreal Engine 5.7 C++. To build: `cd ProjectArchive/legacy && ./build_5.7.sh [path/to/UnrealEngine]`
- **ProjectArchive/GodotPrototype/** — Godot prototype
- **ProjectArchive/docs/** — Design and architecture docs

## License

Educational and development purposes.

## Credits

Inspired by Clair Obscur: Expedition 33 (Sandfall Interactive). Named after "Kynde" from William Langland's Piers Plowman.

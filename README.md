# Kynde Blade

A **high-bit style** turn-based RPG with real-time combat mechanics, built in **Unity**. Merged from KyndeBlade + kyndeblade-5.7 into a single Unity-first project. Named after "Kynde" (Nature) from the medieval poem Piers Plowman.

## Quick Start

1. **Open in Unity** — Unity 6.3 LTS (6000.3) or Unity 2022.3 LTS
   - File → Open Project → Select the **KyndeBlade_Unity** folder (the Unity project root, inside this repo)
2. **Create a scene** — File → New Scene
3. **Add GameManager** — Create empty GameObject, add `KyndeBladeGameManager` component
4. **Press Play** — Characters auto-spawn, combat starts, UI and feedback auto-create

**Book-aligned features** (Hodent + Beginner's Guide): CombatUI, TutorialManager, CombatFeedback, GameStateManager, GameSettings (timing accessibility), SimpleEnemyAI.

## Project Structure

```
Kynde Blade/                    # Repo root
├── KyndeBlade_Unity/          # Unity project root (open this in Unity Hub)
│   ├── Assets/
│   │   └── KyndeBlade/        # Game code, docs, resources
│   ├── ProjectSettings/
│   └── Packages/
├── KyndeBlade_Godot/          # Godot 4 port (sibling to Unity; open this in Godot)
├── ProjectArchive/            # Docs + legacy engines
│   ├── docs/
│   └── legacy/                # Unreal Engine 5.7 (historical)
└── README.md
```

## Game Overview

- **Turn-based + real-time** — Dodge, parry, counter with timing
- **Kynde system** — Generate from melee, spend on skills (Expedition 33–inspired)
- **Break system** — Deplete gauge to stun enemies
- **Status effects** — Hunger (worst), Frost, Burning, Poison
- **Piers Plowman** — Enemies: False, Lady Mede, Wrath; themes of work, poverty, spiritual seeking

## Planning canon (combat + world + structure)

- **Single map of careful work:** [docs/KYNDEBLADE_CAREFUL_CANON.md](docs/KYNDEBLADE_CAREFUL_CANON.md) — worldbuilding, combat planning, TDAD/oracle, links to leaf docs (including UMich *Piers Plowman* full text for dialogue).

## TDAD: demo vs Steam

- **Tier A (playable demo):** [docs/TDAD_RELEASE_PATHS.md](docs/TDAD_RELEASE_PATHS.md) — links to `.tdad/workflows/demo-vertical-slice/`, BDD feature file, and `KyndeBlade_Unity/.../DEMO_RUN.md`.
- **Tier B (Steam / EA planning):** same doc — `.tdad/workflows/steam-early-access/` and `plan-steam-milestones.md` prompt.

## TDAD: Godot full port (gated) + Unity archive

- **Milestones M1–M6:** [`.tdad/workflows/godot-full-port/godot-full-port.workflow.json`](.tdad/workflows/godot-full-port/godot-full-port.workflow.json) — Unity **demo-vertical-slice** is the oracle until M6 sign-off.
- **Parity slice (Godot):** [`.tdad/workflows/godot-parity-slice/godot-parity-slice.workflow.json`](.tdad/workflows/godot-parity-slice/godot-parity-slice.workflow.json) and [`.tdad/bdd/godot-parity-slice.feature`](.tdad/bdd/godot-parity-slice.feature).
- **Godot project:** [`KyndeBlade_Godot/`](KyndeBlade_Godot/README.md) — headless tests: [`docs/CI_GODOT_TESTS.md`](docs/CI_GODOT_TESTS.md).
- **Unity archive slot (after M6 only):** [`ProjectArchive/UnityKyndeBlade/README.md`](ProjectArchive/UnityKyndeBlade/README.md) — `KyndeBlade_Unity/` stays at repo root until then.

## Cult-ship, coauthorship, voice

- **[docs/CULT_AND_COAUTHORSHIP.md](docs/CULT_AND_COAUTHORSHIP.md)** — index to ship bar, commission riders, community poetry gate, authorial override, release-note voice template (`KyndeBlade_Godot/docs/`).

## Design Docs

Design docs live in **ProjectArchive/docs/** so the repo root stays clean (Unity Hub sees only KyndeBlade_Unity).

| Doc | Description |
|-----|-------------|
| `ProjectArchive/docs/VISUAL_DESIGN_ALAN_LEE.md` | High-bit art direction, palettes, sprites |
| `ProjectArchive/docs/GAMEPLAY_DESIGN.md` | Combat mechanics, Haunting Actions |
| `ProjectArchive/docs/CODE_ARCHITECTURE.md` | One-script-one-job, refactor plan |

## ProjectArchive (Unreal, Godot, Docs)

Non-Unity work and docs live in **ProjectArchive/** so Unity Hub recognizes the project from **KyndeBlade_Unity/** only.

- **ProjectArchive/legacy/** — Unreal Engine 5.7 C++. To build: `cd ProjectArchive/legacy && ./build_5.7.sh [path/to/UnrealEngine]`
- **KyndeBlade_Godot/** — Godot port (sibling to `KyndeBlade_Unity/`)
- **ProjectArchive/docs/** — Design and architecture docs

## License

Educational and development purposes.

## Credits

Inspired by Clair Obscur: Expedition 33 (Sandfall Interactive). Named after "Kynde" from William Langland's Piers Plowman.

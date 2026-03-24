# Kynde Blade 16 Bit

A **hi-bit style** turn-based RPG with real-time combat mechanics (Expedition 33–inspired defensive windows). The **shipping slice** is **Godot 4** under `KyndeBlade_Godot/`. **`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/`** is a **reference archive** for export-to-Godot, historical parity, and TDAD oracle workflows — see [`ProjectArchive/UnityKyndeBlade/UNITY_REFERENCE_ARCHIVE.md`](ProjectArchive/UnityKyndeBlade/UNITY_REFERENCE_ARCHIVE.md). Formal M6 “Godot-only oracle” cutover remains human-gated. Named after "Kynde" (Nature) from the medieval poem *Piers Plowman*.

**Branching:** [`docs/BRANCH_POLICY.md`](docs/BRANCH_POLICY.md) — use `main`; the old integration branch `unity` is superseded.

## Quick Start

### Godot (primary)

1. Open **`KyndeBlade_Godot/`** in **Godot 4.6.x** — see [`KyndeBlade_Godot/README.md`](KyndeBlade_Godot/README.md).

### Unity (archived reference / export only)

1. **Policy:** [`ProjectArchive/UnityKyndeBlade/UNITY_REFERENCE_ARCHIVE.md`](ProjectArchive/UnityKyndeBlade/UNITY_REFERENCE_ARCHIVE.md).
2. **Open in Unity** — Unity 6.3 LTS (6000.3) or Unity 2022.3 LTS  
   - File → Open Project → **`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity`**
3. Use **KyndeBlade → Export Slice Data for Godot** when updating `KyndeBlade_Godot/data/exported_from_unity.json`.
4. **Play** and tests per [`DEMO_RUN.md`](ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/DEMO_RUN.md) and [`docs/CI_UNITY_TESTS.md`](docs/CI_UNITY_TESTS.md) when validating the archive tree.

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
│   ├── docs/
│   └── legacy/                # Unreal Engine 5.7 (historical)
└── README.md
```

**Merge note:** Root-level `ProjectSettings` have been merged into `ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/ProjectSettings`. **Godot** owns the shipping slice; Unity settings remain authoritative **for the Unity editor project only**.

## Game Overview

- **Turn-based + real-time** — Dodge, parry, counter with timing
- **Kynde system** — Generate from melee, spend on skills (Expedition 33–inspired)
- **Break system** — Deplete gauge to stun enemies
- **Status effects** — Hunger (worst), Frost, Burning, Poison
- **Piers Plowman** — Enemies: False, Lady Mede, Wrath; themes of work, poverty, spiritual seeking

## Planning canon (combat + world + structure)

- **Single map of careful work:** [docs/KYNDEBLADE_CAREFUL_CANON.md](docs/KYNDEBLADE_CAREFUL_CANON.md) — worldbuilding, combat planning, TDAD/oracle, links to leaf docs (including UMich *Piers Plowman* full text for dialogue).

## TDAD: demo vs Steam

- **Tier A (playable demo):** [docs/TDAD_RELEASE_PATHS.md](docs/TDAD_RELEASE_PATHS.md) — links to `.tdad/workflows/demo-vertical-slice/`, BDD feature file, and `ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/.../DEMO_RUN.md`.
- **Tier B (Steam / EA planning):** same doc — `.tdad/workflows/steam-early-access/` and `plan-steam-milestones.md` prompt.

## TDAD: Godot full port (gated) + Unity archive

- **Milestones M1–M6:** [`.tdad/workflows/godot-full-port/godot-full-port.workflow.json`](.tdad/workflows/godot-full-port/godot-full-port.workflow.json) — Unity **demo-vertical-slice** remains the behaviour oracle for Tier A until M6 sign-off (see canon); **default git branch** is still `main`, now tracking the Godot integration line.
- **Parity slice (Godot):** [`.tdad/workflows/godot-parity-slice/godot-parity-slice.workflow.json`](.tdad/workflows/godot-parity-slice/godot-parity-slice.workflow.json) and [`.tdad/bdd/godot-parity-slice.feature`](.tdad/bdd/godot-parity-slice.feature).
- **Godot project:** [`KyndeBlade_Godot/`](KyndeBlade_Godot/README.md) — headless tests: [`docs/CI_GODOT_TESTS.md`](docs/CI_GODOT_TESTS.md).
- **Unity reference tree:** [`ProjectArchive/UnityKyndeBlade/README.md`](ProjectArchive/UnityKyndeBlade/README.md) — Unity project folder is **`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/`** (M6 snapshot checklist optional).

## Cult-ship, coauthorship, voice

- **[docs/CULT_AND_COAUTHORSHIP.md](docs/CULT_AND_COAUTHORSHIP.md)** — index to ship bar, commission riders, community poetry gate, authorial override, release-note voice template (`KyndeBlade_Godot/docs/`).

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
- **KyndeBlade_Godot/** — Godot port (sibling to `ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/`)
- **ProjectArchive/docs/** — Design and architecture docs

## License

**All rights reserved.** See [`LICENSE`](LICENSE) at the repository root. Third-party assets may have separate terms ([`docs/ASSET_LICENSES.md`](docs/ASSET_LICENSES.md)).

## Credits

Inspired by Clair Obscur: Expedition 33 (Sandfall Interactive). Named after "Kynde" from William Langland's Piers Plowman.

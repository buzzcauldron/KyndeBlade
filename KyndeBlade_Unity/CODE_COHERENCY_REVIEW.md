# Code coherency review

**Date:** 2025-02-21

## Namespaces

- **KyndeBlade** — Core game types (managers, UI, narrative, save, map).
- **KyndeBlade.Combat** — Combat-specific types (movesets, actions, status effects, character stats, enemy AI).
- **KyndeBlade.Editor** — Editor-only scripts (menu items, level data creation, dialogue tree generator).
- **KyndeBlade.Tests** — Play mode tests.

**Change applied:** Editor scripts `DialogueTreeGenerator`, `CreateVision2LevelData`, and `CreateMainScene` were moved from `namespace KyndeBlade` to `namespace KyndeBlade.Editor` with `using KyndeBlade` so all Editor code lives under `KyndeBlade.Editor` per the Editor asmdef rootNamespace.

## Assembly definitions

- `KyndeBlade.Core` — Core, Save, Narrative, Visual, Game, Map.
- `KyndeBlade.Combat` — Combat, characters, UI, turn flow. References Core.
- `KyndeBlade.Editor` — Editor tools. References Core + Combat.
- `PlayModeTests` — Tests. References Core + Combat.

Dependency flow: Editor/Tests → Combat → Core (no circular refs).

## API usage

- `FindFirstObjectByType<T>()` used consistently (Unity 6–recommended; `FindObjectOfType` deprecated).
- `GameRuntime` used as optional central registry; components fall back to `FindFirstObjectByType` when refs are null.

## Minor note

- `CharacterStats.cs` and `StatusEffect.cs` live under `Code/Core` but use `namespace KyndeBlade.Combat` (they are combat types). Folder/namespace mismatch is intentional to avoid moving files and breaking refs; consider moving later if reorganizing.

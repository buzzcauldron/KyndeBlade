# Port wave tracker

Maps the **careful full port** waves to TDAD root workflow folders. Authoritative folder list: [`.tdad/workflows/root.workflow.json`](../.tdad/workflows/root.workflow.json). TDAD Godot track: [`.tdad/workflows/godot-full-port/godot-full-port.workflow.json`](../.tdad/workflows/godot-full-port/godot-full-port.workflow.json).

**TDAD M1–M6 ↔ waves (planning bridge)**

| Milestone | Primary waves | Notes |
|-----------|---------------|--------|
| M1 Foundation | W0 | Menu → hub → combat entry, `STEAM_BUILD.md` QA |
| M2 Combat parity | W4, W5 | `EncounterDef`, deterministic windows, False encounter |
| M3 Save / UI | W0, W1 | Continue/New, pause, settings, save parity documented |
| M4 Map / narrative | W2, W3 | Location data + pre-combat flavor; `exported_from_unity.json` |
| M5 Systems depth | W6 (+ W4–W5 extensions) | Hazards, Kynde, break, AI — incremental tests |
| M6 Cutover | All + archive | **Human-gated** — see [`M6_CUTOVER_CHECKLIST.md`](M6_CUTOVER_CHECKLIST.md) |

**Current wave (edit when you advance):** `W4` — combat core stable; expand W6/W7 hooks from Steam-slice baseline.

**Combat presentation (Unity-aligned, TDAD-segmented):** see [`docs/TDAD_COMBAT_PRESENTATION_PLAN.md`](docs/TDAD_COMBAT_PRESENTATION_PLAN.md) — segments **CP-01…CP-06** (API → stage → poses → parry/dodge eye → audio → docs/BDD). Extends W4/W5 **presentation** without changing combat rules.

## Checklist template (repeat per wave)

- [ ] Inventory Unity paths under `ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/`
- [ ] One-page spec: inputs, outputs, state, edge cases
- [ ] Data exported or versioned schema (`version` field)
- [ ] Godot implementation + headless tests where applicable
- [ ] Manual row in [`STEAM_BUILD.md`](STEAM_BUILD.md) or wave doc
- [ ] [`PARITY_GAPS.md`](PARITY_GAPS.md) updated

## Waves

| Wave | TDAD groups (summary) | Status |
|------|---------------------|--------|
| W0 | scenes, input, ui-shell | **Slice met** (M1 scope) |
| W1 | save-system, game-state | **Slice met** + meta flags in save (M3 subset) |
| W2 | map-progression | **Partial** — Unity export + hub; full graph TBD |
| W3 | narrative | **Partial** — tower + Fair Field `StoryBeatOnArrival` text via export (`tower_intro.tscn`, hub flavor) |
| W4 | combat-core, combat-actions, characters, enemies | **Partial** — Fair Field / False |
| W5 | combat-defense, combat-damage | **Partial** — dodge/parry windows |
| W6 | combat-kynde, combat-break, combat-status, combat-hazards | **Stub** — hazard damage hook returns 0 |
| W7 | audio, meta-progression, perf, a11y-l10n | **Partial** — Master + Music/SFX buses; meta flags persisted |

## Free-licensed assets

Visual/audio drops: see [repo `docs/ASSET_LICENSES.md`](../docs/ASSET_LICENSES.md) and [`assets/third_party/README.md`](assets/third_party/README.md). Placement follows [`ART_DIRECTION.md`](../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md) (Lane A vs B).

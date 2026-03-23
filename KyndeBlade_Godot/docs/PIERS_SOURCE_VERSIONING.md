# Piers Plowman — source versioning & narrative framing (Godot)

This document frames **William Langland’s *Piers Plowman*** for Kynde Blade **data and writers**: which **text version** we align to by default, how **Visio** vs **Vita** maps onto our **campaign spine**, and where to look for **symbols** and **geography** without copying third-party prose into the repo.

## Default narrative alignment

- **Player-facing passus labels and slice copy** should assume the **B-text** unless you explicitly ship A- or C-text variants later. Kynde Blade’s `locations_registry.json` already carries `passus_title` / indices; optional `passus_anchor_b` marks writer-facing B-text anchors where useful.
- **Middle English quotations** in-game remain governed by repo canon: [`docs/KYNDEBLADE_CAREFUL_CANON.md`](../../docs/KYNDEBLADE_CAREFUL_CANON.md) (Corpus of Middle English and project leaf docs).

## A-text, B-text, C-text (structural)

| Version | Role (summary) | Notes for designers |
|--------|----------------|---------------------|
| **A** | Earlier, shorter witness | Different passus numbering and emphasis; use only if you author an A-lane. |
| **B** | Wider transmission; common study reference | **Default** for spine labels and symbol hooks in `text_version_manifest.json`. |
| **C** | Revised, politically sharper in places | Optional future `piers_text_edition` branch in save / `GameState`. |

Line counts and manuscript history vary by witness — treat `passus_count_hint` and edition notes in JSON as **hints**, not legal claims.

## Visio vs Vita (Dowel, Dobet, Dobest)

Study outlines often divide the poem into a **Visio** (visions of society, Meed, sins, pilgrimage imagery) and a **Vita** (the quest for **Do-Wel**, **Do-Bet**, **Do-Best**). In Kynde Blade:

| Phase id (`text_version_manifest.json`) | Game mapping |
|----------------------------------------|----------------|
| `visio` | `campaign_spine.json` → `vision_0_fair_field_arc` + optional branches (`green_chapel_lane`, `orfeo_alternate_lane`). |
| `vita_dowel` | Primarily `vision_2_do_wel_arc` (Do-Wel quest naming in our ids). |
| `vita_dobet` / `vita_dobest` | Reserved for later spine keys when Vision II+ beats are expanded; empty `ordered_location_ids` until authored. |

**Dream-within-dream** and **returns to the field** inform runtime counters (`fair_field_return_count`, `dream_iteration`) and optional `repetition_tags` on beats — see `data/world/narrative_beats_skeleton.json` and `GameState`.

## External references (link out; do not paste long excerpts)

| Source | Use in-repo |
|--------|-------------|
| [LitCharts — *Piers Plowman* symbols](https://www.litcharts.com/lit/piers-plowman/symbols) | **Paraphrased** symbol ids + short in-house blurbs in `piers_symbols.json`; link for full analysis. |
| [Fair Field — placenames map (blog)](https://thisfairfield.com/2017/04/08/all-the-placenames-in-piers-plowman-mapped-for-the-first-time/) | Design note + geography flavor; do **not** scrape their map data without permission. |
| [Encyclopedia summary — *Piers Plowman*](https://medieval_literature.en-academic.com/475/Piers_Plowman) | High-level **A/B/C** and **Visio / Vita** framing. |
| [UMich — Corpus of Middle English](https://quod.lib.umich.edu/c/cme/) | Bookmark for diplomatic/edition work aligned with careful-canon policy. |

## Data files

| File | Purpose |
|------|---------|
| `data/world/text_version_manifest.json` | Default edition, edition hints, `narrative_phases` → spine keys. |
| `data/world/piers_symbols.json` | Symbol registry (ids, display names, **original** short paraphrases, related locations/beats). |
| `data/world/locations_registry.json` | Optional `piers_symbols`, `text_edition_notes`, `passus_anchor_b` per location. |
| `scripts/world/piers_symbols.gd` | `PiersSymbolCatalog` loader. |
| `scripts/world/narrative_context.gd` | Stub `arrival_variant_key()` for future dialogue variants. |

## Attribution

Symbol **names** and **concepts** follow common study-guide groupings (e.g. LitCharts topic list). **Descriptions** in JSON are **original paraphrases** for game use, not quotations from study sites.

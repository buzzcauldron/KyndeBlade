# Kynde Blade — careful canon (planning, combat, worldbuilding, structure)

This document **recreates in one place** the careful framing that otherwise lives across Unity docs, Godot docs, TDAD workflows, and `ProjectArchive/`. It is an **index and synthesis**, not a substitute: when detail conflicts, follow the **leaf document** and then update this file.

---

## 1. North star and oracle

**Git default branch:** **`main`** — Godot shipping slice + Unity sibling in-tree; see [`docs/BRANCH_POLICY.md`](BRANCH_POLICY.md). This does not by itself change the **Tier A behaviour oracle** (Unity slice + BDD) until you explicitly adopt a new policy at M6 or equivalent.

| Idea | Where it lives |
|------|----------------|
| **What we’re building** | A **Piers Plowman**–rooted, **Middle English / archaic-modern** RPG: turn-based flow with **real-time defensive windows**, **Kynde** and **Break**-style depth (Expedition 33–inspired), high-bit Lane B combat read + manuscript-style UI. |
| **Shippable loop (Tier A)** | Unity: [`PLAYABLE_SLICE.md`](../KyndeBlade_Unity/Assets/KyndeBlade/Docs/PLAYABLE_SLICE.md), [`DEMO_RUN.md`](../KyndeBlade_Unity/Assets/KyndeBlade/Docs/DEMO_RUN.md). TDAD: [`.tdad/workflows/demo-vertical-slice/`](../.tdad/workflows/demo-vertical-slice/demo-vertical-slice.workflow.json), [`.tdad/bdd/demo-vertical-slice.feature`](../.tdad/bdd/demo-vertical-slice.feature). |
| **Godot Steam / retail slice** | [`KyndeBlade_Godot/STEAM_BUILD.md`](../KyndeBlade_Godot/STEAM_BUILD.md), [`KyndeBlade_Godot/docs/GODOT_TARGET_EXPERIENCE.md`](../KyndeBlade_Godot/docs/GODOT_TARGET_EXPERIENCE.md) (north star), [`KyndeBlade_Godot/docs/VISION_CRAWL_NOITA_E33.md`](../KyndeBlade_Godot/docs/VISION_CRAWL_NOITA_E33.md) (short vision: fake-voxel crawl, 3D combat target, Noita mood, E33, oracle), [`.tdad/workflows/godot-parity-slice/`](../.tdad/workflows/godot-parity-slice/godot-parity-slice.workflow.json), [`.tdad/bdd/godot-parity-slice.feature`](../.tdad/bdd/godot-parity-slice.feature). |
| **Oracle rule** | Until **M6 archive** sign-off: if behaviour disagrees, **Unity slice + DEMO_RUN** win unless you **update both sides** and BDD. See [`KyndeBlade_Godot/PARITY_GAPS.md`](../KyndeBlade_Godot/PARITY_GAPS.md) for intentional differences. |
| **Dual engines** | **Unity** — full game and authoring (`KyndeBlade_Unity/`). **Godot** — cheaper second target, headless tests, export-driven text (`KyndeBlade_Godot/`). Map: [`UNITY_GODOT_MODULE_MAP.md`](UNITY_GODOT_MODULE_MAP.md). |
| **Unity story / spawn digest** | Single reference crawl (party spawn, encounters, faerie, Green Knight): [`UNITY_STORY_AND_SPAWN_DIGEST.md`](UNITY_STORY_AND_SPAWN_DIGEST.md). |
| **Medieval texts → Godot movesets** | Data + design: [`KyndeBlade_Godot/data/medieval_text_unlocks.json`](../KyndeBlade_Godot/data/medieval_text_unlocks.json), [`KyndeBlade_Godot/docs/MEDIEVAL_TEXT_MOVESET_DESIGN.md`](../KyndeBlade_Godot/docs/MEDIEVAL_TEXT_MOVESET_DESIGN.md). |
| **TDAD repo map** | [`.tdad/README.md`](../.tdad/README.md) — workflows, root graph, BDD, prompts; Godot ship layer [`godot-steam-build`](../.tdad/workflows/godot-steam-build/godot-steam-build.workflow.json). |
| **Full game skeleton (Godot)** | Unity `LocationNode` graph + beats + encounter index as JSON + atlas UI: [`KyndeBlade_Godot/docs/GAME_SKELETON.md`](../KyndeBlade_Godot/docs/GAME_SKELETON.md) (`data/world/`, `WorldNav` autoload). |

---

## 2. Worldbuilding pillar

### 2.1 Literary and linguistic sources

| Source | Role in Kynde Blade | Credit / doc |
|--------|---------------------|--------------|
| **William Langland, *Piers Plowman*** | Dream-vision spine: Fair Field, Tower on the Toft, folk, labour, Grace withheld. | [`SOURCES_AND_CREDITS.md`](../KyndeBlade_Unity/Assets/KyndeBlade/Docs/SOURCES_AND_CREDITS.md) |
| **Jos Charles, *feeld*** | Spellings **feeld** / **folke**, fractured readable lyric register. | Same |
| ***Sir Gawain and the Green Knight*** | Green Knight covenant, tone, superboss lane. | Same |
| **Sir Orfeo** (and related) | Otherworld / boundary choices in broader campaign docs. | [`STORY_AND_CAMPAIGN_DESIGN.md`](../ProjectArchive/docs/STORY_AND_CAMPAIGN_DESIGN.md), [`DIALOGUE_SOURCE_TEXT_PLAN.md`](../ProjectArchive/docs/DIALOGUE_SOURCE_TEXT_PLAN.md) |

### 2.2 Electronic text for dialogue and beat planning

For **close Middle English wording**, line references, and scholarly layout, use the **Corpus of Middle English** edition of *Piers Plowman* (University of Michigan):

- **Full-text view (bookmark for writers):** [PPlLan — Corpus of Middle English](https://quod.lib.umich.edu/c/cme/PPlLan?rgn=main;view=fulltext)

Pair this with [`DIALOGUE_SOURCE_TEXT_PLAN.md`](../ProjectArchive/docs/DIALOGUE_SOURCE_TEXT_PLAN.md) (beat → excerpt → choice structure) and keep **credits** in [`SOURCES_AND_CREDITS.md`](../KyndeBlade_Unity/Assets/KyndeBlade/Docs/SOURCES_AND_CREDITS.md) accurate for any long quotations.

### 2.3 Protagonist, tone, and long-form campaign

| Topic | Document |
|-------|----------|
| **Will / Wille**, party, Acts, Fair Field opening | [`STORY_AND_CAMPAIGN_DESIGN.md`](../ProjectArchive/docs/STORY_AND_CAMPAIGN_DESIGN.md) |
| **Visions / Passus**, hours, unresolved Grace | [`CAMPAIGN_LEVEL_DESIGN.md`](../ProjectArchive/docs/CAMPAIGN_LEVEL_DESIGN.md) |
| **Canonical IDs and names** (locations, display strings, Kynde/Break constants) | [`WORLD_DEPTH_EXPEDITION33.md`](../KyndeBlade_Unity/Assets/KyndeBlade/Docs/WORLD_DEPTH_EXPEDITION33.md), `GameWorldConstants` in Unity code |
| **Dream / Malvern** mood | [`DREAM_REAL_LIFE_MALVERN.md`](../ProjectArchive/docs/DREAM_REAL_LIFE_MALVERN.md) |

### 2.4 Voice, ship bar, and intentional non-parity

| Topic | Document |
|-------|----------|
| **Cult-ship bar** (voice, weird rule, aesthetic, checklists) | [`KyndeBlade_Godot/docs/CULT_SHIP_BAR.md`](../KyndeBlade_Godot/docs/CULT_SHIP_BAR.md) |
| **One Godot-first “scar”** (documented override) | [`KyndeBlade_Godot/docs/AUTHORIAL_OVERRIDE.md`](../KyndeBlade_Godot/docs/AUTHORIAL_OVERRIDE.md) |
| **Hub / menu copy** (archaic-modern, not Unity chrome) | Noted in [`PARITY_GAPS.md`](../KyndeBlade_Godot/PARITY_GAPS.md); implement in `hub_map.gd` / export text |

---

## 3. Combat planning pillar

### 3.1 Slice combat (what must work first)

| Step | Unity | Godot |
|------|-------|--------|
| Hub / map → **Fair Field** | `Loc_tour` → `fayre_felde`, `StoryBeatOnArrival` | `hub_map.tscn`, `exported_from_unity.json`, `slice_locations.json` |
| **Encounter False** | `FayreFeldeEncounter` | `encounter_fair_field.tres`, `CombatManager` |
| **Strike / dodge / parry / feint** | `TurnManager`, real-time window, `ParryDodgeZoneIndicator` | Deterministic swing pattern; `window_duration`, `window_phase_t()`; [`TDAD_COMBAT_PRESENTATION_PLAN.md`](../KyndeBlade_Godot/docs/TDAD_COMBAT_PRESENTATION_PLAN.md) |
| **Save on victory** | `SaveManager`, `GameStateManager` | `SaveService`, `GameState` |

**Design depth (full game target):** [`EXPEDITION_33_COMBAT_DESIGN.md`](../ProjectArchive/docs/EXPEDITION_33_COMBAT_DESIGN.md) — hybrid turn + real-time, **Kynde** economy, **Break** gauge, elements, Escapade/Ward analogues → mapped in Unity; many systems **stubbed or partial** in the Godot Steam slice (see `PARITY_GAPS.md`).

### 3.2 Unity combat stack (reference implementation)

| Piece | Role | Pointer |
|-------|------|---------|
| **Bootstrap** | Combat camera, backdrop, optional hazard strip, `CombatUI`, feedback | [`KyndeBladeGameManager.cs`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/KyndeBladeGameManager.cs), [`ARCHITECTURE.md`](../KyndeBlade_Unity/Assets/KyndeBlade/ARCHITECTURE.md) |
| **Turn + window** | `CombatState`, damage resolution | `TurnManager`, [`TIMING_AND_SIGNALS.md`](../KyndeBlade_Unity/Assets/KyndeBlade/Docs/TIMING_AND_SIGNALS.md) |
| **Eye / React** | Phased defensive read | [`ParryDodgeZoneIndicator.cs`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/UI/ParryDodgeZoneIndicator.cs), [`CombatUI.cs`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/UI/CombatUI.cs) |

### 3.3 Godot combat presentation (staged parity)

| Piece | Location |
|-------|----------|
| Stage + placeholders + motion | `scenes/combat.tscn`, `scripts/combat_presentation.gd` |
| Parry/dodge eye + **React!** | `scenes/parry_dodge_eye.tscn`, `scripts/parry_dodge_eye.gd` |
| Headless scenarios | `tests/combat_scenarios.gd`, `tests/run_headless_tests.gd` |

### 3.4 Art direction in combat

| Lane | Use | Doc |
|------|-----|-----|
| **Lane A** | Vista / hub / story backdrops | [`ART_DIRECTION.md`](../KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md) |
| **Lane B** | Combat void, high-contrast sprites, SNES read | Same |
| **Manuscript** | UI chrome | [`UI_MANUSCRIPT_THEME.md`](../ProjectArchive/docs/UI_MANUSCRIPT_THEME.md), [`ART_DIRECTION_GODOT.md`](../KyndeBlade_Godot/docs/ART_DIRECTION_GODOT.md) |
| **Grimdark / boss palettes** | Pride, Hunger, Green Knight | [`ART_DIRECTION_GRIMDARK_MEDIEVAL.md`](../ProjectArchive/docs/ART_DIRECTION_GRIMDARK_MEDIEVAL.md) |

---

## 4. Structural pillar (milestones, waves, CI)

| Layer | Document / path |
|-------|-----------------|
| **Repo overview** | [`README.md`](../README.md) |
| **M1–M6 ↔ W0–W7** | [`KyndeBlade_Godot/PORT_WAVE.md`](../KyndeBlade_Godot/PORT_WAVE.md) |
| **Godot full port workflow** | [`.tdad/workflows/godot-full-port/`](../.tdad/workflows/godot-full-port/godot-full-port.workflow.json) |
| **M6 human gate** | [`KyndeBlade_Godot/M6_CUTOVER_CHECKLIST.md`](../KyndeBlade_Godot/M6_CUTOVER_CHECKLIST.md) |
| **Release paths (demo vs Steam)** | [`TDAD_RELEASE_PATHS.md`](TDAD_RELEASE_PATHS.md) |
| **Unity CI** | [`CI_UNITY_TESTS.md`](CI_UNITY_TESTS.md) |
| **Godot headless** | [`CI_GODOT_TESTS.md`](CI_GODOT_TESTS.md) |
| **Unity → Godot JSON export** | `Assets/Editor/ExportGodotSliceData.cs` → `KyndeBlade_Godot/data/exported_from_unity.json` |

---

## 5. Curated index — where to go first

### Unity (authoritative gameplay)

- `KyndeBlade_Unity/Assets/KyndeBlade/Docs/PLAYABLE_SLICE.md`
- `KyndeBlade_Unity/Assets/KyndeBlade/Docs/DEMO_RUN.md`
- `KyndeBlade_Unity/Assets/KyndeBlade/ARCHITECTURE.md`
- `KyndeBlade_Unity/Assets/KyndeBlade/Docs/SOURCES_AND_CREDITS.md`
- `KyndeBlade_Unity/Assets/KyndeBlade/Docs/WORLD_DEPTH_EXPEDITION33.md`

### Godot (Steam build + voice experiments)

- `KyndeBlade_Godot/STEAM_BUILD.md` (redirect: `GODOT_DEMO.md`)
- `KyndeBlade_Godot/docs/GODOT_TARGET_EXPERIENCE.md`
- `KyndeBlade_Godot/PARITY_GAPS.md`
- `KyndeBlade_Godot/docs/TDAD_COMBAT_PRESENTATION_PLAN.md`
- `KyndeBlade_Godot/docs/CULT_SHIP_BAR.md`

### Archive (campaign breadth, legacy design)

- `ProjectArchive/docs/STORY_AND_CAMPAIGN_DESIGN.md`
- `ProjectArchive/docs/CAMPAIGN_LEVEL_DESIGN.md`
- `ProjectArchive/docs/DIALOGUE_SOURCE_TEXT_PLAN.md`
- `ProjectArchive/docs/EXPEDITION_33_COMBAT_DESIGN.md`

---

## 6. Maintenance

When you **change** slice behaviour, combat rules, or voice contracts:

1. Update the **leaf** doc and tests/BDD.
2. Add or adjust a row in **`PARITY_GAPS.md`** if Godot intentionally differs.
3. Adjust **this file** only if pillars or links shift (keep §5 accurate).

**Version:** 1.0 — careful scaffold recreated as a single map; electronic PPl text URL anchored for dialogue work.

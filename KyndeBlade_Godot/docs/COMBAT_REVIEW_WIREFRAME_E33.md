# Combat review — wireframe vs Expedition 33 read (Godot)

**Scope:** Post–mini-act slice work (commit band around `126bd07`): graybox **3D** combat arena, **2D** Lane B stage, manuscript HUD, defensive **wind-up** + optional **bleed**, and audio/read hooks. This doc is the engineering ledger for **E33-style** expectations (turn flow + real-time defensive window) without claiming full parity to another product.

**Oracle:** Unity combat UI / `TurnManager` / `CombatUI` under [`ProjectArchive/UnityKyndeBlade/...`](../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/); product differences live in [`PARITY_GAPS.md`](../PARITY_GAPS.md). Segmented plan: [`TDAD_COMBAT_PRESENTATION_PLAN.md`](TDAD_COMBAT_PRESENTATION_PLAN.md). Vision capsule: [`VISION_CRAWL_NOITA_E33.md`](VISION_CRAWL_NOITA_E33.md). **Re-verify** rules + presentation: [`WIREFRAME_COMBAT_CHECKLIST.md`](WIREFRAME_COMBAT_CHECKLIST.md).

---

## 1. What shipped in the graybox

| Layer | Role | Key files |
|-------|------|-----------|
| **3D arena** | Cosmetic motion: camera sway, telegraph hold, strike lunge, mesh hit flash, camera shake, HP-delta reactions | [`combat_presentation_3d.gd`](../scripts/combat_presentation_3d.gd), nodes under [`combat.tscn`](../scenes/combat.tscn) |
| **2D Lane B stage** | Manuscript silhouettes, mist/canopy layers, optional hazard strip | [`combat_presentation.gd`](../scripts/combat_presentation.gd) |
| **Scene shell** | Pause, outcome → hub, atmosphere from world, action bar, **ParryDodgeEye** setup, wind-up SFX line, defensive window tones | [`combat_root.gd`](../scripts/combat_root.gd) |
| **Rules** | `DEFENSE_WINDUP` / window signals; wind-up + bleed on `EncounterDef`; **dodge** = partial real-swing damage (`DODGE_REAL_SWING_FRACTION`); **parry** = smaller partial chip (`PARRY_INCOMING_FRACTION`) **+** riposte (0.3–0.7× strike); feint + wasted defend = **5** chip | [`combat_manager.gd`](../scripts/combat_manager.gd), [`encounter_def.gd`](../data/encounter_def.gd) |

**Binding:** Presentation scripts resolve `CombatManager` from the parent combat root, subscribe to `turn_changed`, `stats_changed`, and (where implemented) state transitions to drive motion. **No** presentation script applies damage; bleed, strikes, and parry riposte remain in `CombatManager` + encounter data.

**Coexistence:** HUD (`CanvasLayer`: eye, React label, bars) stays readable above **either** 2D or 3D stage nodes, matching the target described in [`COMBAT_VOXEL_STAGE_FUTURE.md`](COMBAT_VOXEL_STAGE_FUTURE.md) (full-screen world + overlay HUD).

---

## 2. E33 read checklist

| Expectation | Status | Notes |
|-------------|--------|--------|
| Clear **turn** vs **enemy swing** phase | **Partial** | `TelegraphLabel` / state copy + `DEFENSE_WINDUP` precedes `REAL_TIME_WINDOW`; 3D telegraph hold syncs to wind-up feel |
| **Wind-up** before reactive window | **Met** | `EncounterDef.defensive_windup_sec`; Fair Field data **0** with `MIN_GATHER_WHEN_WINDUP_ZERO_SEC` for playable gather; Dongeoun gate authored **> 0** |
| **Reactive window** duration per encounter | **Met** | `enemy_attack_reaction_window_seconds`; gated in enemy turn path per save/settings |
| **Feint vs real** read | **Partial** | `is_enemy_swing_real()`, eye phases, procedural window **beeps** (pitch); 3D albedo/telegraph tint where wired — verify per encounter in play |
| **Bleed** / between-turn pressure | **Met (data)** | `enemy_turn_bleed_damage` on `EncounterDef`; **0** Fair Field, non-zero on stress encounters (e.g. Dongeoun gate) |
| **Audio** supports read | **Met** | `CombatWindowTone` + `WindowSfx` on window open; kill thump + desat on victory (see [`PARITY_GAPS.md`](../PARITY_GAPS.md) audio row) |
| Full-screen **voxel** production stage | **Prototype** | Procedural voxel floor/wall grid [`combat_voxel_arena.gd`](../scripts/combat_voxel_arena.gd); **rim** on player/enemy `StandardMaterial3D` in `combat.tscn`; imported art + outline shader TBD — [`COMBAT_VOXEL_STAGE_FUTURE.md`](COMBAT_VOXEL_STAGE_FUTURE.md) |

---

## 3. Gaps vs TDAD combat presentation (CP) matrix

| Segment | Advance from wireframe slice | Still open / manual |
|---------|------------------------------|---------------------|
| **CP-01** API (`window_duration`, `window_remaining`, `window_phase_t`, `presentation_tick`) | **Done** — manager emits `presentation_tick` on window open + each `tick_window` frame | Subscribers should not duplicate phase work in `_process` unless ordering requires it |
| **CP-02** Stage | **Done+** — Lane B 2D + **3D** graybox arena in same scene | Asset parity, hazard strip default off; orthographic/voxel “final” stage TBD |
| **CP-03** Poses / motion | **Partial → strong** — lunge, telegraph snap, bob, hit feedback in 2D + 3D | Fine animation curves vs Unity `EnsureCharacterVisual` |
| **CP-04** Parry/dodge eye + React label | **Done** — wired from `combat_root.gd` | BDD scenario may still be `@manual` for curve QA |
| **CP-05** Audio / feedback | **Partial** — window tones + kill FX; not full Unity `CombatFeedback` parity | More one-shots, bus polish |
| **CP-06** Docs / BDD | **This doc + PARITY_GAPS** | Mark CP segments DONE in TDAD plan when you freeze a release branch |

---

## 4. Follow-ups

1. **Headless / BDD:** Keep **`combat_scenarios.gd`** focused on rules (instant resolution); do **not** require GPU for CI. Optional future: scene-tree smoke that `CombatPresentation3D` exists under `combat.tscn` (fragile — document in [`docs/CI_GODOT_TESTS.md`](../../docs/CI_GODOT_TESTS.md) if added).
2. **`godot-demo-components`:** Reuse existing `gdc-combat-*` nodes; add a **new** granular node only if a stable automated proof exists (e.g. non-GPU signal ordering test).
3. **TDAD mirrors:** Subsystem workflows `godot-combat-defense`, `godot-combat-actions`, `godot-scenes` should cite this file and the scripts above for traceability (see `.tdad/workflows/GODOT_SUBSYSTEM_CONVENTIONS.md`).

---

## 5. Quick manual pass (QA)

Use the ordered **rules + presentation** matrix in [`WIREFRAME_COMBAT_CHECKLIST.md`](WIREFRAME_COMBAT_CHECKLIST.md) for a full greybox audit; below is a **short** subset.

1. Fair Field: confirm gather + eye + React countdown feel correct with **feint** pattern from hub misstep (see [`STEAM_BUILD.md`](../STEAM_BUILD.md)).
2. Dongeoun gate (or any encounter with `defensive_windup_sec > 0` + bleed): confirm longer wind-up and tick damage between enemy actions match data.
3. Toggle 2D vs 3D presentation path as authored in `combat.tscn` — HUD remains usable and SFX fire on window open.

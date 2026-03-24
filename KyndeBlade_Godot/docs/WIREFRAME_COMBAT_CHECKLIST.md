# Wireframe combat checklist (graybox slice)

Single repeatable pass for **rules**, **presentation/audio read**, and **regression** mapping. Oracle notes: [`COMBAT_REVIEW_WIREFRAME_E33.md`](COMBAT_REVIEW_WIREFRAME_E33.md), [`PARITY_GAPS.md`](../PARITY_GAPS.md). **Manual QA shortcut:** [`STEAM_BUILD.md`](../STEAM_BUILD.md) § *Wireframe combat pass (greybox)*.

---

## 1. Rules matrix (code constants + data)

| Rule | Value / behaviour | Where |
|------|-------------------|--------|
| Player-phase swing damage | `PLAYER_PHASE_SWING_DAMAGE` (**20**) | [`combat_manager.gd`](../scripts/combat_manager.gd) |
| Dodge vs real swing | Partial chip: **20 × DODGE_REAL_SWING_FRACTION** (default **0.6 → 12**) | same |
| Parry vs real swing | Smaller chip: **20 × PARRY_INCOMING_FRACTION** (default **0.3 → 6**) **+ riposte** | same |
| Parry riposte multiplier | **0.3–0.7** × `_lowest_equipped_attack_damage()`, keyed by `_defense_windows_resolved` (window index) | `_parry_counterstrike_multiplier()` |
| Feint + wasted defend | **5** HP chip | `_resolve_window` / enemy-turn branches |
| Enemy turn base hit | `EncounterDef.enemy_turn_damage` (Fair Field **18** in default `.tres`) | [`encounter_def.gd`](../data/encounter_def.gd), encounter resources |
| Enemy-turn dodge/parry on real swing | Same fractions applied to **`enemy_turn_damage + hazard`** (hazard stub **0** until Piers hazards port) | `combat_manager.gd`, `apply_piers_hazard_damage()` |
| Bleed between enemy strikes | `EncounterDef.enemy_turn_bleed_damage` (**0** Fair Field; non-zero e.g. Dongeoun gate) | encounter `.tres` |
| Wind-up before window | `EncounterDef.defensive_windup_sec`; if **0**, playable gather uses `MIN_GATHER_WHEN_WINDUP_ZERO_SEC` | `combat_manager.gd` |
| Real-time defense on enemy turn | `SaveService.load_enable_real_time_defense_on_enemy_turn()` — **false** skips reaction window (instant damage path after telegraph timers) | [`save_service.gd`](../scripts/save_service.gd) |
| Headless resolution | `CombatManager.use_instant_resolution_for_tests` — no wall-clock timers; optional `force_enemy_turn_reaction_window_in_tests` for enemy-turn reaction path | `combat_manager.gd` |

---

## 2. Deterministic rows ↔ headless tests

| Matrix row | Automated proof |
|------------|-----------------|
| Victory by strikes only | `victory_false_by_strikes_only` in [`tests/combat_scenarios.gd`](../tests/combat_scenarios.gd) |
| Dodge mitigates player-phase real swing (12 chip) | `dodge_mitigates_first_swing` |
| Parry chip + riposte (6 chip; first-window mult 0.3) | `parry_reduces_swing_damage` |
| Feint + wasted defend (5); strike then enemy phase (18) chain | `feint_chips_if_you_defend` |
| Misstep inverts first window to feint | `misstep_inverts_first_defensive_window` |
| Strike blocked during defensive window | `strike_ignored_during_defensive_window` |
| Stamina gate on strike | `strike_fails_when_stamina_too_low` |
| Enemy-turn reaction dodge partial | `enemy_turn_reaction_dodge_partial` |
| Enemy-turn reaction parry partial + riposte | `enemy_turn_reaction_parry_partial_and_riposte` |
| Parry window ms in authored band | `parry_window_ms_stays_in_band` |
| Brutal enemy turn → defeat | `defeat_on_brutal_enemy_turn` |

**Runner:** all of the above are invoked from [`tests/run_headless_tests.gd`](../tests/run_headless_tests.gd) → step `combat_scenarios` (after scene smokes).

**Gaps (manual or future automation):**

- **3D arena motion curves**, **full HUD pixel alignment**, and **long feint/real audio A/B** — manual or `@manual` BDD; keep logic in `combat_scenarios.gd` for numbers.
- **Bleed stacking** over multiple enemy turns with non-zero `enemy_turn_bleed_damage` — covered by data on stress encounters; add a **dedicated headless scenario** if you need a numeric assert on a fixed turn count.

---

## 3. Entry paths (manual)

1. **Main menu** → **New Game** or **Continue** → **hub** (`hub_map.tscn`). *(Malvern yard / tower intro are optional scenes, not the default New Game chain.)*
2. **Fair Field:** map or button → **counsel** (Trewthe / Mede / Name hunger) → **combat** (`combat.tscn`).
3. **Dongeoun:** after Fair Field cleared, hub travel → flavor → **Enter combat** → gate encounter (wind-up **> 0**, bleed as authored).
4. **Gauntlet / reaction loop** (if enabled from menu): use as a fast multi-fight stress pass after changing combat presentation.

---

## 4. Presentation and audio (quick verify)

| Check | Files |
|-------|--------|
| Lane B 2D stage (silhouettes, mist, hazard strip) | [`combat_presentation.gd`](../scripts/combat_presentation.gd), `combat.tscn` |
| 3D graybox (telegraph, strike lunge, hit flash, sway) | [`combat_presentation_3d.gd`](../scripts/combat_presentation_3d.gd) |
| Parry/dodge **eye**, **React** label, control hints, feint-read line | [`combat_root.gd`](../scripts/combat_root.gd), `ParryDodgeEye` |
| Defensive window **beeps** (feint vs real pitch) | [`combat_window_tone.gd`](../scripts/combat_window_tone.gd) |
| Wind-up line before reactive window | `combat_root.gd` (wind-up SFX / copy) |
| Victory **kill thump** + desat | `combat_window_tone.gd` / victory path in `combat_root.gd` |

---

## 5. TDAD / BDD trace

- Workflow nodes **`gdc-combat-*`:** [`.tdad/workflows/godot-demo-components/godot-demo-components.workflow.json`](../../.tdad/workflows/godot-demo-components/godot-demo-components.workflow.json).
- Gherkin: [`.tdad/bdd/godot-parity-slice.feature`](../../.tdad/bdd/godot-parity-slice.feature) — timing/UI rows tagged **`@manual`** where proof is editor-only.

Re-run after any change to **`combat_manager.gd`**, **`EncounterDef`** fields on shipped encounters, or **presentation/audio** wiring above.

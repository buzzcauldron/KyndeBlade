# World Depth: Expedition 33–Style Consistency

This doc ties **gameplay constants**, **narrative terminology**, and **world structure** into one reference so code, content, and UI stay consistent (Expedition 33–style depth). See also ProjectArchive/docs/EXPEDITION_33_COMBAT_DESIGN.md and **GameWorldConstants.cs**.

---

## 1. Canonical Names and IDs

**Characters (display / code):**
- **Wille** — The dreamer, player protagonist. Fixed Dreamer class. Use `GameWorldConstants.PlayerDreamerName` ("Wille").
- **The Green Knight** — Superboss from Sir Gawain; wild nature, cyclical beheading. Use `GameWorldConstants.GreenKnightDisplayName`.
- **The Pilgrim** — Alternate or UI label for the seeker (e.g. manuscript UI). Use `GameWorldConstants.PilgrimDisplayName` when that framing is used.

**Locations (save, transitions, narrative):**
- `malvern` — Start of journey, dream/vision space.
- `green_chapel` — Green Knight encounter; wrong choice → Green Knight appears randomly later.
- `otherworld` — Orfeo's Otherworld (alternate ending); inescapable.
- `field_of_grace` — Post–final combat; waiting for Grace (she does not come).
- `years_pass` — Final combat location (Vision 2).

Use **GameWorldConstants.LocationX** in code instead of string literals.

---

## 2. Combat and Resources (Expedition 33–Aligned)

**Kynde (nature / spirit):**
- **Meaning:** Nature and spiritual/natural strength (Piers Plowman / Expedition 33 AP).
- **Generation:** Melee attacks, successful parry, perfect dodge (amounts in actions).
- **Consumption:** Ranged attacks, skills, heals (Kynde cost on action).
- **Default max:** 10 (`GameWorldConstants.DefaultMaxKynde`).

**Break (shield / corruption):**
- **Meaning:** Breaking through enemy defence or corruption (Expedition 33 break gauge).
- **Effect when depleted:** Target is **stunned** for **2 seconds** (`GameWorldConstants.BreakStunDurationSeconds`), takes **50% more damage** (`GameWorldConstants.BrokenDamageMultiplier`), then recovers and gauge refills.
- **Sources:** Break damage on actions, counter-attacks, some elemental skills.

**Elements (virtues / vices):**
- **Flamme** — Fire, burns.
- **Frost** — Ice, slows.
- **Thunder** — Lightning, stuns.
- **Trewthe** — Truth/holy; extra vs corruption.
- **Fals** — False/dark; corrupts.
- **Kynde** — Nature; heals, buffs.

**Elemental affinity (future):** Weakness = 2× damage, resistance = 0.5× (`GameWorldConstants.ElementalWeaknessMultiplier`, `ElementalResistanceMultiplier`). Wire per-enemy in `CombatCalculator.GetElementalMultiplier` when data exists.

---

## 3. Real-Time Defence (Escapade / Ward)

- **Escapade** — Dodge window; success avoids damage and can grant Kynde.
- **Ward** — Parry window; success reduces damage, grants Kynde, enables **Counter** (150% damage + break, no Kynde cost).

**Default windows:** 2 s for both (`GameWorldConstants.DefaultParryWindowSeconds`, `DefaultDodgeWindowSeconds`). Timing and multipliers are in **GameSettings** (e.g. `TimingWindowMultiplier`) and action **SuccessWindow** / **CastTime**. Eye phases and imminent sound cue: see **TIMING_AND_SIGNALS.md**.

---

## 4. Narrative and Thematic Hooks

**Vision / Passus:**
- **LocationNode** has `VisionIndex` and `PassusIndex` / `PassusTitle` for poem structure.
- Use for story beats, music, and which encounters are available.

**Defeat / endings:**
- **Green Chapel defeat:** "The Green Knight hath taken thy head. A form of thee ends in Orfeo's Otherworld." → Restart at Malvern (`GameWorldConstants.DefeatGreenChapelMessage`).
- **Sin miniboss defeat:** "The sin hath rent thee…" → Transition to Orfeo's Otherworld (`GameWorldConstants.DefeatSinOrfeoMessage`).
- **Death of old age:** "Death of old age. Wille's years have run their course. Grace did not come…" (`GameWorldConstants.DefeatDeathOfOldAgeMessage`).

**Otherworld / Grace:**
- Otherworld is inescapable; bodies and souls accrue there (save progress).
- Field of Grace: best outcome is waiting; Grace does not come (no true ending).

---

## 5. UI and Goal Strings

All goal/state and key defeat strings live in **GameWorldConstants** (e.g. `GoalDefeatAllEnemies`, `GoalYourTurnSelectAction`, `StateSelectAction`). Use these in **CombatUI**, **GameStateManager**, and any tutorial or narrative UI so copy is consistent and localisable later.

---

## 6. Implementation Checklist

- [x] **GameWorldConstants.cs** — Single source for names, location IDs, combat numbers, UI strings.
- [x] **CombatCalculator** — Uses constants for damage formula; `GetElementalMultiplier` stub for future affinity.
- [x] **MedievalCharacter** — Break stun duration from constants.
- [x] **CombatAction / RobustMoveset** — Broken damage multiplier from constants.
- [x] **CombatUI** — Goal and state text from constants.
- [x] **GameStateManager** — Defeat messages and location IDs from constants.
- [x] **GreenKnightCharacter** — Display name from constants.
- [ ] **Optional:** Per-enemy elemental weakness/resistance data and wire into `GetElementalMultiplier`.
- [ ] **Optional:** Aging modifier in `CombatCalculator.CalculateDamage` when AgingManager provides a stat modifier.

Use this doc and **GameWorldConstants** when adding new locations, bosses, or UI so the world stays consistent and Expedition 33–style depth is preserved.

# Building Mechanics: 15+ Iterative Playthroughs

Design for meta-progression that makes the game **change slightly** across 15+ playthroughs. One persistent counter drives small, cumulative shifts in combat, narrative, and world so each run feels like the same journey seen again, with the world “remembering.”

---

## 1. Persistent meta progress (survives NewGame)

**Problem:** `GameProgress` is wiped on `SaveManager.NewGame()`, so run-scoped data (locations, ethical missteps) resets. We need a **lifetime** layer that never resets so we can key mechanics to “how many times the player has played.”

**Proposal: MetaProgress (separate persistence)**

- **Storage:** Same place as save (e.g. `PlayerPrefs` key `"KyndeBlade_Meta"`) or a small `MetaProgress` object saved alongside. Loaded in `SaveManager.Awake()` (or first access) and never cleared by `NewGame()`.
- **Fields (suggested):**
  - **`LifetimeRunCount`** (int): Number of **completed runs** (run “ends” on victory at final combat, or on defeat that triggers restart / New Game). Increment when:
    - Victory at final combat (e.g. `years_pass`), or
    - Defeat at Green Chapel → before calling `NewGame("malvern")`, or
    - Death of old age → before restart.
  - **`LifetimeRunAttempts`** (int, optional): Total runs **started** (increment in `NewGame()` or on first-ever load). Use if you want “attempts” vs “completions” separately.

**Primary driver for “building” mechanics:** `LifetimeRunCount` (or a clamped/capped value like `Mathf.Min(LifetimeRunCount, 20)` so effects plateau).

**Where to implement:**

- Add `MetaProgress` class (or struct) with `LifetimeRunCount`, serialization, and static/instance load/save.
- `SaveManager`: hold `MetaProgress`, load in `Awake`, expose e.g. `int LifetimeRunCount { get; }`, `void IncrementLifetimeRunCount()` (and save meta).
- **Increment points:**
  - **GameStateManager:** In `Victory()` when `IsFinalCombatLocation(gm.LastEncounterLocation?.LocationId)` → before showing victory panel, call `SaveManager.IncrementLifetimeRunCount()` (or “complete run”).
  - **GameStateManager:** In `RestartGameAfterDefeat()` and in `TriggerDeathOfOldAge()` path when restart = New Game → call `SaveManager.IncrementLifetimeRunCount()` before `NewGame("malvern")`.
  - Optionally in `NewGame()`: increment `LifetimeRunAttempts` only (so “runs started” vs “runs ended” are both available).

---

## 2. Mechanics that “build” across 15+ runs

Each mechanic should **change slightly** as `LifetimeRunCount` (or capped value) goes from 1 → 15+. Prefer soft scaling and branching over hard unlocks so the experience evolves rather than spikes.

---

### 2.1 Crit system (TurnManager)

- **Current:** Crit pressure builds per hit, caps at `CritPressureCap`, resets on crit; multiplier `CritDamageMultiplier`.
- **Building change:** Slight scaling with run count so later runs feel a bit more “tense” or rewarding.
  - **Option A – Pressure builds faster:**  
    `effectiveCritBuildPerDeal = CritBuildPerDeal * (1f + 0.02f * runIndex)`  
    e.g. run 1: 0.08, run 10: ~0.096, run 15: ~0.104. Small; crits come a bit sooner in long fights.
  - **Option B – Cap eases up:**  
    `effectiveCritPressureCap = Mathf.Min(0.5f, CritPressureCap + 0.01f * runIndex)`  
    so max crit chance creeps up over runs.
  - **Option C – Multiplier creeps:**  
    `effectiveCritMultiplier = CritDamageMultiplier + 0.02f * runIndex`  
    (e.g. 1.5 → 1.8 by run 15). Keep capped (e.g. 2f) to avoid one-shots.

Use **one** of these (or a single combined scaling factor from run count) so the system doesn’t explode. Expose `LifetimeRunCount` (or capped) to `TurnManager` and apply in `ApplyCritToDamage` / `RecordDamageDealt` (or in a small helper that returns the scale factor for the current run).

---

### 2.2 Damage taken / ethical weight (SaveManager)

- **Current:** `GetDamageTakenMultiplier()` uses `EthicalMisstepCount` (per-run).
- **Building change:** Slight “memory” of past runs so repeated playthroughs carry a bit more consequence.
  - **Idea:** `effectiveDamageTaken = baseDamageTaken * (1f + 0.02f * Min(LifetimeRunCount, 15))`.  
    So by run 15, everyone takes ~30% more damage regardless of this run’s choices — the world is heavier. Optionally only apply after run 5 so early runs stay “vanilla.”
  - Alternative: use `LifetimeRunCount` only for **narrative** (e.g. dialogue branches) and leave damage formula run-scoped; then this subsection is “optional / narrative-only.”

---

### 2.3 Kynde gain (combat / wisdom)

- **Current:** Kynde from actions (e.g. Strike 10, parry/dodge windows).
- **Building change:** Slightly more Kynde per action as runs increase, so later runs “reward wisdom” a bit more.
  - **Formula:** `effectiveKyndeGain = baseKyndeGain * (1f + 0.03f * Min(LifetimeRunCount, 15))`.  
    Run 1: 1.0x, run 15: 1.45x. Keeps economy stable but makes high-run play feel slightly more generous.
  - Hook: wherever `CalculateKyndeGain` or the actual Kynde add is applied, multiply by a factor from a small helper that reads `GameRuntime.SaveManager` (or `GameProgress` if you store a copy of run count there) and returns the scale.

---

### 2.4 Elde / aging (AgingManager)

- **Current:** `EldeHitsAccrued` (and possibly aging tiers) affect characters.
- **Building change:** Slight increase in aging **impact** over runs (e.g. each Elde hit ages a bit more in run 10 than in run 1), or Elde appears **slightly** earlier in the encounter order after many runs.  
  - Keep the effect small (e.g. 2–5% more aging per run index, capped) so it’s felt as “the world remembers” rather than a difficulty spike.

---

### 2.5 Narrative / dialogue (memory of runs)

- **Building change:** Use `LifetimeRunCount` (and optionally `GreenChapelBodiesAccrued` if you persist it across NewGame — currently it’s lost on NewGame) for **dialogue branches** or **narrative state**.
  - **Green Knight / Chapel:** After many runs, one extra line: “Thou hast stood before me many times.” (Branch on `LifetimeRunCount >= 5` or similar.)
  - **Orfeo’s Otherworld:** “The forms of thy former selves gather here.” (Branch on `LifetimeRunCount` or `OtherworldBodiesFromDeath` if that’s ever made to persist.)
  - **Fae / Wode-Wo:** Unlock a single extra line or beat after run 10 or 15 so the story “opens” a little more.

Implementation: narrative system (e.g. Yarn / DialogueSystem) reads a variable like `LifetimeRunCount` from SaveManager (or a bridge) and branches in script or node conditions.

---

### 2.6 Encounter / Fae appearance (world state)

- **Current:** `EncountersSinceLastGreenKnight`, Fae appearance logic.
- **Building change:** Slight increase in **Fae appearance chance** (or frequency of “special” encounters) as run count increases, so the otherworld feels closer in later runs.
  - **Formula:** `extraFaeChance = 0.01f * Min(LifetimeRunCount, 15)` (add 1% per run, cap 15%).  
  - Hook: wherever Fae appearance is rolled, add this to the roll or probability.

---

### 2.7 Optional: One soft “unlock” after N runs

- **Idea:** After `LifetimeRunCount >= 10`, one small mechanical unlock: e.g. one extra starting Kynde, or one additional dialogue option that leads to a minor bonus (extra heal, one-time damage reduction).  
- Keep it **soft** (slight numeric or one extra option) so the game “changes slightly” rather than a big new system.

---

## 3. Implementation order (suggested)

1. **Meta progress:** Add `MetaProgress`, persist under `"KyndeBlade_Meta"`, load in SaveManager; add `LifetimeRunCount` and `IncrementLifetimeRunCount()`; call increment in GameStateManager on victory-at-final, Green Chapel restart, and death-of-old-age restart.
2. **Crit (TurnManager):** Expose run count to TurnManager (or a small `RunScaling` helper); apply one of 2.1 A/B/C so crit “builds” slightly across runs.
3. **Kynde (2.3):** Apply run-count scale to Kynde gain.
4. **Narrative (2.5):** Expose `LifetimeRunCount` to dialogue/narrative and add 1–2 branches for Green Knight / Otherworld.
5. **Optional:** Damage taken (2.2), Elde (2.4), Fae (2.6), soft unlock (2.7) as desired.

---

## 4. Summary

- **Persistent run count** that survives `NewGame()` and increments when a run **ends** (final victory, Green Chapel defeat, or death of old age).
- **Small, scaling effects** keyed to that count (crit, Kynde, optional damage taken, Elde, Fae) so the game changes **slightly** over 15+ runs.
- **Narrative “memory”** via 1–2 dialogue branches so the world acknowledges repeated journeys.
- All formulas use a **capped** run index (e.g. `Min(LifetimeRunCount, 15)` or `20`) so effects plateau and the game stays stable for 15+ playthroughs.

This keeps the current feel for run 1 and gradually shifts tension, reward, and tone as the player plays the game 15+ times iteratively.

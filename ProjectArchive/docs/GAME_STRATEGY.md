# Game Strategy & Design Decisions
## Kynde Blade — Consolidated Strategy Content

This document consolidates all strategic design decisions, implementation status, and gameplay systems. See also: [PHASE3_PLAN.md](PHASE3_PLAN.md), [WODE_WO_COMPANION.md](WODE_WO_COMPANION.md), [DREAM_REAL_LIFE_MALVERN.md](DREAM_REAL_LIFE_MALVERN.md).

---

## 1. Core Design Principle: No Satisfying Ending

**Grace is never found. The quest remains unresolved.** (GameProgress.cs)

- **Best outcome**: Reaching the Field of Grace — a place of unresolved waiting
- **Field of Grace**: No combat, no next locations
- **Perfect run (Wode-Wo alive)**: Wode-Wo makes it to the field with you — *"Wode-Wo standeth beside thee. Together ye waitest for Grace. She does not come."*
- **Wode-Wo dead**: *"Thou waitest for Grace. She does not come. The game runs."*
- **Death of old age**: While waiting, time passes (accelerated at the field). When AgeTier >= threshold, player dies — *"Death of old age. Wille's years have run their course. Grace did not come."* — Restart = New Game.
- **Flow**: Years Pass victory → "Thou hast reached the field. Continue." → TransitionTo(field_of_grace) → StoryBeat → Map with waiting message
- **LocationNode.IsWaitingForGrace**: Marks the unresolved waiting state

---

## 2. Map Flow & Linear Path

**MVP linear path:**
```
Malvern → Fayre Felde → Tour → Dongeoun → Piers Field → Seven Sins → Quest Do-Wel → Dongeoun Depths → Years Pass → Field of Grace
```

- **Buildings** (LocationNode.IsBuilding): Tour, Dongeoun, Dongeoun Depths — enable Green Knight first appearance and Green Chapel random discovery
- **Poverty locations**: Dongeoun (PovertyLevelOnArrival: 1), Dongeoun Depths (2)

---

## 3. Green Knight Cycle

### First Appearance
- **Trigger**: Entering any building (Dongeoun, Dongeoun Depths) with combat
- **Chance**: `GreenKnightFirstAppearChanceInBuilding` (default 15%)
- **Effect**: Spawns Green Knight, sets `GreenKnightWillAppearRandomly = true` — starts the cycle

### Subsequent Appearances
- **Trigger**: Wrong dialogue choice at Green Chapel (PreCombatChoiceBeat) OR first appearance already happened
- **Chance**: `GreenKnightRandomAppearChance` (default 25%) per encounter
- **Excluded**: Green Chapel (never random there)

### Technical
- KyndeBladeGameManager.StartEncounterFromConfig: rolls for first appearance (buildings) then subsequent (cycle active)
- SaveManager.SetGreenKnightWillAppearRandomly, GameProgress.GreenKnightWillAppearRandomly

---

## 4. Green Chapel Discovery

- **Random destination**: When at a building, `GetNextLocations()` rolls `GreenChapelRandomAppearChance` (default 20%)
- **Effect**: Green Chapel added to destination list, unlocked for travel
- **Always loaded**: Green Chapel loaded regardless of MVPLinearOnly
- **WorldMapManager**: GetNextLocations includes random Green Chapel when CurrentLocation.IsBuilding

---

## 5. Poverty System

### Location-Based
- **LocationNode.PovertyLevelOnArrival** (0–5, -1 = no change)
- **WorldMapManager.TransitionTo**: On arrival, if PovertyLevelOnArrival >= 0, calls SaveManager.SetPovertyLevel
- **Dongeoun**: PovertyLevel 1
- **Dongeoun Depths**: PovertyLevel 2

### Combat Modifiers
- **PovertyManager.GetStaminaRegenMultiplier()**: Called in MedievalCharacter.RestoreStamina
- **PovertyManager.GetKyndeGenMultiplier()**: Called in MedievalCharacter.GainKynde
- **Source**: PovertyManager.CurrentPovertyLevel reads from SaveManager.CurrentProgress.PovertyLevel

---

## 6. Victory & Defeat Flow

### Victory
- **Continue button**: Returns to map or transitions to Field of Grace (if final combat at years_pass)
- **Final victory text**: "Thou hast reached the field. Continue."
- **Normal victory text**: "Victory! +X XP"
- **Checkpoint**: Saved before transition

### Defeat
- **Restart button**: When CanRestartAtCheckpoint; calls RestartAtCheckpoint
- **Major boss defeat**: Wode-Wo death cutscene before defeat panel
- **Sin miniboss defeat**: Transition to Orfeo Otherworld
- **Green Chapel death**: Increments GreenChapelBodiesAccrued

---

## 7. Wode-Wo (Install-Permanent Companion)

- **Death trigger**: Wrong dialogue choice OR defeat to major boss (Hunger, Green Knight, Pride, sin miniboss)
- **Persistence**: InstallState.WodeWoIsDead — never cleared by NewGame
- **Map reminder**: "Wode-Wo's scattered remains lie at Malvern. The forest mourns."
- **StoryBeatOnArrivalWhenWodeWoDead**: Shown instead of StoryBeatOnArrival when returning to Malvern

---

## 8. Combat Strategy Systems

### Kynde Economy
- **Generate**: Melee hits, perfect dodge/parry timing
- **Spend**: Skills, Heal, special actions
- **Modifiers**: Poverty, Hunger, Age, HungerScar reduce gains

### Break System
- **Break gauge**: Depleted by BreakDamage; at 0 → Broken (stun)
- **Recovery**: After stun duration, gauge refills

### Real-Time Window (Escapade/Ward)
- **Dodge (Escapade)**: Space / A
- **Parry (Ward)**: Left Shift / X
- **Counter**: E / Y (after parry)
- **Perfect timing**: Last 25–30% of window grants bonus Kynde

### Adaptive AI
- Enemies learn from dodge/parry success; favor Strike when Escapade/Ward succeed often

---

## 9. Accrual Systems

| System | Trigger | Stored In |
|--------|---------|-----------|
| Green Chapel bodies | Death at Green Chapel | GameProgress.GreenChapelBodiesAccrued |
| Otherworld living | Enter Otherworld (tree choice or sin defeat) | GameProgress.OtherworldLivingCharactersAccrued |
| Encounters since Green Knight | Incremented each encounter; reset when Green Knight appears | GameProgress.EncountersSinceLastGreenKnight |
| Hunger scars | HasEverHadHunger; permanent scars | GameProgress.HasEverHadHunger |

---

## 10. LocationNode Flags Reference

| Flag | Use |
|------|-----|
| IsBuilding | Green Knight first appearance; Green Chapel random destination |
| IsWaitingForGrace | Map shows "Thou waitest for Grace. She does not come. The game runs." |
| IsInescapable | Orfeo Otherworld; no return to map |
| IsAlternateEnding | Marks alternate ending paths |
| IsRealLife | Malvern, England interstitial locations |
| PovertyLevelOnArrival | Sets GameProgress.PovertyLevel on arrival (0–5, -1 = no change) |

---

## 11. Configuration Reference

| Component | Setting | Default |
|-----------|---------|---------|
| KyndeBladeGameManager | GreenKnightFirstAppearChanceInBuilding | 0.15 |
| KyndeBladeGameManager | GreenKnightRandomAppearChance | 0.25 |
| WorldMapManager | GreenChapelRandomAppearChance | 0.2 |
| WorldMapManager | MVPLinearOnly | true |
| PovertyManager | StaminaRegenModifierPerLevel | 0.85 |
| PovertyManager | KyndeGenModifierPerLevel | 0.9 |
| AgingManager | AgeTierDeathThreshold | 10 |
| AgingManager | FieldOfGraceTimeMultiplier | 60 (1 real sec = 1 min aging at field) |

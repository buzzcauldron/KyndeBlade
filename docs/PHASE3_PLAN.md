# Phase 3 Plan
## Kynde Blade — Next Development Phase

**Scope:** Polish, integration, and remaining systems. Builds on Phase 1–2, Wode-Wo, Hunger scars, fairy/sin mechanics.

---

## 1. Critical (Must-Have)

### 1.1 Victory → Return to Map
- [ ] Victory panel "Continue" button returns player to map (WorldMapManager)
- [ ] Save checkpoint on victory before transition
- [ ] Ensure MapLevelSelectUI refreshes with new location

### 1.2 Apply Poverty Modifiers in Combat
- [ ] Verify PovertyManager.GetStaminaRegenMultiplier() is called in RestoreStamina (done)
- [ ] Verify PovertyManager.GetKyndeGenMultiplier() is called in GainKynde (done)
- [ ] Ensure PovertyLevel is set from GameProgress (SaveManager) at encounter start
- [ ] Test: poverty level 1+ reduces stamina/Kynde gains visibly

### 1.3 Level Data & Prefab Assignment
- [ ] Run **KyndeBlade > Create MVP Level Data (Linear)** in Unity
- [ ] Assign WillePrefab, HungerPrefab, PridePrefab, GreenKnightPrefab on KyndeBladeGameManager
- [ ] Assign WodeWoDeathBeat on WodeWoManager (or rely on Resources load)
- [ ] Verify Malvern → Fayre → Tour → Dongeoun → Piers → Seven Sins → Quest Do-Wel → Dongeoun Depths → Years Pass flow

### 1.4 Ethical Misstep Counter (Iterative Punishment)
- [ ] Add `EthicalMisstepCount` to GameProgress
- [ ] Increment on wrong dialogue choice (sin-aligned or Green Chapel refuse)
- [ ] Apply cumulative modifiers: e.g. +X% damage taken per misstep, or harder encounters
- [ ] Design: how many missteps before noticeable punishment? (e.g. 1–5 scale)

---

## 2. Recommended

### 2.1 Dialogue from Source Texts
- [ ] Decide language level (Middle English / Modern / Hybrid) per DIALOGUE_SOURCE_TEXT_PLAN.md
- [ ] Update Green Chapel DialogueChoiceBeat with chosen text
- [ ] Update Boundary Tree (Orfeo) DialogueChoiceBeat
- [ ] Update Malvern/Fayre/Tour/Piers/SevenSins StoryBeats with source excerpts

### 2.2 Wode-Wo Polish
- [ ] Add Wode-Wo portrait/sprite for dialogue (optional)
- [ ] Ensure tutorial shows Wode-Wo beats when UseWodeWoVoice = true
- [ ] Test: wrong choice → Wode-Wo death cutscene → flow continues
- [ ] Test: defeat to Hunger → Wode-Wo death cutscene → defeat panel

### 2.3 Dream vs Real-Life Malvern
- [ ] Add interstitial Malvern nodes (St. Anne's Well, Great Malvern Priory) if desired
- [ ] MapLevelSelectUI shows "Malvern, England" for IsRealLife locations (done)
- [ ] Story beats for real-life transitions ("Wille wakes. The Beacon stands above.")

### 2.4 Combat Feedback
- [ ] Parry/Dodge input: confirm Space/Shift/E work in real-time window
- [ ] Kynde and Stamina display during combat (CombatUI)
- [ ] Action tooltips: show Stamina/Kynde cost on buttons

---

## 3. Nice-to-Have

### 3.1 Art & Audio
- [ ] Placeholder sprites for characters (colored quads if no art)
- [ ] Combat background: tiled or simple parallax
- [ ] Sound effects: assign clips to CombatFeedback
- [ ] Orfeo theme: verify haunting filters (pitch, lowpass, reverb)

### 3.2 Testing
- [ ] Fill TEST_REPORT_PHASE2.md after playtest
- [ ] Add PlayMode tests for critical paths (optional)
- [ ] InstallState.ResetForTesting for Wode-Wo (dev only)

---

## 4. Phase 3 Order of Operations

1. **Bootstrap** — Level data creation, prefab assignment, full run-through
2. **Victory flow** — Return to map, checkpoint on win
3. **Poverty** — Verify modifiers in combat
4. **Ethical misstep** — Counter + cumulative punishment
5. **Dialogue** — Source text decisions + asset updates
6. **Polish** — Wode-Wo, UI, feedback

---

## 5. Success Criteria

- [ ] Player can complete Malvern → Years Pass (or Green Chapel / Otherworld) without blockers
- [ ] Victory returns to map; defeat shows restart when applicable
- [ ] Poverty and aging affect combat meaningfully
- [ ] Wode-Wo tutorial and death flow work as designed
- [ ] Wrong choices and major boss defeats trigger Wode-Wo death (install-permanent)

# Next Moves — Kynde Blade Phase 3

**Last updated:** After implementing Ethical Misstep counter and damage-taken penalty.

---

## Done This Session

- **Ethical Misstep Counter**
  - `GameProgress.EthicalMisstepCount` added; serialized in save.
  - Incremented in `WorldMapManager.OnChoiceProceed` when player picks a wrong dialogue choice (Green Chapel refuse/flee, Boundary Tree “follow music”, or any sin-aligned choice).
  - **Cumulative punishment:** `SaveManager.GetDamageTakenMultiplier()` returns `1 + EthicalMisstepCount * 0.1` (10% more damage taken per misstep). Applied in `MedievalCharacter.ApplyCustomDamage` for player party only (via `PiersAppearanceData.IsPlayer`).

- **Phase 3 plan** updated: 1.4 Ethical Misstep items marked complete.

---

## Immediate Next (Unity Editor)

1. **Prefab assignment (KyndeBladeGameManager)**  
   In Unity: select the GameObject with `KyndeBladeGameManager`, in the Inspector assign:
   - **WillePrefab** — player dreamer (or leave unset to fall back to KnightPrefab).
   - **HungerPrefab**, **PridePrefab**, **GreenKnightPrefab** — create or use existing character prefabs for these bosses.  
   If unassigned, the manager spawns placeholder characters via `GetEnemyPrefabByType` / `GetEnemyPrefabBySin`; assigning prefabs gives correct visuals and behaviour.

2. **Wode-Wo death beat**  
   There is no separate `WodeWoManager` in the project. Wode-Wo is driven by:
   - `LocationNode.StoryBeatOnArrivalWhenWodeWoDead` (e.g. Malvern shows WodeWoRemains when Wode-Wo is dead).
   - Story beats (WodeWoBaby, WodeWoCare, WodeWoGrown, WodeWoDeath, WodeWoRemains) are created in `CreateVision1LevelData` and referenced from location nodes.  
   No code change needed for “Resources load”; ensure those StoryBeat assets exist under `Assets/Resources/Data/` and are assigned on the relevant LocationNodes.

3. **Poverty test**  
   - Set a location’s `PovertyLevelOnArrival` to 1+ (e.g. in a LocationNode asset or via debug).
   - Run combat and confirm stamina regen and Kynde gains are reduced (PovertyManager multipliers are already wired in RestoreStamina / GainKynde).

---

## Recommended Next

- **Dialogue from source texts**  
  Decide language (Middle English / Modern / Hybrid) per `docs/DIALOGUE_SOURCE_TEXT_PLAN.md`, then update:
  - Green Chapel `DialogueChoiceBeat`
  - Boundary Tree (Orfeo) `DialogueChoiceBeat`
  - Malvern/Fayre/Tour/Piers/Seven Sins StoryBeats

- **Wode-Wo polish**  
  - Optional: Wode-Wo portrait/sprite for dialogue.
  - Test: wrong choice → Wode-Wo death cutscene → flow continues; defeat to Hunger → Wode-Wo death → defeat panel.
  - Use `LocationNode.StoryBeatOnArrival` / `StoryBeatOnArrivalWhenWodeWoDead` and existing WodeWo story beats; no WodeWoManager required.

- **Dream vs real-life Malvern**  
  - Add interstitial nodes (e.g. St. Anne's Well, Great Malvern Priory) if desired.
  - Add story beats for real-life transitions; keep using `IsRealLife` and `MapLevelSelectUI` “Malvern, England” for real-life locations.

---

## Testing

- **Playtest:** Run full path Malvern → Years Pass (or Green Chapel / Otherworld), make one wrong dialogue choice, then take combat damage and confirm it’s 10% higher (one misstep). Repeat with 2–3 wrong choices to confirm scaling.
- **Phase 2 report:** Fill `docs/TEST_REPORT_PHASE2.md` after a full playtest.

---

## Commit Summary (this session)

- `GameProgress` + `GameProgressData`: added `EthicalMisstepCount` and serialization.
- `SaveManager`: `IncrementEthicalMisstep()`, `GetDamageTakenMultiplier()`.
- `WorldMapManager.OnChoiceProceed`: call `IncrementEthicalMisstep()` when `!isCorrect`.
- `MedievalCharacter.ApplyCustomDamage`: apply damage-taken multiplier for player party.
- `docs/PHASE3_PLAN.md`: 1.4 Ethical Misstep checked off.
- `docs/NEXT_MOVES.md`: added (this file).
- Uncommitted asset changes (GreenChapelChoice, BoundaryTreeChoice, Loc_malvern) remain as local edits; commit with the above if desired.

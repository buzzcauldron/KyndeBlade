# Unity Kynde Blade — story, spawn, and combat mechanics (extracted digest)

This document **re-derives** design facts from the **Unity** implementation under [`KyndeBlade_Unity/`](../KyndeBlade_Unity/) (C#, `LocationNode`, `EncounterConfig`, `StoryBeat` YAML, `GameProgress`). Use it as the **authoritative bridge** when porting narrative and encounter logic to Godot or when authoring **medieval-text → moveset** data in [`../KyndeBlade_Godot/data/medieval_text_unlocks.json`](../KyndeBlade_Godot/data/medieval_text_unlocks.json).

**Sources of truth in Unity**

| System | Primary code / assets |
|--------|------------------------|
| Encounter spawn | [`KyndeBladeGameManager.StartEncounterFromConfig`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/KyndeBladeGameManager.cs), [`EncounterConfig`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/Map/EncounterConfig.cs) |
| Party spawn order | `SpawnPartyInPoemOrder` in same file |
| Green Knight injection | Same file + [`SaveManager`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Core/Save/SaveManager.cs) flags |
| Fairy form | [`FaeAppearanceManager`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/Game/FaeAppearanceManager.cs), [`MedievalCharacter.ApplyFairyForm`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/Characters/MedievalCharacter.cs) |
| Map / arrival flow | [`WorldMapManager`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/Map/WorldMapManager.cs), [`LocationNode`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/Map/LocationNode.cs) |
| Progress / ethics / hunger | [`GameProgress`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Core/Save/GameProgress.cs) |
| Player + enemy moves | [`Expedition33Moveset`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/Expedition33Moveset.cs), [`GreenKnightMoveset`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/GreenKnightMoveset.cs) |
| World IDs / tuning | [`GameWorldConstants`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Core/Game/GameWorldConstants.cs) |

---

## 1. Party (character) spawn

**Rule (poem order):** [`SpawnPartyInPoemOrder`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/KyndeBladeGameManager.cs)

| Slot | Character | Condition | Position (approx.) | Class |
|------|-----------|-----------|-------------------|--------|
| Always | **Wille** (dreamer) | Always | `(-4, 0, 0)` | `CharacterClass.Dreamer` (falls through to basic Expedition set in `ApplyToCharacter`) |
| Optional | **Piers** | `SaveManager.HasVisited("piers")` | `(-3.5, -1, 0)` | Knight → `KnightSet()` |
| Optional | **Conscience** | Visited `quest_do_wel` **or** `years_pass` **or** `field_of_grace` | `(-3.5, 1, 0)` | Mage → `MageSet()` |

**Moveset application:** `SpawnCharacter` calls `Expedition33Moveset.ApplyToCharacter(c, false)` when `UseExpedition33Moveset` is true. **Dreamer** is not Knight/Mage/Archer/Rogue → gets **`BasicSet()`** (Melee, Ranged, Escapade, Ward, Rest). Allies get class-specific subsets.

---

## 2. Enemy spawn (EncounterConfig)

**Rule:** [`StartEncounterFromConfig`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/KyndeBladeGameManager.cs)

1. Clear party/enemy lists; spawn party (above).
2. For each `EncounterConfig.EnemySpawnEntry`: instantiate `Prefab` or resolve `CharacterTypeName` via `GetEnemyPrefabByType`, place at `Position` or auto-formation around `EnemyFormationOffset` / `EnemySpacing`.
3. If `BossCharacterType` / `BossPrefab` set, spawn boss at `BossPosition`.
4. Configure **Piers hazards** from encounter list or `LocationNode.CombatHazards` (unless location `SuppressCombatHazards`).
5. `ApplyHungerScarToParty`, ensure **FaeAppearanceManager**, then start combat.

**Example — vertical slice (Fair Field):** [`FayreFeldeEncounter.asset`](../KyndeBlade_Unity/Assets/Resources/Data/Vision1/FayreFeldeEncounter.asset) — one row: `CharacterTypeName: False`, position `(4, -0.75, 0)`.

**Example — Green Chapel:** [`GreenChapelEncounter.asset`](../KyndeBlade_Unity/Assets/Resources/Data/GreenChapel/GreenChapelEncounter.asset) — `BossCharacterType: GreenKnight`, empty `Enemies` list.

---

## 3. Green Knight spawn (extra boss injection)

**Not** part of the encounter list alone: logic in `StartEncounterFromConfig` after normal spawns.

| Phase | Condition | Effect |
|-------|-----------|--------|
| First appearance | `location.IsBuilding`, not `green_chapel`, `!GreenKnightWillAppearRandomly`, random `< GreenKnightFirstAppearChanceInBuilding` (default **0.15**) | Spawns **GreenKnightPrefab** offset from boss position; sets `GreenKnightWillAppearRandomly = true`; `EncountersSinceLastGreenKnight = 0` |
| Subsequent | `GreenKnightWillAppearRandomly`, not at `green_chapel`, random `< GreenKnightRandomAppearChance` (default **0.25**) | Same spawn; resets encounter counter |
| Counter | Any encounter without GK | `EncountersSinceLastGreenKnight++` |

**Narrative / dialogue:** Wrong choices at Green Chapel (and dialogue consequences) tie into `SetGreenKnightWillAppearRandomly` (see [`DialogueTreeGenerator`](../KyndeBlade_Unity/Assets/Editor/DialogueTreeGenerator.cs) patterns).

---

## 4. Faerie (fairy form) spawn / transform

**Component:** [`FaeAppearanceManager`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/Game/FaeAppearanceManager.cs)

- On a timer (after `InitialDelay`, then every `MinIntervalBetweenFae`), rolls **base** chance `BaseFaeChance` (**0.15**).
- **Bonus** `NearGreenKnightFaeBonus` (**0.25**) if `GreenKnightWillAppearRandomly` **or** `EncountersSinceLastGreenKnight <= 2`.
- Picks **one random living** character from **union** of player + enemy lists (excluding already-fairy); applies `ApplyFairyForm(FairyFormDuration)` (default **25** s).
- Clears form on combat end.

**Alignment note:** Fae is a **runtime transform** on an existing spawned character, not a separate prefab row in `EncounterConfig`.

---

## 5. Locations and story beats (data inventory)

**Location IDs** (non-exhaustive; from `Assets/Resources/Data/**/*.asset`):

| LocationId | Display / role | Combat / notes |
|------------|----------------|------------------|
| `malvern` | Real-life / prologue | `MalvernPrologue` beat |
| `tour` | Tower on the toft | `StoryBeatOnArrival` → **tower vista** (`BeatId: tower_vista`); **no** encounter; `SuppressCombatHazards: 1` |
| `fayre_felde` | Fair Field | `FayreFeldeEncounter` — **False** |
| `dongeoun` | Dungeon | `DongeounEncounter` |
| `piers_field` | Piers field | `PiersFieldEncounter` |
| `seven_sins` | Seven sins | `SevenSinsEncounter` |
| `green_chapel` | Green Chapel | `GreenChapelEncounter` — **GreenKnight** boss; `PreCombatChoiceBeat` |
| `boundary_tree` | Boundary / Orfeo branch | `BoundaryTreeEncounter`, choice beat |
| `otherworld` | Fairy realm / inescapable | `OtherworldEncounter`, `OtherworldArrival` |
| `quest_do_wel` | Do-Wel quest | `QuestDoWelEncounter` |
| `years_pass` | Years pass | `YearsPassEncounter` |
| `field_of_grace` | Field of Grace | (grace theme) |
| `dongeoun_depths` | Depths | `DongeounDepthsEncounter` |

**Story beat IDs** (from `BeatId` in assets): `tower_vista`, `MalvernPrologue`, `TourDongeoun`, `FayreFelde`, `hunger_reflection`, Wode-Wo chain (`WodeWoBaby`, `WodeWoCare`, `WodeWoGrown`, `WodeWoRemains`, `WodeWoDeath`), `PiersField`, `SevenSins`, `QuestDoWel`, `YearsPass`, `field_of_grace`, `field_of_grace_wodewo_dead`, `DongeounDepths`, `GreenChapelChallenge`, `BoundaryTree`, `OtherworldArrival`.

**LocationNode narrative hooks:** `StoryBeatOnArrival`, `StoryBeatSequenceOnArrival`, `StoryBeatOnArrivalWhenHasEverHadHunger`, `StoryBeatOnArrivalWhenWodeWoComplete`, `StoryBeatOnArrivalWhenWodeWoDead`, `PreCombatChoiceBeat`.

---

## 6. Save / progression (mechanical fields)

From [`GameProgress`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Core/Save/GameProgress.cs):

- `CurrentLocationId`, `VisitedLocationIds`, `UnlockedLocationIds`, `VisionIndex`
- `GreenKnightWillAppearRandomly`, `EncountersSinceLastGreenKnight`
- `OrfeoOtherworldTriggered`, `GreenChapelBodiesAccrued`, `OtherworldLivingCharactersAccrued`, `OtherworldBodiesFromDeath`
- `EldeHitsAccrued`, `RunAppearanceSeed`
- `HasEverHadHunger`, `EthicalMisstepCount`
- `WodeWoArcStage`, `WodeWoUnlocked`

---

## 7. Movesets (mechanical summary)

### Expedition 33–style (party / generic enemies)

- **Melee** generates **Kynde**; **ranged / skills** consume Kynde. Actions include Melee/Heavy/Break strike, Ranged/Piercing, elemental (Flamme, Frost, Thunder, Trewthe), Kynde Heal, **Escapade** (dodge window), **Ward** (parry window), **Counter**, **Rest**.
- **Enemy set:** Melee/Heavy/Break, Escapade, Ward, Rest (no heal/ranged).
- **Class sets:** Knight, Mage, Archer, Rogue subsets in `ApplyToCharacter`.

### Green Knight (boss)

[`GreenKnightMoveset`](../KyndeBlade_Unity/Assets/KyndeBlade/Code/Combat/GreenKnightMoveset.cs): **Beheading Blow**, **Wild Nature's Wrath** (AoE party), **The Green Chapel's Curse** (self heal + defense buff), plus Melee Strike + Rest.

---

## 8. Godot alignment

- **Party spawn lane** ↔ `party_wille` / `party_ally` in [`medieval_text_unlocks.json`](../KyndeBlade_Godot/data/medieval_text_unlocks.json).
- **Fae lane** ↔ `fae_modifies` (chance / duration modifiers; future: hook when Godot gains fae system).
- **Green Knight lane** ↔ `green_knight_ecology` (spawn probability / counter modifiers; future: export from Unity save flags).

See [`../KyndeBlade_Godot/docs/MEDIEVAL_TEXT_MOVESET_DESIGN.md`](../KyndeBlade_Godot/docs/MEDIEVAL_TEXT_MOVESET_DESIGN.md) for the **text-read → moveset grant** model and **hidden detrimental** rules.

# Kynde Blade — Dialogue & Choice Structure from Source Texts

**Purpose:** Map conversation lines and response choices to Piers Plowman, Sir Gawain and the Green Knight, and Sir Orfeo. Use this plan to decide tone, language level, and which excerpts to use.

---

## Quick Reference: What Needs Dialogue

| Location | Type | Source Text | Current Status |
|----------|------|-------------|----------------|
| Malvern Hilles | StoryBeat | Piers Plowman Prologue | Placeholder (ME fragment) |
| Fayre Felde | StoryBeat | Piers Plowman Prologue | Placeholder (1 line) |
| Tour / Dongeoun | StoryBeat | Piers Plowman | Placeholder (1 line) |
| Piers' Field | StoryBeat | Piers Plowman Passus V–VI | Placeholder (summary) |
| Seven Sins | StoryBeat | Piers Plowman Passus VI–VII | Placeholder (summary) |
| Green Chapel | DialogueChoiceBeat | Sir Gawain Fitt I | Placeholder (modern paraphrase) |
| Boundary Tree | DialogueChoiceBeat | Sir Orfeo | Placeholder (modern paraphrase) |
| Orfeo Otherworld | StoryBeat | Sir Orfeo | Placeholder (modern) |

---

## Decisions You Need to Make

| Decision | Options | Notes |
|----------|---------|-------|
| **Language level** | Middle English / Modernized / Hybrid | Middle English is authentic but harder to read; hybrid = key phrases in ME, rest modern |
| **Length** | Full excerpt / Condensed / Single line | Full preserves atmosphere; condensed fits UI; single line for quick beats |
| **Speaker attribution** | Narrator / Character / Mixed | Piers Plowman is dream-vision (narrator = Wille); Gawain/Orfeo have direct speech |
| **Choices** | From text / Thematic / Player-authored | From text = lines characters might say; thematic = player paraphrases; player-authored = "I accept" style |

---

## 1. Piers Plowman — Story Beats (Narrator / Wille)

### Malvern Hilles (Prologue)

**Source:** Opening lines, B-Text Prologue  
**Current placeholder:** "In a somer seson, whan softe was the sonne... Wille falls asleep on Malvern Hilles and begins his first vision."

**Excerpts from text:**
- *"In a somer seson, whan softe was the sonne, / I shope me into shroudes as I a shepe were"* (I dressed in clothes as if I were a shepherd)
- *"In habite as an heremite unholy of werkes / Went wyde in this world wondres to here"* (In the habit of a hermit, unholy in works, I went wide in this world wonders to hear)
- *"Ac on a May morwenynge on Malverne hilles / Me bifel a ferly, of fairie me thoghte"* (But on a May morning on Malvern Hills, a marvel befell me, of fairy it seemed)

**Proposed structure:**
| BeatId | Text (choose one or blend) | Speaker |
|-------|----------------------------|---------|
| MalvernPrologue | Option A: Full ME (3–4 lines) / Option B: Condensed ME + modern gloss / Option C: Single line + "Wille falls asleep..." | Narrator |

---

### Fayre Felde (Passus I)

**Source:** Prologue, Fair Field passage  
**Current placeholder:** "A fair feeld ful of folke fonde I there bitwene."

**Excerpts from text:**
- *"A fair feeld ful of folke fond I ther bitwene"* (A fair field full of folk I found there between)
- *"Of alle manere of men, the meene and the riche, / Werchynge and wandrynge as the world asketh"* (Of all manner of men, the mean and the rich, working and wandering as the world asks)
- *"Somme putten hem to the plow, pleiden ful selde"* (Some put themselves to the plow, played full seldom)

**Proposed structure:**
| BeatId | Text | Speaker |
|--------|------|---------|
| FayreFelde | Option A: "A fair feeld ful of folke fond I ther bitwene. Of alle manere of men..." / Option B: Single line + modern summary | Narrator |

---

### Tour on Toft & Dongeoun (Passus II–III)

**Source:** Tower and Dungeon imagery  
**Current placeholder:** "A tour on a toft, trieliche ymaked. Wille sees the Tower of Trewthe and the Dungeon representing Hell."

**Excerpts from text:**
- *"A tour on a toft, trieliche ymaked"* (A tower on a hill, truly made)
- *"A deep dale bynethe, a dongeoun therinne"* (A deep dale beneath, a dungeon therein)
- Tower = Truth; Dungeon = Hell/Falsehood

**Proposed structure:**
| BeatId | Text | Speaker |
|--------|------|---------|
| TourDongeoun | Option A: ME couplet / Option B: ME + "The Tower of Truth and the Dungeon of Hell" | Narrator |

---

### Piers' Field (Passus IV–V)

**Source:** Piers the Plowman's appearance  
**Current placeholder:** "Piers the Plowman appears in his humble field, guiding Wille toward the path of Do-Wel."

**Excerpts from text:**
- Piers as honest laborer, guide to Do-Wel
- *"Do-wel and do-bet and do-best"* — the three degrees
- Relationship of physical work to spiritual work

**Proposed structure:**
| BeatId | Text | Speaker |
|--------|------|---------|
| PiersField | Option A: Piers speaks? (rare in poem—usually allegorical) / Option B: Narrator describes Piers / Option C: "Piers the Plowman appears..." (current) | Narrator or Piers |

---

### Seven Sins (Passus VI–VII)

**Source:** Confession of the Sins, allegorical personifications  
**Current placeholder:** "The Seven Deadly Sins gather. Wille must face them before the quest can continue."

**Excerpts from text:**
- Pride, Envy, Wrath, Sloth, Covetise, Gluttony, Lechery — each has a speech/confession
- Could pull one line per sin for variety

**Proposed structure:**
| BeatId | Text | Speaker |
|--------|------|---------|
| SevenSins | Option A: Single narrator line / Option B: One ME line per sin (7 beats) / Option C: Condensed "The Seven Deadly Sins gather..." | Narrator |

---

## 2. Sir Gawain and the Green Knight — Dialogue Choice (Green Chapel)

**Source:** Beheading game challenge, Fitt I  
**Current placeholder:** "I am the Knight of the Green Chapel. I offer thee a game: strike me once, and in a year and a day I shall return the blow. Dost thou accept the covenant?"

**Excerpts from text (Weston translation / Middle English):**
- Challenge: *"If any so hardy in this hous holdes hymselven... / That dar stond stroke for stroke"* — any knight may strike, and receive the blow back a year hence
- Green Knight: *"Nowe hegeg, and let se how thou hatcheg"* (Now hurry, and let's see how you strike)
- Gawain accepts: *"I wyl wel"* (I will well) / *"I schal gif the my giserne"* (I shall give you my answer)

**Proposed structure:**

| Field | Option A (ME-flavored) | Option B (Modern) | Option C (Hybrid) |
|-------|------------------------|-------------------|-------------------|
| **Main text** | "I am the Knight of the Grene Chapel. I offre the a game: smyte me ones, and in a yere and a day I schal quyte the. Wolt thou acorde?" | "I am the Knight of the Green Chapel. I offer thee a game: strike me once, and in a year and a day I shall return the blow. Dost thou accept?" | Mix: "I am the Knight of the Grene Chapel. I offer thee a game: strike me once, and in a year and a day I shall return the blow. Dost thou accept?" |
| **Choice 1 (correct)** | "I acorde. I schal mete the at the Grene Chapel." | "I accept. I will meet thee at the Green Chapel." | "I accept. I will meet thee at the Green Chapel." |
| **Choice 2 (wrong)** | "I refuse. I wyl not pleye thi game." | "I refuse. I will not play thy game." | Same |
| **Choice 3 (wrong)** | "I fle. This is no stede for me." | "I flee. This is no place for me." | Same |

**Decisions:**
- [ ] Use Option A / B / C for main text
- [ ] Keep 3 choices or reduce to 2?
- [ ] Add a 4th choice from text? (e.g. "I delay" — Gawain does delay before going)

---

## 3. Sir Orfeo — Dialogue Choice (Boundary Tree)

**Source:** Orfeo follows fairy ladies through rock into Otherworld; music as threshold  
**Current placeholder:** "At the boundary between worlds, a tree stands. Music drifts from beyond—melancholic, otherworldly. Dost thou follow?"

**Excerpts from text:**
- *"With fairi forth y-nome"* — taken by fairy
- Fairy realm as paradise-like: *"It seemed the proud court of Paradise"*
- Music: Orfeo's harp enables passage; fairy ladies' song
- Boundary: *"Thurch a roche"* (through a rock) — permeable threshold

**Proposed structure:**

| Field | Option A (ME-flavored) | Option B (Modern) | Option C (From poem) |
|-------|------------------------|-------------------|----------------------|
| **Main text** | "At the bounde bitwene worldes, a tre stondeth. Musyk drifteth from byonde—sorweful, unerthly. Wolt thou folwe?" | "At the boundary between worlds, a tree stands. Music drifts from beyond—melancholic, otherworldly. Dost thou follow?" | "Musyk drifteth from byonde the roche. The fairi court awaiteth. Wolt thou folwe?" |
| **Choice 1 (correct)** | "The path is uncerteyn. I torne ayen." | "The path is uncertain. I turn back." | "I torne ayen. The world I know is here." |
| **Choice 2 (wrong)** | "I folwe the musyk into the twylyght." | "I follow the music into the twilight." | "I folwe the musyk. With fairi forth y-nome." |

**Decisions:**
- [ ] Use Option A / B / C
- [ ] Speaker: Narrator (current) or "A Voice" / "The Music"?
- [ ] Add 3rd choice? (e.g. "I listen but do not enter")

---

## 4. Orfeo Otherworld — Arrival (Inescapable)

**Source:** Orfeo's entry into fairy realm  
**Current placeholder:** "The boundary between worlds dissolves. Thou hast followed the music into the Fairy Realm. There is no return."

**Excerpts from text:**
- Crystal palace, paradise-like
- *"He com in-to a fair cuntray"* — fair country
- No return motif: Orfeo must win Heurodis back; in our game, player is trapped

**Proposed structure:**
| BeatId | Text | Speaker |
|--------|------|---------|
| OtherworldArrival | Option A: "The bounde bitwene worldes dissolveth. Thou hast folwed the musyk into the Fairi Realm. Ther is no returne." / Option B: Modern (current) / Option C: "He com in-to a fair cuntray... There is no return." (blend) | Narrator |

---

## 5. Vision II — Optional Story Beats (for future)

| Location | Source | Suggested excerpt |
|----------|--------|-------------------|
| Quest Do-Wel | Piers Plowman Passus VIII | "What is Do-Wel?" — Wille's question |
| Dongeoun Depths | Hunger, poverty | Hunger's speech from poem |
| Years Pass | Time, aging | Reflective passage on years |

---

## 6. File Structure (for implementation)

```
Assets/Resources/Data/
├── Vision1/
│   ├── MalvernPrologue.asset      (StoryBeat)
│   ├── FayreFelde.asset          (StoryBeat)
│   ├── TourDongeoun.asset        (StoryBeat)
│   ├── PiersField.asset           (StoryBeat)
│   └── SevenSins.asset            (StoryBeat)
├── GreenChapel/
│   └── GreenChapelChoice.asset    (DialogueChoiceBeat)
└── OrfeoOtherworld/
    ├── BoundaryTreeChoice.asset   (DialogueChoiceBeat)
    └── OtherworldArrival.asset    (StoryBeat)
```

---

## 7. Next Steps (Your Decisions)

1. **Language:** Choose Middle English / Modern / Hybrid for each domain (Piers / Gawain / Orfeo).
2. **Length:** Full excerpt vs. condensed vs. single line per beat.
3. **Green Chapel choices:** Confirm 3 choices and which is "correct"; adjust wording.
4. **Boundary Tree choices:** Confirm 2 choices; add 3rd if desired.
5. **Speaker names:** Keep "Narrator" for Piers beats? Use "The Green Knight" / "A Voice" for choices?
6. **Editor scripts:** After decisions, update `CreateVision1LevelData`, `CreateGreenChapelLevelData`, `CreateOrfeoOtherworldLevelData` to use chosen text.

---

## References

- **Piers Plowman:** B-Text, Prologue and Passus I–VII; Middle English from various editions (A.V.C. Schmidt, etc.)
- **Sir Gawain and the Green Knight:** Cotton Nero A.x; Weston translation (Gutenberg); Middle English editions
- **Sir Orfeo:** Auchinleck MS; TEAMS Middle English Breton Lays

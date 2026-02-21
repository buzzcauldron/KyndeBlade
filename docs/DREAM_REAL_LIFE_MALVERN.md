# Dream vs. Real Life — Malvern, England

**Design principle:** Levels are dream (Wille's visions); interstitial elements are real life, grounded in Malvern, Worcestershire, England.

---

## Structure

| Layer | Setting | Content |
|-------|---------|---------|
| **Dream** | Allegorical visions | Combat levels, Passus content (Fayre Felde, Tour, Dongeoun, Piers' Field, Seven Sins, etc.) |
| **Real life** | Malvern, England | Hub, transitions, framing narrative, map locations between dreams |

---

## Real Life — Malvern, Worcestershire

**Primary reference:** Piers Plowman opens *"on a May morwenynge on Malverne hilles"* — Wille falls asleep on the Malvern Hills. The interstitial layer grounds the player in this real place.

### Key Malvern Locations (for interstitial content)

| Location | Use |
|----------|-----|
| **Malvern Hills** | Where Wille sleeps and wakes; the threshold between real and dream |
| **Worcestershire Beacon** | Highest point (425m); Iron Age British Camp; panoramic views |
| **Great Malvern** | Town on the eastern slopes; medieval priory, spa town |
| **Great Malvern Priory** | Medieval church; some of England's oldest stained glass |
| **St. Anne's Well** | Historic spring; pilgrims, walkers, the "waters of Malvern" |
| **British Camp** | Iron Age hill fort on the Beacon; ancient earthworks |
| **The Wyche** | Pass through the hills; paths between settlements |

### Interstitial Moments (real life)

- **Map/hub:** Wille in Malvern — "The Worcestershire Beacon, a soft summer morning" / "St. Anne's Well, between dreams"
- **Pre-dream:** "Wille rests on the hillside. The sun is soft. Sleep comes."
- **Post-dream:** "Wille wakes. The Beacon stands above. The dream fades, but the quest remains."
- **Between visions:** "Wille walks the path to the Priory" / "At the Wyche, the boundary blurs"

---

## Dream — Allegorical Levels

The combat/vision content remains allegorical (Piers Plowman, Gawain, Orfeo):

- **Malvern Hilles** (Prologue) — Threshold: still Malvern, but the dream begins
- **Fayre Felde** — Fair Field full of folk
- **Tour on Toft / Dongeoun** — Tower of Truth, Dungeon
- **Piers' Field** — Piers the Plowman
- **Seven Sins** — Allegorical gathering
- **Green Chapel** — Gawain (optional). Bodies accrue from failed run attempts (deaths).
- **Boundary Tree / Otherworld** — Orfeo (optional). Living characters accrue in the fairy world when entering.

---

## Accrual (Green Chapel & Otherworld)

- **Green Chapel bodies**: Each death at the Green Chapel increments `GreenChapelBodiesAccrued`. Persisted in GameProgress.
- **Otherworld living**: Each character who enters the Otherworld (tree choice or sin miniboss defeat) increments `OtherworldLivingCharactersAccrued`. Persisted in GameProgress.
- Map UI displays accrued counts when viewing these locations.

---

## Implementation Notes

- **LocationNode.IsRealLife** — Marks interstitial (Malvern) vs dream (vision) nodes
- **LocationNode.RealLifeLocationId** — Optional: "worcestershire_beacon", "st_annes_well", "great_malvern_priory"
- **Story beats** — Interstitial beats use Malvern place names; dream beats use allegorical text
- **Map flow** — Real-life nodes can sit between dream nodes (e.g. Wake at St. Anne's → Choose next vision)

---

## Tone

- **Real life:** Grounded, specific, English landscape — hills, wells, priory, paths
- **Dream:** Allegorical, medieval, symbolic — fields, towers, sins, quest
- **Transition:** "The dream begins" / "Wille wakes" — clear but gentle shifts

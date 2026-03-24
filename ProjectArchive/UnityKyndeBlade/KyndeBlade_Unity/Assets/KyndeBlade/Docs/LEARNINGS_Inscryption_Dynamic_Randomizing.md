# Learnings: Building Dynamic & Randomizing from Inscryption

Design takeaways from **Inscryption** (Daniel Mullins) for run structure, procedural content, and making each playthrough feel different while staying readable. Use these to inform KyndeBlade’s map, encounters, and building mechanics (see [BUILDING_MECHANICS_15_RUNS.md](BUILDING_MECHANICS_15_RUNS.md)).

---

## 1. Randomization at the right layer

**Inscryption:** Randomization is applied to **run progression** (which nodes appear, in what order, what cards are offered), not to moment-to-moment RNG inside a single fight. Individual battles have some draw-order variance, but the big “different every time” feeling comes from **which path you take** and **what you’re offered**, not from dice rolls inside combat.

**Takeaway for us:**  
- Keep combat resolution **readable** (e.g. crit pressure, damage formula); let **run structure** carry the variety: which locations/encounters appear, in what order, and what choices (rewards, narrative, difficulty) they offer.  
- Optional: light input randomness (e.g. crit roll, 5–10% damage variance) is fine if it’s bounded and doesn’t decide whole runs.

---

## 2. Procedural map with a fixed pattern

**Inscryption:** The map isn’t fully random. It follows a **repeating pattern** so each run has structure:

- **Pattern (Act 1 / Kaycee’s Mod):** A **card-gain node** → then **two non-battle nodes** (utility: campfire, mycologists, backpack, etc.) → then a **battle node** (normal or totem) → repeat until the boss.
- **Choice:** At forks, the player picks **between nodes of the same category** (e.g. which battle, which utility). So you get meaningful decisions without an overwhelming number of paths.
- **Map-specific rules:** Some nodes only appear on certain “maps” (e.g. Map 1 has no Bone Lord/Mycologists/Goobert; the final map only has Backpack, Sigil Transfer, Trader, Woodcarver). That keeps early runs simpler and final stretch focused.

**Takeaway for us:**  
- **World map / run structure:** Define a small set of **node types** (e.g. combat, narrative, rest/upgrade, merchant, hazard) and a **pattern or rules** for how they can appear (e.g. “after every N combats, offer one narrative node,” “boss only after X nodes”).  
- **Path choice:** Let the player choose **between a few options of the same kind** (e.g. “this combat vs that combat,” “this dialogue beat vs that one”) so runs feel different without a fully random graph.  
- **Location gating:** Like Inscryption’s map-specific nodes, we can restrict certain encounters or events to certain “chapters” or location sets (e.g. Green Chapel only after Malvern, Otherworld only after a specific trigger).

---

## 3. Node categories, not a soup of one-offs

**Inscryption:** Nodes are grouped into **categories**:

- **Card gain:** Random card, card by cost, card by tribe, cave trial, Prospector, Trapper, Trader.  
- **Utility:** Backpack, Bone Lord, Campfire, Goobert, Mycologists, Sigil transfer, Woodcarver.  
- **Battle:** Normal battle, Totem battle.

When the map is built, the generator picks from these pools so the run always has a mix: you don’t get five battles in a row or five campfires. The **pattern** (e.g. 1 gain, 2 utility, 1 battle) enforces rhythm.

**Takeaway for us:**  
- Classify **location/encounter types** (combat, narrative, rest/save, optional challenge, merchant, one-way story).  
- When building a “run” or a path through the world, use **rules** like “at least one narrative node per region” or “no two boss combats back-to-back” so pacing stays consistent.  
- Our existing `LocationNode` and “next locations” can be driven by **category + pattern** rather than a flat random list.

---

## 4. Difficulty and scaling as a dial (Kaycee’s Mod)

**Inscryption (Kaycee’s Mod):** Difficulty is explicit: players choose **challenges** (e.g. No Hook, Cloverless, Single Candle). Each challenge has a **Challenge Point** value; total CP unlocks new content and scales difficulty. So “harder” is a **visible choice**, and the run is built with that in mind.

**Takeaway for us:**  
- We can expose **run modifiers** (e.g. “more Elde,” “fewer rest nodes,” “Green Knight appears earlier”) as optional toggles or as rewards for lifetime run count (see building mechanics doc).  
- **Scaling over 15+ runs** can be “the world gets slightly harder *or* more generous” (e.g. more Kynde gain but also more damage taken) so the game **changes** without requiring a separate “challenge select” screen—though we could add one later.

---

## 5. Let the player “break” the game (on purpose)

**Inscryption:** Strong combos and “broken” decks are **intended**. The game is designed so that:

- Early game is **brutal**; repeated play teaches you the systems.  
- As you get better and find synergies, you get moments of **feeling overpowered**.  
- The game can still **push back** later (harder nodes, boss mechanics) so it doesn’t stay trivial.  
- **Narratively**, “breaking the game” (escaping the cabin, breaking the fourth wall) is the goal—so mechanical breaking and story breaking align.

**Takeaway for us:**  
- Don’t over-tune to prevent every dominant strategy. Allow **strong builds** (e.g. crit + high Kynde, or specific sin counters) and let players feel powerful sometimes.  
- Use **late-run or optional difficulty** (e.g. Green Knight, Elde, or future bosses) to create tension so the run isn’t trivial.  
- Thematic fit: our “building mechanics” and “world remembers” can mirror “the game changes as you play again”—so **meta progression** and **narrative** both acknowledge repeated journeys.

---

## 6. Persistent progression between runs

**Inscryption:** Failed runs still matter: you **unlock new cards and mechanics** over time. Later runs are **strategically different** (more options, different synergies) even though each run starts fresh. Knowledge + unlocks = feeling of progress.

**Takeaway for us:**  
- **Meta progress** (e.g. `LifetimeRunCount`, optional unlocks) should make run N+1 **meaningfully different** from run N (slightly different tuning, new dialogue, optional modifiers), not just “same game again.”  
- Keep **run state** (current location, choices, deck/loadout) separate from **lifetime state** (run count, unlocks, narrative flags) so we can “New Game” the run without wiping the meta.

---

## 7. Summary: what to steal

| Inscryption idea | Use in KyndeBlade |
|------------------|-------------------|
| Randomize **run structure**, not every dice roll | Variety in **which encounters/locations** and in what order; keep combat math readable. |
| **Pattern** for procedural map (e.g. gain → utility → battle) | Define **node categories** and a **pattern or rules** for our world map / encounter flow. |
| **Node categories** (gain / utility / battle) | Combat / narrative / rest / merchant / hazard; enforce mix via rules. |
| **Path choice** = pick between same-category options | Forks = “this combat vs that,” “this narrative beat vs that,” not 10 random nodes. |
| **Map-/chapter-specific** node sets | Restrict certain events to certain locations or run phases (e.g. Green Chapel, Otherworld). |
| **Difficulty as a dial** (challenges / CP) | Optional run modifiers or scaling from lifetime run count (building mechanics). |
| **Let players get overpowered** sometimes | Allow strong synergies; use late-run bosses and scaling to add tension. |
| **Unlocks and meta progression** between runs | Lifetime run count + small scaling effects + narrative “memory” (see building mechanics doc). |

---

## References (for further learning)

- **Kaycee’s Mod – Map Nodes:** [sites.google.com/view/kaycees-mod-general-info](https://sites.google.com/view/kaycees-mod-general-info) (node types, maps, pattern).  
- **“Let The Player Break The Game” (Inscryption, Isaac):** [parryeverything.com](https://parryeverything.com/2022/01/07/let-the-player-break-the-game-already-inscryption-isaac-and-others/) – design philosophy.  
- **Inscryption – A Card Game Made to be Broken:** [authory.com/StevenScaife](https://authory.com/StevenScaife/Inscryption-is-a-Card-Game-Made-to-be-Broken-a459e3d2ebb2e4e5ebcc20a94ba835a2e) – emergent combos and narrative.  
- **Going Rogue: Inscryption (Gwen C. Katz):** [medium.com](https://medium.com/@gwenckatz/going-rogue-inscryption-415a6a3a8358) – difficulty curve and procedural progression.  
- **Design Delve / Daniel Mullins (YouTube):** “How Inscryption Compels You With Meta Game Design,” “Discussing Inscryption’s Design” – direct creator insight.

Use this doc when designing or refactoring **run structure**, **world map flow**, **encounter ordering**, and **building mechanics** so KyndeBlade’s 15+ playthroughs stay dynamic and readable in an Inscryption-inspired way.

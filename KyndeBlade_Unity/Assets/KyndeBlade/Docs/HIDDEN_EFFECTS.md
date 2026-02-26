# Hidden-but-Discernable Effects

Some effects are **hidden** (no upfront tutorial) but **discernable** through play: environment, tooltips, or repeated cause–effect. They **modulate** outcomes (e.g. damage taken, Kynde gain, hazards) rather than override skill. This supports the game’s thematic model (Piers Plowman, Alan Lee / Pre-Raphaelite): consequences are felt and inferable, labor and suffering are visible in the world, and success remains earned through attention and choice—no deus ex machina.

## Skill centrality

Success in **parry/dodge** and **combat decisions** is the main driver of outcomes. Permanent and hidden effects support rather than replace skill:

- **Permanent effects** (e.g. ethical missteps, hunger scar) change numbers (damage taken multiplier, status) so observant players can infer cause–effect.
- **Hidden effects** (e.g. location modifiers) have no on-screen label but can be inferred from narrative or environment.
- We avoid invisible one-shot kills or unavoidable failures; effects are **modulate**, not **override**.

---

## Permanent effects (discernable in narrative)

- **HasEverHadHunger** — When true, a narrative branch can show a different arrival beat (e.g. “Thou hast borne hunger”) at locations that define `StoryBeatOnArrivalWhenHasEverHadHunger`. The hunger **scar** is also applied to the party in combat (visible status). Discernable: different story text on arrival and scar in combat.
- **EthicalMisstepCount** — Increases damage taken (`SaveManager.GetDamageTakenMultiplier`). Set by dialogue choices (e.g. Green Chapel). Discernable: taking more damage after choices; optional later dialogue can reference “thy steps have strayed”.
- **GreenKnightWillAppearRandomly** — Set by wrong choice at Green Chapel. Discernable: Green Knight can appear later as a destination or encounter.

---

## Environmental / location modifier

- **SuppressCombatHazards** (`LocationNode`) — When true, combat at this location has **no hazards** (no exhaustion, poverty, labor, hunger ticks from the environment). Intended for “peaceful” places (e.g. tower on the toft). **How to discern:** The location’s story beat text can hint at peace or safety (e.g. confined, lovely, “no trial here”) so observant players infer that hazards do not apply at that place. No on-screen label; infer from tone and subsequent combat (no hazard triggers).

---

## Status effects (hidden until they matter)

- **HungerScar** — Applied when `HasEverHadHunger` is true. Can be given a “discoverable” tooltip or revealed in narrative after the first time it matters.
- Other status effects (e.g. damage modifiers) can use optional **hideFromUI** or **discoverableDescription** in data so that after the first relevant moment, a tooltip or beat can reveal the rule.

This design keeps skill central while allowing permanent and hidden effects to enrich cause–effect and world depth.

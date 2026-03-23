# Medieval text reads → movesets (Godot + Unity bridge)

## Intent

When the player **reads / completes** a medieval text beat (Unity `StoryBeat` / future Godot reader UI), the party **unlocks modifier codes** that change combat presentation and (eventually) full action lists. Some unlocks are **detrimental** or **partially hidden**: the in-world blurb may omit the true cost until the player feels it in combat.

**Tone / bar:** *Odd, punishing, occasionally rewarding* — lettres should skew feints, parry windows, and stamina in ways that feel **unfair at first** but **legible after a beating**. **Hunger** (once named in save) adds a **deterministic but varying** parry tax (`PlayerMovesetModifiers.hunger_parry_ms_penalty()` + `parry_stamina_total()`). **Fae** and **Green Chapel** lanes now feed **combat** aggregates (damage, dodge tax, feint offset, parry pressure) while remaining hooks for world spawn when those systems exist.

**Canonical Unity extraction** of locations, spawns, GK, fae, and moves: [`docs/UNITY_STORY_AND_SPAWN_DIGEST.md`](../../docs/UNITY_STORY_AND_SPAWN_DIGEST.md).

## Spawn lanes (alignment)

| Lane | Unity analogue | Purpose in data |
|------|----------------|-----------------|
| `party_wille` | First slot in `SpawnPartyInPoemOrder` (Wille) | Strike/dodge/parry numeric hooks in Godot `CombatManager` / encounter |
| `party_ally` | Piers / Conscience slots | Future: per-ally action lists |
| `fae_modifies` | `FaeAppearanceManager` rolls | Future: `fae_chance_delta` on transform |
| `green_knight_ecology` | GK injection + `EncountersSinceLastGreenKnight` | Future: spawn weight / narrative pressure |

JSON catalog: [`data/medieval_text_unlocks.json`](../data/medieval_text_unlocks.json).

## Runtime (Godot)

| Piece | Role |
|-------|------|
| `GameState.read_medieval_text_ids` | `PackedStringArray` of catalog `id` values the player has read (persisted in save). |
| `MedievalTextCatalog` | Loads JSON; resolves grants and **aggregates** numeric deltas. |
| `PlayerMovesetModifiers` | `aggregate_totals()` → strike cost/damage, feint offset, dodge extras; `parry_window_ms()` clamps **170–230 ms** using catalog penalties + **varying** hunger + misstep cap; `parry_stamina_total()` adds hunger whimsy to parry cost. |
| `PiersCombatVoice` | [`scripts/piers_combat_voice.gd`](../scripts/piers_combat_voice.gd) — combat HUD copy (*Plouȝ-trewe stroke*, *Shelde of Conscience*, kennings per unlocked `moveset_codes` key). |
| `MedievalTextCatalog.list_granted_codes_in_order()` | Stable list of granted codes for rubric lines. |

### Recording a read

Call `GameState.mark_medieval_text_read("tower_vista")` when the player finishes a beat (or Godot analogue). Id must exist in `medieval_texts[].id` (or still stored for forward-compatible narrative).

### Detrimental / hidden

- `detrimental: true` on a text row is for **tooling and UI** (e.g. warn writers).
- `hidden_drawback` is **author-facing** copy; the game may show a shorter `player_facing_name` only.

## Unity port checklist (later)

- [ ] Emit text-read ids from narrative completion into save / export JSON.
- [ ] Mirror `aggregate_totals` in C# or share JSON-driven loader.
- [ ] Apply party_ally lane when spawning Piers/Conscience.
- [ ] Wire `fae_modifies` into `FaeAppearanceManager` chance.
- [ ] Wire `green_knight_ecology` into GK random rolls.

# Playtest Checklist
## Kynde Blade — Pre-Playtest Review

---

## Code Review Summary

### Strengths
- **Combat flow**: TurnManager, TurnOrder, ExecuteAction pipeline is clear
- **Expedition 33 moveset**: Full melee/ranged/skills/heal with Kynde economy
- **Adaptive AI**: Enemies learn from dodge/parry success rates
- **16-bit side-view layout**: Players left, enemies right (FF/DQ style)
- **Illumination moments**: 2D→3D breaks on victory/defeat

### Areas to Verify
1. **Victory/Defeat panels**: GameStateManager expects VictoryPanel/DefeatPanel — may be null if not assigned; CombatUI sets GoalText but not dedicated panels
2. **Escapade/Ward damage**: Real-time window starts but damage application for Escapade/Ward may need verification
3. **Heal targeting**: Heal targets self; no ally-select UI yet
4. **SpriteRenderer**: CombatFeedback.FlashCharacter expects SpriteRenderer — characters without sprites won't flash

---

## Next Steps Before Playtest

### Critical (must-have)
1. **Add placeholder sprites** — MedievalCharacter needs SpriteRenderer for feedback; add simple colored quads if no art
2. **Ensure Victory/Defeat feedback** — Create minimal Victory/Defeat panels in CombatUI or GameStateManager if null
3. **Test full combat loop** — Win a fight, lose a fight; confirm illumination plays

### Recommended
4. **Kynde UI** — Show current Kynde on screen during combat
5. **Stamina display** — Show stamina for current character
6. **Action tooltips** — Show Stamina/Kynde cost on hover or in button label
7. **Parry/Dodge input** — Confirm real-time window has working input (key/button) for player to dodge/parry

### Nice-to-have
8. **Combat background** — Replace solid color with simple tiled/baked background
9. **Sound effects** — Assign clips to CombatFeedback (DodgeSuccessClip, etc.)
10. **Tutorial prompt** — First-time hint: "Select an action" / "Press [key] to dodge"

---

## Input Scheme (Keyboard Primary, Controller Add-in)

| Action | Keyboard | Controller |
|--------|----------|------------|
| Dodge (Escapade) | Space | A / Cross |
| Parry (Ward) | Left Shift | X / Square |
| Counter (after parry) | E | Y / Triangle |
| Action selection | Mouse click | D-pad + A (UI navigation) |

---

## Quick Test Script

1. Open project in Unity 6
2. Create empty scene, add KyndeBladeGameManager
3. Press Play
4. **Win path**: Use Strike/Ranged/Heal; defeat all 3 enemies
5. **Lose path**: Let enemies defeat both players
6. **Illumination**: Observe 3D sphere fade-in on victory/defeat
7. **Adaptive AI**: Parry/dodge (if wired); see if enemy favors Strike over Escapade after successful defenses

---

## Known Gaps

| Gap | Workaround |
|-----|------------|
| No sprites | Add SpriteRenderer + colored Material to character GameObjects |
| No parry/dodge input | ParryDodgeInputHandler: Space/Shift/E (keyboard), A/X/Y (controller) |
| VictoryPanel null | GameStateManager creates no panels; assign in Inspector or add auto-create |
| Escapade/Ward damage | Check TurnManager ProcessActionExecution flow |

---

## Line Count

**~2,788 lines** of C# in `Assets/` (as of last count).

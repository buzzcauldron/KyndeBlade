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
1. **Victory/Defeat panels**: GameStateManager auto-creates VictoryPanel/DefeatPanel with Continue/Restart buttons; Victory → map or Field of Grace
2. **Escapade/Ward damage**: Real-time window starts but damage application for Escapade/Ward may need verification
3. **Heal targeting**: Heal targets self; no ally-select UI yet
4. **SpriteRenderer**: CombatFeedback.FlashCharacter expects SpriteRenderer — characters without sprites won't flash

---

## Next Steps Before Playtest

### Critical (must-have)
1. **Add placeholder sprites** — MedievalCharacter needs SpriteRenderer for feedback; add simple colored quads if no art
2. ~~**Ensure Victory/Defeat feedback**~~ — GameStateManager creates Victory/Defeat panels with Continue/Restart
3. **Test full combat loop** — Win a fight, lose a fight; confirm illumination plays; Victory → map or Field of Grace

### Recommended
4. ~~**Kynde UI**~~ — CombatUI auto-creates KyndeText; shows current Kynde during combat
5. ~~**Stamina display**~~ — CombatUI auto-creates StaminaText; shows stamina for current character
6. ~~**Action tooltips**~~ — Action buttons show Stamina/Kynde cost in label (e.g. "Strike [S:15]")
7. **Parry/Dodge input** — Confirm real-time window has working input (Space/Shift/E) for player to dodge/parry

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
2. Create empty scene, add KyndeBladeGameManager; StartWithMap = true
3. Press Play
4. **Map flow**: Malvern → Fayre → Tour → Dongeoun → Piers → Seven Sins → Quest Do-Wel → Dongeoun Depths → Years Pass
5. **Win path**: Use Strike/Ranged/Heal; defeat all enemies; Victory panel → Continue → map or Field of Grace
6. **Field of Grace**: At Years Pass victory → "Thou hast reached the field. Continue." → Field of Grace → "Thou waitest for Grace. She does not come. The game runs."
7. **Green Chapel**: When at buildings (Dongeoun, Dongeoun Depths), 20% chance Green Chapel appears as destination
8. **Green Knight**: 15% chance first appearance in buildings; 25% subsequent if cycle started (wrong Green Chapel choice)
9. **Poverty**: Dongeoun = level 1, Dongeoun Depths = level 2; reduces stamina/Kynde gains
10. **Illumination**: Observe 3D sphere fade-in on victory/defeat

---

## Known Gaps

| Gap | Workaround |
|-----|------------|
| No sprites | Add SpriteRenderer + colored Material to character GameObjects |
| No parry/dodge input | ParryDodgeInputHandler: Space/Shift/E (keyboard), A/X/Y (controller) |
| VictoryPanel null | GameStateManager auto-creates Victory/Defeat panels with Continue/Restart |
| Escapade/Ward damage | Check TurnManager ProcessActionExecution flow |

---

## Line Count

**~2,788 lines** of C# in `Assets/` (as of last count).

# Gameplay Design Document - Kynde Blade

## Core Concept

Kynde Blade is a turn-based RPG that combines strategic planning with real-time reflexes. Players must think ahead while also reacting quickly to incoming attacks, creating a unique hybrid combat experience. Named after "Kynde" (Nature) from Piers Plowman, representing the natural order and balance in combat.

## Combat Mechanics

### Turn-Based Foundation

- **Turn Order**: Determined by character Speed stat
- **Action Selection**: Each character selects one action per turn
- **Resource Management**: Actions consume Stamina (and Mana for mages)
- **Strategic Depth**: Players must balance offense, defense, and resource conservation

### Real-Time Elements

During combat, certain actions trigger **Real-Time Windows**:

1. **Dodge Window** (1.5 seconds)
   - Player must press dodge button within the window
   - Success: Avoid all damage
   - Failure: Take full damage
   - Timing is critical

2. **Parry Window** (1.0 second)
   - Smaller window than dodge, more skill required
   - Success: Reduce damage by 70%, enable counter-attack
   - Failure: Take full damage
   - Rewards precise timing

3. **Counter Window** (0.5 seconds after successful parry)
   - Very short window
   - Success: Deal 150% damage counter-attack
   - Failure: Miss the opportunity

### Action Types

#### Basic Actions
- **Attack**: Standard damage dealing, moderate stamina cost
- **Wait**: Skip turn, restore some stamina
- **Guard**: Reduce incoming damage, restore stamina

#### Advanced Actions
- **Dodge**: Enter dodge stance, consume stamina, activate dodge window
- **Parry**: Enter parry stance, consume stamina, activate parry window
- **Counter**: Only available after successful parry

#### Class-Specific Actions

**Knight:**
- Shield Bash: Stun enemy, moderate damage
- Defensive Stance: Increase defense for several turns
- Charge Attack: High damage, high stamina cost

**Mage:**
- Fireball: High magic damage, mana cost
- Heal: Restore health to ally, mana cost
- Lightning Bolt: Very high damage, stun chance, high mana cost

**Archer:**
- Quick Shot: Fast, low damage, low stamina
- Aimed Shot: High damage, requires setup turn
- Poison Arrow: Damage over time effect

**Rogue:**
- Backstab: High damage if behind enemy
- Evade: Extended dodge window
- Critical Strike: Chance for massive damage

## Character Progression

### Stats System

- **Health**: Maximum hit points
- **Stamina**: Resource for physical actions
- **Mana**: Resource for magical actions (Mage only)
- **Attack Power**: Base damage multiplier
- **Defense**: Damage reduction
- **Speed**: Turn order and dodge/parry window timing

### Leveling

- Characters gain experience from combat
- Level up increases all stats
- Unlock new abilities at certain levels
- Stat points can be allocated on level up

## Medieval Theme Integration

### Narrative Elements

The game follows a party of medieval heroes on an expedition to defeat a dark force threatening the kingdom. Similar to Expedition 33's "Gommage" mechanic, the medieval version could feature:

- **The Curse**: A magical curse that affects certain age groups or social classes
- **The Dark Lord**: A powerful enemy that must be defeated
- **The Expedition**: A quest to reach the enemy's stronghold
- **Kynde's Balance**: The natural order that must be restored

### Visual Design (16-Bit)

- **Environments**: Tile-based—castles, dungeons, villages, forests (16×16 tiles, parallax)
- **Characters**: 32×48 px sprites—knights, mages, archers, rogues (4-6 frame animations)
- **Combat**: Sprite impact frames, screen shake, spell effect sprites (8×8 or 16×16)
- **UI**: Pixel-art menus, medieval heraldry, parchment-style borders

### World Building

- **Factions**: Different kingdoms and orders
- **Locations**: Multiple regions with unique challenges
- **NPCs**: Merchants, quest givers, allies
- **Lore**: Rich backstory about the medieval world and its conflicts, inspired by medieval literature like Piers Plowman

## Difficulty Scaling

### Easy Mode
- Longer real-time windows
- More forgiving stamina costs
- Enemies deal less damage

### Normal Mode
- Standard timing windows
- Balanced resource costs
- Standard enemy difficulty

### Hard Mode
- Shorter real-time windows
- Higher stamina costs
- Enemies are more aggressive and deal more damage

### Expert Mode
- Very short timing windows
- Limited resources
- Enemies use advanced tactics

## Multiplayer Considerations

### Co-op Mode
- Players control different characters in the party
- Turn-based coordination required
- Real-time windows are individual

### PvP Mode
- Players face off in combat
- Real-time windows become competitive
- Strategic mind games with action selection

## Accessibility Features

- **Timing Window Adjustments**: Slider to adjust real-time window duration
- **Visual Indicators**: Clear feedback for timing windows
- **Audio Cues**: Sound effects for dodge/parry opportunities
- **Auto-Actions**: Option to automate basic actions
- **Tutorial System**: Step-by-step combat tutorial

## Haunting Actions (from Limbo)

*Gameplay inspiration from Limbo (Playdead)—the weight and consequence of actions, without the visual aesthetic.*

**Environmental Hazards**
- Hidden traps and dangers in the world (bear traps, pitfalls, environmental threats)
- Hazards that emerge from the environment—the world itself is dangerous
- Trial-and-error: players learn through failure; death teaches
- Repurposing: some hazards can be used for progression (e.g., trap an enemy, create a path)

**Consequences and Weight**
- Actions have weight—mistimed dodge/parry leads to real damage
- Death or defeat feels consequential; restart at checkpoint with lesson learned
- No hand-holding: players discover through doing
- Vulnerability: the party is small against a large, indifferent world

**Environmental Storytelling**
- Story and mood conveyed through the environment and hazards
- Traps and obstacles tell of poverty, decay, danger
- Sound and ambient cues hint at nearby threats or solutions
- Minimal exposition—show, don't tell

**Integration with Kynde Blade**
- Overworld/exploration: environmental hazards between combats
- Combat: real-time windows already create tension and consequence
- Dungeons: traps, hazards, repurposable elements
- Tone: aligns with melancholic, vulnerable themes of Piers Plowman

---

## Future Expansion Ideas

1. **Equipment System**: Weapons, armor, accessories that modify stats
2. **Skill Trees**: Customize character abilities
3. **Party Formation**: Position characters for tactical advantages
4. **Environmental Hazards**: Traps, obstacles, interactive elements (see Haunting Actions)
5. **Status Effects**: Poison, burn, freeze, stun, etc.
6. **Combo System**: Chain actions for bonus effects
7. **Boss Battles**: Unique mechanics for major encounters
8. **Random Encounters**: Procedural combat scenarios
9. **Crafting**: Create potions, weapons, armor
10. **Story Mode**: Full narrative campaign

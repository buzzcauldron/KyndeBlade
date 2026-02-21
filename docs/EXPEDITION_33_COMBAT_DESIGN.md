# Expedition 33-Inspired Combat System Design
## Kynde Blade Combat Mechanics

This document outlines the combat system inspired by Clair Obscur: Expedition 33, adapted for the medieval Piers Plowman setting.

---

## Core Mechanics from Expedition 33

### 1. Hybrid Turn-Based and Real-Time Combat

**Turn-Based Phase:**
- Players select actions during their turn
- Actions include: Melee Strike (generates Kynde), Ranged Strike (consumes Kynde), Skills (consume Kynde), Escapade, Ward
- Turn order based on Speed stat

**Real-Time Phase:**
- During enemy turns, players can react in real-time
- **Escapade** (Kynde's Evasion): Press button during attack window to avoid damage
- **Ward** (Trewthe's Sheeld): Precise timing to block and gain Kynde + counter opportunity
- **Jump Over**: Some attacks can be jumped over (medieval version: duck/roll)

---

### 2. Kynde System

**Kynde Generation:**
- **Melee Attacks**: Generate 1-3 Kynde (based on attack type)
- **Successful Parry**: Generate 2-4 Kynde (based on timing)
- **Perfect Dodge**: Generate 1-2 Kynde

**Kynde Consumption:**
- **Ranged Attacks**: Consume 2-4 Kynde
- **Skills/Abilities**: Consume 3-8 Kynde (varies by skill)
- **Special Actions**: Consume varying amounts

**Kynde Management:**
- Max Kynde: 10 (can be increased with equipment)
- Kynde persists between turns
- Strategic decision: Generate Kynde with melee or spend on powerful skills

---

### 3. Break System

**Break Gauge:**
- Each enemy has a Break Gauge (visible above health bar)
- Certain attacks/skills deal Break damage
- When Break Gauge is depleted:
  - Enemy is **Stunned** for 1-2 turns
  - Enemy takes **50% more damage** while broken
  - Break Gauge resets after stun ends

**Break Damage Sources:**
- Specific "Break" skills
- Parry counter-attacks
- Certain elemental attacks
- Coordinated party attacks

**Strategy:**
- Coordinate party to break enemies
- Time powerful attacks for when enemy is broken
- Break system rewards tactical play

---

### 4. Elemental Affinities and Weaknesses

**Elements:**
- **Fire** (Flamme) - Burns, damage over time
- **Ice** (Frost) - Slows, can freeze
- **Lightning** (Thunder) - Stuns, high damage
- **Holy** (Trewthe) - Extra damage to undead/corrupted
- **Dark** (Fals) - Corrupts, reduces stats
- **Nature** (Kynde) - Heals, buffs

**Weakness System:**
- Enemies have elemental weaknesses (2x damage)
- Enemies have elemental resistances (0.5x damage)
- Some enemies absorb elements (heal from damage)
- Exploiting weaknesses generates extra AP

**Visual Indicators:**
- Weakness: Glowing indicator
- Resistance: Dimmed indicator
- Absorption: Pulsing indicator

---

### 5. Real-Time Defense Mechanics

**Escapade** (Kynde's Evasion):
- Window: 1.0-2.0 seconds (varies by attack)
- Success: Avoid all damage, gain 1-2 Kynde
- Perfect Escapade: Last 30% of window, gain 2 Kynde + momentum bonus
- Failure: Take full damage

**Ward** (Trewthe's Sheeld):
- Window: 0.5-1.0 seconds (smaller, more skill)
- Success: Reduce damage by 80%, gain 2-4 Kynde, enable counter
- Perfect Ward: Last 25% of window, gain 4 Kynde + counter opportunity
- Failure: Take full damage

**Counter-Attack:**
- Available after successful parry
- Window: 0.5 seconds
- Success: Deal 150% damage + break damage
- Consumes no Kynde (reward for good parry)

**Jump Over / Duck:**
- Some attacks can be avoided by jumping/ducking
- Medieval version: Roll, duck, or sidestep
- Timing-based, similar to dodge

---

### 6. Skill System

**Skill Types:**
- **Melee Skills**: Generate Kynde, deal break damage
- **Ranged Skills**: Consume Kynde, high damage
- **Support Skills**: Consume Kynde, buff/heal party
- **Break Skills**: Focus on breaking enemies
- **Elemental Skills**: Exploit weaknesses

**Skill Trees:**
- Each character has unique skill tree
- Unlock skills through leveling
- Skills can be upgraded
- Some skills have multiple variants

---

### 7. Character Customization

**Equipment System:**
- **Weapons**: Affect damage, Kynde generation, break damage
- **Armor**: Affect defense, elemental resistance
- **Accessories**: Provide unique perks and bonuses
- **Relics** (medieval version of Pictos): Special items with passive bonuses

**Relic System:**
- Equipable items with unique effects
- After using a Relic in combat X times, unlock its passive (Lumina)
- Passives can be shared between characters
- Adds strategic depth to character building

---

### 8. Party Coordination

**Combo System:**
- Chain attacks between party members
- Combos generate extra AP
- Combos deal extra break damage
- Rewards tactical party management

**Formation System:**
- Party positioning matters
- Front row: Tanks, melee
- Back row: Ranged, support
- Some attacks affect rows differently

---

## Implementation Plan

### Phase 1: Core Kynde System
1. Add Kynde to character stats
2. Implement Kynde generation from melee attacks
3. Implement Kynde consumption for skills
4. Update UI to show Kynde

### Phase 2: Break System
1. Add Break Gauge to enemies
2. Implement Break damage calculation
3. Add Stun status effect
4. Visual indicators for broken state

### Phase 3: Elemental System
1. Add elemental types
2. Implement weakness/resistance system
3. Elemental damage calculation
4. Visual indicators

### Phase 4: Enhanced Real-Time Mechanics
1. Improve dodge/parry timing
2. Add counter-attack system
3. Add jump/duck mechanics
4. AP rewards for perfect timing

### Phase 5: Skills and Customization
1. Skill tree system
2. Equipment system
3. Relic system
4. Character progression

---

## Combat Flow (Expedition 33 Style)

1. **Player Turn:**
   - Select action (melee generates Kynde, ranged/skills consume Kynde)
   - Choose target
   - Action executes

2. **Enemy Turn:**
   - Enemy selects action
   - **Real-Time Window Opens**
   - Player can Escapade/Ward/jump
   - Success: Avoid/reduce damage, gain Kynde
   - Failure: Take damage

3. **Break State:**
   - When enemy break gauge depleted
   - Enemy stunned, takes extra damage
   - Opportunity for powerful attacks

4. **Kynde Management:**
   - Balance generating Kynde (melee) vs spending (skills)
   - Save Kynde for powerful combos
   - Use Ward to generate Kynde during enemy turns

---

## Medieval Adaptation

**Expedition 33 → Kynde Blade:**
- AP System → **Kynde** - representing Nature and spiritual/natural strength
- Break System → **Shield Break** - breaking enemy defenses
- Elements → **Virtues/Vices** - Trewthe (Truth), Fals (False), Kynde (Nature), etc.
- Pictos → **Relics** - medieval artifacts with spiritual power
- Camp System → **Monastery/Tavern** - places of rest and reflection

**Themes:**
- Kynde represents Nature and spiritual/natural strength
- Break represents breaking through corruption/falsehood
- Elements represent virtues and vices from Piers Plowman
- Combat is both physical and spiritual

---

This system creates dynamic, engaging combat that rewards both strategic planning and quick reflexes, just like Expedition 33, but with a medieval Piers Plowman theme.

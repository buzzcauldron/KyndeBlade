# Test Level Setup Guide - Tower on the Toft

This guide will help you create the test level **"Tower on the Toft"** with three enemy types based on characters from Piers Plowman.

**The Tower on the Toft** is a reference from Piers Plowman, representing Truth and the abode of God. In this level, players must defend the tower against the forces of False, Lady Mede, and Wrath.

## Enemy Types Created

### 1. **False** (Deceptive Rogue)
- **Source**: Personification of falsehood and deception from Piers Plowman
- **Class Type**: Rogue
- **Stats**: High speed (16), moderate attack (16), moderate health (110)
- **Abilities**:
  - **Deceptive Strike**: 1.5x damage multiplier
  - **Feint Attack**: Reduces target's defense
  - **Shadow Step**: Teleport behind and backstab for 2x damage

### 2. **Lady Mede** (Corrupt Mage)
- **Source**: Personification of bribery and corruption from Piers Plowman
- **Class Type**: Mage
- **Stats**: High magic attack (22), low defense (4), moderate health (95)
- **Abilities**:
  - **Corrupting Gold**: 35 damage, corruption effect
  - **Bribery Charm**: Charms target, may cause them to skip turn
  - **Wealth Bolt**: 40 damage, gold-themed magic

### 3. **Wrath** (Berserker Warrior)
- **Source**: One of the Seven Deadly Sins from Piers Plowman
- **Class Type**: Knight
- **Stats**: Very high health (180), high attack (18), slower speed (7)
- **Special Mechanic**: Rage system - becomes stronger as health decreases
- **Abilities**:
  - **Rage Strike**: Damage scales with rage level
  - **Berserker Rage**: Enter berserk mode, +50% attack, -30% defense
  - **Furious Charge**: 2x damage charge attack (3x if berserk)

## Setting Up the Test Level

### Step 1: Create a New Level

1. In Unreal Editor, go to **File → New Level**
2. Choose **Empty Level** or **Basic** template
3. Save it as `L_TowerOnTheToft` (or `L_TowerOnTheToft_Level`)
4. This level represents "The Tower on the Toft" from Piers Plowman - the Tower on the Hill symbolizing Truth

### Step 2: Set Up Game Mode

1. Create a Blueprint based on `KyndeBladeGameMode`
   - Right-click in Content Browser → Blueprint Class
   - Search for "Kynde Blade Game Mode"
   - Name it `BP_KyndeBladeGameMode`
2. Set it as default:
   - Edit → Project Settings → Game → Default Game Mode
   - Select `BP_KyndeBladeGameMode`

### Step 3: Create Player Characters

1. Create Blueprints for player characters:
   - `BP_PlayerKnight` (based on `KnightCharacter` or `MedievalCharacter`)
   - `BP_PlayerMage` (based on `MageCharacter` or `MedievalCharacter`)
   - Or use any combination you prefer

2. Place them in the level at desired positions (e.g., left side)

### Step 4: Create Enemy Blueprints

1. **False Enemy**:
   - Right-click → Blueprint Class
   - Search for "False Character" → Select
   - Name it `BP_False`
   - Add a Skeletal Mesh component (use any medieval rogue mesh)
   - Set visual appearance

2. **Lady Mede Enemy**:
   - Right-click → Blueprint Class
   - Search for "Lady Mede Character" → Select
   - Name it `BP_LadyMede`
   - Add a Skeletal Mesh component (use any mage/royal mesh)
   - Set visual appearance

3. **Wrath Enemy**:
   - Right-click → Blueprint Class
   - Search for "Wrath Character" → Select
   - Name it `BP_Wrath`
   - Add a Skeletal Mesh component (use any warrior/berserker mesh)
   - Set visual appearance

### Step 5: Add Test Level Setup Actor

1. **Create Blueprint from TestLevelSetup**:
   - Right-click → Blueprint Class
   - Search for "Test Level Setup" → Select
   - Name it `BP_TestLevelSetup`

2. **Configure the Blueprint**:
   - Open `BP_TestLevelSetup`
   - In the Details panel:
     - **Player Characters**: Add your player character references
     - **False Spawn Location**: Set to (500, -300, 0) or desired position
     - **Lady Mede Spawn Location**: Set to (500, 0, 0) or desired position
     - **Wrath Spawn Location**: Set to (500, 300, 0) or desired position
     - **False Class**: Set to `BP_False`
     - **Lady Mede Class**: Set to `BP_LadyMede`
     - **Wrath Class**: Set to `BP_Wrath`

3. **Place in Level**:
   - Drag `BP_TestLevelSetup` into the level
   - Position it anywhere (it will spawn enemies at configured locations)

### Step 6: Create Combat UI

1. Create a Blueprint Widget based on `CombatUI`:
   - Right-click → User Widget
   - Search for "Combat UI" → Select
   - Name it `WBP_CombatUI`

2. Design the UI:
   - Add health bars for each character
   - Add stamina/mana bars
   - Add action buttons (Attack, Dodge, Parry, etc.)
   - Add turn indicator
   - Connect to Turn Manager events

3. Display in Game:
   - Create a Player Controller Blueprint
   - In BeginPlay, create and add `WBP_CombatUI` to viewport

### Step 7: Test the Level

1. **Press Play** (or PIE - Play In Editor)
2. The `TestLevelSetup` actor will:
   - Spawn all three enemies at their configured locations
   - Wait 1 second
   - Automatically start combat

3. **Combat Flow**:
   - Turn order determined by Speed stat
   - Select actions for your characters
   - Enemies will act on their turns (you'll need AI or manual control)
   - Combat continues until one side is defeated

## Enemy AI (Future Enhancement)

Currently, enemies need to be controlled manually or you'll need to implement AI. To add basic AI:

1. Create Behavior Trees for each enemy type
2. Create Blackboard assets for AI state
3. Add AI Controller classes
4. Configure enemy Blueprints to use AI Controllers

## Visual Polish

### Suggested Visual Themes

- **False**: Dark, shadowy appearance, rogue outfit, daggers
- **Lady Mede**: Opulent, gold-themed, noble/royal appearance, golden magic effects
- **Wrath**: Red-themed, berserker appearance, rage aura effects

### Particle Effects

Add visual effects for:
- False's Shadow Step (teleportation effect)
- Lady Mede's Corrupting Gold (golden corruption particles)
- Wrath's Rage (red aura that intensifies with rage level)

## Troubleshooting

### Enemies Don't Spawn
- Check that `TestLevelSetup` is placed in the level
- Verify spawn locations are set correctly
- Check that enemy class Blueprints are assigned

### Combat Doesn't Start
- Ensure Game Mode is set correctly
- Verify player characters are added to `TestLevelSetup`
- Check that Turn Manager is spawned by Game Mode

### Enemies Don't Act
- Implement AI Controllers or manual turn control
- Check that enemies are added to combat properly
- Verify Turn Manager is functioning

## Next Steps

1. Add animations for each enemy type
2. Implement AI behavior trees
3. Create unique visual effects for each enemy
4. Add sound effects and voice lines
5. Create more complex combat scenarios
6. Add environmental storytelling related to Piers Plowman themes

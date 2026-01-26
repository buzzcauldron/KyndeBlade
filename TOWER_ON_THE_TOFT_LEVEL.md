# Tower on the Toft - Level Design Document

## Level Overview

**Level Name**: Tower on the Toft  
**Project**: Kynde Blade  
**Theme**: Based on "The Tower on the Hill" from Piers Plowman

### Narrative Context

In Piers Plowman, "The Tower on the Hill" (also called "The Tower on the Toft") symbolizes Truth and the abode of God. It stands in contrast to "The Dungeon in the Valley" which represents Hell.

In this level, players must defend the Tower on the Toft against three forces of corruption and sin:
- **False**: The personification of falsehood and deception
- **Lady Mede**: The personification of bribery and corruption  
- **Wrath**: One of the Seven Deadly Sins

## Level Layout

### Visual Design

The level should feature:

1. **The Tower** (Player's Position)
   - A medieval tower or keep on elevated ground (the "toft")
   - Represents Truth, safety, and the player's stronghold
   - Should be visually distinct and prominent
   - Players start here or nearby

2. **The Approach** (Combat Area)
   - Open field or courtyard between the tower and enemy spawn points
   - "The Fair Field Full of Folk" - the battleground
   - Flat terrain suitable for turn-based combat positioning

3. **Enemy Spawn Points** (Base of the Hill)
   - Three distinct spawn locations for the enemies
   - Spread out to create tactical positioning
   - Lower elevation than the tower (valley approach)

### Recommended Layout

```
                    [TOWER ON THE TOFT]
                         (Truth)
                            |
                            |
                    [Combat Field]
                            |
        [False]    [Lady Mede]    [Wrath]
      (Deception)  (Corruption)   (Anger)
```

## Level Setup Instructions

### 1. Create the Level

- **Level Name**: `L_TowerOnTheToft`
- **Template**: Empty Level or Basic template
- **Theme**: Medieval tower/fortress setting

### 2. Build the Environment

#### The Tower
- Place a tower or keep structure on elevated terrain
- Add battlements, walls, or defensive structures
- This represents the "Tower on the Toft" (toft = homestead/hill)

#### The Combat Field
- Create a flat combat area between tower and enemy spawns
- This represents "The Fair Field Full of Folk"
- Add medieval terrain: grass, dirt, stone paths

#### Enemy Approach
- Lower terrain at enemy spawn points
- Represents the valley approach to the tower
- Can add obstacles or cover for tactical gameplay

### 3. Place TestLevelSetup Actor

1. Drag `BP_TestLevelSetup` into the level
2. Position it near the tower or in the combat area
3. Configure spawn locations:
   - **False**: Base of hill, left side
   - **Lady Mede**: Base of hill, center
   - **Wrath**: Base of hill, right side

### 4. Player Starting Position

- Place player characters near or in the tower
- Represents defenders of Truth
- Higher ground advantage (if implementing elevation mechanics)

## Combat Scenario

### Objective
Defend the Tower on the Toft by defeating all three enemies:
- False (Deceptive Rogue)
- Lady Mede (Corrupt Mage)
- Wrath (Berserker Warrior)

### Tactical Considerations

1. **False** approaches from the left
   - Fast, deceptive attacks
   - Use defensive positioning

2. **Lady Mede** approaches from center
   - High magic damage
   - Focus fire priority target

3. **Wrath** approaches from right
   - High health, rage mechanic
   - Becomes stronger when damaged
   - Tank role - engage carefully

## Visual Storytelling

### Environmental Details

- **Tower**: Should feel like a place of truth and safety
  - Light colors, clean architecture
  - Maybe add banners or symbols of truth/justice
  - Elevated position suggests moral/spiritual height

- **Combat Field**: The battleground of morality
  - Neutral ground between good and evil
  - Can add symbolic elements (crossroads, standing stones)

- **Enemy Approach**: The forces of corruption
  - Darker, more chaotic terrain
  - Represents the valley/dungeon approach

### Lighting

- **Tower**: Bright, warm lighting (represents truth)
- **Combat Field**: Neutral, clear lighting
- **Enemy Spawns**: Darker, more ominous lighting

## Level Progression Ideas

### Future Expansions

1. **The Dungeon in the Valley** (Opposite Level)
   - Dark, underground level
   - Represents Hell from Piers Plowman
   - Could be a later level or boss arena

2. **The Fair Field Full of Folk** (Expanded)
   - Larger battlefield with more enemies
   - Multiple waves of combat
   - Represents the world of humankind

3. **Malvern Hills** (Starting Level)
   - Where Will the Dreamer begins his journey
   - Tutorial or opening level
   - Peaceful introduction before combat

## References to Piers Plowman

This level directly references:
- **The Tower on the Hill/Toft**: Truth, Heaven, God's abode
- **The Fair Field Full of Folk**: The world, the battleground
- **Enemy Characters**: False, Lady Mede, Wrath from the poem

The level name "Tower on the Toft" uses medieval terminology:
- **Tower**: The structure representing Truth
- **Toft**: Old English for a homestead or piece of elevated land
- Together: The tower on the elevated homestead - a place of truth and safety

## Blueprint Setup

When creating Blueprints for this level:

1. **Level Blueprint**: 
   - Name: `BP_TowerOnTheToft_Level`
   - Handle level-specific events

2. **TestLevelSetup Blueprint**:
   - Name: `BP_TowerOnTheToft_Setup`
   - Configure enemy spawns for this specific layout

3. **Level Streaming** (Optional):
   - Can stream in different sections
   - Tower, field, enemy approach as separate levels

## Testing Checklist

- [ ] Level loads correctly
- [ ] Tower is visible and prominent
- [ ] Combat field is clear and accessible
- [ ] Enemies spawn at correct locations
- [ ] Combat starts automatically
- [ ] Turn order works correctly
- [ ] All three enemy types appear
- [ ] Visual theme matches Piers Plowman references
- [ ] Level name displays as "Tower on the Toft"

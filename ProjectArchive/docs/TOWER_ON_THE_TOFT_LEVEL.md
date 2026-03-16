# Tower on the Toft - Level Design Document

## Level Overview

**Level Name**: Tower on the Toft  
**Project**: Kynde Blade (16-Bit Style)  
**Theme**: Based on "The Tower on the Hill" from Piers Plowman

### Narrative Context

In Piers Plowman, "The Tower on the Hill" (also called "The Tower on the Toft") symbolizes Truth and the abode of God. It stands in contrast to "The Dungeon in the Valley" which represents Hell.

In this level, players must defend the Tower on the Toft against three forces of corruption and sin:
- **False**: The personification of falsehood and deception
- **Lady Mede**: The personification of bribery and corruption  
- **Wrath**: One of the Seven Deadly Sins

## Level Layout (16-Bit)

### Visual Design

The level uses **tile-based 16-bit graphics**:

1. **The Tower** (Player's Position)
   - Large structure sprite or tile assembly (64×64 or 96×96 px)
   - Elevated on hill tiles (parallax or elevation layer)
   - Represents Truth, safety—use lighter palette (grays, stone)
   - Visually distinct against darker valley below

2. **The Approach** (Combat Area)
   - Flat tile layer: grass, dirt, stone path tiles (16×16)
   - "The Fair Field Full of Folk" battleground
   - 2-3 parallax layers for depth

3. **Enemy Spawn Points** (Base of the Hill)
   - Three tile positions for spawns
   - Lower elevation = darker palette or lower parallax layer
   - Valley approach tiles

### Recommended Layout

```
                    [TOWER ON THE TOFT]
                    (64×64 or 96×96 sprite)
                         (Truth)
                            |
                    [Combat Field - 16×16 tiles]
                            |
        [False]    [Lady Mede]    [Wrath]
      (32×48)       (32×48)       (32×48)
```

## Level Setup Instructions (16-Bit)

### 1. Tile Set Requirements

- **Level Name**: `L_TowerOnTheToft`
- **Base Resolution**: 256×224 or 512×448 (integer scale)
- **Theme**: Medieval tower—stone tiles, grass, dirt

### 2. Build the Environment

#### The Tower
- Tower tiles: 32×32 or 48×48 modular stone blocks
- Or single 64×64 / 96×96 tower sprite
- Battlements as tile accents
- Palette: Light grays, stone browns (8-12 colors)

#### The Combat Field
- 16×16 grass/dirt/stone path tiles
- "Fair Field" = open, flat tile map
- Earth tone palette (browns, muted greens)

#### Enemy Approach
- Darker tiles for valley (lower area)
- Optional: 2-frame tree or rock sprites for cover

### 3. Spawn Configuration

1. Define spawn tile coordinates in level data
2. **False**: Left spawn position
3. **Lady Mede**: Center spawn position
4. **Wrath**: Right spawn position

### 4. Player Starting Position

- Player sprites start near tower tiles
- Higher elevation = upper screen or foreground layer

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

## Visual Storytelling (16-Bit)

### Environmental Details

- **Tower**: Place of truth and safety
  - Lighter palette (grays, off-white)
  - Banner/symbol tiles (8×8 or 16×16)
  - Elevated = upper screen or foreground parallax

- **Combat Field**: Battleground of morality
  - Neutral earth-tone palette
  - Optional: crossroads tile, standing stone sprite

- **Enemy Approach**: Forces of corruption
  - Darker palette (desaturated, more gray)
  - Valley tiles

### Palette (replaces "Lighting")

- **Tower**: Lighter palette (represents truth)
- **Combat Field**: Neutral, desaturated
- **Enemy Spawns**: Darker palette

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

## Level Data Setup

When creating this level:

1. **Level Data**: 
   - Name: `L_TowerOnTheToft`
   - Tile map, spawn coordinates, parallax config

2. **Spawn Configuration**:
   - False, Lady Mede, Wrath at defined tile positions
   - Sprite IDs for each enemy

3. **Asset References**:
   - Tower sprite/tiles
   - Combat field tile set
   - Enemy sprites (32×48 px each)

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

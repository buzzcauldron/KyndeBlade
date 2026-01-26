# Quick Setup Guide - Getting Started with Your Level

Since you have a bare landscape, here's how to quickly set up combat:

## Step 1: Set the Game Mode

1. In Unreal Editor, go to **Edit → Project Settings**
2. Navigate to **Game → Default Game Mode**
3. Click the dropdown and select **KyndeBladeGameMode** (or create a Blueprint from it first)
4. If you need to create a Blueprint:
   - Right-click in Content Browser → **Blueprint Class**
   - Search for "Kynde Blade Game Mode" → Select
   - Name it `BP_KyndeBladeGameMode`
   - Then set it as the Default Game Mode

## Step 2: Place TestLevelSetup Actor

1. In the **Place Actors** panel (or press `Shift+1`), search for **"Test Level Setup"**
2. Drag **TestLevelSetup** into your level
3. Position it anywhere (it will spawn characters at configured locations)

## Step 3: Configure TestLevelSetup (Optional)

The actor will auto-spawn everything by default, but you can customize:

1. Select the **TestLevelSetup** actor in the level
2. In the **Details** panel, you'll see:
   - **Auto Spawn Players**: Enabled by default - will spawn a Knight and Mage
   - **Player 1 Spawn Location**: Default (-500, -150, 0) - adjust as needed
   - **Player 2 Spawn Location**: Default (-500, 150, 0) - adjust as needed
   - **False Spawn Location**: Default (500, -300, 0) - enemy spawn point
   - **Lady Mede Spawn Location**: Default (500, 0, 0) - enemy spawn point
   - **Wrath Spawn Location**: Default (500, 300, 0) - enemy spawn point

3. If you want to use custom Blueprints:
   - Create Blueprints from the character classes first
   - Then assign them in the **TestLevelSetup** Details panel

## Step 4: Press Play!

1. Press **Play** (or PIE button)
2. The system will automatically:
   - Spawn 2 player characters (Knight and Mage) on the left side
   - Spawn 3 enemies (False, Lady Mede, Wrath) on the right side
   - Start combat after 1 second

## What You'll See

- **Player Characters** (left side):
  - Knight (tank, high defense)
  - Mage (spellcaster, high magic)

- **Enemy Characters** (right side):
  - False (deceptive rogue, fast)
  - Lady Mede (corrupt mage, high magic damage)
  - Wrath (berserker warrior, high health, rage mechanic)

## Troubleshooting

### No characters spawn
- Make sure **TestLevelSetup** is placed in the level
- Check that the Game Mode is set correctly
- Verify spawn locations are reasonable (not underground, etc.)

### Combat doesn't start
- Ensure Game Mode is **KyndeBladeGameMode** (or Blueprint based on it)
- Check that at least one player and one enemy spawned

### Characters spawn but nothing happens
- The combat system is turn-based - you'll need to implement UI or AI to control turns
- Check the Output Log for any errors

## Next Steps

1. **Add Visuals**: Create Blueprints with Skeletal Meshes for each character
2. **Create UI**: Build a Combat UI widget to show health, stamina, and actions
3. **Add AI**: Implement AI controllers so enemies act automatically
4. **Polish**: Add animations, effects, and sound

## Default Spawn Layout

```
Left Side (Players)          Right Side (Enemies)
-500, -150  (Knight)          500, -300  (False)
-500, 150   (Mage)            500, 0     (Lady Mede)
                              500, 300   (Wrath)
```

Adjust these coordinates in the **TestLevelSetup** Details panel to match your level layout!

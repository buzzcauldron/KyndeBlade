# Debugging: Characters Not Spawning

## Step 1: Check Output Log

**This is the most important step!**

1. Open **Window → Developer Tools → Output Log**
2. Press **Play** in your level
3. Look for these messages:

### Expected Messages (if working):
```
=== KyndeBladeGameMode::BeginPlay called ===
bAutoSpawnTestCharacters = true
TurnManager spawned successfully
Setting timer to spawn characters in 0.5 seconds
=== AutoSpawnTestCharacters called ===
World is valid, checking classes...
DefaultKnightClass: KnightCharacter
DefaultMageClass: MageCharacter
...
✓ SUCCESS: Spawned Knight at X, Y, Z
✓ SUCCESS: Spawned Mage at X, Y, Z
...
Auto-starting combat with 2 players and 3 enemies
```

### If you see errors:
- **"No World available"** → GameMode isn't being used
- **"DefaultKnightClass: NULL"** → Classes aren't being set
- **"FAILED: Could not spawn Knight"** → Spawn failed (check for errors above it)
- **No messages at all** → GameMode BeginPlay isn't being called

## Step 2: Verify Game Mode is Set

1. **Edit → Project Settings → Game → Default Game Mode**
2. Should be: **KyndeBladeGameMode**
3. If it says "None" or something else, that's the problem!

### If Game Mode isn't in the list:
- The project might need to be recompiled
- Close and reopen the editor
- Or create a Blueprint from KyndeBladeGameMode first

## Step 3: Manual Spawn (Console Command)

If auto-spawn isn't working, try manual spawn:

1. Press **Play** to enter game mode
2. Press **`** (backtick) to open console
3. Type: `SpawnTestCharacters`
4. Press Enter

This will manually trigger character spawning.

## Step 4: Check World Outliner

After pressing Play:

1. Open **World Outliner** panel
2. Look for these actors:
   - "Player Knight"
   - "Player Mage"
   - "False"
   - "Lady Mede"
   - "Wrath"

If they're in the list but not visible, they spawned but need meshes.

## Step 5: Check for Debug Spheres

When characters spawn, colored spheres should appear:
- 🟢 Green = Knight (around -500, -150, 200)
- 🔵 Blue = Mage (around -500, 150, 200)
- 🔴 Red = False (around 500, -300, 200)
- 🟡 Yellow = Lady Mede (around 500, 0, 200)
- 🟣 Magenta = Wrath (around 500, 300, 200)

**Move your camera to these locations** - the spheres last 30 seconds.

## Step 6: Blueprint Method (Alternative)

If C++ GameMode isn't working, use Blueprint:

1. Right-click in Content Browser → **Blueprint Class**
2. Search for "Kynde Blade Game Mode" → Select
3. Name it `BP_KyndeBladeGameMode`
4. Open it
5. In the **Event Graph**, add:
   - **Event BeginPlay** node
   - **Auto Spawn Test Characters** node (from your GameMode)
   - Connect them
6. Set this Blueprint as Default Game Mode

## Common Issues & Solutions

### Issue: "No messages in Output Log"
**Solution**: GameMode isn't being used
- Check Project Settings → Default Game Mode
- Make sure you're in Play mode (not just viewing level)

### Issue: "DefaultKnightClass: NULL"
**Solution**: Classes aren't being initialized
- This shouldn't happen with the current code
- Try recompiling
- Check that character classes exist

### Issue: "FAILED: Could not spawn Knight"
**Solution**: Spawn is failing
- Check for errors above this message
- Might be collision issues
- Try different spawn locations

### Issue: "GameMode BeginPlay not called"
**Solution**: 
- GameMode isn't set correctly
- Or you're not in Play mode
- Make sure you press Play, not just open the level

### Issue: Characters spawn but are invisible
**Solution**: This is normal!
- Characters work but need Skeletal Meshes to be visible
- Check World Outliner - they should be there
- Create Blueprints with meshes (see CREATE_BLUEPRINTS.md)

## Quick Test Checklist

- [ ] Game Mode is set to KyndeBladeGameMode
- [ ] Output Log shows "BeginPlay called"
- [ ] Output Log shows "Auto-spawning test characters"
- [ ] Output Log shows "SUCCESS: Spawned" messages
- [ ] Debug spheres appear (colored circles)
- [ ] Characters appear in World Outliner
- [ ] No error messages in Output Log

If all checked, characters are spawning! They just need visual meshes.

## Still Not Working?

1. **Share the Output Log** - Copy all messages starting with "===" or "KyndeBlade"
2. **Check Game Mode** - Screenshot of Project Settings → Game → Default Game Mode
3. **Verify Compilation** - Make sure project compiled without errors

The enhanced logging will tell us exactly what's happening!

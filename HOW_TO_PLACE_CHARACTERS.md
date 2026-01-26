# How to Place Characters - Step by Step

## Method 1: Place TestLevelSetup Actor (Easiest)

### In the Unreal Editor:

1. **Open Place Actors Panel**:
   - Press **Shift+1** (or click the "Place Actors" button in the toolbar)
   - This opens the Place Actors panel on the left side

2. **Search for TestLevelSetup**:
   - In the search box at the top of Place Actors panel
   - Type: **"Test Level Setup"** or **"TestLevelSetup"**
   - It should appear in the list

3. **Drag into Level**:
   - **Click and drag** "Test Level Setup" from the panel
   - **Drop it** anywhere in your level viewport
   - You should see a small cube/box appear (that's the actor)

4. **Verify Placement**:
   - Open **World Outliner** (top right panel)
   - Look for **"TestLevelSetup"** or **"Test Level Setup"** in the list
   - If you see it, it's placed correctly!

5. **Press Play**:
   - Click the **Play** button
   - Characters will spawn automatically

## Method 2: Console Command (If Actor Doesn't Work)

### While in Play Mode:

1. **Press Play** to enter game mode
2. **Press `** (backtick/tilde key) to open console
3. **Type this command**:
   ```
   SpawnTestCharacters
   ```
4. **Press Enter**
5. Characters should spawn immediately

## Method 3: Manual Character Placement

### Place Individual Characters:

1. **Open Place Actors** (Shift+1)
2. **Search for character classes**:
   - "Knight Character"
   - "Mage Character"
   - "False Character"
   - "Lady Mede Character"
   - "Wrath Character"

3. **Drag each one** into the level
4. **Position them** where you want:
   - Players: Left side (around -500, -150, 200)
   - Enemies: Right side (around 500, 0, 200)

5. **Select TestLevelSetup** (if placed)
6. **In Details panel**:
   - Add your placed characters to **Player Characters** array
   - Or leave it empty to auto-spawn

## Method 4: Python Console (Advanced)

### In Unreal Editor Python Console:

1. **Window → Developer Tools → Python Console**
2. **Run this code**:
   ```python
   import unreal
   
   # Get the world
   world = unreal.EditorLevelLibrary.get_editor_world()
   
   # Spawn TestLevelSetup
   test_setup_class = unreal.EditorAssetLibrary.load_class(None, "/Script/KyndeBlade.TestLevelSetup")
   if test_setup_class:
       location = unreal.Vector(0, 0, 0)
       rotation = unreal.Rotator(0, 0, 0)
       test_setup = unreal.EditorLevelLibrary.spawn_actor_from_class(test_setup_class, location, rotation)
       print("Spawned TestLevelSetup!")
   ```

## Quick Reference: Keyboard Shortcuts

- **Shift+1**: Open Place Actors panel
- **F**: Focus camera on selected actor
- **G**: Toggle game mode (preview)
- **Alt+P**: Play in Editor
- **`** (backtick): Open console (in Play mode)
- **Esc**: Stop Play mode

## Troubleshooting

### "Test Level Setup" not in Place Actors:
- **Solution 1**: Close and reopen editor (hot-reload)
- **Solution 2**: Create Blueprint from it first:
  - Right-click in Content Browser → Blueprint Class
  - Search "Test Level Setup" → Create
  - Place the Blueprint instead

### Can't see the actor after placing:
- Press **F** to focus camera on it
- Check **World Outliner** - it should be listed
- The actor is a small cube, might be hard to see

### Characters still don't spawn:
- Check **Output Log** for error messages
- Verify TestLevelSetup is selected and **Auto Spawn Players** is checked
- Try the console command: `SpawnTestCharacters`

## Recommended Approach

**Use Method 1** (Place TestLevelSetup Actor):
- Simplest and most reliable
- Automatically spawns everything
- No manual configuration needed (uses defaults)

Just drag "Test Level Setup" into your level and press Play!

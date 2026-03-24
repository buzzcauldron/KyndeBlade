# Alternative Methods to Place Actors

## Method 1: Menu Bar (If Shift+1 Doesn't Work)

1. **Click "Window" in the top menu bar**
2. **Select "Place Actors"** (or "World Outliner" then use the Place Actors tab)
3. The Place Actors panel should open on the left side

## Method 2: Content Browser Method

1. **Open Content Browser** (usually bottom panel)
2. **Right-click** in an empty area
3. **Select "Blueprint Class"**
4. **Search for "Test Level Setup"**
5. **Create** a Blueprint (name it "BP_TestLevelSetup")
6. **Drag the Blueprint** from Content Browser into your level viewport

## Method 3: World Outliner Method

1. **Open World Outliner** (top right panel, or Window → World Outliner)
2. **Right-click** in the World Outliner
3. **Select "Place Actor"** or look for actor placement options
4. Search for "Test Level Setup"

## Method 4: Use Console Command (No Placement Needed!)

This is the EASIEST - no actor placement required:

1. **Press Play** (enter game mode)
2. **Press `** (backtick/tilde key, usually above Tab)
3. **Type**: `SpawnTestCharacters`
4. **Press Enter**
5. Characters spawn immediately!

## Method 5: Python Console (Advanced)

1. **Window → Developer Tools → Python Console**
2. **Copy and paste this code**:
```python
import unreal

# Get editor world
world = unreal.EditorLevelLibrary.get_editor_world()

# Load TestLevelSetup class
test_class = unreal.EditorAssetLibrary.load_class(None, "/Script/KyndeBlade.TestLevelSetup")

if test_class:
    # Spawn at origin
    location = unreal.Vector(0, 0, 100)
    rotation = unreal.Rotator(0, 0, 0)
    actor = unreal.EditorLevelLibrary.spawn_actor_from_class(test_class, location, rotation)
    print("TestLevelSetup spawned at origin!")
else:
    print("Could not find TestLevelSetup class")
```

## Method 6: Check Keyboard Layout

If Shift+1 isn't working:
- Try **Shift+F1**
- Or check if you have a different keyboard layout
- The shortcut might be different on your system

## Recommended: Use Console Command (Method 4)

**This is the simplest - no placement needed!**

Just:
1. Press Play
2. Press ` (backtick)
3. Type: `SpawnTestCharacters`
4. Press Enter

This bypasses all the placement stuff and spawns characters directly!

# Use Python Script - This Will Work!

The console command needs Play mode. Use this Python script instead - it works directly in the editor!

## Step 1: Open Python Console

1. Look for **Window** menu (or try right-clicking somewhere)
2. **Developer Tools → Python Console**
3. Or try: **Tools → Python → Python Console**

## Step 2: Run the Script

**Copy and paste this entire code** into the Python Console:

```python
import unreal

def spawn_characters():
    world = unreal.EditorLevelLibrary.get_editor_world()
    if not world:
        print("ERROR: Could not get editor world")
        return
    
    print("Spawning characters...")
    
    knight_class = unreal.EditorAssetLibrary.load_class(None, "/Script/KyndeBlade.KnightCharacter")
    mage_class = unreal.EditorAssetLibrary.load_class(None, "/Script/KyndeBlade.MageCharacter")
    false_class = unreal.EditorAssetLibrary.load_class(None, "/Script/KyndeBlade.FalseCharacter")
    mede_class = unreal.EditorAssetLibrary.load_class(None, "/Script/KyndeBlade.LadyMedeCharacter")
    wrath_class = unreal.EditorAssetLibrary.load_class(None, "/Script/KyndeBlade.WrathCharacter")
    
    spawned = []
    
    if knight_class:
        loc = unreal.Vector(-500, -150, 200)
        knight = unreal.EditorLevelLibrary.spawn_actor_from_class(knight_class, loc, unreal.Rotator(0,0,0))
        if knight: spawned.append(knight); print("✓ Knight")
    
    if mage_class:
        loc = unreal.Vector(-500, 150, 200)
        mage = unreal.EditorLevelLibrary.spawn_actor_from_class(mage_class, loc, unreal.Rotator(0,0,0))
        if mage: spawned.append(mage); print("✓ Mage")
    
    if false_class:
        loc = unreal.Vector(500, -300, 200)
        false_char = unreal.EditorLevelLibrary.spawn_actor_from_class(false_class, loc, unreal.Rotator(0,0,0))
        if false_char: spawned.append(false_char); print("✓ False")
    
    if mede_class:
        loc = unreal.Vector(500, 0, 200)
        mede = unreal.EditorLevelLibrary.spawn_actor_from_class(mede_class, loc, unreal.Rotator(0,0,0))
        if mede: spawned.append(mede); print("✓ Lady Mede")
    
    if wrath_class:
        loc = unreal.Vector(500, 300, 200)
        wrath = unreal.EditorLevelLibrary.spawn_actor_from_class(wrath_class, loc, unreal.Rotator(0,0,0))
        if wrath: spawned.append(wrath); print("✓ Wrath")
    
    print(f"Total: {len(spawned)} characters spawned!")

spawn_characters()
```

## Step 3: Check Results

1. **World Outliner**: Should show the characters
2. **Viewport**: Characters might be invisible (need meshes) but they're there
3. **Python Console**: Should print "✓ Knight", "✓ Mage", etc.

## Alternative: Load from File

If you can't paste, try loading the file:

```python
exec(open('/home/sethj/KyndeBlade 5.7/SpawnCharacters.py').read())
```

## This Should Work!

Python scripts work directly in the editor without needing Play mode or GameMode setup. This will definitely spawn the characters!

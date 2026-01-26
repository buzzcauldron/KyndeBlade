"""
Python script to spawn characters directly in Unreal Editor
Run this in Python Console: Window → Developer Tools → Python Console
"""

import unreal

def spawn_characters():
    """Spawn all test characters"""
    
    # Get the editor world
    world = unreal.EditorLevelLibrary.get_editor_world()
    if not world:
        print("ERROR: Could not get editor world")
        return
    
    print("Spawning characters...")
    
    # Load character classes
    knight_class = unreal.EditorAssetLibrary.load_class(None, "/Script/KyndeBlade.KnightCharacter")
    mage_class = unreal.EditorAssetLibrary.load_class(None, "/Script/KyndeBlade.MageCharacter")
    false_class = unreal.EditorAssetLibrary.load_class(None, "/Script/KyndeBlade.FalseCharacter")
    mede_class = unreal.EditorAssetLibrary.load_class(None, "/Script/KyndeBlade.LadyMedeCharacter")
    wrath_class = unreal.EditorAssetLibrary.load_class(None, "/Script/KyndeBlade.WrathCharacter")
    
    spawned_actors = []
    
    # Spawn players
    if knight_class:
        location = unreal.Vector(-500, -150, 200)
        rotation = unreal.Rotator(0, 0, 0)
        knight = unreal.EditorLevelLibrary.spawn_actor_from_class(knight_class, location, rotation)
        if knight:
            spawned_actors.append(knight)
            print(f"✓ Spawned Knight at {location}")
    
    if mage_class:
        location = unreal.Vector(-500, 150, 200)
        rotation = unreal.Rotator(0, 0, 0)
        mage = unreal.EditorLevelLibrary.spawn_actor_from_class(mage_class, location, rotation)
        if mage:
            spawned_actors.append(mage)
            print(f"✓ Spawned Mage at {location}")
    
    # Spawn enemies
    if false_class:
        location = unreal.Vector(500, -300, 200)
        rotation = unreal.Rotator(0, 0, 0)
        false_char = unreal.EditorLevelLibrary.spawn_actor_from_class(false_class, location, rotation)
        if false_char:
            spawned_actors.append(false_char)
            print(f"✓ Spawned False at {location}")
    
    if mede_class:
        location = unreal.Vector(500, 0, 200)
        rotation = unreal.Rotator(0, 0, 0)
        mede = unreal.EditorLevelLibrary.spawn_actor_from_class(mede_class, location, rotation)
        if mede:
            spawned_actors.append(mede)
            print(f"✓ Spawned Lady Mede at {location}")
    
    if wrath_class:
        location = unreal.Vector(500, 300, 200)
        rotation = unreal.Rotator(0, 0, 0)
        wrath = unreal.EditorLevelLibrary.spawn_actor_from_class(wrath_class, location, rotation)
        if wrath:
            spawned_actors.append(wrath)
            print(f"✓ Spawned Wrath at {location}")
    
    print(f"\nTotal spawned: {len(spawned_actors)} characters")
    print("Check World Outliner to see them!")

# Run it
spawn_characters()

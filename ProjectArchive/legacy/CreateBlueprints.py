"""
Unreal Engine Python Script to Create All Required Blueprints
Run this in the Unreal Editor Python Console (Window -> Developer Tools -> Python Console)

Or execute from command line:
    unreal_engine_python_execute CreateBlueprints.py
"""

import unreal

def create_blueprint_from_class(base_class_path, blueprint_name, folder_path="/Game"):
    """
    Create a Blueprint from a C++ class
    
    Args:
        base_class_path: Path to the C++ class (e.g., "/Script/KyndeBlade.KnightCharacter")
        blueprint_name: Name for the new Blueprint
        folder_path: Where to save the Blueprint
    """
    try:
        # Get the class
        base_class = unreal.load_class(None, base_class_path)
        if not base_class:
            print(f"ERROR: Could not find class {base_class_path}")
            return None
        
        # Create Blueprint factory
        factory = unreal.BlueprintFactory()
        factory.set_editor_property("parent_class", base_class)
        
        # Create asset
        asset_tools = unreal.AssetToolsHelpers.get_asset_tools()
        blueprint = asset_tools.create_asset(
            asset_name=blueprint_name,
            package_path=folder_path,
            asset_class=unreal.Blueprint,
            factory=factory
        )
        
        if blueprint:
            print(f"✓ Created {blueprint_name} at {folder_path}/{blueprint_name}")
            return blueprint
        else:
            print(f"✗ Failed to create {blueprint_name}")
            return None
            
    except Exception as e:
        print(f"✗ Error creating {blueprint_name}: {str(e)}")
        return None

def create_widget_blueprint(base_class_path, widget_name, folder_path="/Game"):
    """Create a Widget Blueprint from a C++ class"""
    try:
        base_class = unreal.load_class(None, base_class_path)
        if not base_class:
            print(f"ERROR: Could not find class {base_class_path}")
            return None
        
        factory = unreal.WidgetBlueprintFactory()
        factory.set_editor_property("parent_class", base_class)
        
        asset_tools = unreal.AssetToolsHelpers.get_asset_tools()
        widget = asset_tools.create_asset(
            asset_name=widget_name,
            package_path=folder_path,
            asset_class=unreal.WidgetBlueprint,
            factory=factory
        )
        
        if widget:
            print(f"✓ Created {widget_name} at {folder_path}/{widget_name}")
            return widget
        else:
            print(f"✗ Failed to create {widget_name}")
            return None
            
    except Exception as e:
        print(f"✗ Error creating {widget_name}: {str(e)}")
        return None

def main():
    """Create all required blueprints"""
    print("=" * 60)
    print("Creating KyndeBlade Blueprints")
    print("=" * 60)
    
    # Create folder structure
    base_folder = "/Game/KyndeBlade"
    characters_folder = f"{base_folder}/Characters"
    enemies_folder = f"{characters_folder}/Enemies"
    ui_folder = f"{base_folder}/UI"
    combat_folder = f"{base_folder}/Combat"
    game_folder = f"{base_folder}/Game"
    
    # Create folders (if they don't exist)
    unreal.EditorAssetLibrary.make_directory(characters_folder)
    unreal.EditorAssetLibrary.make_directory(enemies_folder)
    unreal.EditorAssetLibrary.make_directory(ui_folder)
    unreal.EditorAssetLibrary.make_directory(combat_folder)
    unreal.EditorAssetLibrary.make_directory(game_folder)
    
    print("\n--- Creating Game Mode Blueprint ---")
    create_blueprint_from_class(
        "/Script/KyndeBlade.KyndeBladeGameMode",
        "BP_KyndeBladeGameMode",
        game_folder
    )
    
    print("\n--- Creating Player Character Blueprints ---")
    create_blueprint_from_class(
        "/Script/KyndeBlade.KnightCharacter",
        "BP_PlayerKnight",
        characters_folder
    )
    
    create_blueprint_from_class(
        "/Script/KyndeBlade.MageCharacter",
        "BP_PlayerMage",
        characters_folder
    )
    
    print("\n--- Creating Enemy Character Blueprints ---")
    create_blueprint_from_class(
        "/Script/KyndeBlade.FalseCharacter",
        "BP_False",
        enemies_folder
    )
    
    create_blueprint_from_class(
        "/Script/KyndeBlade.LadyMedeCharacter",
        "BP_LadyMede",
        enemies_folder
    )
    
    create_blueprint_from_class(
        "/Script/KyndeBlade.WrathCharacter",
        "BP_Wrath",
        enemies_folder
    )
    
    create_blueprint_from_class(
        "/Script/KyndeBlade.HungerCharacter",
        "BP_Hunger",
        enemies_folder
    )
    
    print("\n--- Creating Level Setup Blueprint ---")
    create_blueprint_from_class(
        "/Script/KyndeBlade.TestLevelSetup",
        "BP_TestLevelSetup",
        base_folder
    )
    
    print("\n--- Creating UI Widget Blueprint ---")
    create_widget_blueprint(
        "/Script/KyndeBlade.CombatUI",
        "WBP_CombatUI",
        ui_folder
    )
    
    print("\n--- Creating Combat Action Blueprints ---")
    create_blueprint_from_class(
        "/Script/KyndeBlade.CombatAction",
        "BP_Attack",
        combat_folder
    )
    
    create_blueprint_from_class(
        "/Script/KyndeBlade.CombatAction",
        "BP_Dodge",
        combat_folder
    )
    
    create_blueprint_from_class(
        "/Script/KyndeBlade.CombatAction",
        "BP_Parry",
        combat_folder
    )
    
    create_blueprint_from_class(
        "/Script/KyndeBlade.CombatAction",
        "BP_Rest",
        combat_folder
    )
    
    print("\n" + "=" * 60)
    print("Blueprint creation complete!")
    print("=" * 60)
    print("\nNext steps:")
    print("1. Open each Blueprint and configure:")
    print("   - Add Skeletal Mesh components to characters")
    print("   - Set up Combat Action properties")
    print("   - Configure TestLevelSetup spawn locations")
    print("2. Set BP_KyndeBladeGameMode as Default Game Mode")
    print("3. Place BP_TestLevelSetup in your level")
    print("\nAll blueprints are in: /Game/KyndeBlade/")

if __name__ == "__main__":
    main()

# How to Run Kynde Blade (Unity)

## Quick start

1. **One-time setup**: In Unity, go to **KyndeBlade > Setup Project (Create Scene + Build)**
   - Ensures MainCamera tag, creates `Assets/Scenes/Main.unity`, adds to Build Settings
   - Or use **KyndeBlade > Create Main Scene** if you only need the scene

2. **Press Play** (▶) in the Unity toolbar

The game will bootstrap at runtime: map UI, combat canvas, and managers are created automatically.

## If the scene is empty

If you see only the default grid/camera/light:

- The scene needs **KyndeBladeGameManager** on a GameObject
- Run **KyndeBlade > Create Main Scene** to add it
- Or manually: Create Empty → Add Component → KyndeBladeGameManager

## Build settings

The Create Main Scene menu also adds the scene to **File → Build Settings**. For editor play, this isn’t required.

## Prefabs (optional)

Assign character prefabs on KyndeBladeGameManager in the Inspector for full combat. Without them, the game uses fallback spawning.

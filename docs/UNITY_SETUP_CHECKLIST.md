# Unity Setup Checklist (Kynde Blade)

Essential Unity project setup elements and how this project handles them.

## One-time setup

Run **KyndeBlade > Setup Project (Create Scene + Build)** in Unity. This:

- Ensures `MainCamera` tag exists
- Creates `Assets/Scenes/Main.unity` if missing
- Adds the scene to Build Settings

## Checklist

| Item | Status | Notes |
|------|--------|-------|
| **Scene in build** | ✓ | Added by Setup Project / Create Main Scene |
| **Main Camera** | ✓ | Tagged `MainCamera`, orthographic, AudioListener |
| **EventSystem** | ✓ | Created at runtime (StandaloneInputModule) |
| **AudioListener** | ✓ | On Main Camera |
| **Canvas + Scaler** | ✓ | Combat, Map, Dialogue canvases use Scale With Screen Size (1920×1080) |
| **Input** | ✓ | Legacy Input Manager (Horizontal, Vertical, Fire1, Submit, Cancel) |
| **Tags** | ✓ | MainCamera ensured by Setup Project |
| **Layers** | ✓ | Default, UI, etc. |
| **Physics 2D** | ✓ | Physics2DSettings.asset present |
| **Render pipeline** | ✓ | Built-in (default) |

## Project specifics

- **2D orthographic** – Camera is orthographic, sprites for characters
- **Runtime bootstrap** – KyndeBladeGameManager creates managers, UI, and camera at runtime
- **No prefab scene** – Main.unity is minimal; content is spawned in code

## If something breaks

1. Run **KyndeBlade > Setup Project** again
2. Ensure `Assets/Scenes/Main.unity` is in **File → Build Settings**
3. Check **Edit → Project Settings → Tags and Layers** for `MainCamera`

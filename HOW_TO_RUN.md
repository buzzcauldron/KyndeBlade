# How to Run Kynde Blade

## Quick Start (Linux)

### Option 1: Using Unreal Engine Launcher/Installed Version

1. **Locate Unreal Engine Installation:**
   - If installed via Epic Games Launcher, it's typically at:
     - `~/UnrealEngine/Engine/Binaries/Linux/UnrealEditor`
     - Or check `~/.config/Epic/UnrealEngineLauncher/LauncherInstalled.dat` for installation path

2. **Open the Project:**
   ```bash
   # Replace with your actual Unreal Engine path
   /path/to/UnrealEngine/Engine/Binaries/Linux/UnrealEditor KyndeBlade.uproject
   ```
   
   Or simply:
   ```bash
   # Double-click KyndeBlade.uproject in your file manager
   # Or right-click → Open With → Unreal Editor
   ```

3. **First Time Setup:**
   - Unreal Engine will prompt to rebuild modules - click **"Yes"**
   - Wait for compilation to complete (this may take several minutes)
   - The editor will open automatically when done

### Option 2: Using Unreal Engine from Source

If you've built Unreal Engine from source:

```bash
cd /path/to/UnrealEngine
./Engine/Binaries/Linux/UnrealEditor /home/sethj/KyndeBlade/KyndeBlade.uproject
```

### Option 3: Generate Project Files and Compile Manually

If the project needs to be compiled first:

1. **Generate Project Files:**
   ```bash
   cd /home/sethj/KyndeBlade
   # If you have UnrealVersionSelector:
   UnrealVersionSelector -projectfiles -project="$(pwd)/KyndeBlade.uproject" -game -rocket -progress
   
   # Or manually with Unreal Build Tool:
   /path/to/UnrealEngine/Engine/Build/BatchFiles/Linux/Build.sh KyndeBladeEditor Linux Development -Project="$(pwd)/KyndeBlade.uproject" -WaitMutex -FromMsBuild
   ```

2. **Compile:**
   ```bash
   # Using Unreal Build Tool directly:
   /path/to/UnrealEngine/Engine/Build/BatchFiles/Linux/Build.sh KyndeBladeEditor Linux Development "$(pwd)/KyndeBlade.uproject"
   ```

3. **Open in Editor:**
   ```bash
   /path/to/UnrealEngine/Engine/Binaries/Linux/UnrealEditor KyndeBlade.uproject
   ```

## Running the Game

### In the Editor (Play in Editor - PIE)

1. **Open a Level:**
   - If you have a test level created, open it (File → Open Level)
   - Or create a new level (File → New Level)

2. **Set Game Mode:**
   - Edit → Project Settings → Game → Default Game Mode
   - Set to `BP_KyndeBladeGameMode` (or create one if needed)

3. **Press Play:**
   - Click the **Play** button in the toolbar (or press `Alt+P`)
   - The game will run in the viewport

### Packaging for Distribution

To create a standalone executable:

1. **File → Package Project → Linux**
2. Select output directory
3. Wait for packaging to complete
4. Run the executable from the packaged folder:
   ```bash
   cd /path/to/packaged/game
   ./KyndeBlade.sh
   ```

## Troubleshooting

### "Project files are missing"
**Solution:** Generate project files (see Option 3 above)

### "Modules are out of date"
**Solution:** Click "Yes" when prompted to rebuild, or manually compile

### "Can't find Unreal Engine"
**Solution:** 
- Install Unreal Engine 5.3+ from Epic Games Launcher
- Or build from source: https://github.com/EpicGames/UnrealEngine

### "Compilation errors"
**Solution:**
- Ensure you have Unreal Engine 5.3 or later
- Check that all C++ development tools are installed:
  ```bash
  # On Fedora:
  sudo dnf install clang cmake make
  ```

### "Project won't open"
**Solution:**
- Check that `KyndeBlade.uproject` is in the correct location
- Verify Unreal Engine version matches (5.3)
- Try regenerating project files

## Quick Test

To quickly test if everything works:

1. Open the project in Unreal Editor
2. Create a new level (File → New Level → Empty Level)
3. Add a simple test:
   - Place a `BP_TestLevelSetup` actor (if you've created it)
   - Or manually spawn characters using Blueprints
4. Press Play (Alt+P)
5. If combat starts, you're good to go!

## Next Steps

Once the project is running:
- See `SETUP_GUIDE.md` for detailed setup instructions
- See `TEST_LEVEL_GUIDE.md` for creating a test combat level
- See `CAMPAIGN_LEVEL_DESIGN.md` for the full game structure

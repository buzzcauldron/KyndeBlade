# Quick Start Checklist - Run the Level

## ✅ Pre-Flight Checks

### 1. Game Mode is Set
- **Edit → Project Settings → Game → Default Game Mode**
- Should be: **KyndeBladeGameMode**
- If not set, select it from the dropdown

### 2. Project is Compiled
- The code should auto-compile when editor opens
- If you see errors, check Output Log

## 🚀 Running the Level

### Step 1: Open Your Level
- Open any level (or create a new Empty Level)
- The GameMode will automatically spawn characters

### Step 2: Press Play
- Click the **Play** button (or press `Alt+P`)
- The game will start in the viewport

### Step 3: What to Look For

**Visual Indicators:**
- **Colored Debug Spheres** should appear:
  - 🟢 Green sphere = Player Knight (left side)
  - 🔵 Blue sphere = Player Mage (left side)
  - 🔴 Red sphere = False enemy (right side)
  - 🟡 Yellow sphere = Lady Mede enemy (right side)
  - 🟣 Magenta sphere = Wrath enemy (right side)

**In Output Log:**
- Open: **Window → Developer Tools → Output Log**
- Look for messages:
  - "KyndeBladeGameMode: Auto-spawning test characters"
  - "Spawned Knight at..."
  - "Spawned Mage at..."
  - "Auto-starting combat with X players and Y enemies"

**In World Outliner:**
- After pressing Play, check World Outliner
- Look for:
  - "Player Knight"
  - "Player Mage"
  - "False"
  - "Lady Mede"
  - "Wrath"

## 🐛 Troubleshooting

### No Spheres Appear
- Check Output Log for errors
- Verify Game Mode is set correctly
- Make sure you're in Play mode (not just viewing the level)

### Spheres Appear But No Characters
- Characters are spawning but invisible (need Skeletal Meshes)
- Check World Outliner - characters should be listed there
- This is normal - characters work but need visual meshes

### "GameMode is not KyndeBladeGameMode" Error
- Set Default Game Mode in Project Settings
- Or create a Blueprint from KyndeBladeGameMode and set that

### Characters Spawn Underground
- They spawn at Z=200, should be above landscape
- If still underground, adjust spawn locations in GameMode code

## 📍 Spawn Locations

Characters spawn at:
- **Players**: (-500, -150, 200) and (-500, 150, 200)
- **Enemies**: (500, -300, 200), (500, 0, 200), (500, 300, 200)

Move your camera to these locations to see the debug spheres.

## ✅ Success Indicators

You'll know it's working if you see:
1. ✅ Debug spheres appear when you press Play
2. ✅ Output Log shows spawn messages
3. ✅ Characters appear in World Outliner
4. ✅ Combat starts automatically after 1 second

If all of these happen, the system is working! Characters just need visual meshes to be visible.

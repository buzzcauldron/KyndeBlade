# Once the Editor Opens - Quick Start

The Unreal Editor is launching. Once it opens, follow these steps:

## Step 1: Wait for Editor to Load
- Wait for the editor to fully open
- Let it finish any compilation if needed

## Step 2: Open a Level
- Open any level (or create a new Empty Level)
- You should see your landscape

## Step 3: Use Console Command (Easiest Method)

1. **Press Play** (or Alt+P) to enter game mode
2. **Press `** (backtick key, above Tab) to open console
3. **Type**: `SpawnTestCharacters`
4. **Press Enter**
5. Characters spawn immediately!

## Step 4: Check Results

### Output Log:
- **Window → Developer Tools → Output Log**
- Look for messages:
  - "KyndeBladeGameMode: Auto-spawning test characters"
  - "✓ SUCCESS: Spawned Knight at..."
  - "Auto-starting combat with 2 players and 3 enemies"

### Visual:
- **Colored spheres** should appear:
  - 🟢 Green = Knight
  - 🔵 Blue = Mage
  - 🔴 Red = False
  - 🟡 Yellow = Lady Mede
  - 🟣 Magenta = Wrath

### World Outliner:
- Open **World Outliner** panel
- Should see: "Player Knight", "Player Mage", "False", "Lady Mede", "Wrath"

## If Console Command Doesn't Work:

### Alternative: Place TestLevelSetup Actor

1. **Window → Place Actors** (or try Shift+1 again)
2. **Search**: "Test Level Setup"
3. **Drag** into level
4. **Press Play**

## Quick Checklist:
- [ ] Editor fully loaded
- [ ] Level open
- [ ] Pressed Play
- [ ] Opened console (` key)
- [ ] Typed: `SpawnTestCharacters`
- [ ] Pressed Enter
- [ ] Checked Output Log
- [ ] Looked for colored spheres
- [ ] Checked World Outliner

The editor should be opening now. Once it's ready, use the console command method - it's the simplest!

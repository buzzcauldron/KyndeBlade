# Making Characters Visible - Step by Step

## Good News! ✅
Your characters ARE spawning! The colored spheres prove it. They just need visual meshes to be visible.

## Step 1: Create Character Blueprints

### For Player Knight:
1. **Right-click in Content Browser** (bottom panel)
2. **Blueprint Class**
3. **Search**: "Knight Character"
4. **Select** and name it: `BP_PlayerKnight`
5. **Double-click** to open the Blueprint

### In the Blueprint Editor:
1. Look at the **Components** panel (left side)
2. You should see a **Skeletal Mesh Component** (or need to add one)
3. **Select** the Skeletal Mesh Component
4. In **Details** panel (right side), find **Mesh** or **Skeletal Mesh**
5. **Click the dropdown** and either:
   - Use a mesh from **Starter Content** (if you have it)
   - Or **Import** a character mesh from Marketplace/your assets

### Repeat for Other Characters:
- `BP_PlayerMage` (from Mage Character)
- `BP_False` (from False Character)
- `BP_LadyMede` (from Lady Mede Character)
- `BP_Wrath` (from Wrath Character)

## Step 2: Update Spawner to Use Blueprints

### Option A: Update GameMode
1. Find **Project Settings** → **Game** → **Default Game Mode**
2. If there's a Blueprint version, open it
3. Set **Default Knight Class** to `BP_PlayerKnight`
4. Set **Default Mage Class** to `BP_PlayerMage`
5. Set enemy classes to their Blueprints

### Option B: Update TestLevelSetup
1. If you placed TestLevelSetup actor, select it
2. In **Details** panel:
   - Set **Player Knight Class** to `BP_PlayerKnight`
   - Set **Player Mage Class** to `BP_PlayerMage`
   - Set enemy classes to their Blueprints

## Step 3: Quick Test with Starter Content

If you have **Starter Content**:
1. In Content Browser, search for "Mannequin" or "Character"
2. Use the **ThirdPerson** character mesh
3. Assign it to your character Blueprints
4. This gives you a basic humanoid character to see

## Step 4: Test

1. **Press Play**
2. You should now see actual character models instead of just spheres!
3. The spheres will still appear (debug markers) but characters will be visible too

## Alternative: Use Placeholder Meshes

If you don't have character meshes yet:
1. Use **Cube** or **Sphere** static meshes as placeholders
2. At least you'll see something representing the characters
3. Replace with proper meshes later

## What You Should See

**Before:** Just colored spheres
**After:** Character models at the sphere locations

The spheres are debug markers - the actual characters are there, just invisible without meshes!

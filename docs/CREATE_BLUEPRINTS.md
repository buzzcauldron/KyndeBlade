# Creating Blueprints - Automated Script

This guide shows you how to automatically create all required Blueprints using a Python script.

## Method 1: Using Python Console (Recommended)

1. **Open Unreal Editor** with your project

2. **Open Python Console**:
   - Go to **Window → Developer Tools → Python Console**

3. **Run the Script**:
   - In the Python Console, type:
   ```python
   exec(open('/home/sethj/KyndeBlade 5.7/CreateBlueprints.py').read())
   ```
   - Or copy-paste the contents of `CreateBlueprints.py` into the console

4. **Check Output**:
   - The script will print progress messages
   - All blueprints will be created in `/Game/KyndeBlade/`

## Method 2: Manual Creation (If Script Doesn't Work)

If the Python script doesn't work, create blueprints manually:

### 1. Game Mode Blueprint

1. Right-click in Content Browser → **Blueprint Class**
2. Search for "Kynde Blade Game Mode" → Select
3. Name it `BP_KyndeBladeGameMode`
4. Save in `/Game/KyndeBlade/Game/`

### 2. Player Character Blueprints

#### BP_PlayerKnight
1. Right-click → **Blueprint Class**
2. Search for "Knight Character" → Select
3. Name it `BP_PlayerKnight`
4. Save in `/Game/KyndeBlade/Characters/`
5. **Configure**:
   - Open the Blueprint
   - In Components, add or configure **Skeletal Mesh Component**
   - Assign a character mesh (use Starter Content or Marketplace)
   - Set **Character Name** to "Player Knight"
   - Set **Character Class** to "Knight"

#### BP_PlayerMage
1. Right-click → **Blueprint Class**
2. Search for "Mage Character" → Select
3. Name it `BP_PlayerMage`
4. Save in `/Game/KyndeBlade/Characters/`
5. **Configure**:
   - Add Skeletal Mesh Component
   - Assign a mage/wizard mesh
   - Set **Character Name** to "Player Mage"
   - Set **Character Class** to "Mage"

### 3. Enemy Character Blueprints

#### BP_False
1. Right-click → **Blueprint Class**
2. Search for "False Character" → Select
3. Name it `BP_False`
4. Save in `/Game/KyndeBlade/Characters/Enemies/`
5. **Configure**:
   - Add Skeletal Mesh Component
   - Assign a rogue/thief mesh
   - Set **Character Name** to "False"

#### BP_LadyMede
1. Right-click → **Blueprint Class**
2. Search for "Lady Mede Character" → Select
3. Name it `BP_LadyMede`
4. Save in `/Game/KyndeBlade/Characters/Enemies/`
5. **Configure**:
   - Add Skeletal Mesh Component
   - Assign a noble/mage mesh
   - Set **Character Name** to "Lady Mede"

#### BP_Wrath
1. Right-click → **Blueprint Class**
2. Search for "Wrath Character" → Select
3. Name it `BP_Wrath`
4. Save in `/Game/KyndeBlade/Characters/Enemies/`
5. **Configure**:
   - Add Skeletal Mesh Component
   - Assign a warrior/berserker mesh
   - Set **Character Name** to "Wrath"

#### BP_Hunger (Optional Boss)
1. Right-click → **Blueprint Class**
2. Search for "Hunger Character" → Select
3. Name it `BP_Hunger`
4. Save in `/Game/KyndeBlade/Characters/Enemies/`

### 4. Level Setup Blueprint

#### BP_TestLevelSetup
1. Right-click → **Blueprint Class**
2. Search for "Test Level Setup" → Select
3. Name it `BP_TestLevelSetup`
4. Save in `/Game/KyndeBlade/`
5. **Configure**:
   - Open the Blueprint
   - In Details panel:
     - **Player Knight Class**: Set to `BP_PlayerKnight`
     - **Player Mage Class**: Set to `BP_PlayerMage`
     - **False Class**: Set to `BP_False`
     - **Lady Mede Class**: Set to `BP_LadyMede`
     - **Wrath Class**: Set to `BP_Wrath`
     - **Auto Spawn Players**: Checked (true)
     - Adjust spawn locations as needed

### 5. Combat UI Widget

#### WBP_CombatUI
1. Right-click → **User Widget**
2. Search for "Combat UI" → Select
3. Name it `WBP_CombatUI`
4. Save in `/Game/KyndeBlade/UI/`
5. **Design**:
   - Add health bars
   - Add stamina/mana bars
   - Add action buttons
   - Add turn indicator
   - Connect to Turn Manager events

### 6. Combat Action Blueprints

#### BP_Attack
1. Right-click → **Blueprint Class**
2. Search for "Combat Action" → Select
3. Name it `BP_Attack`
4. Save in `/Game/KyndeBlade/Combat/`
5. **Configure**:
   - **Action Type**: Strike
   - **Damage**: 15.0
   - **Stamina Cost**: 10.0
   - **Kynde Generated**: 2.0 (melee attacks generate Kynde)
   - **Break Damage**: 5.0

#### BP_Dodge
1. Right-click → **Blueprint Class**
2. Search for "Combat Action" → Select
3. Name it `BP_Dodge`
4. Save in `/Game/KyndeBlade/Combat/`
5. **Configure**:
   - **Action Type**: Escapade
   - **Stamina Cost**: 15.0
   - **Success Window**: 1.5 seconds

#### BP_Parry
1. Right-click → **Blueprint Class**
2. Search for "Combat Action" → Select
3. Name it `BP_Parry`
4. Save in `/Game/KyndeBlade/Combat/`
5. **Configure**:
   - **Action Type**: Ward
   - **Stamina Cost**: 20.0
   - **Success Window**: 1.0 seconds

#### BP_Rest
1. Right-click → **Blueprint Class**
2. Search for "Combat Action" → Select
3. Name it `BP_Rest`
4. Save in `/Game/KyndeBlade/Combat/`
5. **Configure**:
   - **Action Type**: Rest
   - **Stamina Cost**: 0.0
   - (Rest restores stamina)

## After Creating Blueprints

### 1. Set Default Game Mode
- **Edit → Project Settings → Game → Default Game Mode**
- Set to `BP_KyndeBladeGameMode`

### 2. Configure TestLevelSetup
- Open `BP_TestLevelSetup`
- Set all class references to the blueprints you created
- Adjust spawn locations if needed

### 3. Add to Level
- Drag `BP_TestLevelSetup` into your level
- Press Play to test

## Troubleshooting

### "Class not found" errors
- Make sure the project is compiled
- Close and reopen Unreal Editor
- Check that C++ classes are marked as `BlueprintType`

### Blueprints appear but are empty
- This is normal - you need to configure them
- Add components (Skeletal Meshes) to character blueprints
- Set properties in Combat Action blueprints

### Characters still invisible
- Make sure Skeletal Mesh Components are added
- Assign actual mesh assets to the components
- Check that meshes are visible in the viewport

## Quick Reference: All Blueprints Needed

```
/Game/KyndeBlade/
├── Game/
│   └── BP_KyndeBladeGameMode
├── Characters/
│   ├── BP_PlayerKnight
│   ├── BP_PlayerMage
│   └── Enemies/
│       ├── BP_False
│       ├── BP_LadyMede
│       ├── BP_Wrath
│       └── BP_Hunger (optional)
├── Combat/
│   ├── BP_Attack
│   ├── BP_Dodge
│   ├── BP_Parry
│   └── BP_Rest
├── UI/
│   └── WBP_CombatUI
└── BP_TestLevelSetup
```

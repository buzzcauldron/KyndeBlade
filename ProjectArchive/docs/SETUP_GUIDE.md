# Setup Guide for Kynde Blade

## Quick Start

### Step 1: Open the Project
1. Ensure you have Unreal Engine 5.3 or later installed
2. Double-click `KyndeBlade.uproject`
3. Unreal Engine will prompt to rebuild modules - click "Yes"

### Step 2: First-Time Setup

If the project doesn't compile automatically:

1. **Close Unreal Editor**
2. **Generate Project Files:**
   - Right-click `KyndeBlade.uproject`
   - Select "Generate Visual Studio project files" (Windows)
   - Or use terminal: `UnrealVersionSelector.exe /projectfiles "path/to/KyndeBlade.uproject"`

3. **Compile the Project:**
   - Open the `.sln` file in Visual Studio
   - Set configuration to "Development Editor"
   - Build Solution (F7)
   - Wait for compilation to complete

4. **Reopen the Project:**
   - Double-click `KyndeBlade.uproject` again
   - The project should now load successfully

## Creating Your First Combat Scene

### 1. Create Character Blueprints

1. In Content Browser, right-click → Blueprint Class
2. Search for "Medieval Character" → Select
3. Name it `BP_Knight` (or similar)
4. Open the Blueprint:
   - Set **Character Class** to "Knight"
   - Set **Character Name** to "Sir Galahad" (or your choice)
   - Adjust **Stats** as desired
   - Add a Skeletal Mesh component for visual representation

### 2. Create Combat Actions

1. Right-click → Blueprint Class
2. Search for "Combat Action" → Select
3. Create actions like:
   - `BP_Attack`: Action Type = Attack, Damage = 15, Stamina Cost = 10
   - `BP_Dodge`: Action Type = Dodge, Stamina Cost = 15, Success Window = 1.5
   - `BP_Parry`: Action Type = Parry, Stamina Cost = 20, Success Window = 1.0

### 3. Set Up Game Mode

1. Right-click → Blueprint Class
2. Search for "Kynde Blade Game Mode" → Select
3. Name it `BP_KyndeBladeGameMode`
4. Set as default in Edit → Project Settings → Game → Default Game Mode

### 4. Create Combat UI

1. Right-click → User Widget
2. Create `WBP_CombatUI`
3. Design your combat interface:
   - Health/Stamina bars
   - Action buttons
   - Turn indicator
   - Character portraits

### 5. Create a Test Level

1. Create a new level (File → New Level)
2. Add your character Blueprints to the level
3. Create a Blueprint that:
   - Gets references to player and enemy characters
   - Calls `StartCombatSequence` on the Game Mode
   - Displays the Combat UI

## Testing Combat

### Manual Testing Steps

1. **Start Play in Editor (PIE)**
2. **Trigger Combat:**
   - Use a trigger volume or button
   - Or call `StartCombatSequence` from Blueprint

3. **Test Turn System:**
   - Verify characters act in speed order
   - Check that turns cycle correctly

4. **Test Actions:**
   - Select different actions
   - Verify stamina consumption
   - Check damage application

5. **Test Real-Time Mechanics:**
   - Use Dodge/Parry actions
   - Verify timing windows work
   - Test counter-attacks

## Common Issues and Solutions

### Issue: Project won't compile
**Solution:**
- Ensure Unreal Engine 5.3+ is installed
- Check that all required modules are enabled
- Verify C++ development tools are installed

### Issue: Blueprints can't find C++ classes
**Solution:**
- Recompile the project
- Close and reopen Unreal Editor
- Check that classes are marked as `BlueprintType` and `Blueprintable`

### Issue: Combat doesn't start
**Solution:**
- Verify Turn Manager is spawned
- Check that characters are added to combat arrays
- Ensure `StartCombat` is called after `InitializeCombat`

### Issue: Real-time mechanics not working
**Solution:**
- Verify timing windows are set correctly
- Check that input is being captured during real-time window
- Ensure character's dodge/parry states are being updated

## Next Steps

1. **Add Visual Polish:**
   - Import character models and animations
   - Create particle effects for combat
   - Design UI with medieval theme

2. **Expand Combat:**
   - Add more action types
   - Create special abilities per class
   - Implement status effects

3. **Create Content:**
   - Design levels and environments
   - Write narrative and quests
   - Add sound effects and music

4. **Polish Gameplay:**
   - Balance character stats
   - Tune combat timing
   - Add tutorial system

## Resources

- [Unreal Engine 5 Documentation](https://docs.unrealengine.com/5.3/en-US/)
- [C++ Programming Guide](https://docs.unrealengine.com/5.3/en-US/programming-with-cpp-in-unreal-engine/)
- [Blueprint Visual Scripting](https://docs.unrealengine.com/5.3/en-US/blueprints-visual-scripting-for-unreal-engine/)

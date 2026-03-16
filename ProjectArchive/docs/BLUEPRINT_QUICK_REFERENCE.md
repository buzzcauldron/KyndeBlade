# Blueprint Quick Reference

## Run the Automated Script

**In Unreal Editor Python Console** (Window → Developer Tools → Python Console):

```python
exec(open('/home/sethj/KyndeBlade 5.7/CreateBlueprints.py').read())
```

This will create all blueprints automatically in `/Game/KyndeBlade/`

## Required Blueprints Checklist

- [ ] **BP_KyndeBladeGameMode** (Game Mode)
- [ ] **BP_PlayerKnight** (Player Character)
- [ ] **BP_PlayerMage** (Player Character)
- [ ] **BP_False** (Enemy)
- [ ] **BP_LadyMede** (Enemy)
- [ ] **BP_Wrath** (Enemy)
- [ ] **BP_TestLevelSetup** (Level Setup)
- [ ] **WBP_CombatUI** (UI Widget)
- [ ] **BP_Attack** (Combat Action)
- [ ] **BP_Dodge** (Combat Action)
- [ ] **BP_Parry** (Combat Action)
- [ ] **BP_Rest** (Combat Action)

## Quick Setup Steps

1. **Run the Python script** (see above) OR create manually (see CREATE_BLUEPRINTS.md)

2. **Configure Character Blueprints**:
   - Open each character blueprint
   - Add **Skeletal Mesh Component**
   - Assign a mesh asset
   - Set Character Name

3. **Configure BP_TestLevelSetup**:
   - Set Player Knight Class → `BP_PlayerKnight`
   - Set Player Mage Class → `BP_PlayerMage`
   - Set False Class → `BP_False`
   - Set Lady Mede Class → `BP_LadyMede`
   - Set Wrath Class → `BP_Wrath`

4. **Set Game Mode**:
   - Edit → Project Settings → Game → Default Game Mode
   - Select `BP_KyndeBladeGameMode`

5. **Place in Level**:
   - Drag `BP_TestLevelSetup` into level
   - Press Play

## Blueprint Locations

All blueprints should be in:
```
/Game/KyndeBlade/
```

See CREATE_BLUEPRINTS.md for detailed folder structure.

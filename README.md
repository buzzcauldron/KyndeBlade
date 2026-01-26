# Kynde Blade

A turn-based RPG with real-time combat mechanics, inspired by Expedition 33 (Clair Obscur: Expedition 33) but set in a medieval fantasy world. Named after "Kynde" (Nature) from the medieval poem Piers Plowman.

## Game Overview

Kynde Blade combines traditional turn-based RPG combat with real-time mechanics, allowing players to dodge, parry, and counter attacks during combat. The game features a medieval fantasy setting with knights, mages, archers, and rogues battling against dark forces.

## Key Features

### Combat System
- **Turn-Based Core**: Traditional turn-based combat where characters act in order based on speed
- **Real-Time Mechanics**: During combat, players can:
  - **Dodge**: Avoid incoming attacks with precise timing
  - **Parry**: Block attacks and reduce damage, with counter-attack opportunities
  - **Counter**: Deal increased damage after successful parries
- **Stamina System**: Actions consume stamina, requiring strategic resource management
- **Class-Based Combat**: Each character class has unique stats and abilities

### Character Classes

1. **Knight**
   - High health and defense
   - Moderate attack power
   - Best suited for tanking and melee combat

2. **Mage**
   - High attack power and stamina
   - Low health and defense
   - Special abilities and magic attacks

3. **Archer**
   - Balanced stats
   - High speed
   - Ranged combat specialist

4. **Rogue**
   - High speed and stamina
   - Moderate attack power
   - Evasion and critical strike abilities

## Project Structure

```
KyndeBlade/
├── Source/
│   ├── KyndeBlade/
│   │   ├── Characters/          # Character classes and stats
│   │   ├── Combat/              # Combat system and turn management
│   │   ├── Game/                # Game mode and core gameplay
│   │   ├── UI/                  # User interface widgets
│   │   └── Input/               # Input handling components
│   ├── KyndeBlade.Target.cs
│   └── KyndeBladeEditor.Target.cs
└── KyndeBlade.uproject
```

## Setup Instructions

### Prerequisites
- Unreal Engine 5.3 or later
- Visual Studio 2022 (Windows) or Xcode (Mac) or appropriate IDE for Linux
- C++ development tools

### Building the Project

1. **Open the Project**
   - Double-click `KyndeBlade.uproject`
   - If prompted, allow Unreal Engine to rebuild modules

2. **Generate Project Files** (if needed)
   - Right-click `KyndeBlade.uproject`
   - Select "Generate Visual Studio project files" (Windows) or equivalent for your platform

3. **Compile**
   - Open the generated solution file in your IDE
   - Build the project (F7 in Visual Studio)
   - Or use Unreal Editor's compile button

### Creating Blueprints

The C++ classes are designed to be extended with Blueprints:

1. **Character Blueprints**
   - Create Blueprints based on `MedievalCharacter`
   - Set character class, stats, and appearance
   - Add animations and visual effects

2. **Combat Actions**
   - Create Blueprint classes based on `CombatAction`
   - Define different attack types, special abilities, etc.
   - Configure damage, stamina cost, and timing windows

3. **UI Widgets**
   - Create Blueprint widgets based on `CombatUI`
   - Design the combat interface
   - Connect to turn manager events

4. **Game Mode**
   - Create a Blueprint game mode based on `KyndeBladeGameMode`
   - Set default classes and game rules

## Gameplay Flow

1. **Combat Initialization**
   - Player and enemy characters are added to combat
   - Turn order is calculated based on speed
   - First character's turn begins

2. **Turn Execution**
   - Current character can select an action
   - Actions include: Attack, Dodge, Parry, Counter, Special, Wait
   - Target selection (for targeted actions)

3. **Real-Time Window** (for Dodge/Parry)
   - Time window opens for real-time input
   - Player must time their dodge/parry correctly
   - Success/failure affects damage taken

4. **Action Resolution**
   - Damage is calculated and applied
   - Stamina is consumed
   - Status effects are processed

5. **Turn End**
   - Next character in turn order acts
   - Process continues until one side is defeated

## Extending the Game

### Adding New Character Classes
1. Extend `ECharacterClass` enum in `MedievalCharacter.h`
2. Add initialization logic in `MedievalCharacter::BeginPlay()`
3. Create Blueprint character with custom stats

### Creating New Combat Actions
1. Create Blueprint class based on `CombatAction`
2. Set `ActionData` properties (damage, stamina cost, timing window)
3. Implement `OnActionExecuted` event for visual effects

### Customizing Combat Rules
- Modify `TurnManager` for different turn order systems
- Adjust damage formulas in `MedievalCharacter::TakeDamage()`
- Add status effects and buffs/debuffs

## Medieval Theme Elements

The game is designed with medieval fantasy in mind:
- Character classes reflect medieval archetypes
- Combat mechanics emphasize melee combat with shields and weapons
- Environment should feature castles, dungeons, and medieval landscapes
- Narrative can involve quests, knights, and mythical creatures
- Named after "Kynde" (Nature) from Piers Plowman, representing the natural order

## Future Enhancements

- Equipment system (weapons, armor, accessories)
- Skill trees and character progression
- Party management and formation
- Environmental interactions
- Story mode with quests and dialogue
- Multiplayer support
- Advanced AI for enemy characters

## License

This project is created for educational and development purposes.

## Credits

Inspired by Clair Obscur: Expedition 33 by Sandfall Interactive, adapted with a medieval fantasy theme. Project name inspired by "Kynde" (Nature) from William Langland's Piers Plowman.

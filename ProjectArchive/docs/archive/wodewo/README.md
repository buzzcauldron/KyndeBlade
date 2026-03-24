# Wode-Wo Archive

Wode-Wo material removed from active game files. Preserved here for reference or future restoration.

## Contents

- `WodeWoManager.cs` - Main Wode-Wo logic
- `InstallState.cs` - Install-level persistence for Wode-Wo death
- `WODE_WO_COMPANION.md` - Design documentation

## Original Integration Points

- **GameStateManager**: DefeatWithWodeWoDeath, isMajorBoss check
- **KyndeBladeGameManager**: WodeWoManager creation in EnsureMapPipeline
- **TutorialManager**: UseWodeWoVoice, GetTutorialBeat
- **MapLevelSelectUI**: InstallState.WodeWoIsDead label
- **WorldMapManager**: AfterWodeWoDeathCutscene, StoryBeatOnArrivalWhenWodeWoDead
- **LocationNode**: StoryBeatOnArrivalWhenWodeWoDead field (kept in script, unused)

# Wode-Wo: The Kind Forest Monster Companion

Wode-Wo is a kind forest spirit—moss and leaf, root and bark—who guides the dreamer through the tutorial. His death is **permanent to the game installation**: once he dies, he never returns, even across New Game.

## Design

- **Introduction**: Malvern Hills prologue. The player finds Wode-Wo as a **baby**—a tiny seedling, moss and leaf, root and bark, no bigger than a fist. Thou shelterest it beneath the oak, givest it water from St Anne's well, tendest it as dream-days pass. It grows. Roots deepen. Bark hardens. One morning it speaks: *"Wode-Wo. I am the keeper of this threshold. I shall walk with thee, dreamer. Fear not."*
- **Tutorial**: Wode-Wo's voice guides each phase (Attack, Dodge, Parry, Counter). On completion: *"Thou hast learned. I shall stay by thy side."*
- **Death trigger**: The **first** of either:
  1. **Wrong dialogue choice** (e.g. at the Green Chapel tree)
  2. **Defeat to a major boss** (Hunger, Green Knight, Pride, sin miniboss)
- **Death cutscene**: The fae take him—pale hands from the gloaming, pulling him apart root from branch from breath. His last words: *"Dreamer... I tried my best... to..."* Cold fingers close. What remains is scattered. The forest weeps. He does not rise again.
- **Persistence**: Stored in `PlayerPrefs` under `KyndeBlade_Install_WodeWo_IsDead`. **NewGame does not clear this.** Only `InstallState.ResetForTesting(includeWodeWo: true)` resets it (for development).
- **Body persistence**: Once Wode-Wo is dead, his scattered remains persist at Malvern across all runs. Returning to Malvern shows the WodeWoRemains story beat: *"The seedling thou once raised—root and branch and breath—never to rise again."* The map UI always shows: *"Wode-Wo's scattered remains lie at Malvern. The forest mourns."* — reinforcing the permanent loss and building cumulative punishment/frustration.

## Technical

| Component | Role |
|-----------|------|
| `InstallState` | Install-level persistence. `WodeWoIsDead` never cleared by save/load. |
| `WodeWoManager` | Singleton. Tracks Wode-Wo state, triggers death cutscene, provides tutorial beats. |
| `GameStateManager.Defeat()` | On defeat to major boss → `TriggerWodeWoDeath()` before defeat panel. |
| `WorldMapManager.OnChoiceProceed` | On wrong choice → `AfterWodeWoDeathCutscene()` before continuing. |
| `LocationNode.StoryBeatSequenceOnArrival` | When non-empty, show beats in sequence (e.g. Malvern: baby → care → grown). |
| `LocationNode.StoryBeatOnArrivalWhenWodeWoDead` | Optional beat shown on arrival when Wode-Wo is dead (e.g. Malvern → WodeWoRemains). |
| `WorldMapManager.TransitionTo` | Uses `StoryBeatOnArrivalWhenWodeWoDead` instead of `StoryBeatOnArrival` when `InstallState.WodeWoIsDead`. |
| `MapLevelSelectUI.Refresh` | Appends Wode-Wo remains reminder to current location label when `InstallState.WodeWoIsDead`. |
| `TutorialManager` | Uses `WodeWoManager.GetTutorialBeat()` for Wode-Wo voice when `UseWodeWoVoice` is true. |

## Major Boss Encounters

- Hunger (Dongeoun Depths)
- Green Knight (Green Chapel)
- Pride
- Any sin miniboss (from wrong dialogue choice)

## Assets

- **WodeWoBaby**: Finding the seedling at Malvern (CreateVision1LevelData)
- **WodeWoCare**: Caring for and raising it—shelter, water, growth (CreateVision1LevelData)
- **WodeWoGrown**: It speaks, becomes companion (CreateVision1LevelData)
- **WodeWoDeath**: Story beat at `Data/Vision1/WodeWoDeath.asset` (auto-loaded by WodeWoManager if not assigned)
- **WodeWoRemains**: Story beat shown when returning to Malvern after Wode-Wo's death (CreateVision1LevelData, assigned to `malvern.StoryBeatOnArrivalWhenWodeWoDead`)
- **Tutorial beats**: WodeWoManager creates defaults per phase if not assigned in Inspector

## Testing

To reset Wode-Wo for testing:
```csharp
InstallState.ResetForTesting(includeWodeWo: true);
```

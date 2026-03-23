# Godot audio design (open-license policy)

Combat audio in `KyndeBlade_Godot` is built so **shipping builds need no uncleared samples** unless you deliberately add packs under `assets/third_party/` with `LICENSE.txt`.

## Principles

1. **Procedural first** — Short cues are generated at runtime as 16-bit PCM (`CombatWindowTone`) so they are **project-authored** (same license as game code). Document rows in repo [`docs/ASSET_LICENSES.md`](../../docs/ASSET_LICENSES.md).
2. **CC0 / CC BY for any file-based SFX or music** — Follow [`ProjectArchive/docs/MUSIC_SOURCES.md`](../../ProjectArchive/docs/MUSIC_SOURCES.md); mirror license text in-repo.
3. **Buses** — `AudioBusSetup` creates **Music** and **SFX**; Master volume from `SaveService` scales the Master bus. Keep combat one-shots on **SFX**.

## Current cues

| Moment | Implementation | Notes |
|--------|----------------|--------|
| Defensive window opens | `CombatWindowTone.make_tone_hz` (real vs feint pitch) | Duration scales down when the window is short so the click fits inside the **parry band** (~170–230 ms). |
| Enemy kill (victory) | `CombatWindowTone.make_enemy_kill_impact()` | Sub-heavy thump + decay + short noise; plays during slow-motion + desaturate pass (`combat_root.gd`). |

## Parry timing vs sound

Parry reaction time is **170–230 ms** from `PlayerMovesetModifiers.parry_window_ms()` (hunger, ethical missteps, `parry_window_ms_penalty` in `medieval_text_unlocks.json`). Window-open cues use **~32–38 ms** envelopes in that regime so players still hear the edge of the window.

## Replacing with licensed libraries

When swapping in **Kenney**, **CC0 OGA**, or similar packs, keep filenames + `LICENSE.txt` per pack and add a row to `ASSET_LICENSES.md`. Prefer **mono** SFX at a consistent loudness; use Godot’s **RandomPitchStream** (or small `pitch_scale` variance) sparingly so Lane B stays readable.

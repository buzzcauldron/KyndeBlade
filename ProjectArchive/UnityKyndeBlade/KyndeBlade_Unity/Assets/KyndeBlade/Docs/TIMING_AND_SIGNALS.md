# Parry/Dodge Timing and Signalling

Human-oriented timings and clear cues so success is driven by **skill** (timing, attention), not guesswork.

## Default windows

- **Parry (Ward):** `GameWorldConstants.DefaultParryWindowSeconds` = **2 seconds**.
- **Dodge (Escapade):** `GameWorldConstants.DefaultDodgeWindowSeconds` = **2 seconds**.

`TurnManager` uses the action’s `SuccessWindow`; that value is set from these constants in `Expedition33Moveset` and `DefaultCombatActions`. **GameSettings** can scale the window for difficulty/accessibility (`GetAdjustedWindow`).

## Eye phases (ParryDodgeZoneIndicator)

Animation is phased so the “input now” moment is readable:

| Phase        | Window share | Visual |
|-------------|--------------|--------|
| **Open**    | First ~15%   | Pupil at start scale, eyelids open. |
| **Steady**  | Middle ~60%  | Little change; player can prepare. |
| **Strike imminent** | Last ~25% | Pupil contracts, eyelids close with SmoothStep; clear “press now” moment. |

Configurable via `OpenPhaseEnd` (0.15) and `ImminentPhaseStart` (0.75). Optional: subtle pulse or gold tint in the last 0.3–0.5 s can be added for a “perfect” window cue.

## Aural cues

- **Window start:** One-shot from `ParryDodgeZoneSoundBank` (strike warning by attacker theme).
- **Imminent:** When remaining time &lt; `ImminentThresholdSeconds` (default 0.4 s), a second clip plays (e.g. rising tone or tick). Assign `ImminentClip` in the Inspector.

Skill (timing, attention) is the primary driver of success; these cues support readability without removing the challenge.

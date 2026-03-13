# Core Loop Definition Of Done

Core combat loop is considered locked only when all criteria are satisfied.

## Stability
- No crashes or blockers in a 10-minute sandbox run.
- Combat snapshot save/load fields persist and clear correctly.

## Readability
- New players can explain why they took damage in most deaths.
- Telegraph and hit feedback are consistently visible.

## Balance Targets
- Baseline enemy time-to-kill: 8-14 seconds.
- Baseline player time-to-die under repeated mistakes: 10-18 seconds.
- Difficulty scalars produce predictable shifts across Easy/Normal/Hard.

## Input & Feel
- Input buffer and defense coyote windows feel reliable.
- No visible action lock or state-transition desync in normal flow.

## Performance
- Combat frame time remains within budget targets during peak wave load.
- No sustained spike warnings from performance budget monitor.

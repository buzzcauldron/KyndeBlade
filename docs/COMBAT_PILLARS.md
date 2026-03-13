# Combat Pillars (Week 1 Baseline)

## Purpose
Define the minimum feel and balance targets for the first combat sandbox pass.

## Pillar 1: Responsive Control
- Input-to-action response feels immediate and predictable.
- Target: player can trigger dodge/parry/attack without noticeable delay at stable frame rate.
- Guardrail: avoid adding new cast delays unless they improve readability.

## Pillar 2: Readable Cause and Effect
- Players should understand why damage happened.
- Target: in quick playtests, players correctly explain damage cause in most deaths.
- Guardrail: telegraphs and hit feedback must be clear before increasing enemy complexity.

## Pillar 3: Tension With Recoverability
- Encounters should pressure players without feeling unfair.
- Target: baseline encounter should be winnable without perfect execution, but punish repeated mistakes.
- Guardrail: prevent overlap chains that remove player response windows.

## Initial Numeric Targets
- Time-to-kill (single baseline enemy): 8-14 seconds.
- Time-to-die (baseline player under repeated mistakes): 10-18 seconds.
- Parry window baseline: 0.30-0.45 seconds.
- Dodge window baseline: 0.35-0.50 seconds.
- Turn-to-turn downtime target: under 2.0 seconds for common actions.

## Week 1 Metrics To Capture
- `ability_used`: action type and actor.
- `hit`: source, target, and inferred hit damage.
- `damage_taken`: source, target, and amount.
- `death_cause`: defeated target and last damage source.
- `wave_time`: encounter duration and aggregate counters.

## Definition of Done (Week 1)
- One repeatable sandbox encounter is playable end-to-end.
- Metrics are emitted for full encounter runs.
- No crash or blocker during a 10-minute combat session.
- Baseline tuning values are documented after first playtest pass.

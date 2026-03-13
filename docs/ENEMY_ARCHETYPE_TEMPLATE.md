# Enemy Archetype Template

Use `EnemyArchetypeTemplate` assets to keep enemy tuning consistent.

## Fields
- Identity: display name, class, sound theme.
- Combat stats: health, stamina, attack, defense, speed.
- Readability timing: baseline telegraph seconds and minimum recovery.

## Recommended Usage
1. Create template asset from `KyndeBlade/Enemy Archetype Template`.
2. Assign it to enemy components such as `MeleePressureEnemy` or `RangedPressureEnemy`.
3. Tune stats in one place; avoid per-prefab drift.

## Initial Archetype Intent
- Melee pressure: durable frontline, break threat, slower speed.
- Ranged pressure: lower durability, higher speed, chip/punish pattern.

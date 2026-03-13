# Combat QA Checklist (Repeatable)

Run this checklist after each tuning pass.

1. Start sandbox wave 1 and complete encounter.
2. Start sandbox wave 2 and verify no spawn overlap at start.
3. Use `Strike` repeatedly until stamina low; confirm `Rest` recovers.
4. Trigger `Escapade` window and test early-input forgiveness.
5. Trigger `Ward` window and test early-input forgiveness.
6. Validate counter window triggers after successful parry.
7. Confirm hit feedback appears (camera shake + floating damage text).
8. Confirm telegraph action labels are visible before impact.
9. Verify `CombatMetric` logs include `ability_used`, `hit`, `damage_taken`.
10. Verify `death_cause` log appears on at least one defeat.
11. Verify `wave_time` metric prints on encounter completion.
12. Change difficulty to Easy and verify enemy durability drops.
13. Change difficulty to Hard and verify enemy durability increases.
14. Save during active encounter and reload; confirm snapshot fields persist.
15. Complete encounter and verify combat snapshot is cleared.

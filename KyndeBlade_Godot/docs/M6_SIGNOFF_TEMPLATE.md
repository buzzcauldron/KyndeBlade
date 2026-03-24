# M6 sign-off record (template)

Copy this file to `docs/M6_SIGNOFF_YYYY-MM-DD.md` (or attach contents to the M6 PR). **Human-gated** — see [`M6_CUTOVER_CHECKLIST.md`](../M6_CUTOVER_CHECKLIST.md) and [`DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md`](DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md) §5.

## Metadata

| Field | Value |
|-------|--------|
| Date | |
| Git SHA (`main`) | |
| Unity Editor version (archive tree) | |
| Signatories (names + roles) | |

## Evidence checklist

- [ ] `godot --path KyndeBlade_Godot --headless res://tests/headless_main.tscn` → `HEADLESS_TESTS: PASS` (log attached or CI link)
- [ ] [`STEAM_BUILD.md`](../STEAM_BUILD.md) manual QA completed for current slice
- [ ] [`PARITY_GAPS.md`](../PARITY_GAPS.md) reviewed; intentional differences acknowledged
- [ ] Unity **Export Slice Data for Godot** run if `exported_from_unity.json` or slice data changed
- [ ] BDD / `.tdad/bdd/godot-parity-slice.feature` — `@manual` scenarios listed as manual-only in PR body
- [ ] Rollback tag created on parent commit: `pre-m6-oracle-handoff` (or name agreed by leads)

## TDAD / README

- [ ] [`ProjectArchive/UnityKyndeBlade/README.md`](../../ProjectArchive/UnityKyndeBlade/README.md) snapshot table filled
- [ ] Root [`README.md`](../../README.md) states Godot as **live behaviour oracle** for Tier A (wording per [`.tdad/prompts/godot-archive-unity-snapshot.md`](../../.tdad/prompts/godot-archive-unity-snapshot.md))

## Notes

Free text: scope of oracle handoff, known follow-ups, crawl/voxel/GPU CI deferred items.

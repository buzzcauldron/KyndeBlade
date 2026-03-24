# M6 cutover checklist (human-gated)

Use this when TDAD milestone **godot-m6-cutover-archive** is formally approved. **Do not** remove or restructure [`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity`](../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity) without explicit sign-off.

**Design (evidence bundle, RACI, TDAD wording):** [`docs/DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md`](docs/DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md) §5. **Sign-off template:** [`docs/M6_SIGNOFF_TEMPLATE.md`](docs/M6_SIGNOFF_TEMPLATE.md).

## Preconditions

- [ ] Headless tests pass: `godot4 --path KyndeBlade_Godot --headless res://tests/headless_main.tscn` (or `--headless --script res://tests/run_headless_tests.gd` if your runner uses that entry)
- [ ] Manual QA in [`STEAM_BUILD.md`](STEAM_BUILD.md) complete for the shipping slice
- [ ] [`PARITY_GAPS.md`](PARITY_GAPS.md) reviewed; intentional differences accepted by leads
- [ ] Unity **Export Slice Data for Godot** run and `data/exported_from_unity.json` committed if data changed

## Actions (after sign-off only)

1. Run [`.tdad/prompts/godot-archive-unity-snapshot.md`](../.tdad/prompts/godot-archive-unity-snapshot.md) process (metadata, README, TDAD/CI wording).
2. Confirm Unity already lives at [`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity`](../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity); only run a physical `git mv` if you are fixing a legacy layout.
3. Update root [`README.md`](../README.md) and CI paths if needed; declare Godot the live oracle in TDAD docs.

## Rollback

Keep a tagged commit **before** M6 so Unity can be restored if the cutover is reverted.

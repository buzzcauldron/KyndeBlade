# M6 cutover checklist (human-gated)

Use this when TDAD milestone **godot-m6-cutover-archive** is formally approved. **Do not** move or archive [`KyndeBlade_Unity`](../KyndeBlade_Unity) without explicit sign-off.

## Preconditions

- [ ] Headless tests pass: `godot4 --path KyndeBlade_Godot --headless --script res://tests/run_headless_tests.gd`
- [ ] Manual QA in [`STEAM_BUILD.md`](STEAM_BUILD.md) complete for the shipping slice
- [ ] [`PARITY_GAPS.md`](PARITY_GAPS.md) reviewed; intentional differences accepted by leads
- [ ] Unity **Export Slice Data for Godot** run and `data/exported_from_unity.json` committed if data changed

## Actions (after sign-off only)

1. Run [`.tdad/prompts/godot-archive-unity-snapshot.md`](../.tdad/prompts/godot-archive-unity-snapshot.md) process.
2. Move Unity tree to [`ProjectArchive/UnityKyndeBlade/`](../ProjectArchive/UnityKyndeBlade/README.md) per team convention.
3. Update root [`README.md`](../README.md) and CI paths; declare Godot the live oracle in TDAD docs.

## Rollback

Keep a tagged commit **before** M6 so Unity can be restored if the cutover is reverted.

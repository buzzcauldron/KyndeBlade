# Unity KyndeBlade — reference archive policy

## Status

- **`KyndeBlade_Godot/`** is the **primary** integration target for the shipping slice: gameplay, headless CI, Steam-oriented saves, and TDAD **godot-*** workflows.
- **`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/`** is kept in-repo as a **frozen reference**: behaviour oracle for historical parity, **Export Slice Data for Godot**, and design docs that still cite C# types.

This is **not** a deletion of Unity; it is an explicit **roles split**. New combat/world features ship in Godot first unless a maintainer documents an exception in [`KyndeBlade_Godot/PARITY_GAPS.md`](../../KyndeBlade_Godot/PARITY_GAPS.md).

## Human-gated M6 cutover

Formal “Unity archived, Godot canonical for all tests” sign-off remains **human-gated** per [`.tdad/prompts/godot-archive-unity-snapshot.md`](../../.tdad/prompts/godot-archive-unity-snapshot.md) and [`KyndeBlade_Godot/M6_CUTOVER_CHECKLIST.md`](../../KyndeBlade_Godot/M6_CUTOVER_CHECKLIST.md). Until then, treat Unity **DEMO_RUN** / **PLAYABLE_SLICE** as the **behaviour oracle** where Godot has not yet documented intentional drift.

## What maintainers still do in Unity

- Run **KyndeBlade → Export Slice Data for Godot** when narrative/export JSON must change.
- Consult **TurnManager**, **CombatUI**, and TDAD-linked C# when aligning or documenting parity.
- Run Unity EditMode/PlayMode tests only when validating the archive tree itself ([`docs/CI_UNITY_TESTS.md`](../../docs/CI_UNITY_TESTS.md)).

## Related

- [`README.md`](README.md) — Unity folder layout and M6 snapshot table  
- [Root `README.md`](../../README.md) — repo quick start  
- [`docs/KYNDEBLADE_CAREFUL_CANON.md`](../../docs/KYNDEBLADE_CAREFUL_CANON.md) — planning canon  

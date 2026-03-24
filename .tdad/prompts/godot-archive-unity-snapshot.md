# Archive Unity snapshot (M6 only — human-gated)

**Repo state:** The Unity project already lives at **`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/`**. Use this checklist for **M6 sign-off** (frozen SHA, Unity version, README/TDAD/CI updates), not for a first-time `git mv` unless you are recovering from a legacy layout.

Run this **only after** `godot-m6-cutover-archive` exit criteria are met: `godot-parity-slice` green, exported Godot build smoke OK, lead sign-off. **Evidence bundle / RACI:** [`KyndeBlade_Godot/docs/DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md`](../../KyndeBlade_Godot/docs/DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md) §5.

## Pre-flight

- [ ] Record **git SHA**: `git rev-parse HEAD`
- [ ] Record **Unity Editor version** last used on `ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity`
- [ ] Optional: export **.unitypackage** or zip of `ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity` for cold storage (not required for git history)

## Repository steps (destructive — confirm with team)

1. If Unity is **not** yet under `ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity`, create `ProjectArchive/UnityKyndeBlade/` if missing and move: `git mv KyndeBlade_Unity ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity` (or OS move + `git add -A`). *(Skip if already done.)*
2. Add/update [`ProjectArchive/UnityKyndeBlade/README.md`](../../ProjectArchive/UnityKyndeBlade/README.md): frozen SHA, Unity version, **reason** (Godot cutover), link to Godot project path.
3. Update root [`README.md`](../../README.md): primary engine = Godot; Unity = oracle under ProjectArchive.
4. Update [`docs/TDAD_RELEASE_PATHS.md`](../../docs/TDAD_RELEASE_PATHS.md): note **live oracle** is Godot; Unity workflows remain as **historical spec** where applicable.
5. Commit message suggestion: `chore: M6 Unity archive metadata and Godot oracle handoff`

## Do not

- Archive before **M4** minimum slice path in Godot (plan risk).
- `git push --force` or destructive git without explicit owner approval.

## Post-archive

- CI: disable or retarget Unity batchmode jobs; enable Godot headless/GUT per [`docs/CI_GODOT_TESTS.md`](../../docs/CI_GODOT_TESTS.md).

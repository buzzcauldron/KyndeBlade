# Archive Unity snapshot (M6 only — human-gated)

Run this **only after** `godot-m6-cutover-archive` exit criteria are met: `godot-parity-slice` green, exported Godot build smoke OK, lead sign-off.

## Pre-flight

- [ ] Record **git SHA**: `git rev-parse HEAD`
- [ ] Record **Unity Editor version** last used on `KyndeBlade_Unity`
- [ ] Optional: export **.unitypackage** or zip of `KyndeBlade_Unity` for cold storage (not required for git history)

## Repository steps (destructive — confirm with team)

1. Create `ProjectArchive/UnityKyndeBlade/` if missing.
2. Move Unity project: `git mv KyndeBlade_Unity ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity` (or OS move + `git add -A`).
3. Add/update [`ProjectArchive/UnityKyndeBlade/README.md`](../../ProjectArchive/UnityKyndeBlade/README.md): frozen SHA, Unity version, **reason** (Godot cutover), link to Godot project path.
4. Update root [`README.md`](../../README.md): primary engine = Godot; Unity = archived under ProjectArchive.
5. Update [`docs/TDAD_RELEASE_PATHS.md`](../../docs/TDAD_RELEASE_PATHS.md): note **live oracle** is Godot; Unity workflows remain as **historical spec**.
6. Commit message suggestion: `chore: archive Unity under ProjectArchive after Godot M6 sign-off`

## Do not

- Archive before **M4** minimum slice path in Godot (plan risk).
- `git push --force` or destructive git without explicit owner approval.

## Post-archive

- CI: disable or retarget Unity batchmode jobs; enable Godot headless/GUT per [`docs/CI_GODOT_TESTS.md`](../../docs/CI_GODOT_TESTS.md).

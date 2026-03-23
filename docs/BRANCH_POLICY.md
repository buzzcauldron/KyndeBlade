# Git branch policy

## Default branch: `main` (Godot-first)

The **`main`** branch is the **integration default** for day-to-day work. It carries:

- **`KyndeBlade_Godot/`** — primary shipping target for the slice (Steam / desktop); headless tests in CI.
- **`KyndeBlade_Unity/`** — retained for parity, export pipeline (**Export Slice Data for Godot**), and TDAD/oracle workflows until archive policy changes.
- **`.tdad/`**, **`docs/`**, **`ProjectArchive/`** — shared planning and history.

**Behaviour oracle** for the Tier A demo slice is still defined in [`KYNDEBLADE_CAREFUL_CANON.md`](KYNDEBLADE_CAREFUL_CANON.md) (Unity `PLAYABLE_SLICE` / `DEMO_RUN` until M6 sign-off, unless you update both engines and BDD). Branch name does not change that rule.

## Legacy integration branch: `unity`

Historical Godot + Unity integration work lived on **`unity`**. That line is **merged into `main`**; use **`main`** for new clones and PRs.

- If you still have a local **`unity`** branch, it is safe to delete after `git fetch origin && git checkout main && git pull`:  
  `git branch -d unity`

## Pre-cutover `main` snapshot

The remote tip that was **`main` before** the Godot-first cutover is preserved as **`legacy/main-before-godot-cutover`** (same commit graph as before the force push). Use it only if you need to compare or recover something that never landed on the Godot integration line.

## Other branches

- **`unreal`** — Unreal / experimental line; not the default product path.
- **`kyndeblade-5.7`** remote — separate fork pointer; ignore unless you work that remote.

## Clone / PR checklist

```bash
git clone <url> KyndeBlade
cd KyndeBlade
git checkout main
git pull origin main
```

Open **`KyndeBlade_Godot/`** in **Godot 4.6.1** (or **4.6.x**) for the shipping slice; open **`KyndeBlade_Unity/`** in Unity when editing the oracle project or running the export menu.

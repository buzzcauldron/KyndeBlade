# UI manuscript motifs (shipped pixels)

**Cleared** textures for **in-game** UI (ornaments, panels), produced with the **pixel reference pipeline** below.

## Pipeline (from staging references)

1. **Source** — copy only **cleared** references into the repo (never commit **copyrighted modern trade covers** or unlicensed third-party game screenshots). Staging drops under editor/chat paths must be **vetted** before copy.
2. **Crop** — isolate the motif (historiated letter, margin figure, tier ornament).
3. **Downscale** — **nearest-neighbor** to target size (e.g. 64–256 px on the long edge).
4. **Palette** — optional quantize toward colors in [`scripts/kyndeblade_art_palette.gd`](../../scripts/kyndeblade_art_palette.gd) (gold, lapis, rubrication, parchment ramp).
5. **Hand pass** — 1–2 px outlines if readability fails at game resolution.
6. **Ship gate** — row in [`docs/ASSET_LICENSES.md`](../../../docs/ASSET_LICENSES.md) with **provenance** (institution, shelfmark, or “project-authored derivative”).

**Regenerate shipped motif:** [`tools/pixel_ref_manuscript_motif.py`](../tools/pixel_ref_manuscript_motif.py) (defaults to the staging manuscript fragment path; use `--source` / `--output` as needed).

## Files

| File | Use |
|------|-----|
| [`historiated_margin_crowned_figure.png`](historiated_margin_crowned_figure.png) | Margin-style illumination motif on hub Fair Field panel (`hub_map.tscn`). |

**Import:** In Godot, set this texture’s import **Filter** to **Nearest** if scaling blurs the pixel read.

## Excluded from builds

- **Modern edition jackets** (e.g. trade **Sir Gawain** covers) — **reference only**; do not vendor or pixelate for redistribution.
- **Staging-only** PNGs under `.cursor/.../assets/` — copy **only** after rights review; prefer **known open-access** manuscript digitizations for retail.

See also [`assets/reference_manuscript/README.md`](../reference_manuscript/README.md).

# Manuscript reference art (pixel pipeline)

Use this folder for **cleared**, **in-repo** manuscript-derived textures after you confirm **license / scan terms** (see [`docs/ASSET_LICENSES.md`](../../docs/ASSET_LICENSES.md)).

**Shipped UI crops** (game-ready ornaments) live under [`assets/ui_manuscript/`](../ui_manuscript/README.md) once processed.

## Workflow

1. Crop the motif from a high-res reference.
2. Downscale with **nearest-neighbor**; optional quantize to palette colors from `KyndeBladeArtPalette`.
3. Commit PNG/WebP + a row in `ASSET_LICENSES.md` (institution, shelfmark or URL, license).

Do **not** ship **copyrighted modern covers** (e.g. trade editions) as textures — recreate original silhouettes instead.

## Staging outside the repo

Files dropped under editor or chat asset paths should be **copied here** before they are referenced by Godot scenes so exports and CI stay reproducible.

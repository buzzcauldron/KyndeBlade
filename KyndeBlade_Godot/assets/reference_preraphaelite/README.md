# Pre-Raphaelite / Salome reference art (optional)

Drop **cleared** reference images here when you want stable, in-repo mood boards for:

- **Salome**-like figures: jewel crimson, emerald, ultramarine/violet shadows, dramatic stillness ([`ART_DIRECTION_VISUAL_BIBLE.md`](../../../ProjectArchive/docs/ART_DIRECTION_VISUAL_BIBLE.md)).
- **Alan Lee**–adjacent environment refs (separate files).
- **Manuscript / illumination plates** you author or license for KyndeBlade — same rules as any raster (row in [`docs/ASSET_LICENSES.md`](../../../docs/ASSET_LICENSES.md)).

## Before you commit any PNG/JPEG

1. Confirm **license** (public domain, CC, or team-authored).
2. Add a row to repo [`docs/ASSET_LICENSES.md`](../../../docs/ASSET_LICENSES.md) with path, source URL, and attribution if required.
3. Prefer **WebP** or **PNG**; keep file size reasonable for Git.

## Code hook

Runtime colors are driven by [`JEWEL_*` constants in `scripts/kyndeblade_art_palette.gd`](../../scripts/kyndeblade_art_palette.gd), not by sampling these files automatically. When you lock a final plate, you may **manually** adjust those hex values to match.

The visual bible also links an external Salome reference from [`VISUAL_DESIGN_ALAN_LEE.md`](../../../ProjectArchive/docs/VISUAL_DESIGN_ALAN_LEE.md) — useful until vendored art lands here.

## Other external mood (do not vendor without rights)

- **Coastal / atmosphere photography:** [sethcoastisthebestcoast](https://www.instagram.com/sethcoastisthebestcoast/) — inspiration for light, weather, and palette restraint; see [`docs/ART_DIRECTION_GODOT.md`](../../docs/ART_DIRECTION_GODOT.md) → *Photographic mood*. Same rule: **no commits** from the feed without license/permission and [`docs/ASSET_LICENSES.md`](../../../docs/ASSET_LICENSES.md).

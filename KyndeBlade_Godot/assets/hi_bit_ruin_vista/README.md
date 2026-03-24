# Hi-bit ruin vista — visual target (Lane B / exploration)

## Reference image

| File | Role |
|------|------|
| [`reference_style_target.png`](reference_style_target.png) | **Style bible** — hi-bit pixel landscape: overgrown medieval ruins, viaduct scale, tiny figure, dawn/teal sky. |

Use this file for **mood boards**, **texture paint-over**, or (when licensed for ship) a **full-screen backdrop** in Godot (`TextureRect` under `combat.tscn` → `BackdropLayer`, expand mode full rect, behind `ManuscriptBackdrop` or replace procedural layer).

## Style notes (for artists & shaders)

- **Read:** Modern hi-bit pixel — texture, atmospheric depth, side-on exploration framing (parallax-friendly).
- **Palette (code):** [`KyndeBladeArtPalette`](../../scripts/kyndeblade_art_palette.gd) **`HI_BIT_*`** constants are a **high-contrast, higher-saturation** read of this PNG (coral clouds, vivid teal–mint sky, lime foliage, rich terracotta, deep slate shadows). Adjust there first; procedural backdrop uses the same ramps.
- **Palette (art):** Sage greens, teals in mist; **terracotta / warm brick** on stone; sky **teal → peach/apricot** (dusk/dawn glow).
- **Layers (4–5):** Distant pale-teal cliffs → misty mid-cliffs → main stonework (bridge/piers) → foreground foliage framing.
- **Technique:** Soft dither in clouds and stone shading; diffused light from the glowing sky; no harsh cut shadows.
- **Mood:** Serene, ancient, lonely — fits dream-vision, *Piers* field, Green Knight decay, ink-combat foreground.

## Code hook

Procedural combat backdrop approximates these colors in [`scripts/combat_manuscript_backdrop.gd`](../../scripts/combat_manuscript_backdrop.gd) via [`KyndeBladeArtPalette`](../../scripts/kyndeblade_art_palette.gd) **`HI_BIT_*`** constants.

## License / provenance

**Treat as project reference until cleared for distribution.** If this PNG is third-party art, replace this README row in [`docs/ASSET_LICENSES.md`](../../../docs/ASSET_LICENSES.md) with source URL + license before Steam ship. If it is team-authored, mark *project-authored* in that table.

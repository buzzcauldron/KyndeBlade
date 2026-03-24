#!/usr/bin/env python3
"""Regenerate `assets/ui_manuscript/historiated_margin_crowned_figure.png`.

Pipeline: crop (right margin) → nearest-neighbor resize → quantize to KyndeBladeArtPalette.
Requires Pillow. Default source is the staging manuscript fragment under the Cursor project
assets folder; override with --source.

See `assets/ui_manuscript/README.md` and `docs/ASSET_LICENSES.md`.
"""
from __future__ import annotations

import argparse
from pathlib import Path

from PIL import Image

# Manuscript-relevant colors from `scripts/kyndeblade_art_palette.gd` (RGB)
PALETTE: list[tuple[int, int, int]] = [
    (0x0A, 0x08, 0x06),  # INK_PRIMARY
    (0x2A, 0x20, 0x18),  # INK_SECONDARY
    (0xFF, 0xF4, 0xDC),  # PARCHMENT_LIGHT
    (0xF2, 0xE0, 0xBC),  # PARCHMENT
    (0xDE, 0xB8, 0x82),  # PARCHMENT_AGED
    (0xE8, 0xA0, 0x10),  # GOLD
    (0xA0, 0x68, 0x20),  # GOLD_DARK
    (0xBC, 0x10, 0x20),  # RUBRICATION
    (0x18, 0x58, 0xC8),  # LAPIS
    (0x5C, 0x48, 0x38),  # INK_LIGHT
]


def _nearest(rgb: tuple[int, ...]) -> tuple[int, int, int]:
    r, g, b = rgb[:3]
    best = PALETTE[0]
    bd = 10**9
    for p in PALETTE:
        d = (r - p[0]) ** 2 + (g - p[1]) ** 2 + (b - p[2]) ** 2
        if d < bd:
            bd = d
            best = p
    return best


def main() -> None:
    repo_godot = Path(__file__).resolve().parents[1]
    default_staging = Path.home() / ".cursor/projects/Users-halxiii-Projects-KyndeBlade/assets/image-e4dd8dc1-d508-4568-b5af-61663d8fc93f.png"
    out_default = repo_godot / "assets/ui_manuscript/historiated_margin_crowned_figure.png"

    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("--source", type=Path, default=default_staging, help="Staging manuscript PNG")
    p.add_argument("--output", type=Path, default=out_default, help="Output path")
    p.add_argument("--crop-left-fraction", type=float, default=0.58, help="Crop from this x-fraction to right edge")
    p.add_argument("--max-edge", type=int, default=128, help="Longest edge after resize (nearest-neighbor)")
    args = p.parse_args()

    if not args.source.is_file():
        raise SystemExit(f"missing source: {args.source}")

    im = Image.open(args.source).convert("RGB")
    w, h = im.size
    x0 = int(w * args.crop_left_fraction)
    crop = im.crop((x0, 0, w, h))
    scale = args.max_edge / max(crop.size)
    nw = max(1, int(crop.size[0] * scale))
    nh = max(1, int(crop.size[1] * scale))
    small = crop.resize((nw, nh), Image.Resampling.NEAREST)
    pix = small.load()
    assert pix is not None
    for y in range(nh):
        for x in range(nw):
            pix[x, y] = _nearest(pix[x, y])

    args.output.parent.mkdir(parents=True, exist_ok=True)
    small.save(args.output, "PNG")
    print(f"wrote {args.output} ({nw}x{nh})")


if __name__ == "__main__":
    main()

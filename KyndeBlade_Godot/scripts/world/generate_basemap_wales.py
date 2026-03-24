#!/usr/bin/env python3
"""Legacy simplified Wales silhouette (PIL polygon). Shipped basemap uses Natural Earth — see generate_basemap_wales_natural_earth.py and docs/NAV_MAP_BASEMAP.md."""

from __future__ import annotations

import sys
from pathlib import Path

try:
	from PIL import Image, ImageDraw
except ImportError as e:
	print("Install Pillow: pip install Pillow", file=sys.stderr)
	raise SystemExit(1) from e

# Must match `basemap.bounds` in data/world/hub_route_map.json
WEST, EAST, SOUTH, NORTH = -5.55, -2.65, 51.35, 53.55
W, H = 512, 384

# Simplified closed loop in WGS84 (placeholder silhouette, not survey data)
WALES_LONLAT: list[tuple[float, float]] = [
	(-5.28, 51.42),
	(-4.95, 51.38),
	(-3.55, 51.52),
	(-2.95, 51.62),
	(-2.78, 52.05),
	(-3.0, 52.55),
	(-3.15, 52.95),
	(-3.45, 53.25),
	(-3.95, 53.42),
	(-4.55, 53.52),
	(-5.05, 53.35),
	(-5.38, 52.75),
	(-5.45, 52.1),
	(-5.35, 51.55),
	(-5.28, 51.42),
]


def ll_to_px(lon: float, lat: float) -> tuple[float, float]:
	x = (lon - WEST) / (EAST - WEST) * W
	y = (NORTH - lat) / (NORTH - SOUTH) * H
	return x, y


def main() -> None:
	root = Path(__file__).resolve().parents[2]
	out = root / "assets" / "world" / "basemap_wales.png"
	out.parent.mkdir(parents=True, exist_ok=True)
	img = Image.new("RGB", (W, H), (0xE8, 0xDC, 0xC8))
	draw = ImageDraw.Draw(img)
	poly = [ll_to_px(a, b) for a, b in WALES_LONLAT]
	draw.polygon(poly, fill=(0xA8, 0x9A, 0x88), outline=(0x78, 0x6C, 0x5C))
	img.save(out, "PNG")
	print(out)


if __name__ == "__main__":
	main()

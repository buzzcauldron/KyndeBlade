#!/usr/bin/env python3
"""Rasterize Wales land from Natural Earth admin-1 (UK regions tagged Wales). See assets/third_party/natural_earth_10m_admin1/README.md."""

from __future__ import annotations

import sys
from pathlib import Path

try:
	import shapefile
	from PIL import Image, ImageDraw
except ImportError as e:
	print("pip install pyshp Pillow", file=sys.stderr)
	raise SystemExit(1) from e

# Must match `basemap.bounds` in data/world/hub_route_map.json
WEST, EAST, SOUTH, NORTH = -5.55, -2.65, 51.35, 53.55
MAX_W = 1024


def _lonlat_to_px(lon: float, lat: float, w: int, h: int) -> tuple[float, float]:
	x = (lon - WEST) / (EAST - WEST) * w
	y = (NORTH - lat) / (NORTH - SOUTH) * h
	return x, y


def _wales_shape_indices(reader: shapefile.Reader) -> list[int]:
	out: list[int] = []
	for i, rec in enumerate(reader.records()):
		d = rec.as_dict()
		if d.get("admin") != "United Kingdom":
			continue
		if "Wales" in str(d.get("region", "")):
			out.append(i)
	return out


def _iter_rings(shape: shapefile.Shape) -> list[list[tuple[float, float]]]:
	pts = shape.points
	parts = list(shape.parts) + [len(pts)]
	rings: list[list[tuple[float, float]]] = []
	for pi in range(len(parts) - 1):
		a, b = parts[pi], parts[pi + 1]
		ring = [(float(p[0]), float(p[1])) for p in pts[a:b]]
		if len(ring) >= 3:
			rings.append(ring)
	return rings


def main() -> None:
	shp_dir = Path(sys.argv[1] if len(sys.argv) > 1 else "/tmp/ne_basemap")
	shp_path = shp_dir / "ne_10m_admin_1_states_provinces.shp"
	if not shp_path.is_file():
		print(f"Missing shapefile: {shp_path}", file=sys.stderr)
		print("Download: https://naciscdn.org/naturalearth/10m/cultural/ne_10m_admin_1_states_provinces.zip", file=sys.stderr)
		raise SystemExit(1)

	reader = shapefile.Reader(str(shp_path))
	lon_span = EAST - WEST
	lat_span = NORTH - SOUTH
	h = max(1, int(round(MAX_W * lat_span / lon_span)))
	w = MAX_W

	mask = Image.new("L", (w, h), 0)
	draw = ImageDraw.Draw(mask)
	for idx in _wales_shape_indices(reader):
		shp = reader.shape(idx)
		for ring in _iter_rings(shp):
			pix = [_lonlat_to_px(lon, lat, w, h) for lon, lat in ring]
			try:
				draw.polygon(pix, fill=255, outline=255)
			except (TypeError, ValueError):
				continue

	godot_root = Path(__file__).resolve().parents[2]
	out = godot_root / "assets" / "world" / "basemap_wales.png"
	out.parent.mkdir(parents=True, exist_ok=True)

	parchment = (0xE8, 0xDC, 0xC8)
	land = (0xA8, 0x92, 0x7A)
	rgb = Image.new("RGB", (w, h), parchment)
	rgb_px = rgb.load()
	mpx = mask.load()
	for iy in range(h):
		for ix in range(w):
			if mpx[ix, iy] > 128:
				rgb_px[ix, iy] = land
	rgb.save(out, "PNG")
	print(out)


if __name__ == "__main__":
	main()

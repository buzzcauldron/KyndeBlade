# Natural Earth — admin 1 (10m cultural)

Used to **regenerate** [`../../world/basemap_wales.png`](../../world/basemap_wales.png): Welsh unitary authorities (UK admin-1 features whose `region` contains `"Wales"`) are rasterized inside the WGS84 bbox in [`data/world/hub_route_map.json`](../../../data/world/hub_route_map.json) (`basemap.bounds`).

## Source

- **Dataset:** [Natural Earth 1:10m Admin 1 — States, provinces](https://www.naturalearthdata.com/downloads/10m-cultural-vectors/)
- **Direct zip:** `https://naciscdn.org/naturalearth/10m/cultural/ne_10m_admin_1_states_provinces.zip`

## License / attribution

See [`LICENSE.txt`](LICENSE.txt). Natural Earth is **public domain**; crediting **Natural Earth** and **NACIS** is requested on their [terms page](https://www.naturalearthdata.com/about/terms-of-use/).

## Regenerate basemap

Requires **Python 3** + **Pillow** + **pyshp**:

```bash
pip install Pillow pyshp
# Extract the zip to a folder (e.g. /tmp/ne_basemap) containing ne_10m_admin_1_states_provinces.shp
python3 KyndeBlade_Godot/scripts/world/generate_basemap_wales_natural_earth.py /path/to/folder/with/shapefile
```

Then re-run Godot import on `assets/world/basemap_wales.png` if the importer cache is stale.

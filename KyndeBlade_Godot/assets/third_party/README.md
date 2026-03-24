# Third-party art and audio (Godot)

Drop **CC0** or **CC BY** (etc.) packs here, **one folder per pack**:

```
assets/third_party/<pack_slug>/
  LICENSE.txt          # copy from publisher; required
  *.png / *.ogg / ...  # imported by Godot
```

Then add a row to [repo `docs/ASSET_LICENSES.md`](../../../docs/ASSET_LICENSES.md).

**Lane A** (menu, story, map interstitials): full-width backdrops, parchment-friendly contrast — see [`ART_DIRECTION.md`](../../../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md).

**Lane B** (combat): sprites, hazards — document **PPU** if pixel art; dark void backdrop per direction.

Do **not** commit huge textures unless needed; prefer reasonable resolution and Godot import defaults.

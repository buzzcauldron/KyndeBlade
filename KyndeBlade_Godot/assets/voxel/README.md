# Voxel art pipeline (combat + future crawl)

**Design:** [`docs/DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md`](../../docs/DESIGN_CRAWL_VOXEL_SHADER_CI_M6.md) §2.

## Layout (target)

| Path | Use |
|------|-----|
| `combat/arena/` | Imported GLTF/GLB arena chunks to replace or layer on [`combat_voxel_arena.gd`](../../scripts/combat_voxel_arena.gd) |
| `crawl/tiles/` | Orthographic renders or sprite sheets for fake-voxel `TileMapLayer` |

## Authoring

1. Model in **MagicaVoxel** (or compatible) → `.vox`.  
2. Export **glTF** (binary `.glb` preferred) or render **PNG** tiles for 2D crawl.  
3. Drop under a subfolder here; add **license** file + row in [`docs/ASSET_LICENSES.md`](../../../docs/ASSET_LICENSES.md).

## Import (Godot 4)

Use default 3D importer; prefer **merge meshes** for static arena geometry. Combat actors stay separate `Node3D` children in `combat.tscn`.

# Physics and Materials for Unity Environments

KyndeBlade uses **Unity 3D Physics** for environment collision and optional rigidbodies. This doc describes how to keep physics and materials in place for environments (ground, walls, props).

---

## Quick setup

1. **Create physics materials and layers:** In the Unity menu choose **KyndeBlade > Setup Physics and Layers**.
   - Creates `Assets/KyndeBlade/Physics/` with:
     - **Default** — general use (0.6 friction, no bounce).
     - **Environment** — ground/walls (0.8 friction, no bounce).
     - **Slippery** — low friction (e.g. ice).
   - Adds layers: **Environment** (6), **Player** (7), **Enemy** (8), **Trigger** (9).
   - Sets the project **Default Material** (Edit > Project Settings > Physics) to Default.

2. **Environment objects:** Add the **EnvironmentPhysicsSetup** component to any GameObject that should act as ground, wall, or movable prop.
   - **Body Type:** Static (collider only), Kinematic (moved by script), or Dynamic (full rigidbody).
   - **Material:** Assign **Environment** (or Default/Slippery) from `KyndeBlade/Physics`.
   - If the object has no Collider, a **BoxCollider** is added at runtime using the **Box Size** field.

---

## Component: EnvironmentPhysicsSetup

- **Static** — Adds a Collider if missing; no Rigidbody. Use for floors, walls, immovable scenery.
- **Kinematic** — Adds a Rigidbody with `isKinematic = true` (no gravity, moved by script). Use for moving platforms or doors.
- **Dynamic** — Adds a Rigidbody with gravity and forces. Use for pushable props or debris.

Colliders are created as **BoxCollider** by default; you can replace with MeshCollider or other shapes in the Editor. The component only adds a collider when none is present.

---

## Layers and collision matrix

After running **Setup Physics and Layers**, configure the **Collision Matrix** (Edit > Project Settings > Physics > Layer Collision Matrix) as needed:

- **Environment** — Typically collides with Player, Enemy, and Default (so characters and objects hit the ground and walls).
- **Player** / **Enemy** — Assign these to character prefabs or runtime-spawned characters if you want player–enemy or environment–character separation; combat is turn-based and does not require rigidbodies on characters.
- **Trigger** — Use for trigger volumes (e.g. one-way transitions); disable collision with Environment if triggers should not block movement.

**MedievalCharacter** only requires a **Collider** (no Rigidbody). Assign layer **Player** or **Enemy** to character prefabs if you use the new layers.

---

## Physics materials

| Material    | Use              | Friction (dynamic/static) | Bounce |
|------------|-------------------|---------------------------|--------|
| Default    | General           | 0.6 / 0.6                 | 0      |
| Environment| Ground, walls     | 0.8 / 0.8                 | 0      |
| Slippery   | Ice, slides       | 0.05 / 0.05               | 0.1    |

Create more via **Assets > Create > Physic Material** and assign in **EnvironmentPhysicsSetup** or on Colliders directly.

---

## Scene checklist

- [ ] Run **KyndeBlade > Setup Physics and Layers** once per project.
- [ ] Add **EnvironmentPhysicsSetup** to ground/floor (Static, Material = Environment).
- [ ] Add **EnvironmentPhysicsSetup** to walls or large static meshes (Static, or MeshCollider + Material).
- [ ] Optional: assign **Player** / **Enemy** layers to character prefabs; set **Environment** on environment objects.
- [ ] Optional: kinematic/dynamic props use **EnvironmentPhysicsSetup** with Kinematic or Dynamic and the desired Material.

---

## Notes

- **Gravity** is set in Project Settings > Physics (default -9.81 Y). Combat is turn-based and does not rely on physics for movement; rigidbodies are for environment and optional props.
- **2D physics:** If you add 2D gameplay later, create **Physics Material 2D** assets and use **Rigidbody2D** / **Collider2D**; the same layering and material concepts apply.

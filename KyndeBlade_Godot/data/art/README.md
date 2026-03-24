# Art placeholder data

- **`placeholder_registry.json`** — Maps every `locations_registry.json` **location id** to a **level backdrop preset** (Lane A / hi-bit / void reads from [`docs/ART_DIRECTION_GODOT.md`](../../docs/ART_DIRECTION_GODOT.md)) plus optional **`preview_character`** silhouette.
- **Characters** — Keys such as `will_dreamer`, `langage_false`, `green_knight` match procedural shapes in [`scripts/art/placeholder_silhouette_library.gd`](../../scripts/art/placeholder_silhouette_library.gd).

**Editor:** enable addon **KyndeBlade Art Placeholders** → **Project → Tools → KyndeBlade: Validate art placeholders**.

**Runtime:** [`PlaceholderLocationBackdrop`](../../scripts/art/placeholder_location_backdrop.gd) + [`PlaceholderActor2D`](../../scripts/art/placeholder_actor_2d.gd) on [`scenes/world/location_shell.tscn`](../../scenes/world/location_shell.tscn). Reusable scene: [`scenes/placeholders/character_placeholder.tscn`](../../scenes/placeholders/character_placeholder.tscn).

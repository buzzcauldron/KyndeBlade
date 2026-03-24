# Learning Godot scripting (Kynde Blade)

Use **this repo** as the classroom: the **corrective tutorial** below fixes common misconceptions, then walks you through real files. The **supplement** sections link the official Godot 4 manual and a trace of production navigation code.

**Engine:** Godot **4.6.x**, project root [`KyndeBlade_Godot/`](../), config in [`project.godot`](../project.godot).

---

## Corrective tutorial — learn Godot in Kynde Blade

### Why “corrective”?

Many learners start with [Nodes and Scenes](https://docs.godotengine.org/en/stable/getting_started/step_by_step/nodes_and_scenes.html). That page teaches the **editor** (nodes, Inspector, saving `.tscn`, F5 vs F6), **not** how to write GDScript. Thinking “I finished Nodes and Scenes, so I know Godot coding” is the main trap.

**Corrections:**

| Wrong assumption | Better mental model |
|------------------|---------------------|
| “The first tutorial taught me to code.” | It taught **composition** and **running scenes**. Code starts at [Creating your first script](https://docs.godotengine.org/en/stable/getting_started/step_by_step/scripting_first_script.html). |
| “I should change the main scene to my test scene.” | In **this** project, `run/main_scene` is **`res://scenes/main_menu.tscn`**. For experiments, use **Run Current Scene** (F6) on a sandbox scene instead. |
| “Singletons are advanced; I’ll skip them.” | This game already uses **autoloads** (`GameState`, `SaveService`, `ManuscriptNav`, …). You read them, you don’t invent them on day one. |

### Safety rules for this repo

1. Prefer **Project → Run Current Scene** (F6) for anything under [`scenes/learning/`](../scenes/learning/).
2. Do **not** change `run/main_scene` in `project.godot` unless you intend to change how everyone boots the game.
3. After editing shared scripts (`bootstrap/save_service.gd`, `bootstrap/game_state.gd`, `ui/main_menu.gd`, autoloads), run headless smoke from repo root:  
   `godot4 --path KyndeBlade_Godot --headless res://tests/headless_main.tscn`

---

### Lesson 0 — Orient without writing code

**Goal:** See how *this* project is wired.

1. Open [`project.godot`](../project.godot). Note `run/main_scene="res://scenes/main_menu.tscn"`.
2. In the same file, read the `[autoload]` block: `SaveService`, `GameState`, `ManuscriptNav`, etc. These are **global** — any script can reference them by name.
3. In the Godot editor, open [`scenes/main_menu.tscn`](../scenes/main_menu.tscn). Select the root node `MainMenu`. In the Inspector, confirm the **Script** field points to `main_menu.gd`.

**Checkpoint:** You can name the main scene path and at least two autoloads from memory.

---

### Lesson 1 — Read a real script tied to a scene

**Goal:** Connect “scene root” ↔ “`.gd` file”.

1. Open [`scripts/main_menu.gd`](../scripts/main_menu.gd).
2. Find `const HUB` — this is how the menu refers to another scene by **path string**.
3. Find `_ready()`: notice `SaveService.save_changed.connect(_refresh_continue)` — a **signal** connected in code (not only in the editor).
4. Find `_on_new_game_pressed()` and `_on_continue_pressed()`: two different ways to reach the hub (`ManuscriptNav` vs `change_scene_to_file`).

**Checkpoint:** You can explain in one sentence what happens when **New Game** is pressed (high level: save + state + go to hub).

---

### Lesson 2 — Your first script *inside* this project

**Goal:** Write GDScript without hijacking the main game flow.

1. In the editor: **Scene → New Scene** → root **Control** (or **User Interface** preset).
2. Add a child **Label**; set **Text** to something like `Learning Kynde Blade`.
3. **Attach script** to the root **Control**; save script as `res://scenes/learning/hello_kynde.gd` (create the folder if needed — it exists in repo with `.gitkeep`).
4. Use this minimal body (Godot 4 GDScript):

```gdscript
extends Control

func _ready() -> void:
	print("Kynde Blade learning sandbox OK")
```

5. Save the scene as `res://scenes/learning/hello_kynde.tscn`.
6. With **hello_kynde.tscn** open, press **Run Current Scene** (F6). Watch the **Output** dock for the print. **Do not** press F5 unless you want the full game.

**Checkpoint:** You see your label and the print; the main menu is still the default main scene.

---

### Lesson 3 — See how *this* game handles input

**Goal:** Map the manual’s “input actions” to real action names.

1. Read [`scripts/input_map_setup.gd`](../scripts/input_map_setup.gd) — defaults for strike / dodge / parry / pause when the map is empty.
2. Open [`scripts/combat_root.gd`](../scripts/combat_root.gd) and search for `is_action_pressed`: note `strike`, `dodge`, `parry`, `pause` used on **InputEvent** in `_unhandled_input`.
3. Optional: [`scripts/beginner_loop.gd`](../scripts/beginner_loop.gd) uses `dodge` and `ui_confirm`.

**Checkpoint:** You know where actions are defined and one place they are consumed.

---

### Lesson 4 — Signals: editor wiring you can see

**Goal:** Tie the official [Using signals](https://docs.godotengine.org/en/stable/getting_started/step_by_step/signals.html) doc to this repo.

1. Open [`scenes/main_menu.tscn`](../scenes/main_menu.tscn) as text, or use the **Node** dock → **Signals** on a button.
2. Scroll to the `[connection signal="pressed" ...]` lines at the bottom: each connects a **Button** to a method on the root (`_on_new_game_pressed`, etc.).
3. Compare with `SaveService.save_changed.connect(...)` in `main_menu.gd` — same idea, done in code.

**Checkpoint:** You can point to one editor connection and one `connect()` in code.

---

### Lesson 5 — Custom signal: hub route map

**Goal:** Follow a **project-defined** signal across two scripts.

1. Open [`scripts/hub_route_map.gd`](../scripts/hub_route_map.gd); find `signal travel_requested(location_id: String)`.
2. Open [`scripts/hub_map.gd`](../scripts/hub_map.gd); in `_ready()`, find `route_map.travel_requested.connect(_on_route_travel_requested)`.
3. Read `_on_route_travel_requested` (and helpers it calls) to see how **location_id** drives UI and travel.

**Checkpoint:** You can describe “map UI emits → hub reacts” in your own words.

---

### Lesson 6 — Autoload: scene transition

**Goal:** See an autoload as a **service**, not magic.

1. Open [`scripts/manuscript_nav.gd`](../scripts/manuscript_nav.gd).
2. Read `turn_page_to`: tween overlay, then scene change. This is why **New Game** can `await ManuscriptNav.turn_page_to(HUB)` from the menu.

**Checkpoint:** You know why `ManuscriptNav` is not a node in every scene tree — it lives above as autoload.

---

### Lesson 7 — What to do next

- Work through the **official step-by-step** list in the supplement (instancing → first script → input → signals) using **their** tiny demo *or* repeat Lesson 2–4 with harder experiments in `scenes/learning/`.
- Optional bigger exercise: [Your first 2D game](https://docs.godotengine.org/en/stable/getting_started/first_2d_game/index.html) in a **separate** Godot project if you want a clean sandbox; bring patterns back here.

---

## Supplement — official manual order

1. [Nodes and Scenes](https://docs.godotengine.org/en/stable/getting_started/step_by_step/nodes_and_scenes.html) — editor, `.tscn`, F5 vs F6, main scene.
2. [Overview of Godot’s key concepts](https://docs.godotengine.org/en/stable/getting_started/introduction/key_concepts_overview.html) — tree, signals.
3. [Creating instances](https://docs.godotengine.org/en/stable/getting_started/step_by_step/instancing.html)
4. [Scripting languages](https://docs.godotengine.org/en/stable/getting_started/step_by_step/scripting_languages.html) — GDScript here.
5. [Creating your first script](https://docs.godotengine.org/en/stable/getting_started/step_by_step/scripting_first_script.html)
6. [Listening to player input](https://docs.godotengine.org/en/stable/getting_started/step_by_step/scripting_player_input.html)
7. [Using signals](https://docs.godotengine.org/en/stable/getting_started/step_by_step/signals.html)

**Checklist:** Attach `.gd`, use `_ready()`, read an action name, connect `Button.pressed`.

---

## Supplement — trace: main menu → hub

| Step | File | Role |
|------|------|------|
| Main scene | [`scenes/main_menu.tscn`](../scenes/main_menu.tscn) | Root `Control` + [`scripts/main_menu.gd`](../scripts/main_menu.gd). |
| Hub | [`scenes/hub_map.tscn`](../scenes/hub_map.tscn) | [`scripts/hub_map.gd`](../scripts/hub_map.gd). |

- **New Game:** `ManuscriptNav.turn_page_to(HUB)` after save/state setup.
- **Continue:** `get_tree().change_scene_to_file(HUB)` after `GameState.apply_loaded(...)`.
- **Signals:** Editor `pressed` on buttons; `SaveService.save_changed`; `route_map.travel_requested` on hub.

Further autoloads: see `[autoload]` in `project.godot`. Reading: [Autoloads versus regular nodes](https://docs.godotengine.org/en/stable/tutorials/best_practices/autoloads_versus_internal_nodes.html).

---

## Supplement — optional 2D game tutorial

- [Your first 2D game](https://docs.godotengine.org/en/stable/getting_started/first_2d_game/index.html) — full loop (player, mobs, HUD).

---

## Supplement — IDE and reference

- [Visual Studio Code + Godot](https://docs.godotengine.org/en/stable/engine_details/development/configuring_an_ide/visual_studio_code.html)
- [Scene organization](https://docs.godotengine.org/en/stable/tutorials/best_practices/scene_organization.html)
- Editor **Help → Search** for class reference; manual **Scripting → GDScript**

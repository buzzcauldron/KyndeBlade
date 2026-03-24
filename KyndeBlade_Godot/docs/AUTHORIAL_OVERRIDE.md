# Authorial override (one intentional scar)

**Problem:** Full Unity ↔ Godot parity can **smooth** bold aesthetic choices. Cult work needs **one** place you refuse to match the oracle.

## Process

1. **Pick one screen** (only one active scar at a time unless you consciously add more):
   - e.g. `tower_intro.tscn`, defeat outcome, post-victory hub, main menu.
2. **Describe the override** (1–3 bullets): what differs from Unity and **why** it’s better for soul.
3. **Document in** [`PARITY_GAPS.md`](../PARITY_GAPS.md) — row: *Intentional; not gameplay regression.*
4. **Implement in Godot** (shader, layout, copy, audio) without waiting for Unity to match.

## Current override (edit when you ship one)

| Field | Value |
|--------|--------|
| **Scene / resource** | *(e.g. `scenes/tower_intro.tscn`)* — **TBD** |
| **Override** | *(e.g. Godot-only parchment shader + rubrication)* |
| **Parity** | Unity oracle waived for **this screen** until optional future align |
| **Owner** | *(name)* |
| **Date** | *(YYYY-MM-DD)* |

## When to clear or promote

- **Clear:** scar becomes default on both engines → remove from “override” and delete PARITY_GAPS exception.  
- **Promote:** Unity adopts the scar → update oracle + DEMO_RUN screenshot.

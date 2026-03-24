# Plan Steam / Early Access milestones (Tier B)

You are working in the **KyndeBlade** repo. Tier B is **planning and store operations**, not Unity feature implementation unless explicitly requested.

**TDAD map:** [`.tdad/README.md`](../README.md). **Godot Steam ship layer** (paths, autosave, CI, export): [`godot-steam-build`](../workflows/godot-steam-build/godot-steam-build.workflow.json) + [`STEAM_BUILD.md`](../../KyndeBlade_Godot/STEAM_BUILD.md).

## Rules

1. **Do not** generate application source under `ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity` unless the user asks for a code change.
2. **Read first:** [`.tdad/workflows/steam-early-access/steam-early-access.workflow.json`](../workflows/steam-early-access/steam-early-access.workflow.json) for milestone order and exit criteria in each node description.
3. **Tier A gate:** The playable demo path must meet **Tier A DoD** in [`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/DEMO_RUN.md`](../../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/DEMO_RUN.md) before treating Steam milestones as “ready to ship.” Parallel work is OK for **non-blocking** assets (e.g. capsule sketches) if they do not depend on false promises vs the current build.
4. **Output:** For each milestone node, produce:
   - **Checklist** (concrete tasks, owners optional)
   - **Risks** (what could slip; what to cut)
   - **Proof** (what artifact proves exit criteria—link or file path)
   - **Demo dependency** (what the Tier A zip must already prove for this milestone to be honest)

## Milestone order (summary)

1. Scope lock  
2. Store page + capsule art (depends on 1)  
3. Trailer + screenshots (depends on 2)  
4. Depots + build branches (depends on 1; parallel with 2–3 where possible)  
5. Playtest wave (depends on 4 and 3)  
6. Pricing + release window (depends on 5)

## Input (optional)

{{#if focusMilestoneId}}
**Focus milestone:** `{{focusMilestoneId}}` — expand only this node into a full checklist.
{{/if}}

{{#if extraContext}}
### Extra context
{{extraContext}}
{{/if}}

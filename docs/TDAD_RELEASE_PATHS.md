# TDAD release paths (demo vs Steam)

Short map of **Test-Driven Agile Development (TDAD)** artifacts in this repo for shipping.

**Master index:** [`.tdad/README.md`](../.tdad/README.md) — tiers, root graph, BDD, prompts.

## Tier A — playable demo (build)

| Artifact | Path |
|----------|------|
| Dependency graph (root) | [`.tdad/workflows/root.workflow.json`](../.tdad/workflows/root.workflow.json) |
| Demo workflow | [`.tdad/workflows/demo-vertical-slice/demo-vertical-slice.workflow.json`](../.tdad/workflows/demo-vertical-slice/demo-vertical-slice.workflow.json) |
| Gherkin scenarios | [`.tdad/bdd/demo-vertical-slice.feature`](../.tdad/bdd/demo-vertical-slice.feature) |
| Player / QA runbook | [`KyndeBlade_Unity/Assets/KyndeBlade/Docs/DEMO_RUN.md`](../KyndeBlade_Unity/Assets/KyndeBlade/Docs/DEMO_RUN.md) |
| Slice narrative path | [`KyndeBlade_Unity/Assets/KyndeBlade/Docs/PLAYABLE_SLICE.md`](../KyndeBlade_Unity/Assets/KyndeBlade/Docs/PLAYABLE_SLICE.md) |

**Prompts:** [`.tdad/prompts/generate-bdd.md`](../.tdad/prompts/generate-bdd.md), [`.tdad/prompts/generate-tests.md`](../.tdad/prompts/generate-tests.md) — use when adding new Tier A nodes.

## Tier B — Steam / EA (plan)

| Artifact | Path |
|----------|------|
| Milestone workflow | [`.tdad/workflows/steam-early-access/steam-early-access.workflow.json`](../.tdad/workflows/steam-early-access/steam-early-access.workflow.json) |
| Expansion prompt | [`.tdad/prompts/plan-steam-milestones.md`](../.tdad/prompts/plan-steam-milestones.md) |

Tier B **depends on** Tier A in the root graph (`steam-early-access` → `demo-vertical-slice`) to keep ordering honest: wishlist/store work should not assume features the demo has not proven.

## Godot port + Unity archive (gated)

| Artifact | Path |
|----------|------|
| Full port milestones (M1–M6) | [`.tdad/workflows/godot-full-port/godot-full-port.workflow.json`](../.tdad/workflows/godot-full-port/godot-full-port.workflow.json) |
| Parity slice (mirrors Tier A demo) | [`.tdad/workflows/godot-parity-slice/godot-parity-slice.workflow.json`](../.tdad/workflows/godot-parity-slice/godot-parity-slice.workflow.json) |
| Gherkin (Godot) | [`.tdad/bdd/godot-parity-slice.feature`](../.tdad/bdd/godot-parity-slice.feature) |
| Expand a milestone (prompt) | [`.tdad/prompts/plan-godot-port-milestone.md`](../.tdad/prompts/plan-godot-port-milestone.md) |
| M6 archive checklist (human-gated) | [`.tdad/prompts/godot-archive-unity-snapshot.md`](../.tdad/prompts/godot-archive-unity-snapshot.md) |
| Godot Steam / retail build + QA | [`KyndeBlade_Godot/STEAM_BUILD.md`](../KyndeBlade_Godot/STEAM_BUILD.md) |
| **TDAD plan (Godot ship layer)** | [`.tdad/workflows/godot-steam-build/godot-steam-build.workflow.json`](../.tdad/workflows/godot-steam-build/godot-steam-build.workflow.json) (depends on `godot-parity-slice` in [`root.workflow.json`](../.tdad/workflows/root.workflow.json)) |
| Godot save trace (Unity `save-system`) | Node *Godot SaveService (Steam retail paths)* in [`.tdad/workflows/save-system/save-system.workflow.json`](../.tdad/workflows/save-system/save-system.workflow.json) |
| Godot CI / headless tests | [`docs/CI_GODOT_TESTS.md`](CI_GODOT_TESTS.md) |
| Unity archive target (populate at M6) | [`ProjectArchive/UnityKyndeBlade/README.md`](../ProjectArchive/UnityKyndeBlade/README.md) |

**Oracle:** Until M6 sign-off, Unity **demo-vertical-slice** + `PLAYABLE_SLICE.md` / `DEMO_RUN.md` define expected behavior. After moving `KyndeBlade_Unity` under `ProjectArchive/`, Godot parity tests become the live contract; Unity TDAD nodes remain as historical spec.

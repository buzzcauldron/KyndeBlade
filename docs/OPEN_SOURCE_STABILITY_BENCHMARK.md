# Open-Source Benchmark: Structure and Stability

This study compares KyndeBlade against mature open-source Unity architectures and maps their patterns into concrete stability revisions.

## Repositories and docs reviewed

- [Unity Open Project 1: Game architecture overview](https://github.com/UnityTechnologies/open-project-1/wiki/Game-architecture-overview)
- [Unity Open Project 1: ScriptableObject event system](https://github.com/UnityTechnologies/open-project-1/wiki/Event-system)
- [Unity Boss Room architecture (Netcode sample)](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.7/manual/samples/bossroom/architecture.html)
- [Unity PaddleGameSO](https://github.com/UnityTechnologies/PaddleGameSO)

## Key patterns from those projects

1. **Single bootstrap path**
   - Startup/Initialization scene is enforced.
   - Avoids random scene-start behavior and missing managers.

2. **Composition roots**
   - One application root plus scene-specific roots.
   - Dependencies are bound once, then consumed by systems.

3. **Event-driven decoupling**
   - ScriptableObject event channels or typed Pub/Sub.
   - Reduces hard manager references and hidden order dependencies.

4. **Assembly/domain boundaries**
   - Code grouped by domain (Core, Combat, Editor, etc.).
   - Better compile iteration and fewer accidental cross-module dependencies.

5. **Data-driven actions/config**
   - ScriptableObject-driven actions and configs.
   - Lower code churn for balance/content changes.

6. **Guardrails in editor tooling**
   - Play-mode/startup guards.
   - Pre-flight checks for known regressions.

## KyndeBlade current state (quick audit)

- Already has asmdef modularization (`Core`, `Combat`, `Editor`, test asmdef).
- Has regression guard tooling and stability checks.
- Has repeated runtime failures mainly from:
  - scene/startup variance,
  - fragile manager/camera recovery paths,
  - regressions from API/assembly drift.
- Has PlayMode tests but no visible CI workflow in repository yet.

## Stability-first revisions for KyndeBlade

### Phase 1 (immediate hardening)

1. **Enforce bootstrap play path**
   - Added editor guard to require Play Mode from `Assets/Scenes/Main.unity`.
   - Prevents accidental play from incomplete scenes.

2. **Keep regression guard focused**
   - Block only proven high-risk patterns, avoid false positives.

3. **Camera contract**
   - Single authoritative camera recovery path in `KyndeBladeGameManager`.
   - Disable optional render redirection under SRP.

4. **Spawn safety**
   - Null-safe character spawn fallback and startup spawn recovery.

### Phase 2 (structural cleanup)

1. **Split composition roots**
   - Keep `KyndeBladeGameManager` as app root.
   - Move combat/map bootstrap into dedicated root components with explicit order.

2. **Service access policy**
   - Resolve refs once in Start/Awake.
   - Disallow `FindFirstObjectByType` in Update paths.

3. **Event channel consolidation**
   - Standardize cross-system notifications (combat end, scene transition, rewards) via channels.

4. **Scene contracts**
   - Define required objects/components per scene.
   - Add editor validator that reports missing contract elements before play.

### Phase 3 (verification and regression prevention)

1. **CI pipeline**
   - Add Unity compile + PlayMode test workflow.
   - Gate merges on compile success and core test pass.

2. **Smoke tests**
   - Camera renders in Main.
   - Characters spawn in startup mode.
   - Action buttons display in one row.
   - Victory Continue returns to map progression.

3. **Architecture audits**
   - Scheduled checks for forbidden patterns:
     - stale API fields,
     - Core->Combat compile coupling,
     - unsafe scene loading paths.

## Definition of stable baseline

KyndeBlade is considered stable when:

- Play Mode opens from Main without manual scene surgery.
- At least one camera always renders Display 1.
- Player/enemy characters always spawn in default startup flow.
- Combat UI action buttons lay out correctly and remain actionable.
- Known regression guards produce zero blocking findings on normal play.


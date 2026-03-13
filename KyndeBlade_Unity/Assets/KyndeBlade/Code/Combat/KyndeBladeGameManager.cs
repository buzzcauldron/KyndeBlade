using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Main game bootstrap. Wires systems per Hodent + Beginner guide. Runs first so all pipeline objects exist before other Start()s. The main play scene must contain one KyndeBladeGameManager and be the first scene in Build Settings.</summary>
    [DefaultExecutionOrder(-1000)]
    public class KyndeBladeGameManager : MonoBehaviour
    {
        [Header("References")]
        public TurnManager TurnManager;
        public GameSettings Settings;
        public MedievalCharacter KnightPrefab;
        public MedievalCharacter MagePrefab;
        [Tooltip("Wille the Dreamer - always player. Fixed Dreamer class.")]
        public MedievalCharacter WillePrefab;
        public MedievalCharacter FalsePrefab;
        public MedievalCharacter LadyMedePrefab;
        public MedievalCharacter WrathPrefab;
        public MedievalCharacter HungerPrefab;
        public MedievalCharacter PridePrefab;
        public MedievalCharacter GreenKnightPrefab;
        [Tooltip("Elde - Old Age. When Elde hits a character, they age.")]
        public MedievalCharacter EldePrefab;
        public MedievalCharacter EnvyPrefab;
        public MedievalCharacter SlothPrefab;
        public MedievalCharacter LustPrefab;
        [Tooltip("Piers the Plowman - joins at Passus IV-V (piers location). Honest work.")]
        public MedievalCharacter PiersPrefab;
        [Tooltip("Conscience - joins at Passus XVII-XVIII (quest_do_wel or later). Moral awareness.")]
        public MedievalCharacter ConsciencePrefab;

        [Header("Auto Setup")]
        public bool AutoSpawnTestCharacters = true;
        [Tooltip("Use Expedition 33 moveset (melee/ranged/skills) instead of basic defaults.")]
        public bool UseExpedition33Moveset = true;
        [Tooltip("Start with map/level select instead of auto-spawning combat. Phase 1: hub mode.")]
        public bool StartWithMap = true;
        [Tooltip("Start directly in a fixed-wave sandbox encounter for balancing.")]
        public bool StartWithSandboxEncounter = false;
        [Tooltip("If true, runs two fixed waves in sequence for repeatable tuning.")]
        public bool SandboxTwoWaveSequence = true;

        [Header("Encounter Director Guardrails")]
        [Tooltip("Maximum number of non-boss enemies spawned from EncounterConfig entries.")]
        public int EncounterSpawnBudget = 4;
        [Tooltip("Minimum lane cooldown in spawn order. Prevents repeated same-lane pressure.")]
        public int SpawnLaneCooldownSlots = 1;
        [Tooltip("Minimum vertical spacing between enemy spawns.")]
        public float MinEnemyVerticalSpacing = 1.0f;

        /// <summary>Built-in shader that draws a single color (works with SpriteRenderer). Tries Unity 6 and legacy names.</summary>
        static Shader GetFallbackColorShader()
        {
            var names = new[] { "Unlit/Color", "Unlit/SingleColor", "Sprites/Default", "Legacy Shaders/VertexLit" };
            foreach (var name in names)
            {
                var s = Shader.Find(name);
                if (s != null) return s;
            }
            return null;
        }

        List<MedievalCharacter> _playerChars = new List<MedievalCharacter>();
        List<MedievalCharacter> _enemyChars = new List<MedievalCharacter>();
        EncounterConfig _lastEncounterConfig;
        LocationNode _lastEncounterLocation;
        bool _isSinMinibossEncounter;
        bool _lastEncounterHadGreenKnight;
        string _pendingEncounterMetricsId = "manual_test";
        int _sandboxWaveIndex;
        struct GuardedEnemySpawn
        {
            public EncounterConfig.EnemySpawnEntry Entry;
            public Vector3 Position;
        }

        /// <summary>Creates TurnManager and all pipeline objects so they exist before any other script's Start() runs.</summary>
        void Awake()
        {
            if (TurnManager == null)
                TurnManager = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (TurnManager == null)
            {
                var go = new GameObject("TurnManager");
                TurnManager = go.AddComponent<TurnManager>();
            }
            EnsureCombatPipeline();
            EnsureMapPipeline();
            RegisterGameRuntime();
            if (TurnManager != null)
                TurnManager.OnCombatEnded += OnCombatEnded;
        }

        void RegisterGameRuntime()
        {
            GameRuntime.TurnManager = TurnManager;
            GameRuntime.GameManager = this;
            GameRuntime.SaveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            GameRuntime.CombatUI = UnityEngine.Object.FindFirstObjectByType<CombatUI>();
            GameRuntime.GameStateManager = UnityEngine.Object.FindFirstObjectByType<GameStateManager>();
            GameRuntime.WorldMapManager = UnityEngine.Object.FindFirstObjectByType<WorldMapManager>();
            GameRuntime.NarrativeManager = UnityEngine.Object.FindFirstObjectByType<NarrativeManager>();
            GameRuntime.MusicManager = UnityEngine.Object.FindFirstObjectByType<MusicManager>();
            GameRuntime.DialogueSystem = UnityEngine.Object.FindFirstObjectByType<DialogueSystem>();
        }

        void OnDestroy()
        {
            if (TurnManager != null)
                TurnManager.OnCombatEnded -= OnCombatEnded;
            GameRuntime.Clear();
        }

        /// <summary>Assigns settings, applies StartWithMap, and optionally starts delayed auto-spawn.</summary>
        void Start()
        {
            if (Settings != null && TurnManager != null) TurnManager.Settings = Settings;
            if (StartWithSandboxEncounter)
            {
                StartWithMap = false;
                AutoSpawnTestCharacters = false;
                StartCoroutine(DelayedStartSandbox());
                return;
            }

            if (StartWithMap)
            {
                var wm = UnityEngine.Object.FindFirstObjectByType<WorldMapManager>();
                if (wm != null && wm.StartLocation != null)
                {
                    wm.SetCurrentLocation(wm.StartLocation);
                    AutoSpawnTestCharacters = false;
                }
            }
            if (AutoSpawnTestCharacters)
                StartCoroutine(DelayedAutoSpawn());
        }

        /// <summary>Waits briefly then spawns test party + default enemies (False, Lady Mede, Wrath) and starts combat.</summary>
        IEnumerator DelayedAutoSpawn()
        {
            yield return new WaitForSeconds(0.5f);
            SpawnTestCharacters();
        }

        IEnumerator DelayedStartSandbox()
        {
            yield return new WaitForSeconds(0.25f);
            StartSandboxEncounter();
        }

        /// <summary>Spawns test party in poem order plus three default enemies; applies hunger scar and starts combat.</summary>
        public void SpawnTestCharacters()
        {
            _playerChars.Clear();
            _enemyChars.Clear();
            _pendingEncounterMetricsId = "test_spawn";

            SpawnPartyInPoemOrder(null);

            SpawnEnemy(FalsePrefab, new Vector3(4f, -1.5f, 0f), typeof(FalseCharacter));
            SpawnEnemy(LadyMedePrefab, new Vector3(4f, 0f, 0f), typeof(LadyMedeCharacter));
            SpawnEnemy(WrathPrefab, new Vector3(4f, 1.5f, 0f), typeof(WrathCharacter));

            if (_playerChars.Count > 0 && _enemyChars.Count > 0)
            {
                ApplyHungerScarToParty();
                StartCoroutine(DelayedStartCombat());
            }
        }

        /// <summary>Instantiates one character (or creates placeholder), applies moveset and visual; adds to party or enemy list.</summary>
        void SpawnCharacter(MedievalCharacter prefab, Vector3 pos, CharacterClass cls, string name, bool isPlayer)
        {
            MedievalCharacter c;
            if (prefab != null)
            {
                c = Instantiate(prefab, pos, Quaternion.identity);
                EnsureCharacterVisual(c.gameObject, isPlayer, prefab.CharacterName ?? name ?? "Wille");
            }
            else
            {
                var go = new GameObject(name);
                go.transform.position = pos;
                c = go.AddComponent<MedievalCharacter>();
                EnsureCharacterVisual(go, isPlayer, name ?? "Wille");
            }
            c.CharacterClassType = cls;
            c.CharacterName = name;
            if (UseExpedition33Moveset)
                Expedition33Moveset.ApplyToCharacter(c, false);
            else
                DefaultCombatActions.AddDefaultsTo(c);
            if (isPlayer)
                ApplyPlayerLoopTuning(c);
            if (isPlayer) _playerChars.Add(c);
            else _enemyChars.Add(c);
        }

        /// <summary>Instantiates one enemy (prefab or type), applies moveset and AI; adds to enemy list.</summary>
        void SpawnEnemy(MedievalCharacter prefab, Vector3 pos, System.Type enemyType)
        {
            MedievalCharacter c;
            if (prefab != null)
            {
                c = Instantiate(prefab, pos, Quaternion.identity);
                EnsureCharacterVisual(c.gameObject, false, prefab.CharacterName ?? prefab.name);
            }
            else
            {
                var go = new GameObject(enemyType.Name);
                go.transform.position = pos;
                c = (MedievalCharacter)go.AddComponent(enemyType);
                EnsureCharacterVisual(go, false, enemyType.Name);
            }
            if (UseExpedition33Moveset)
                Expedition33Moveset.ApplyToCharacter(c, true);
            else
                DefaultCombatActions.AddDefaultsTo(c);
            if (c.GetComponent<SimpleEnemyAI>() == null && c.GetComponent<AdaptiveEnemyAI>() == null)
                c.gameObject.AddComponent(UseExpedition33Moveset ? typeof(AdaptiveEnemyAI) : typeof(SimpleEnemyAI));
            ApplyEnemyDifficultyTuning(c);
            ConfigureEnemyAggression(c);
            _enemyChars.Add(c);
        }

        /// <summary>Short delay then starts turn-based combat with current party vs enemies.</summary>
        IEnumerator DelayedStartCombat()
        {
            yield return new WaitForSeconds(1f);
            StartCombatSequence(_playerChars, _enemyChars);
        }

        /// <summary>Initializes TurnManager with given party and enemies and starts combat.</summary>
        public void StartCombatSequence(List<MedievalCharacter> players, List<MedievalCharacter> enemies)
        {
            var metrics = UnityEngine.Object.FindFirstObjectByType<CombatMetricsManager>();
            if (metrics != null)
            {
                var encounterId = string.IsNullOrWhiteSpace(_pendingEncounterMetricsId)
                    ? "manual_test"
                    : _pendingEncounterMetricsId;
                metrics.BeginEncounter(encounterId, players, enemies);
            }

            if (TurnManager != null)
            {
                TurnManager.InitializeCombat(players, enemies);
                TurnManager.StartCombat();
            }

            var save = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            if (save != null)
                save.SaveCombatSnapshot(_pendingEncounterMetricsId, _sandboxWaveIndex, enemies != null ? enemies.Count : 0, 0f);
        }

        [Header("Random Green Knight")]
        [Tooltip("Chance (0-1) per building encounter for Green Knight first appearance (starts the cycle).")]
        public float GreenKnightFirstAppearChanceInBuilding = 0.15f;
        [Tooltip("Chance (0-1) per encounter for Green Knight to appear when cycle started (wrong dialogue at Green Chapel or first appearance in building).")]
        public float GreenKnightRandomAppearChance = 0.25f;

        /// <summary>Start combat from an EncounterConfig (map/level select).</summary>
        public void StartEncounterFromConfig(EncounterConfig config, LocationNode location = null)
        {
            if (config == null) return;
            _isSinMinibossEncounter = false;
            _lastEncounterConfig = config;
            _lastEncounterLocation = location;
            _pendingEncounterMetricsId = location != null && !string.IsNullOrWhiteSpace(location.LocationId)
                ? location.LocationId
                : (config != null ? config.name : "encounter");
            _playerChars.Clear();
            _enemyChars.Clear();

            SpawnPartyInPoemOrder(location);

            var offset = config.EnemyFormationOffset;
            var spacing = config.EnemySpacing;
            var enemies = config.Enemies ?? new List<EncounterConfig.EnemySpawnEntry>();
            var guardedSpawns = BuildGuardedEnemySpawns(config, enemies, offset, spacing);
            for (int i = 0; i < guardedSpawns.Count; i++)
            {
                var e = guardedSpawns[i].Entry;
                var pos = guardedSpawns[i].Position;
                var prefab = e.Prefab ?? GetEnemyPrefabByType(e.CharacterTypeName);
                if (prefab != null)
                    SpawnEnemyFromPrefab(prefab, pos);
            }
            var bossPrefab = config.BossPrefab ?? GetEnemyPrefabByType(config.BossCharacterType);
            if (bossPrefab != null)
                SpawnEnemyFromPrefab(bossPrefab, config.BossPosition);

            var saveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            bool atGreenChapel = location != null && string.Equals(location.LocationId, "green_chapel", System.StringComparison.OrdinalIgnoreCase);
            _lastEncounterHadGreenKnight = false;

            // First appearance: Green Knight can randomly appear in any building to start the cycle
            if (saveManager != null && location != null && location.IsBuilding && !atGreenChapel && !saveManager.GreenKnightWillAppearRandomly && GreenKnightPrefab != null)
            {
                if (UnityEngine.Random.value < GreenKnightFirstAppearChanceInBuilding)
                {
                    var gkPos = config.BossPosition + new Vector3(1.5f, 1.5f, 0f);
                    SpawnEnemyFromPrefab(GreenKnightPrefab, gkPos);
                    _lastEncounterHadGreenKnight = true;
                    saveManager.SetGreenKnightWillAppearRandomly(true);
                    if (saveManager.CurrentProgress != null)
                        saveManager.CurrentProgress.EncountersSinceLastGreenKnight = 0;
                }
            }
            // Subsequent appearances: Green Knight in cycle (wrong Green Chapel choice or first appearance already happened)
            else if (saveManager != null && saveManager.GreenKnightWillAppearRandomly && !atGreenChapel && GreenKnightPrefab != null)
            {
                if (UnityEngine.Random.value < GreenKnightRandomAppearChance)
                {
                    var gkPos = config.BossPosition + new Vector3(1.5f, 1.5f, 0f);
                    SpawnEnemyFromPrefab(GreenKnightPrefab, gkPos);
                    _lastEncounterHadGreenKnight = true;
                    if (saveManager.CurrentProgress != null)
                        saveManager.CurrentProgress.EncountersSinceLastGreenKnight = 0;
                }
            }
            if (saveManager?.CurrentProgress != null && !_lastEncounterHadGreenKnight)
                saveManager.CurrentProgress.EncountersSinceLastGreenKnight++;

            var hazardMgr = UnityEngine.Object.FindFirstObjectByType<CombatHazardManager>();
            if (hazardMgr == null)
                hazardMgr = new GameObject("CombatHazardManager").AddComponent<CombatHazardManager>();
            var hazards = config.Hazards != null && config.Hazards.Count > 0
                ? config.Hazards
                : (location?.CombatHazards);
            if (hazardMgr != null && hazards != null && hazards.Count > 0)
                hazardMgr.SetHazards(hazards);
            else if (hazardMgr != null)
                hazardMgr.SetHazards(null);

            if (_playerChars.Count > 0 && _enemyChars.Count > 0)
            {
                ApplyHungerScarToParty();
                ApplyAgeFromSave();
                EnsureFaeAppearanceManager();
                StartCoroutine(DelayedStartCombat());
            }
        }

        List<GuardedEnemySpawn> BuildGuardedEnemySpawns(
            EncounterConfig config,
            List<EncounterConfig.EnemySpawnEntry> enemies,
            Vector3 offset,
            float spacing)
        {
            var result = new List<GuardedEnemySpawn>();
            if (enemies == null || enemies.Count == 0)
                return result;

            int spawnBudget = Mathf.Max(1, EncounterSpawnBudget);
            int count = Mathf.Min(spawnBudget, enemies.Count);
            int laneCooldown = Mathf.Max(0, SpawnLaneCooldownSlots);
            float laneSpacing = spacing > 0.01f ? spacing : 1f;
            float minVerticalSpacing = Mathf.Max(0.1f, MinEnemyVerticalSpacing);

            var lastSpawnIndexByLane = new Dictionary<int, int>();
            var usedPositions = new List<Vector3>();

            for (int i = 0; i < count; i++)
            {
                var entry = enemies[i];
                var basePos = entry.Position != Vector3.zero
                    ? entry.Position
                    : offset + new Vector3(0f, (i - count * 0.5f) * laneSpacing, 0f);

                int desiredLane = Mathf.RoundToInt((basePos.y - offset.y) / laneSpacing);
                int selectedLane = PickAvailableLane(desiredLane, i, laneCooldown, lastSpawnIndexByLane);
                var guardedPos = basePos;
                guardedPos.y = offset.y + selectedLane * laneSpacing;
                guardedPos = ResolveVerticalOverlap(guardedPos, usedPositions, minVerticalSpacing);

                lastSpawnIndexByLane[selectedLane] = i;
                usedPositions.Add(guardedPos);
                result.Add(new GuardedEnemySpawn { Entry = entry, Position = guardedPos });
            }

            if (enemies.Count > spawnBudget)
            {
                Debug.Log(
                    $"[KyndeBlade] Encounter '{config.name}' trimmed from {enemies.Count} to {spawnBudget} enemies by spawn budget.",
                    this);
            }

            return result;
        }

        static int PickAvailableLane(int desiredLane, int spawnIndex, int laneCooldown, Dictionary<int, int> lastSpawnIndexByLane)
        {
            if (laneCooldown <= 0)
                return desiredLane;

            if (!IsLaneCooling(desiredLane, spawnIndex, laneCooldown, lastSpawnIndexByLane))
                return desiredLane;

            for (int d = 1; d <= 6; d++)
            {
                int upLane = desiredLane + d;
                if (!IsLaneCooling(upLane, spawnIndex, laneCooldown, lastSpawnIndexByLane))
                    return upLane;

                int downLane = desiredLane - d;
                if (!IsLaneCooling(downLane, spawnIndex, laneCooldown, lastSpawnIndexByLane))
                    return downLane;
            }

            return desiredLane;
        }

        static bool IsLaneCooling(int lane, int spawnIndex, int laneCooldown, Dictionary<int, int> lastSpawnIndexByLane)
        {
            if (!lastSpawnIndexByLane.TryGetValue(lane, out int lastIndex))
                return false;
            return spawnIndex - lastIndex <= laneCooldown;
        }

        static Vector3 ResolveVerticalOverlap(Vector3 candidate, List<Vector3> usedPositions, float minVerticalSpacing)
        {
            if (usedPositions == null || usedPositions.Count == 0)
                return candidate;

            var resolved = candidate;
            for (int pass = 0; pass < 10; pass++)
            {
                bool moved = false;
                for (int i = 0; i < usedPositions.Count; i++)
                {
                    if (Mathf.Abs(resolved.y - usedPositions[i].y) < minVerticalSpacing)
                    {
                        resolved.y += minVerticalSpacing;
                        moved = true;
                    }
                }
                if (!moved)
                    break;
            }
            return resolved;
        }

        void ApplyAgeFromSave()
        {
            var save = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            var aging = UnityEngine.Object.FindFirstObjectByType<AgingManager>();
            if (save?.CurrentProgress == null || aging == null) return;
            int eldeHits = save.CurrentProgress.EldeHitsAccrued;
            if (eldeHits <= 0) return;
            foreach (var c in _playerChars)
                if (c != null) aging.ApplyAgeToCharacter(c, eldeHits);
        }

        /// <summary>Spawn party in poem order: Wille (always), Piers (after piers), Conscience (after quest_do_wel).</summary>
        void SpawnPartyInPoemOrder(LocationNode location)
        {
            var saveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            bool hasPiers = saveManager != null && saveManager.HasVisited("piers");
            bool hasConscience = saveManager != null && (
                saveManager.HasVisited("quest_do_wel") ||
                saveManager.HasVisited("years_pass") ||
                saveManager.HasVisited("field_of_grace"));

            var willePrefab = WillePrefab != null ? WillePrefab : KnightPrefab;
            SpawnCharacter(willePrefab, new Vector3(-4f, 0f, 0f), CharacterClass.Dreamer, "Wille", true);

            if (hasPiers)
            {
                var piersPrefab = PiersPrefab != null ? PiersPrefab : KnightPrefab;
                SpawnCharacter(piersPrefab, new Vector3(-3.5f, -1f, 0f), CharacterClass.Knight, "Piers", true);
            }
            if (hasConscience)
            {
                var consciencePrefab = ConsciencePrefab != null ? ConsciencePrefab : MagePrefab;
                SpawnCharacter(consciencePrefab, new Vector3(-3.5f, 1f, 0f), CharacterClass.Mage, "Conscience", true);
            }

            if (saveManager?.CurrentProgress != null)
            {
                foreach (var c in _playerChars)
                {
                    if (c == null) continue;
                    var entry = saveManager.CurrentProgress.GetOrCreateCharacterProgress(c.CharacterName);
                    BlessingSystem.RestoreFromSave(c, entry);
                    c.Stats.FirstHitUsed = false;
                }
            }
        }

        /// <summary>Applies aging stat modifiers to party and re-applies hunger scar.</summary>
        void ApplyAgeToParty()
        {
            var aging = UnityEngine.Object.FindFirstObjectByType<AgingManager>();
            if (aging != null && _playerChars != null && _playerChars.Count > 0)
                aging.ApplyAgeToParty(_playerChars);
            ApplyHungerScarToParty();
        }

        /// <summary>If the run has ever had hunger, applies HungerScar status to all party members.</summary>
        void ApplyHungerScarToParty()
        {
            var save = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            if (save?.CurrentProgress?.HasEverHadHunger != true || _playerChars == null) return;
            foreach (var c in _playerChars)
            {
                if (c == null) continue;
                c.RemoveStatusEffect(StatusEffectType.HungerScar);
                c.ApplyStatusEffect(StatusEffect.CreateHungerScarEffect());
            }
        }

        /// <summary>Creates FaeAppearanceManager if missing (fairy transformation visuals).</summary>
        void EnsureFaeAppearanceManager()
        {
            var fae = UnityEngine.Object.FindFirstObjectByType<FaeAppearanceManager>();
            if (fae == null)
            {
                var go = new GameObject("FaeAppearanceManager");
                go.AddComponent<FaeAppearanceManager>();
            }
        }

        /// <summary>Restart last encounter at checkpoint (Limbo-style).</summary>
        public void RestartAtCheckpoint()
        {
            if (_lastEncounterConfig != null)
                StartEncounterFromConfig(_lastEncounterConfig, _lastEncounterLocation);
        }

        public bool CanRestartAtCheckpoint => _lastEncounterConfig != null && !_isSinMinibossEncounter;
        public bool IsSinMinibossEncounter => _isSinMinibossEncounter;
        public LocationNode LastEncounterLocation => _lastEncounterLocation;
        public EncounterConfig LastEncounterConfig => _lastEncounterConfig;

        /// <summary>Start sin miniboss fight from dialogue choice. Defeat = Orfeo Otherworld.</summary>
        public void StartSinMinibossEncounter(SinType sin, LocationNode location = null)
        {
            _isSinMinibossEncounter = true;
            _lastEncounterConfig = null;
            _lastEncounterLocation = location;
            _lastEncounterHadGreenKnight = false;
            _pendingEncounterMetricsId = "sin_" + sin.ToString().ToLowerInvariant();
            _playerChars.Clear();
            _enemyChars.Clear();

            SpawnPartyInPoemOrder(location);

            var prefab = GetEnemyPrefabBySin(sin);
            if (prefab != null)
                SpawnEnemyFromPrefab(prefab, new Vector3(4f, 0f, 0f));

            var hazardMgr = UnityEngine.Object.FindFirstObjectByType<CombatHazardManager>();
            if (hazardMgr != null && location?.CombatHazards != null && location.CombatHazards.Count > 0)
                hazardMgr.SetHazards(location.CombatHazards);

            if (_playerChars.Count > 0 && _enemyChars.Count > 0)
            {
                ApplyHungerScarToParty();
                EnsureFaeAppearanceManager();
                StartCoroutine(DelayedStartCombat());
            }
        }

        /// <summary>Returns the prefab used for a sin miniboss (e.g. Pride, Hunger, Lady Mede).</summary>
        MedievalCharacter GetEnemyPrefabBySin(SinType sin)
        {
            switch (sin)
            {
                case SinType.Pride: return PridePrefab;
                case SinType.Envy: return EnvyPrefab ?? PridePrefab;
                case SinType.Wrath: return WrathPrefab;
                case SinType.Sloth: return SlothPrefab ?? HungerPrefab;
                case SinType.Greed: return LadyMedePrefab;
                case SinType.Gluttony: return HungerPrefab;
                case SinType.Lust: return LustPrefab ?? LadyMedePrefab;
                default: return PridePrefab;
            }
        }

        /// <summary>Resolves enemy type name (e.g. "Hunger", "GreenKnight") to assigned prefab.</summary>
        MedievalCharacter GetEnemyPrefabByType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;
            var t = typeName.ToLowerInvariant();
            if (t.Contains("false")) return FalsePrefab;
            if (t.Contains("lady") || t.Contains("mede")) return LadyMedePrefab;
            if (t.Contains("wrath")) return WrathPrefab;
            if (t.Contains("hunger")) return HungerPrefab;
            if (t.Contains("pride")) return PridePrefab;
            if (t.Contains("green") || t.Contains("knight")) return GreenKnightPrefab;
            if (t.Contains("elde")) return EldePrefab;
            if (t.Contains("envy")) return EnvyPrefab;
            if (t.Contains("sloth")) return SlothPrefab;
            if (t.Contains("lust")) return LustPrefab;
            return null;
        }

        /// <summary>Instantiates enemy from prefab, applies boss movesets/AI by type, adds to enemy list.</summary>
        void SpawnEnemyFromPrefab(MedievalCharacter prefab, Vector3 pos)
        {
            var c = Instantiate(prefab, pos, Quaternion.identity);
            EnsureCharacterVisual(c.gameObject, false, prefab != null ? prefab.CharacterName ?? prefab.name : "Enemy");
            if (c.GetComponent<HungerCharacter>() != null)
            {
                HungerMoveset.ApplyToCharacter(c);
                if (c.GetComponent<HungerBossAI>() == null)
                    c.gameObject.AddComponent<HungerBossAI>();
            }
            else if (c.GetComponent<PrideCharacter>() != null)
            {
                PrideMoveset.ApplyToCharacter(c);
                if (c.GetComponent<PrideBossAI>() == null)
                    c.gameObject.AddComponent<PrideBossAI>();
            }
            else if (c.GetComponent<GreenKnightCharacter>() != null)
            {
                GreenKnightMoveset.ApplyToCharacter(c);
                if (c.GetComponent<GreenKnightBossAI>() == null)
                    c.gameObject.AddComponent<GreenKnightBossAI>();
            }
            else if (c.GetComponent<WrathCharacter>() != null)
            {
                WrathMoveset.ApplyToCharacter(c);
                if (c.GetComponent<WrathBossAI>() == null)
                    c.gameObject.AddComponent<WrathBossAI>();
            }
            else if (c.GetComponent<FalseCharacter>() != null)
            {
                FalseMoveset.ApplyToCharacter(c);
                if (c.GetComponent<FalseBossAI>() == null)
                    c.gameObject.AddComponent<FalseBossAI>();
            }
            else if (c.GetComponent<LadyMedeCharacter>() != null)
            {
                LadyMedeMoveset.ApplyToCharacter(c);
                if (c.GetComponent<LadyMedeBossAI>() == null)
                    c.gameObject.AddComponent<LadyMedeBossAI>();
            }
            else if (c.GetComponent<EldeCharacter>() != null)
            {
                EldeMoveset.ApplyToCharacter(c);
                if (c.GetComponent<EldeBossAI>() == null)
                    c.gameObject.AddComponent<EldeBossAI>();
            }
            else if (c.GetComponent<EnvyCharacter>() != null)
            {
                EnvyMoveset.ApplyToCharacter(c);
                if (c.GetComponent<EnvyBossAI>() == null)
                    c.gameObject.AddComponent<EnvyBossAI>();
            }
            else if (c.GetComponent<SlothCharacter>() != null)
            {
                SlothMoveset.ApplyToCharacter(c);
                if (c.GetComponent<SlothBossAI>() == null)
                    c.gameObject.AddComponent<SlothBossAI>();
            }
            else if (c.GetComponent<LustCharacter>() != null)
            {
                LustMoveset.ApplyToCharacter(c);
                if (c.GetComponent<LustBossAI>() == null)
                    c.gameObject.AddComponent<LustBossAI>();
            }
            else if (UseExpedition33Moveset)
                Expedition33Moveset.ApplyToCharacter(c, true);
            else
                DefaultCombatActions.AddDefaultsTo(c);
            if (c.GetComponent<HungerBossAI>() == null && c.GetComponent<PrideBossAI>() == null &&
                c.GetComponent<GreenKnightBossAI>() == null && c.GetComponent<WrathBossAI>() == null &&
                c.GetComponent<FalseBossAI>() == null && c.GetComponent<LadyMedeBossAI>() == null &&
                c.GetComponent<EldeBossAI>() == null && c.GetComponent<EnvyBossAI>() == null &&
                c.GetComponent<SlothBossAI>() == null && c.GetComponent<LustBossAI>() == null &&
                c.GetComponent<SimpleEnemyAI>() == null && c.GetComponent<AdaptiveEnemyAI>() == null)
                c.gameObject.AddComponent(UseExpedition33Moveset ? typeof(AdaptiveEnemyAI) : typeof(SimpleEnemyAI));
            ApplyEnemyDifficultyTuning(c);
            ConfigureEnemyAggression(c);
            _enemyChars.Add(c);
        }

        /// <summary>Creates SaveManager, MusicManager, Aging, Poverty, Narrative, Dialogue, and optionally WorldMap + MapLevelSelectUI.</summary>
        void EnsureMapPipeline()
        {
            if (UnityEngine.Object.FindFirstObjectByType<SaveManager>() == null)
                new GameObject("SaveManager").AddComponent<SaveManager>();
            if (UnityEngine.Object.FindFirstObjectByType<SecretCodeListener>() == null)
                new GameObject("SecretCodeListener").AddComponent<SecretCodeListener>();
            if (UnityEngine.Object.FindFirstObjectByType<VolumeManager>() == null)
                new GameObject("VolumeManager").AddComponent<VolumeManager>();
            if (UnityEngine.Object.FindFirstObjectByType<AccessibilityManager>() == null)
                new GameObject("AccessibilityManager").AddComponent<AccessibilityManager>();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (UnityEngine.Object.FindFirstObjectByType<DebugOverlay>() == null)
                new GameObject("DebugOverlay").AddComponent<DebugOverlay>();
#endif
            if (UnityEngine.Object.FindFirstObjectByType<MusicManager>() == null)
                new GameObject("MusicManager").AddComponent<MusicManager>();
            if (UnityEngine.Object.FindFirstObjectByType<AgingManager>() == null)
                new GameObject("AgingManager").AddComponent<AgingManager>();
            if (UnityEngine.Object.FindFirstObjectByType<PovertyManager>() == null)
                new GameObject("PovertyManager").AddComponent<PovertyManager>();
            if (UnityEngine.Object.FindFirstObjectByType<NarrativeManager>() == null)
            {
                var go = new GameObject("NarrativeManager");
                go.AddComponent<NarrativeManager>();
            }
            if (UnityEngine.Object.FindFirstObjectByType<DialogueSystem>() == null)
            {
                var canvas = new GameObject("DialogueCanvas");
                canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                ConfigureCanvasScaler(canvas.AddComponent<CanvasScaler>());
                canvas.AddComponent<GraphicRaycaster>();
                canvas.AddComponent<DialogueSystem>();
            }
            var nm = UnityEngine.Object.FindFirstObjectByType<NarrativeManager>();
            var ds = UnityEngine.Object.FindFirstObjectByType<DialogueSystem>();
            if (nm != null && ds != null && nm.DialogueSystem == null)
                nm.DialogueSystem = ds;
            if (StartWithMap)
            {
                if (UnityEngine.Object.FindFirstObjectByType<WorldMapManager>() == null)
                {
                    var wmGo = new GameObject("WorldMapManager");
                    wmGo.AddComponent<WorldMapManager>();
                }
                if (UnityEngine.Object.FindFirstObjectByType<MapLevelSelectUI>() == null)
                {
                    var mapCanvas = new GameObject("MapCanvas");
                    mapCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                    ConfigureCanvasScaler(mapCanvas.AddComponent<CanvasScaler>());
                    mapCanvas.AddComponent<GraphicRaycaster>();
                    mapCanvas.AddComponent<MapLevelSelectUI>();
                }
            }
        }

        /// <summary>Ensures combat camera (16-bit side-view), CombatUI, CombatFeedback, GameStateManager, IlluminationManager exist.</summary>
        void EnsureCombatPipeline()
        {
            EnsureCombatCamera();
            EnsureCombatBackground();
            if (UnityEngine.Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }
            if (UnityEngine.Object.FindFirstObjectByType<KyndeBladeInputActions>() == null)
            {
                var inputGo = new GameObject("InputActions");
                inputGo.AddComponent<KyndeBladeInputActions>();
            }
            if (UnityEngine.Object.FindFirstObjectByType<CombatUI>() == null)
            {
                var canvas = new GameObject("CombatCanvas");
                canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.AddComponent<CanvasScaler>();
                canvas.AddComponent<GraphicRaycaster>();
                canvas.AddComponent<CombatUI>();
            }
            if (UnityEngine.Object.FindFirstObjectByType<CombatFeedback>() == null)
            {
                var go = new GameObject("CombatFeedback");
                go.AddComponent<AudioSource>();
                go.AddComponent<CombatFeedback>();
            }
            if (UnityEngine.Object.FindFirstObjectByType<CombatFeedbackManager>() == null)
                new GameObject("CombatFeedbackManager").AddComponent<CombatFeedbackManager>();
            if (UnityEngine.Object.FindFirstObjectByType<BattleFeedbackManager>() == null)
                new GameObject("BattleFeedbackManager").AddComponent<BattleFeedbackManager>();
            if (UnityEngine.Object.FindFirstObjectByType<CombatMetricsManager>() == null)
                new GameObject("CombatMetricsManager").AddComponent<CombatMetricsManager>();
            if (UnityEngine.Object.FindFirstObjectByType<CombatPerformanceBudgetMonitor>() == null)
                new GameObject("CombatPerformanceBudgetMonitor").AddComponent<CombatPerformanceBudgetMonitor>();
            if (UnityEngine.Object.FindFirstObjectByType<GameStateManager>() == null)
                new GameObject("GameStateManager").AddComponent<GameStateManager>();
            if (UnityEngine.Object.FindFirstObjectByType<IlluminationManager>() == null)
                new GameObject("IlluminationManager").AddComponent<IlluminationManager>();
            if (UnityEngine.Object.FindFirstObjectByType<ScreenEffects>() == null)
                new GameObject("ScreenEffects").AddComponent<ScreenEffects>();
        }

        [Header("Visual Config")]
        [Tooltip("Assign a CharacterVisualConfig asset to override placeholder sprites with real art.")]
        public CharacterVisualConfig VisualConfig;

        /// <summary>Adds or configures SpriteRenderer, SimpleAnimator, and PiersAppearanceData for a character.</summary>
        void EnsureCharacterVisual(GameObject go, bool isPlayer, string characterKey = null)
        {
            var key = characterKey ?? go.name;
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = go.AddComponent<SpriteRenderer>();
                sr.drawMode = SpriteDrawMode.Simple;
                var shader = GetFallbackColorShader();
                if (shader != null)
                    sr.material = new Material(shader);
            }

            var configEntry = VisualConfig != null ? VisualConfig.Find(key) : null;
            if (configEntry != null && configEntry.Sprite != null)
                sr.sprite = configEntry.Sprite;
            else
                sr.sprite = PlaceholderSpriteFactory.GetSpriteForCharacter(key, isPlayer);

            var saveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            int seed = saveManager?.CurrentProgress?.RunAppearanceSeed ?? UnityEngine.Random.Range(1, int.MaxValue);
            bool hasHungerScar = saveManager?.CurrentProgress?.HasEverHadHunger ?? false;

            Color color;
            if (configEntry != null && configEntry.ColorOverride.a > 0f)
                color = configEntry.ColorOverride;
            else
                color = PlaceholderSpriteFactory.GetColorForCharacter(key, isPlayer);

            if (hasHungerScar)
            {
                Color.RGBToHSV(color, out float h, out float s, out float v);
                color = Color.HSVToRGB(h, s * 0.6f, v * 0.85f);
            }
            sr.color = color;

            Vector3 scale = (configEntry != null && configEntry.ScaleOverride != Vector3.zero)
                ? configEntry.ScaleOverride
                : PlaceholderSpriteFactory.GetScaleForCharacter(key);
            go.transform.localScale = scale;

            if (go.GetComponent<SimpleAnimator>() == null)
            {
                var anim = go.AddComponent<SimpleAnimator>();
                if (configEntry != null)
                {
                    anim.IdleBobSpeed *= configEntry.AnimationSpeed;
                    anim.AttackDuration /= Mathf.Max(0.1f, configEntry.AnimationSpeed);
                }
            }

            var pad = go.GetComponent<PiersAppearanceData>();
            if (pad == null) pad = go.AddComponent<PiersAppearanceData>();
            pad.Seed = seed;
            pad.CharacterKey = key;
            pad.IsPlayer = isPlayer;
            pad.HasHungerScar = hasHungerScar;
        }

        /// <summary>Creates a single full-screen parchment-colored quad behind combat.</summary>
        void EnsureCombatBackground()
        {
            if (GameObject.Find("CombatBackground") != null) return;
            var go = new GameObject("CombatBackground");
            go.transform.position = new Vector3(0f, 0f, 5f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = UnityEngine.Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
            sr.color = new Color(ManuscriptUITheme.ParchmentAged.r, ManuscriptUITheme.ParchmentAged.g, ManuscriptUITheme.ParchmentAged.b, 0.9f);
            var shader = GetFallbackColorShader();
            if (shader != null)
                sr.material = new Material(shader);
            sr.sortingOrder = -100;
            var cam = Camera.main;
            if (cam != null && cam.orthographic)
            {
                float h = cam.orthographicSize * 2f;
                float w = h * cam.aspect;
                go.transform.localScale = new Vector3(w, h, 1f);
            }
            else
            {
                go.transform.localScale = new Vector3(30f, 20f, 1f);
            }
        }

        /// <summary>Finds or creates main camera; sets orthographic, size, background for 16-bit side-view.</summary>
        void EnsureCombatCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var existing = UnityEngine.Object.FindFirstObjectByType<Camera>();
                if (existing != null)
                {
                    existing.gameObject.tag = "MainCamera";
                    if (existing.GetComponent<AudioListener>() == null)
                        existing.gameObject.AddComponent<AudioListener>();
                    cam = existing;
                }
                else
                {
                    var go = new GameObject("Main Camera");
                    go.tag = "MainCamera";
                    cam = go.AddComponent<Camera>();
                    go.AddComponent<AudioListener>();
                }
            }
            cam.gameObject.SetActive(true);
            cam.enabled = true;
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.transform.rotation = Quaternion.identity;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.1f, 0.14f);
            cam.depth = 100;

            var manuscriptOverlay = cam.GetComponent<ManuscriptOverlayEffect>();
            if (manuscriptOverlay != null)
                manuscriptOverlay.enabled = false;

            var pipeline = cam.GetComponent<SixteenBitPipeline>();
            if (pipeline == null)
            {
                pipeline = cam.gameObject.AddComponent<SixteenBitPipeline>();
                pipeline.ApplyManuscriptOverlay = false;
            }
        }

        /// <summary>Sets UI scale mode and reference resolution for manuscript-style UI.</summary>
        static void ConfigureCanvasScaler(CanvasScaler scaler)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
        }

        void OnCombatEnded()
        {
            var save = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            if (save != null)
                save.ClearCombatSnapshot();

            if (!StartWithSandboxEncounter)
                return;

            if (!SandboxTwoWaveSequence)
                return;

            if (_sandboxWaveIndex == 0)
            {
                _sandboxWaveIndex = 1;
                StartCoroutine(StartNextSandboxWaveAfterDelay());
            }
            else
            {
                _sandboxWaveIndex = 0;
            }
        }

        IEnumerator StartNextSandboxWaveAfterDelay()
        {
            yield return new WaitForSeconds(0.75f);
            StartSandboxEncounter();
        }

        public void StartSandboxEncounter()
        {
            _playerChars.Clear();
            _enemyChars.Clear();

            _pendingEncounterMetricsId = _sandboxWaveIndex == 0 ? "sandbox_wave_1" : "sandbox_wave_2";
            SpawnPartyInPoemOrder(null);

            if (_sandboxWaveIndex == 0)
            {
                SpawnEnemy(null, new Vector3(4f, -0.8f, 0f), typeof(MeleePressureEnemy));
                SpawnEnemy(null, new Vector3(4f, 0.9f, 0f), typeof(RangedPressureEnemy));
            }
            else
            {
                SpawnEnemy(null, new Vector3(4f, -1.4f, 0f), typeof(MeleePressureEnemy));
                SpawnEnemy(null, new Vector3(4f, 0f, 0f), typeof(RangedPressureEnemy));
                SpawnEnemy(null, new Vector3(4f, 1.4f, 0f), typeof(MeleePressureEnemy));
            }

            if (_playerChars.Count > 0 && _enemyChars.Count > 0)
                StartCoroutine(DelayedStartCombat());
        }

        void ApplyPlayerLoopTuning(MedievalCharacter character)
        {
            if (Settings == null || character == null || character.AvailableActions == null) return;
            foreach (var action in character.AvailableActions)
            {
                if (action == null || action.ActionData == null) continue;
                var t = action.ActionData.ActionType;
                if (t == CombatActionType.Strike || t == CombatActionType.RangedStrike || t == CombatActionType.Counter)
                    action.ActionData.Damage *= Settings.PlayerDamageMultiplier;
                if (action.ActionData.StaminaCost > 0f)
                    action.ActionData.StaminaCost *= Settings.PlayerStaminaCostMultiplier;
                if (action.ActionData.CastTime > 0f)
                    action.ActionData.CastTime *= Settings.PlayerActionTimingMultiplier;
                if (action.ActionData.ExecutionTime > 0f)
                    action.ActionData.ExecutionTime *= Settings.PlayerActionTimingMultiplier;
            }
        }

        void ApplyEnemyDifficultyTuning(MedievalCharacter enemy)
        {
            if (Settings == null || enemy == null) return;
            float healthMult = Settings.GetEnemyHealthMultiplier();
            enemy.Stats.MaxHealth *= healthMult;
            enemy.Stats.CurrentHealth *= healthMult;
        }

        void ConfigureEnemyAggression(MedievalCharacter enemy)
        {
            if (Settings == null || enemy == null) return;
            float aggroMult = Settings.GetEnemyAggressionMultiplier();
            var simple = enemy.GetComponent<SimpleEnemyAI>();
            if (simple != null) simple.DecisionDelay *= aggroMult;
            var adaptive = enemy.GetComponent<AdaptiveEnemyAI>();
            if (adaptive != null) adaptive.DecisionDelay *= aggroMult;
        }
    }
}

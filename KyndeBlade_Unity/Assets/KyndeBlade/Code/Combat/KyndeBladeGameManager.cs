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
            GameRuntime.Clear();
        }

        /// <summary>Assigns settings, applies StartWithMap, and optionally starts delayed auto-spawn.</summary>
        void Start()
        {
            if (Settings != null && TurnManager != null) TurnManager.Settings = Settings;

            if (StartWithMap)
            {
                // Let WorldMapManager.TryLazyInit set CurrentLocation (DefaultStartLocationId, save, or DemoTestHelper override)
                AutoSpawnTestCharacters = false;
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

        /// <summary>Spawns test party in poem order plus three default enemies; applies hunger scar and starts combat.</summary>
        public void SpawnTestCharacters()
        {
            _playerChars.Clear();
            _enemyChars.Clear();

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
            if (TurnManager != null)
            {
                TurnManager.InitializeCombat(players, enemies);
                TurnManager.StartCombat();
            }
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
            _playerChars.Clear();
            _enemyChars.Clear();

            SpawnPartyInPoemOrder(location);

            var offset = config.EnemyFormationOffset;
            var spacing = config.EnemySpacing;
            var enemies = config.Enemies ?? new List<EncounterConfig.EnemySpawnEntry>();
            for (int i = 0; i < enemies.Count; i++)
            {
                var e = enemies[i];
                var pos = e.Position != Vector3.zero ? e.Position : offset + new Vector3(0f, (i - enemies.Count * 0.5f) * spacing, 0f);
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
            var hazards = (location != null && location.SuppressCombatHazards) ? null
                : (config.Hazards != null && config.Hazards.Count > 0 ? config.Hazards : location?.CombatHazards);
            if (hazardMgr != null && hazards != null && hazards.Count > 0)
                hazardMgr.SetHazards(hazards);
            else if (hazardMgr != null)
                hazardMgr.SetHazards(null);

            if (_playerChars.Count > 0 && _enemyChars.Count > 0)
            {
                ApplyHungerScarToParty();
                EnsureFaeAppearanceManager();
                StartCoroutine(DelayedStartCombat());
            }
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
            _playerChars.Clear();
            _enemyChars.Clear();

            SpawnPartyInPoemOrder(location);

            var prefab = GetEnemyPrefabBySin(sin);
            if (prefab != null)
                SpawnEnemyFromPrefab(prefab, new Vector3(4f, 0f, 0f));

            var hazardMgr = UnityEngine.Object.FindFirstObjectByType<CombatHazardManager>();
            if (hazardMgr != null && location != null && !location.SuppressCombatHazards && location.CombatHazards != null && location.CombatHazards.Count > 0)
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
                case SinType.Envy: return PridePrefab; // placeholder
                case SinType.Wrath: return WrathPrefab;
                case SinType.Sloth: return HungerPrefab; // placeholder
                case SinType.Greed: return LadyMedePrefab;
                case SinType.Gluttony: return HungerPrefab;
                case SinType.Lust: return LadyMedePrefab; // placeholder
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
            else if (c.GetComponent<EldeCharacter>() != null)
            {
                EldeMoveset.ApplyToCharacter(c);
                if (c.GetComponent<SimpleEnemyAI>() == null && c.GetComponent<AdaptiveEnemyAI>() == null)
                    c.gameObject.AddComponent(UseExpedition33Moveset ? typeof(AdaptiveEnemyAI) : typeof(SimpleEnemyAI));
            }
            else if (UseExpedition33Moveset)
                Expedition33Moveset.ApplyToCharacter(c, true);
            else
                DefaultCombatActions.AddDefaultsTo(c);
            if (c.GetComponent<HungerBossAI>() == null && c.GetComponent<PrideBossAI>() == null && c.GetComponent<GreenKnightBossAI>() == null && c.GetComponent<SimpleEnemyAI>() == null && c.GetComponent<AdaptiveEnemyAI>() == null)
                c.gameObject.AddComponent(UseExpedition33Moveset ? typeof(AdaptiveEnemyAI) : typeof(SimpleEnemyAI));
            _enemyChars.Add(c);
        }

        /// <summary>Creates SaveManager, MusicManager, Aging, Poverty, Narrative, Dialogue, and optionally WorldMap + MapLevelSelectUI.</summary>
        void EnsureMapPipeline()
        {
            if (UnityEngine.Object.FindFirstObjectByType<SaveManager>() == null)
                new GameObject("SaveManager").AddComponent<SaveManager>();
            if (UnityEngine.Object.FindFirstObjectByType<SecretCodeListener>() == null)
                new GameObject("SecretCodeListener").AddComponent<SecretCodeListener>();
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
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
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
            if (UnityEngine.Object.FindFirstObjectByType<GameStateManager>() == null)
                new GameObject("GameStateManager").AddComponent<GameStateManager>();
            if (UnityEngine.Object.FindFirstObjectByType<IlluminationManager>() == null)
                new GameObject("IlluminationManager").AddComponent<IlluminationManager>();
        }

        /// <summary>Adds or configures SpriteRenderer and PiersAppearanceData (color, scale) for a character.</summary>
        void EnsureCharacterVisual(GameObject go, bool isPlayer, string characterKey = null)
        {
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = UnityEngine.Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
                sr.drawMode = SpriteDrawMode.Simple;
                var shader = GetFallbackColorShader();
                if (shader != null)
                    sr.material = new Material(shader);
            }

            var saveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            int seed = saveManager?.CurrentProgress?.RunAppearanceSeed ?? UnityEngine.Random.Range(1, int.MaxValue);
            bool hasHungerScar = saveManager?.CurrentProgress?.HasEverHadHunger ?? false;
            var key = characterKey ?? go.name;
            Color color;
            Vector3 scale;
            PiersAppearanceRandomizer.GetAppearance(seed, key, isPlayer, out color, out scale, hasHungerScar);
            sr.color = color;
            go.transform.localScale = scale;

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
    }
}

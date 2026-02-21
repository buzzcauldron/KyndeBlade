using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Main game bootstrap. Wires systems per Hodent + Beginner guide.</summary>
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

        List<MedievalCharacter> _playerChars = new List<MedievalCharacter>();
        List<MedievalCharacter> _enemyChars = new List<MedievalCharacter>();
        EncounterConfig _lastEncounterConfig;
        LocationNode _lastEncounterLocation;
        bool _isSinMinibossEncounter;
        bool _lastEncounterHadGreenKnight;

        void Start()
        {
            if (TurnManager == null)
                TurnManager = FindObjectOfType<TurnManager>();

            if (TurnManager == null)
            {
                var go = new GameObject("TurnManager");
                TurnManager = go.AddComponent<TurnManager>();
            }

            if (Settings != null) TurnManager.Settings = Settings;

            EnsureCombatPipeline();
            EnsureMapPipeline();

            if (StartWithMap)
            {
                var wm = FindObjectOfType<WorldMapManager>();
                if (wm != null && wm.StartLocation != null)
                {
                    wm.SetCurrentLocation(wm.StartLocation);
                    AutoSpawnTestCharacters = false;
                }
            }
            if (AutoSpawnTestCharacters)
                StartCoroutine(DelayedAutoSpawn());
        }

        IEnumerator DelayedAutoSpawn()
        {
            yield return new WaitForSeconds(0.5f);
            SpawnTestCharacters();
        }

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

        IEnumerator DelayedStartCombat()
        {
            yield return new WaitForSeconds(1f);
            StartCombatSequence(_playerChars, _enemyChars);
        }

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

            var saveManager = FindObjectOfType<SaveManager>();
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

            var hazardMgr = FindObjectOfType<CombatHazardManager>();
            if (hazardMgr == null) new GameObject("CombatHazardManager").AddComponent<CombatHazardManager>();
            hazardMgr = FindObjectOfType<CombatHazardManager>();
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
                EnsureFaeAppearanceManager();
                StartCoroutine(DelayedStartCombat());
            }
        }

        /// <summary>Spawn party in poem order: Wille (always), Piers (after piers), Conscience (after quest_do_wel).</summary>
        void SpawnPartyInPoemOrder(LocationNode location)
        {
            var saveManager = FindObjectOfType<SaveManager>();
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

        void ApplyAgeToParty()
        {
            var aging = FindObjectOfType<AgingManager>();
            if (aging != null && _playerChars != null && _playerChars.Count > 0)
                aging.ApplyAgeToParty(_playerChars);
            ApplyHungerScarToParty();
        }

        void ApplyHungerScarToParty()
        {
            var save = FindObjectOfType<SaveManager>();
            if (save?.CurrentProgress?.HasEverHadHunger != true || _playerChars == null) return;
            foreach (var c in _playerChars)
            {
                if (c == null) continue;
                c.RemoveStatusEffect(StatusEffectType.HungerScar);
                c.ApplyStatusEffect(StatusEffect.CreateHungerScarEffect());
            }
        }

        void EnsureFaeAppearanceManager()
        {
            var fae = FindObjectOfType<FaeAppearanceManager>();
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
            _playerChars.Clear();
            _enemyChars.Clear();

            SpawnPartyInPoemOrder(location);

            var prefab = GetEnemyPrefabBySin(sin);
            if (prefab != null)
                SpawnEnemyFromPrefab(prefab, new Vector3(4f, 0f, 0f));

            var hazardMgr = FindObjectOfType<CombatHazardManager>();
            if (hazardMgr != null && location?.CombatHazards != null && location.CombatHazards.Count > 0)
                hazardMgr.SetHazards(location.CombatHazards);

            if (_playerChars.Count > 0 && _enemyChars.Count > 0)
            {
                ApplyHungerScarToParty();
                EnsureFaeAppearanceManager();
                StartCoroutine(DelayedStartCombat());
            }
        }

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

        void EnsureMapPipeline()
        {
            if (FindObjectOfType<SaveManager>() == null)
                new GameObject("SaveManager").AddComponent<SaveManager>();
            if (FindObjectOfType<MusicManager>() == null)
                new GameObject("MusicManager").AddComponent<MusicManager>();
            if (FindObjectOfType<AgingManager>() == null)
                new GameObject("AgingManager").AddComponent<AgingManager>();
            if (FindObjectOfType<PovertyManager>() == null)
                new GameObject("PovertyManager").AddComponent<PovertyManager>();
            if (FindObjectOfType<NarrativeManager>() == null)
            {
                var go = new GameObject("NarrativeManager");
                go.AddComponent<NarrativeManager>();
            }
            if (FindObjectOfType<DialogueSystem>() == null)
            {
                var canvas = new GameObject("DialogueCanvas");
                canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                ConfigureCanvasScaler(canvas.AddComponent<CanvasScaler>());
                canvas.AddComponent<GraphicRaycaster>();
                canvas.AddComponent<DialogueSystem>();
            }
            var nm = FindObjectOfType<NarrativeManager>();
            var ds = FindObjectOfType<DialogueSystem>();
            if (nm != null && ds != null && nm.DialogueSystem == null)
                nm.DialogueSystem = ds;
            if (StartWithMap)
            {
                if (FindObjectOfType<WorldMapManager>() == null)
                {
                    var wmGo = new GameObject("WorldMapManager");
                    wmGo.AddComponent<WorldMapManager>();
                }
                if (FindObjectOfType<MapLevelSelectUI>() == null)
                {
                    var mapCanvas = new GameObject("MapCanvas");
                    mapCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                    ConfigureCanvasScaler(mapCanvas.AddComponent<CanvasScaler>());
                    mapCanvas.AddComponent<GraphicRaycaster>();
                    mapCanvas.AddComponent<MapLevelSelectUI>();
                }
            }
        }

        /// <summary>Ensures combat camera (16-bit side-view), CombatUI, CombatFeedback, GameStateManager exist.</summary>
        void EnsureCombatPipeline()
        {
            EnsureCombatCamera();
            EnsureCombatBackground();
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            if (FindObjectOfType<CombatUI>() == null)
            {
                var canvas = new GameObject("CombatCanvas");
                canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.AddComponent<CanvasScaler>();
                canvas.AddComponent<GraphicRaycaster>();
                canvas.AddComponent<CombatUI>();
            }
            if (FindObjectOfType<CombatFeedback>() == null)
            {
                var go = new GameObject("CombatFeedback");
                go.AddComponent<AudioSource>();
                go.AddComponent<CombatFeedback>();
            }
            if (FindObjectOfType<CombatFeedbackManager>() == null)
                new GameObject("CombatFeedbackManager").AddComponent<CombatFeedbackManager>();
            if (FindObjectOfType<GameStateManager>() == null)
                new GameObject("GameStateManager").AddComponent<GameStateManager>();
            if (FindObjectOfType<IlluminationManager>() == null)
                new GameObject("IlluminationManager").AddComponent<IlluminationManager>();
        }

        void EnsureCharacterVisual(GameObject go, bool isPlayer, string characterKey = null)
        {
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = UnityEngine.Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
                sr.drawMode = SpriteDrawMode.Simple;
            }

            var saveManager = FindObjectOfType<SaveManager>();
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

        void EnsureCombatBackground()
        {
            if (GameObject.Find("CombatBackground") != null) return;
            var go = new GameObject("CombatBackground");
            go.transform.position = new Vector3(0f, 0f, 5f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = UnityEngine.Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
            sr.color = new Color(ManuscriptUITheme.ParchmentAged.r, ManuscriptUITheme.ParchmentAged.g, ManuscriptUITheme.ParchmentAged.b, 0.9f);
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

        void EnsureCombatCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var existing = UnityEngine.Object.FindObjectOfType<Camera>();
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
            cam.enabled = true;
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.transform.rotation = Quaternion.identity;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.1f, 0.14f);
            cam.depth = -1;
        }

        static void ConfigureCanvasScaler(CanvasScaler scaler)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
        }
    }
}

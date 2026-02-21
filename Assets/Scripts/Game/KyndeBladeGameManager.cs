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
        public MedievalCharacter FalsePrefab;
        public MedievalCharacter LadyMedePrefab;
        public MedievalCharacter WrathPrefab;

        [Header("Auto Setup")]
        public bool AutoSpawnTestCharacters = true;
        [Tooltip("Use Expedition 33 moveset (melee/ranged/skills) instead of basic defaults.")]
        public bool UseExpedition33Moveset = true;

        List<MedievalCharacter> _playerChars = new List<MedievalCharacter>();
        List<MedievalCharacter> _enemyChars = new List<MedievalCharacter>();

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

            if (AutoSpawnTestCharacters)
                StartCoroutine(DelayedAutoSpawn());
        }

        IEnumerator DelayedAutoSpawn()
        {
            yield return new WaitForSeconds(0.5f);
            AutoSpawnTestCharacters();
        }

        public void AutoSpawnTestCharacters()
        {
            _playerChars.Clear();
            _enemyChars.Clear();

            // 16-bit side-view: players left, enemies right (FF/DQ/Chrono Trigger style)
            SpawnCharacter(KnightPrefab, new Vector3(-4f, -1f, 0f), CharacterClass.Knight, "Player Knight", true);
            SpawnCharacter(MagePrefab, new Vector3(-4f, 1f, 0f), CharacterClass.Mage, "Player Mage", true);

            SpawnEnemy(FalsePrefab, new Vector3(4f, -1.5f, 0f), typeof(FalseCharacter));
            SpawnEnemy(LadyMedePrefab, new Vector3(4f, 0f, 0f), typeof(LadyMedeCharacter));
            SpawnEnemy(WrathPrefab, new Vector3(4f, 1.5f, 0f), typeof(WrathCharacter));

            if (_playerChars.Count > 0 && _enemyChars.Count > 0)
                StartCoroutine(DelayedStartCombat());
        }

        void SpawnCharacter(MedievalCharacter prefab, Vector3 pos, CharacterClass cls, string name, bool isPlayer)
        {
            MedievalCharacter c;
            if (prefab != null)
            {
                c = Instantiate(prefab, pos, Quaternion.identity);
                EnsureCharacterVisual(c.gameObject, isPlayer);
            }
            else
            {
                var go = new GameObject(name);
                go.transform.position = pos;
                c = go.AddComponent<MedievalCharacter>();
                EnsureCharacterVisual(go, isPlayer);
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
                EnsureCharacterVisual(c.gameObject, false);
            }
            else
            {
                var go = new GameObject(enemyType.Name);
                go.transform.position = pos;
                c = (MedievalCharacter)go.AddComponent(enemyType);
                EnsureCharacterVisual(go, false);
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

        /// <summary>Ensures combat camera (16-bit side-view), CombatUI, CombatFeedback, GameStateManager exist.</summary>
        void EnsureCombatPipeline()
        {
            EnsureCombatCamera();
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

        void EnsureCharacterVisual(GameObject go, bool isPlayer)
        {
            if (go.GetComponent<SpriteRenderer>() != null) return;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = UnityEngine.Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
            sr.color = isPlayer ? new Color(0.3f, 0.5f, 0.8f) : new Color(0.8f, 0.3f, 0.3f);
            sr.drawMode = SpriteDrawMode.Simple;
            go.transform.localScale = new Vector3(0.8f, 1.2f, 1f);
        }

        void EnsureCombatCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                go.tag = "MainCamera";
                cam = go.AddComponent<Camera>();
                go.AddComponent<AudioListener>();
            }
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.transform.rotation = Quaternion.identity;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.1f, 0.14f);
        }
    }
}

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

            // Spawn players (use prefabs or create from base)
            SpawnCharacter(KnightPrefab, new Vector3(-5f, 0f, 0f), CharacterClass.Knight, "Player Knight", true);
            SpawnCharacter(MagePrefab, new Vector3(-5f, 2f, 0f), CharacterClass.Mage, "Player Mage", true);

            // Spawn enemies
            SpawnEnemy(FalsePrefab, new Vector3(5f, -3f, 0f), typeof(FalseCharacter));
            SpawnEnemy(LadyMedePrefab, new Vector3(5f, 0f, 0f), typeof(LadyMedeCharacter));
            SpawnEnemy(WrathPrefab, new Vector3(5f, 3f, 0f), typeof(WrathCharacter));

            if (_playerChars.Count > 0 && _enemyChars.Count > 0)
                StartCoroutine(DelayedStartCombat());
        }

        void SpawnCharacter(MedievalCharacter prefab, Vector3 pos, CharacterClass cls, string name, bool isPlayer)
        {
            MedievalCharacter c;
            if (prefab != null)
                c = Instantiate(prefab, pos, Quaternion.identity);
            else
            {
                var go = new GameObject(name);
                go.transform.position = pos;
                c = go.AddComponent<MedievalCharacter>();
            }
            c.CharacterClassType = cls;
            c.CharacterName = name;
            DefaultCombatActions.AddDefaultsTo(c);
            if (isPlayer) _playerChars.Add(c);
            else _enemyChars.Add(c);
        }

        void SpawnEnemy(MedievalCharacter prefab, Vector3 pos, System.Type enemyType)
        {
            MedievalCharacter c;
            if (prefab != null)
                c = Instantiate(prefab, pos, Quaternion.identity);
            else
            {
                var go = new GameObject(enemyType.Name);
                go.transform.position = pos;
                c = (MedievalCharacter)go.AddComponent(enemyType);
            }
            DefaultCombatActions.AddDefaultsTo(c);
            if (c.GetComponent<SimpleEnemyAI>() == null)
                c.gameObject.AddComponent<SimpleEnemyAI>();
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

        /// <summary>Ensures CombatUI, CombatFeedback, GameStateManager exist (Hodent: Usability).</summary>
        void EnsureCombatPipeline()
        {
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
        }
    }
}

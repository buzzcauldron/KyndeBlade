using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KyndeBlade
{
    /// <summary>Goals, rewards, victory/defeat (Hodent: Motivation; Beginner: Goals and Rewards).</summary>
    public class GameStateManager : MonoBehaviour
    {
        [Header("References")]
        public TurnManager TurnManager;
        public KyndeBladeGameManager GameManager;
        public IlluminationManager IlluminationManager;
        [Tooltip("Assign or leave null to find on Start. Subscribes to OnDeathOfOldAgeRequested.")]
        public AgingManager AgingManager;

        [Header("UI")]
        public GameObject VictoryPanel;
        public GameObject DefeatPanel;
        public Text VictoryText;
        public Text DefeatText;

        [Header("Rewards")]
        public int BaseXPPerEnemy = 50;
        public int BonusXPForVictory = 100;

        public event Action OnVictory;
        public event Action OnDefeat;

        bool _lazyInitDone;

        void Start()
        {
            ResolveRefs();
            TryLazyInit();
        }

        void Update()
        {
            if (_lazyInitDone) return;
            ResolveRefs();
            TryLazyInit();
        }

        void ResolveRefs()
        {
            if (TurnManager == null) TurnManager = GameRuntime.TurnManager ?? UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (IlluminationManager == null) IlluminationManager = UnityEngine.Object.FindFirstObjectByType<IlluminationManager>();
            if (AgingManager == null) AgingManager = UnityEngine.Object.FindFirstObjectByType<AgingManager>();
        }

        void TryLazyInit()
        {
            if (_lazyInitDone) return;
            if (TurnManager == null) return;
            _lazyInitDone = true;
            TurnManager.OnCombatEnded += OnCombatEnded;
            if (AgingManager != null)
                AgingManager.OnDeathOfOldAgeRequested += TriggerDeathOfOldAge;
            EnsureVictoryDefeatPanels();
            if (VictoryPanel != null) VictoryPanel.SetActive(false);
            if (DefeatPanel != null) DefeatPanel.SetActive(false);
        }

        void EnsureVictoryDefeatPanels()
        {
            var combatUI = GameRuntime.CombatUI ?? UnityEngine.Object.FindFirstObjectByType<CombatUI>();
            var canvas = combatUI != null ? combatUI.GetComponent<Canvas>() : UnityEngine.Object.FindFirstObjectByType<Canvas>();
            if (canvas == null) return;

            if (VictoryPanel == null)
            {
                VictoryPanel = CreateManuscriptPanel(canvas.transform, "VictoryPanel");
                VictoryText = CreatePanelText(VictoryPanel.transform, "Victory! +0 XP", 32, true);
                CreateContinueButton(VictoryPanel.transform);
                VictoryPanel.SetActive(false);
            }
            if (DefeatPanel == null)
            {
                DefeatPanel = CreateManuscriptPanel(canvas.transform, "DefeatPanel");
                DefeatText = CreatePanelText(DefeatPanel.transform, "Defeat...", 32, emphasis: false, rubrication: true);
                CreateRestartButton(DefeatPanel.transform);
                DefeatPanel.SetActive(false);
            }
        }

        GameObject CreateManuscriptPanel(Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = new Color(ManuscriptUITheme.ParchmentLight.r, ManuscriptUITheme.ParchmentLight.g, ManuscriptUITheme.ParchmentLight.b, 0.95f);
            return go;
        }

        Text CreatePanelText(Transform parent, string content, int fontSize, bool emphasis = false, bool rubrication = false)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(400, 80);
            rect.anchoredPosition = Vector2.zero;
            var t = go.AddComponent<Text>();
            t.text = content;
            t.fontSize = fontSize;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            ManuscriptUITheme.ApplyToText(t, emphasis, rubrication);
            return t;
        }

        void CreateRestartButton(Transform parent)
        {
            var go = new GameObject("RestartButton");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.4f, 0.15f);
            rect.anchorMax = new Vector2(0.6f, 0.25f);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var btn = go.AddComponent<Button>();
            var img = go.AddComponent<Image>();
            img.color = ManuscriptUITheme.ParchmentAged;
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
            var t = textGo.AddComponent<Text>();
            t.text = "Restart";
            t.fontSize = 18;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            ManuscriptUITheme.ApplyToText(t);
            btn.onClick.AddListener(OnRestartClicked);
        }

        void CreateContinueButton(Transform parent)
        {
            var go = new GameObject("ContinueButton");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.4f, 0.15f);
            rect.anchorMax = new Vector2(0.6f, 0.25f);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var btn = go.AddComponent<Button>();
            var img = go.AddComponent<Image>();
            img.color = ManuscriptUITheme.ParchmentAged;
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
            var t = textGo.AddComponent<Text>();
            t.text = "Continue";
            t.fontSize = 18;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            ManuscriptUITheme.ApplyToText(t);
            btn.onClick.AddListener(OnVictoryContinueClicked);
        }

        bool _deathOfOldAge;

        void OnRestartClicked()
        {
            if (_deathOfOldAge)
            {
                _deathOfOldAge = false;
                StartCoroutine(RestartGameAfterDefeat());
                return;
            }
            var gm = GameManager != null ? GameManager : UnityEngine.Object.FindFirstObjectByType<KyndeBladeGameManager>();
            if (gm != null && gm.CanRestartAtCheckpoint)
            {
                if (DefeatPanel != null) DefeatPanel.SetActive(false);
                gm.RestartAtCheckpoint();
            }
        }

        /// <summary>Called when player dies of old age while waiting at Field of Grace. Restart = New Game.</summary>
        public void TriggerDeathOfOldAge()
        {
            var saveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            if (saveManager != null) saveManager.IncrementOtherworldBodiesFromDeath();
            _deathOfOldAge = true;
            if (IlluminationManager != null)
                StartCoroutine(ShowDeathOfOldAgeAfterIllumination());
            else
                ShowDeathOfOldAgePanel();
        }

        IEnumerator ShowDeathOfOldAgeAfterIllumination()
        {
            IlluminationManager.TriggerDefeatIllumination();
            yield return new WaitForSeconds(4.2f);
            ShowDeathOfOldAgePanel();
        }

        void ShowDeathOfOldAgePanel()
        {
            if (DefeatPanel != null) DefeatPanel.SetActive(true);
            if (DefeatText != null) DefeatText.text = "Death of old age. Wille's years have run their course. Grace did not come. A form of thee ends in Orfeo's Otherworld.";
            var restartBtn = DefeatPanel != null ? DefeatPanel.transform.Find("RestartButton") : null;
            if (restartBtn != null)
            {
                restartBtn.gameObject.SetActive(true);
                var t = restartBtn.GetComponentInChildren<Text>();
                if (t != null) t.text = "New Game";
            }
            OnDefeat?.Invoke();
        }

        void OnVictoryContinueClicked()
        {
            if (VictoryPanel != null) VictoryPanel.SetActive(false);
            var gm = GameManager != null ? GameManager : UnityEngine.Object.FindFirstObjectByType<KyndeBladeGameManager>();
            var loc = gm?.LastEncounterLocation;
            var locId = loc?.LocationId ?? "";

            if (IsFinalCombatLocation(locId))
            {
                var wm = UnityEngine.Object.FindFirstObjectByType<WorldMapManager>();
                var fieldOfGrace = wm?.GetLocation("field_of_grace") ?? Resources.Load<LocationNode>("Data/Vision2/Loc_field_of_grace");
                if (fieldOfGrace != null && wm != null)
                    wm.TransitionTo(fieldOfGrace);
                else
                    ReturnToMap(loc);
            }
            else
            {
                ReturnToMap(loc);
            }
        }

        static bool IsFinalCombatLocation(string locationId)
        {
            if (string.IsNullOrEmpty(locationId)) return false;
            var id = locationId.ToLowerInvariant();
            return id == "years_pass";
        }

        void ReturnToMap(LocationNode loc)
        {
            if (loc == null) return;
            var saveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            var wm = UnityEngine.Object.FindFirstObjectByType<WorldMapManager>();
            if (saveManager != null) saveManager.SaveCheckpoint(loc.LocationId);
            if (wm != null) wm.SetCurrentLocation(loc);
            var mapCanvas = GameObject.Find("MapCanvas");
            var combatCanvas = GameObject.Find("CombatCanvas");
            if (mapCanvas != null) mapCanvas.SetActive(true);
            if (combatCanvas != null) combatCanvas.SetActive(false);
            var mapUI = UnityEngine.Object.FindFirstObjectByType<MapLevelSelectUI>();
            if (mapUI != null && loc != null)
                mapUI.Refresh(loc);
        }

        void OnDestroy()
        {
            if (TurnManager != null) TurnManager.OnCombatEnded -= OnCombatEnded;
            if (AgingManager != null) AgingManager.OnDeathOfOldAgeRequested -= TriggerDeathOfOldAge;
        }

        void OnCombatEnded()
        {
            if (TurnManager == null) return;

            bool victory = AreAllDefeated(TurnManager.EnemyCharacters);
            if (victory)
            {
                Victory();
            }
            else
            {
                Defeat();
            }
        }

        bool AreAllDefeated(System.Collections.Generic.List<MedievalCharacter> list)
        {
            foreach (var c in list)
                if (c != null && c.IsAlive()) return false;
            return true;
        }

        void Victory()
        {
            int xp = CalculateVictoryXP();
            var gm = GameManager != null ? GameManager : UnityEngine.Object.FindFirstObjectByType<KyndeBladeGameManager>();
            bool isFinal = gm != null && IsFinalCombatLocation(gm.LastEncounterLocation?.LocationId ?? "");
            if (IlluminationManager != null)
                StartCoroutine(ShowVictoryAfterIllumination(xp, isFinal));
            else
                ShowVictoryPanel(xp, isFinal);
        }

        void Defeat()
        {
            var saveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            if (saveManager != null) saveManager.IncrementOtherworldBodiesFromDeath();
            var gm = GameManager != null ? GameManager : UnityEngine.Object.FindFirstObjectByType<KyndeBladeGameManager>();

            if (gm != null && gm.IsSinMinibossEncounter)
            {
                StartCoroutine(TransitionToOrfeoOtherworld());
                return;
            }
            bool atGreenChapel = gm != null && gm.LastEncounterLocation != null &&
                string.Equals(gm.LastEncounterLocation.LocationId, "green_chapel", System.StringComparison.OrdinalIgnoreCase);
            if (saveManager != null && atGreenChapel)
                saveManager.IncrementGreenChapelBodies();
            if (atGreenChapel)
            {
                StartCoroutine(RestartGameAfterDefeat());
                return;
            }
            if (IlluminationManager != null)
                StartCoroutine(ShowDefeatAfterIllumination());
            else
                ShowDefeatPanel();
        }

        System.Collections.IEnumerator RestartGameAfterDefeat()
        {
            if (DefeatPanel != null) DefeatPanel.SetActive(false);
            if (IlluminationManager != null)
            {
                IlluminationManager.TriggerDefeatIllumination();
                yield return new WaitForSeconds(4.2f);
            }
            if (DefeatPanel != null) DefeatPanel.SetActive(true);
            if (DefeatText != null) DefeatText.text = "The Green Knight hath taken thy head. A form of thee ends in Orfeo's Otherworld.";
            yield return new WaitForSeconds(2.5f);
            if (DefeatPanel != null) DefeatPanel.SetActive(false);
            var saveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            var wm = UnityEngine.Object.FindFirstObjectByType<WorldMapManager>();
            var malvern = wm != null ? wm.GetLocation("malvern") : Resources.Load<LocationNode>("Data/Vision1/Loc_malvern");
            if (malvern == null)
            {
                var all = Resources.LoadAll<LocationNode>("Data/Vision1");
                if (all != null && all.Length > 0)
                    malvern = all[0];
            }
            if (saveManager != null)
                saveManager.NewGame("malvern");
            if (wm != null && malvern != null)
                wm.TransitionTo(malvern);
            else
                ShowDefeatPanel();
        }

        System.Collections.IEnumerator TransitionToOrfeoOtherworld()
        {
            if (IlluminationManager != null)
            {
                IlluminationManager.TriggerDefeatIllumination();
                yield return new WaitForSeconds(4.2f);
            }
            var wm = UnityEngine.Object.FindFirstObjectByType<WorldMapManager>();
            var otherworld = wm != null ? wm.GetLocation("otherworld") : null;
            if (otherworld == null)
                otherworld = Resources.Load<LocationNode>("Data/OrfeoOtherworld/Loc_otherworld");
            if (otherworld != null)
            {
                if (DefeatPanel != null) DefeatPanel.SetActive(true);
                if (DefeatText != null) DefeatText.text = "The sin hath rent thee. A form of thy body ends in Orfeo's Otherworld.";
                yield return new WaitForSeconds(2f);
                if (DefeatPanel != null) DefeatPanel.SetActive(false);
                if (wm != null)
                    wm.TransitionTo(otherworld);
                else
                {
                    var saveMgr = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
                    if (saveMgr != null) saveMgr.IncrementOtherworldLivingCharacters();
                    var gm = GameManager != null ? GameManager : UnityEngine.Object.FindFirstObjectByType<KyndeBladeGameManager>();
                    if (gm != null && otherworld.Encounter != null)
                        gm.StartEncounterFromConfig(otherworld.Encounter, otherworld);
                }
            }
            else
            {
                ShowDefeatPanel();
            }
        }

        IEnumerator ShowVictoryAfterIllumination(int xp, bool isFinal)
        {
            IlluminationManager.TriggerVictoryIllumination();
            yield return new WaitForSeconds(2.6f);
            ShowVictoryPanel(xp, isFinal);
        }

        System.Collections.IEnumerator ShowDefeatAfterIllumination()
        {
            IlluminationManager.TriggerDefeatIllumination();
            yield return new WaitForSeconds(2.6f);
            ShowDefeatPanel();
        }

        void ShowVictoryPanel(int xp, bool isFinal)
        {
            if (VictoryPanel != null) VictoryPanel.SetActive(true);
            if (VictoryText != null)
                VictoryText.text = isFinal ? "Thou hast reached the field. Continue." : $"Victory! +{xp} XP";
            var continueBtn = VictoryPanel != null ? VictoryPanel.transform.Find("ContinueButton") : null;
            if (continueBtn != null) continueBtn.gameObject.SetActive(true);
            OnVictory?.Invoke();
        }

        void ShowDefeatPanel()
        {
            if (DefeatPanel != null)
            {
                DefeatPanel.SetActive(true);
                var restartBtn = DefeatPanel.transform.Find("RestartButton");
                if (restartBtn != null)
                {
                    var gm = GameManager != null ? GameManager : UnityEngine.Object.FindFirstObjectByType<KyndeBladeGameManager>();
                    restartBtn.gameObject.SetActive(gm != null && gm.CanRestartAtCheckpoint);
                }
            }
            if (DefeatText != null) DefeatText.text = "Thy body is broken. A form of thee ends in Orfeo's Otherworld.";
            OnDefeat?.Invoke();
        }

        int CalculateVictoryXP()
        {
            int xp = BonusXPForVictory;
            if (TurnManager != null)
            {
                foreach (var e in TurnManager.EnemyCharacters)
                    if (e != null && !e.IsAlive()) xp += BaseXPPerEnemy;
            }
            return xp;
        }
    }
}

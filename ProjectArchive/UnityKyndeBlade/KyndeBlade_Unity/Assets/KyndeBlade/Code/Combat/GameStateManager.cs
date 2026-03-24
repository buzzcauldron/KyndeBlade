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

        [Header("Blessings")]
        GameObject _blessingPanel;

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
            if (GameManager == null) GameManager = GameRuntime.GameManager ?? UnityEngine.Object.FindFirstObjectByType<KyndeBladeGameManager>();
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
                VictoryText = CreatePanelText(VictoryPanel.transform, "Victory!", 32, true);
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
            rect.anchorMin = new Vector2(0.25f, 0.15f);
            rect.anchorMax = new Vector2(0.48f, 0.25f);
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
            CreateMenuButton(parent);
        }

        void CreateMenuButton(Transform parent)
        {
            var go = new GameObject("MenuButton");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.52f, 0.15f);
            rect.anchorMax = new Vector2(0.75f, 0.25f);
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
            t.text = "Main Menu";
            t.fontSize = 18;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            ManuscriptUITheme.ApplyToText(t);
            btn.onClick.AddListener(MainMenuManager.ReturnToMainMenu);
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
            StartCoroutine(ShowCreditsSequence());
        }

        IEnumerator ShowCreditsSequence()
        {
            if (DefeatPanel != null) DefeatPanel.SetActive(true);
            if (DefeatText != null) DefeatText.text = "Death of old age.\n\nWille's years have run their course.\nGrace did not come.\n\nA form of thee ends in Orfeo's Otherworld.";
            yield return new WaitForSeconds(5f);

            if (DefeatText != null) DefeatText.text = "\"In a somer seson, whan softe was the sonne,\nI shoop me into shroudes as I a sheep were...\"\n\n— William Langland, Piers Plowman";
            yield return new WaitForSeconds(5f);

            if (DefeatText != null) DefeatText.text = "Kynde Blade\n\nInspired by Clair Obscur: Expedition 33\nNamed after Kynde from William Langland's Piers Plowman\n\nDialogue drawn from Piers Plowman,\nSir Gawain and the Green Knight,\nand Sir Orfeo";
            yield return new WaitForSeconds(6f);

            if (DefeatText != null) DefeatText.text = "The quest for Do-Wel, Do-Bet, and Do-Best\ncontinues. Grace is never found.\nThe dream remains unresolved.\n\nThank you for playing.";
            var restartBtn = DefeatPanel != null ? DefeatPanel.transform.Find("RestartButton") : null;
            if (restartBtn != null)
            {
                restartBtn.gameObject.SetActive(true);
                var t = restartBtn.GetComponentInChildren<Text>();
                if (t != null) t.text = "New Game";
            }
            var menuBtn = DefeatPanel != null ? DefeatPanel.transform.Find("MenuButton") : null;
            if (menuBtn != null) menuBtn.gameObject.SetActive(true);
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
            var wm = UnityEngine.Object.FindFirstObjectByType<WorldMapManager>();
            if (loc != null)
            {
                var saveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
                if (saveManager != null) saveManager.SaveCheckpoint(loc.LocationId);
                if (wm != null) wm.SetCurrentLocation(loc);
            }
            var mapCanvas = GameObject.Find("MapCanvas");
            var combatCanvas = GameObject.Find("CombatCanvas");
            if (mapCanvas != null) mapCanvas.SetActive(true);
            if (combatCanvas != null) combatCanvas.SetActive(false);
            var mapUI = UnityEngine.Object.FindFirstObjectByType<MapLevelSelectUI>();
            var currentLoc = loc ?? wm?.CurrentLocation;
            if (mapUI != null && currentLoc != null)
                mapUI.Refresh(currentLoc);
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
            var gm = GameManager != null ? GameManager : UnityEngine.Object.FindFirstObjectByType<KyndeBladeGameManager>();
            bool isFinal = gm != null && IsFinalCombatLocation(gm.LastEncounterLocation?.LocationId ?? "");
            if (IlluminationManager != null)
                StartCoroutine(ShowVictoryAfterIllumination(isFinal));
            else
                ShowVictoryPanel(isFinal);
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

        IEnumerator ShowVictoryAfterIllumination(bool isFinal)
        {
            IlluminationManager.TriggerVictoryIllumination();
            yield return new WaitForSeconds(2.6f);
            ShowVictoryPanel(isFinal);
        }

        System.Collections.IEnumerator ShowDefeatAfterIllumination()
        {
            IlluminationManager.TriggerDefeatIllumination();
            yield return new WaitForSeconds(2.6f);
            ShowDefeatPanel();
        }

        void ShowVictoryPanel(bool isFinal)
        {
            if (VictoryPanel != null) VictoryPanel.SetActive(true);
            if (isFinal)
            {
                if (VictoryText != null) VictoryText.text = "Thou hast reached the field. Continue to return to map and progress.";
                var continueBtn = VictoryPanel != null ? VictoryPanel.transform.Find("ContinueButton") : null;
                if (continueBtn != null) continueBtn.gameObject.SetActive(true);
                OnVictory?.Invoke();
                return;
            }

            if (VictoryText != null)
                VictoryText.text = "Victory! Choose a Blessing, then press Continue to leave combat.";

            float totalKynde = 0f;
            if (TurnManager != null)
                foreach (var c in TurnManager.PlayerCharacters)
                    if (c != null) totalKynde += c.GetCurrentKynde();

            var save = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            var gm = GameManager ?? UnityEngine.Object.FindFirstObjectByType<KyndeBladeGameManager>();
            var reward = gm?.LastEncounterLocation?.Rewards;
            float tierBonus = reward?.BlessingTierBonus ?? 0f;
            var choices = BlessingSystem.GenerateChoices(totalKynde, save, tierBonus);
            if (reward?.GuaranteedBlessing != null && !choices.Exists(b => b.BlessingId == reward.GuaranteedBlessing.BlessingId))
                choices.Add(reward.GuaranteedBlessing);
            ShowBlessingChoice(choices, save);
        }

        void ShowBlessingChoice(System.Collections.Generic.List<Blessing> choices, SaveManager save)
        {
            if (choices == null || choices.Count == 0)
            {
                if (VictoryText != null) VictoryText.text = "Victory!";
                var fallbackBtn = VictoryPanel != null ? VictoryPanel.transform.Find("ContinueButton") : null;
                if (fallbackBtn != null) fallbackBtn.gameObject.SetActive(true);
                OnVictory?.Invoke();
                return;
            }

            if (_blessingPanel != null) Destroy(_blessingPanel);

            var canvas = VictoryPanel != null ? VictoryPanel.GetComponentInParent<Canvas>() : null;
            Transform parent = canvas != null ? canvas.transform : (VictoryPanel != null ? VictoryPanel.transform : transform);

            _blessingPanel = new GameObject("BlessingChoicePanel");
            _blessingPanel.transform.SetParent(parent, false);
            var panelRect = _blessingPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.05f, 0.05f);
            panelRect.anchorMax = new Vector2(0.95f, 0.55f);
            panelRect.offsetMin = panelRect.offsetMax = Vector2.zero;
            var bg = _blessingPanel.AddComponent<Image>();
            bg.color = ManuscriptUITheme.ParchmentLight;

            var title = new GameObject("Title");
            title.transform.SetParent(_blessingPanel.transform, false);
            var titleRect = title.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.85f);
            titleRect.anchorMax = new Vector2(1, 1f);
            titleRect.offsetMin = titleRect.offsetMax = Vector2.zero;
            var titleText = title.AddComponent<Text>();
            titleText.text = "Choose a Blessing";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 22;
            titleText.alignment = TextAnchor.MiddleCenter;
            ManuscriptUITheme.ApplyToText(titleText, emphasis: true);

            for (int i = 0; i < choices.Count; i++)
            {
                var blessing = choices[i];
                float xMin = 0.02f + i * 0.327f;
                float xMax = xMin + 0.31f;
                CreateBlessingCard(blessing, new Vector2(xMin, 0.05f), new Vector2(xMax, 0.82f), save);
            }
        }

        void CreateBlessingCard(Blessing b, Vector2 anchorMin, Vector2 anchorMax, SaveManager save)
        {
            var card = new GameObject("Card_" + b.BlessingId);
            card.transform.SetParent(_blessingPanel.transform, false);
            var cardRect = card.AddComponent<RectTransform>();
            cardRect.anchorMin = anchorMin;
            cardRect.anchorMax = anchorMax;
            cardRect.offsetMin = cardRect.offsetMax = Vector2.zero;
            var cardImg = card.AddComponent<Image>();

            Color cardColor = b.Category switch
            {
                BlessingCategory.Virtue => ManuscriptUITheme.Parchment,
                BlessingCategory.Sin => new Color(0.45f, 0.3f, 0.3f, 0.9f),
                BlessingCategory.Pilgrimage => new Color(0.35f, 0.42f, 0.5f, 0.9f),
                _ => ManuscriptUITheme.Parchment
            };
            cardImg.color = cardColor;

            var tierLabel = CreateCardText(card.transform, TierLabel(b), 11,
                new Vector2(0.05f, 0.88f), new Vector2(0.95f, 0.98f));
            ManuscriptUITheme.ApplyToText(tierLabel, lapis: true);

            var nameLabel = CreateCardText(card.transform, b.DisplayName, 18,
                new Vector2(0.05f, 0.72f), new Vector2(0.95f, 0.88f));
            ManuscriptUITheme.ApplyToText(nameLabel, emphasis: true);

            var descLabel = CreateCardText(card.transform, b.Description, 13,
                new Vector2(0.05f, 0.38f), new Vector2(0.95f, 0.72f));
            ManuscriptUITheme.ApplyToText(descLabel);

            if (!string.IsNullOrEmpty(b.DrawbackDescription))
            {
                var drawback = CreateCardText(card.transform, b.DrawbackDescription, 12,
                    new Vector2(0.05f, 0.22f), new Vector2(0.95f, 0.38f));
                ManuscriptUITheme.ApplyToText(drawback, rubrication: true);
            }

            var btn = card.AddComponent<Button>();
            ManuscriptUITheme.ApplyToButton(btn, emphasis: b.Category == BlessingCategory.Virtue);
            var capturedBlessing = b;
            var capturedSave = save;
            btn.onClick.AddListener(() => OnBlessingChosen(capturedBlessing, capturedSave));
        }

        void OnBlessingChosen(Blessing b, SaveManager save)
        {
            if (TurnManager != null)
            {
                foreach (var c in TurnManager.PlayerCharacters)
                {
                    if (c == null || !c.IsAlive()) continue;
                    BlessingSystem.ApplyBlessing(c, b, save);
                    c.Stats.BlessingCount++;
                }
            }
            save?.Save();

            if (_blessingPanel != null) Destroy(_blessingPanel);
            if (VictoryText != null)
            {
                string text = $"Blessed with {b.DisplayName}!";
                if (TurnManager != null)
                    foreach (var c in TurnManager.PlayerCharacters)
                        if (c != null)
                            text += $"\n{c.CharacterName} — {c.Stats.ActiveBlessings.Count} blessing{(c.Stats.ActiveBlessings.Count != 1 ? "s" : "")}";
                VictoryText.text = text;
            }
            var continueBtn = VictoryPanel != null ? VictoryPanel.transform.Find("ContinueButton") : null;
            if (continueBtn != null) continueBtn.gameObject.SetActive(true);
            OnVictory?.Invoke();
        }

        Text CreateCardText(Transform parent, string content, int fontSize, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var t = go.AddComponent<Text>();
            t.text = content;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = fontSize;
            t.alignment = TextAnchor.MiddleCenter;
            return t;
        }

        string TierLabel(Blessing b) => b.Category switch
        {
            BlessingCategory.Virtue => $"Virtue — {b.Tier}",
            BlessingCategory.Sin => $"Sin Temptation — {b.Tier}",
            BlessingCategory.Pilgrimage => "Pilgrimage",
            _ => b.Tier.ToString()
        };

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

    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Combat UI per Hodent: clear hierarchy, turn order, actionable elements.</summary>
    [RequireComponent(typeof(Canvas))]
    public class CombatUI : MonoBehaviour
    {
        [Header("References")]
        public TurnManager TurnManager;
        public GameSettings Settings;
        public CombatFeedback Feedback;
        public ParryDodgeZoneIndicator ParryDodgeIndicator;

        [Header("Prefabs (optional)")]
        public GameObject TurnOrderSlotPrefab;
        public GameObject ActionButtonPrefab;

        [Header("UI Roots (assign or auto-create)")]
        public Transform TurnOrderRoot;
        public Transform ActionButtonsRoot;
        public Transform CharacterStatsRoot;
        public Text GoalText;
        public Text StateText;
        public Text RealTimeWindowText;
        public Text StaminaText;
        public Text KyndeText;

        List<GameObject> _turnOrderSlots = new List<GameObject>();
        List<Button> _actionButtons = new List<Button>();
        MedievalCharacter _subscribedCharacter;
        ParryDodgeInputHandler _parryDodgeInputHandler;
        // Built-in font only; avoid static refs to large/scene assets (Unity lifecycle).
        static Font _cachedFont;
        bool _lazyInitDone;

        static Font DefaultFont => _cachedFont ?? (_cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        void Start()
        {
            if (TurnManager == null) TurnManager = GameRuntime.TurnManager ?? UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (TurnManager != null)
                TryLazyInit();
        }

        void Update()
        {
            if (!_lazyInitDone)
            {
                if (TurnManager == null) TurnManager = GameRuntime.TurnManager ?? UnityEngine.Object.FindFirstObjectByType<TurnManager>();
                TryLazyInit();
                return;
            }
            if (TurnManager != null && TurnManager.State == CombatState.RealTimeWindow && RealTimeWindowText != null)
                RealTimeWindowText.text = $"React! {TurnManager.RealTimeWindowRemaining:F1}s";
        }

        void TryLazyInit()
        {
            if (TurnManager == null || _lazyInitDone) return;
            _lazyInitDone = true;
            EnsureUIRoots();
            EnsureParryDodgeIndicator();
            ApplyManuscriptTheme();
            TurnManager.OnTurnChanged += OnTurnChanged;
            TurnManager.OnCombatEnded += OnCombatEnded;
            if (TurnManager.State != CombatState.CombatEnded)
                RefreshAll();
        }

        void OnDestroy()
        {
            UnsubscribeFromCurrentCharacter();
            if (_parryDodgeInputHandler != null)
            {
                _parryDodgeInputHandler.OnDodgePressed -= OnDodgePressedReceived;
                _parryDodgeInputHandler.OnParryPressed -= OnParryPressedReceived;
                _parryDodgeInputHandler = null;
            }
            if (TurnManager != null)
            {
                TurnManager.OnTurnChanged -= OnTurnChanged;
                TurnManager.OnCombatEnded -= OnCombatEnded;
            }
        }

        void UnsubscribeFromCurrentCharacter()
        {
            if (_subscribedCharacter != null)
            {
                _subscribedCharacter.OnStaminaChanged -= RefreshStaminaKynde;
                _subscribedCharacter.OnKyndeChanged -= RefreshStaminaKynde;
                _subscribedCharacter = null;
            }
        }

        void SubscribeToCurrentCharacter(MedievalCharacter c)
        {
            UnsubscribeFromCurrentCharacter();
            _subscribedCharacter = c;
            if (c != null)
            {
                c.OnStaminaChanged += RefreshStaminaKynde;
                c.OnKyndeChanged += RefreshStaminaKynde;
            }
        }

        void RefreshStaminaKynde(float _, float __) { RefreshStaminaKyndeDisplay(); }

        void EnsureUIRoots()
        {
            if (TurnOrderRoot == null)
            {
                var go = new GameObject("TurnOrder");
                go.transform.SetParent(transform);
                var vlg = go.AddComponent<VerticalLayoutGroup>();
                vlg.spacing = 4;
                vlg.childAlignment = TextAnchor.UpperLeft;
                TurnOrderRoot = go.transform;
            }
            if (ActionButtonsRoot == null)
            {
                var go = new GameObject("ActionButtons");
                go.transform.SetParent(transform);
                var hlg = go.AddComponent<HorizontalLayoutGroup>();
                hlg.spacing = 8;
                ActionButtonsRoot = go.transform;
            }
            if (GoalText == null)
            {
                var go = new GameObject("GoalText");
                go.transform.SetParent(transform);
                var t = go.AddComponent<Text>();
                t.text = GameWorldConstants.GoalDefeatAllEnemies;
                t.font = DefaultFont;
                t.fontSize = 24;
                ManuscriptUITheme.ApplyToText(t);
                GoalText = t;
            }
            if (StaminaText == null)
            {
                var go = new GameObject("StaminaText");
                go.transform.SetParent(transform);
                var t = go.AddComponent<Text>();
                t.text = "Stamina: —";
                t.font = DefaultFont;
                t.fontSize = 14;
                var rect = go.GetComponent<RectTransform>();
                if (rect == null) rect = go.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                rect.anchoredPosition = new Vector2(10, -10);
                rect.sizeDelta = new Vector2(150, 24);
                ManuscriptUITheme.ApplyToText(t);
                StaminaText = t;
            }
            if (KyndeText == null)
            {
                var go = new GameObject("KyndeText");
                go.transform.SetParent(transform);
                var t = go.AddComponent<Text>();
                t.text = "Kynde: —";
                t.font = DefaultFont;
                t.fontSize = 14;
                var rect = go.GetComponent<RectTransform>();
                if (rect == null) rect = go.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                rect.anchoredPosition = new Vector2(10, -36);
                rect.sizeDelta = new Vector2(150, 24);
                ManuscriptUITheme.ApplyToText(t);
                KyndeText = t;
            }
        }

        void ApplyManuscriptTheme()
        {
            if (GoalText != null) ManuscriptUITheme.ApplyToText(GoalText);
            if (StateText != null) ManuscriptUITheme.ApplyToText(StateText);
            if (RealTimeWindowText != null) ManuscriptUITheme.ApplyToText(RealTimeWindowText);
            if (StaminaText != null) ManuscriptUITheme.ApplyToText(StaminaText);
            if (KyndeText != null) ManuscriptUITheme.ApplyToText(KyndeText);
        }

        void EnsureParryDodgeIndicator()
        {
            if (ParryDodgeIndicator != null) return;
            var soundBank = UnityEngine.Object.FindFirstObjectByType<ParryDodgeZoneSoundBank>();
            if (soundBank == null && Feedback != null)
            {
                soundBank = Feedback.gameObject.AddComponent<ParryDodgeZoneSoundBank>();
            }
            var go = new GameObject("ParryDodgeZoneIndicator");
            go.transform.SetParent(transform, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(200, 100);
            rect.anchoredPosition = Vector2.zero;
            ParryDodgeIndicator = go.AddComponent<ParryDodgeZoneIndicator>();
            ParryDodgeIndicator.TurnManager = TurnManager;
            ParryDodgeIndicator.SoundBank = soundBank;

            var inputHandler = go.GetComponent<ParryDodgeInputHandler>();
            if (inputHandler == null) inputHandler = go.AddComponent<ParryDodgeInputHandler>();
            inputHandler.TurnManager = TurnManager;
            _parryDodgeInputHandler = inputHandler;
            inputHandler.OnDodgePressed += OnDodgePressedReceived;
            inputHandler.OnParryPressed += OnParryPressedReceived;
        }

        void OnDodgePressedReceived(float windowRemaining)
        {
            var defender = TurnManager?.DefenderDuringWindow;
            if (defender != null) defender.StartDodge(windowRemaining);
        }

        void OnParryPressedReceived(float windowRemaining)
        {
            var defender = TurnManager?.DefenderDuringWindow;
            if (defender != null) defender.StartParry(windowRemaining);
        }

        void OnTurnChanged(MedievalCharacter current)
        {
            SubscribeToCurrentCharacter(current);
            RefreshTurnOrder();
            RefreshActionButtons();
            RefreshStateText();
            RefreshStaminaKyndeDisplay();
            if (current != null && TurnManager.IsPlayerTurn())
                SetGoal(GameWorldConstants.GoalYourTurnSelectAction);
            else if (current != null)
                SetGoal(GameWorldConstants.GoalEnemyTurnPrepare);
        }

        void OnCombatEnded()
        {
            RefreshAll();
            bool victory = TurnManager != null && AreAllDefeated(TurnManager.EnemyCharacters);
            SetGoal(victory ? GameWorldConstants.GoalVictory : GameWorldConstants.GoalDefeat);
            SetState("");
            ClearActionButtons();
        }

        bool AreAllDefeated(List<MedievalCharacter> list)
        {
            foreach (var c in list)
                if (c != null && c.IsAlive()) return false;
            return true;
        }

        void RefreshAll()
        {
            RefreshTurnOrder();
            RefreshActionButtons();
            RefreshStateText();
            RefreshGoal();
        }

        void RefreshTurnOrder()
        {
            if (TurnManager == null || TurnOrderRoot == null) return;

            foreach (var slot in _turnOrderSlots)
                if (slot != null) Destroy(slot);
            _turnOrderSlots.Clear();

            foreach (var c in TurnManager.TurnOrder)
            {
                if (c == null || !c.IsAlive()) continue;
                var slot = CreateTurnSlot(c);
                slot.transform.SetParent(TurnOrderRoot);
                _turnOrderSlots.Add(slot);
            }
        }

        GameObject CreateTurnSlot(MedievalCharacter c)
        {
            var go = new GameObject($"Slot_{c.CharacterName}");
            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 6;
            layout.childAlignment = TextAnchor.MiddleLeft;

            var font = DefaultFont;
            var nameObj = new GameObject("Name");
            nameObj.transform.SetParent(go.transform);
            var nameText = nameObj.AddComponent<Text>();
            nameText.text = c.CharacterName;
            nameText.font = font;
            nameText.fontSize = 14;
            ManuscriptUITheme.ApplyToText(nameText, emphasis: c == TurnManager.CurrentCharacter);
            var nameLe = nameObj.AddComponent<LayoutElement>();
            nameLe.preferredWidth = 90f;

            bool isPlayer = TurnManager.PlayerCharacters.Contains(c);
            var bar = ManuscriptHealthBar.Create(go.transform, 70f, 8f, isPlayer);
            bar.SetCharacter(c);

            return go;
        }

        void RefreshActionButtons()
        {
            ClearActionButtons();
            if (TurnManager == null || !TurnManager.IsPlayerTurn() || TurnManager.CurrentCharacter == null) return;

            var actions = TurnManager.CurrentCharacter.AvailableActions;
            if (actions == null || actions.Count == 0) return;

            foreach (var action in actions)
            {
                if (action == null) continue;
                var target = action.ActionData.ActionType == CombatActionType.Heal
                    ? TurnManager.CurrentCharacter
                    : GetFirstEnemy();
                var btn = CreateActionButton(action, target);
                if (btn != null)
                {
                    btn.transform.SetParent(ActionButtonsRoot);
                    _actionButtons.Add(btn);
                }
            }
        }

        Button CreateActionButton(CombatAction action, MedievalCharacter target)
        {
            var go = new GameObject($"Btn_{action.ActionData.ActionName}");
            var img = go.AddComponent<Image>();
            img.color = ManuscriptUITheme.ParchmentAged;
            var btn = go.AddComponent<Button>();

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(go.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
            var text = textObj.AddComponent<Text>();
            var costStr = FormatActionCost(action);
            text.text = string.IsNullOrEmpty(costStr) ? action.ActionData.ActionName : $"{action.ActionData.ActionName} [{costStr}]";
            text.font = DefaultFont;
            text.fontSize = 16;
            text.alignment = TextAnchor.MiddleCenter;

            var rect = go.GetComponent<RectTransform>();
            if (rect != null) rect.sizeDelta = new Vector2(120, 36);

            var current = TurnManager.CurrentCharacter;
            bool canAfford = current != null && current.GetCurrentStamina() >= action.ActionData.StaminaCost &&
                (action.ActionData.KyndeCost <= 0f || current.GetCurrentKynde() >= action.ActionData.KyndeCost);
            btn.interactable = canAfford;
            if (canAfford)
                ManuscriptUITheme.ApplyToText(text);
            else
                text.color = ManuscriptUITheme.InkLight;

            ManuscriptUITheme.ApplyToButton(btn);

            var a = action;
            btn.onClick.AddListener(() => TurnManager.ExecuteAction(a, target));

            return btn;
        }

        string FormatActionCost(CombatAction action)
        {
            var d = action.ActionData;
            var parts = new List<string>();
            if (d.StaminaCost > 0f) parts.Add($"S:{d.StaminaCost:0}");
            if (d.KyndeCost > 0f) parts.Add($"K:{d.KyndeCost:0}");
            return parts.Count > 0 ? string.Join("/", parts) : "";
        }

        MedievalCharacter GetFirstEnemy()
        {
            if (TurnManager == null) return null;
            foreach (var e in TurnManager.EnemyCharacters)
                if (e != null && e.IsAlive()) return e;
            return null;
        }

        void ClearActionButtons()
        {
            foreach (var b in _actionButtons)
                if (b != null) Destroy(b.gameObject);
            _actionButtons.Clear();
        }

        void RefreshStateText()
        {
            if (StateText == null) return;
            switch (TurnManager?.State)
            {
                case CombatState.WaitingForInput: SetState(GameWorldConstants.StateSelectAction); break;
                case CombatState.ExecutingAction: SetState(GameWorldConstants.StateExecuting); break;
                case CombatState.RealTimeWindow:
                    SetState($"Dodge/Parry! {TurnManager.RealTimeWindowRemaining:F1}s");
                    break;
                case CombatState.ProcessingResults: SetState(GameWorldConstants.StateResolving); break;
                case CombatState.CombatEnded: SetState(GameWorldConstants.StateCombatEnded); break;
                default: SetState(""); break;
            }
        }

        void RefreshStaminaKyndeDisplay()
        {
            var c = TurnManager?.CurrentCharacter;
            if (StaminaText != null)
                StaminaText.text = c != null ? $"Stamina: {c.GetCurrentStamina():0}/{c.GetMaxStamina():0}" : "Stamina: —";
            if (KyndeText != null)
                KyndeText.text = c != null ? $"Kynde: {c.GetCurrentKynde():0}/{c.GetMaxKynde():0}" : "Kynde: —";
        }

        void RefreshGoal()
        {
            if (TurnManager == null) return;
            if (TurnManager.State == CombatState.CombatEnded)
            {
                bool victory = AreAllDefeated(TurnManager.EnemyCharacters);
                SetGoal(victory ? GameWorldConstants.GoalVictory : GameWorldConstants.GoalDefeat);
            }
            else
            {
                SetGoal(GameWorldConstants.GoalDefeatAllEnemies);
            }
        }

        void SetGoal(string s) { if (GoalText != null) GoalText.text = s; }
        void SetState(string s) { if (StateText != null) StateText.text = s; }
    }
}

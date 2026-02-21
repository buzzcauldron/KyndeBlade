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

        List<GameObject> _turnOrderSlots = new List<GameObject>();
        List<Button> _actionButtons = new List<Button>();
        static Font _cachedFont;

        static Font DefaultFont => _cachedFont ?? (_cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        void Start()
        {
            if (TurnManager == null) TurnManager = FindObjectOfType<TurnManager>();
            if (TurnManager == null) return;

            EnsureUIRoots();
            EnsureParryDodgeIndicator();
            TurnManager.OnTurnChanged += OnTurnChanged;
            TurnManager.OnCombatEnded += OnCombatEnded;

            if (TurnManager.State != CombatState.CombatEnded)
                RefreshAll();
        }

        void OnDestroy()
        {
            if (TurnManager != null)
            {
                TurnManager.OnTurnChanged -= OnTurnChanged;
                TurnManager.OnCombatEnded -= OnCombatEnded;
            }
        }

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
                t.text = "Defeat all enemies";
                t.font = DefaultFont;
                t.fontSize = 24;
                GoalText = t;
            }
        }

        void EnsureParryDodgeIndicator()
        {
            if (ParryDodgeIndicator != null) return;
            var soundBank = FindObjectOfType<ParryDodgeZoneSoundBank>();
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
        }

        void OnTurnChanged(MedievalCharacter current)
        {
            RefreshTurnOrder();
            RefreshActionButtons();
            RefreshStateText();
            if (current != null && TurnManager.IsPlayerTurn())
                SetGoal("Your turn — select an action");
            else if (current != null)
                SetGoal("Enemy turn — prepare to dodge or parry");
        }

        void OnCombatEnded()
        {
            RefreshAll();
            bool victory = TurnManager != null && AreAllDefeated(TurnManager.EnemyCharacters);
            SetGoal(victory ? "Victory!" : "Defeat...");
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
            layout.spacing = 4;
            layout.childAlignment = TextAnchor.MiddleLeft;

            var font = DefaultFont;
            var nameText = new GameObject("Name").AddComponent<Text>();
            nameText.transform.SetParent(go.transform);
            nameText.text = c.CharacterName;
            nameText.font = font;
            nameText.fontSize = 14;

            var hpText = new GameObject("HP").AddComponent<Text>();
            hpText.transform.SetParent(go.transform);
            hpText.text = $"{c.GetCurrentHealth():0}/{c.GetMaxHealth():0}";
            hpText.font = font;
            hpText.fontSize = 12;

            bool isCurrent = c == TurnManager.CurrentCharacter;
            nameText.color = isCurrent ? Color.yellow : Color.white;

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
            var btn = go.AddComponent<Button>();
            var text = new GameObject("Text").AddComponent<Text>();
            text.transform.SetParent(go.transform);
            text.text = action.ActionData.ActionName;
            text.font = DefaultFont;
            text.fontSize = 16;
            text.alignment = TextAnchor.MiddleCenter;

            var a = action;
            btn.onClick.AddListener(() => TurnManager.ExecuteAction(a, target));

            return btn;
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
                case CombatState.WaitingForInput: SetState("Select action"); break;
                case CombatState.ExecutingAction: SetState("Executing..."); break;
                case CombatState.RealTimeWindow:
                    SetState($"Dodge/Parry! {TurnManager.RealTimeWindowRemaining:F1}s");
                    break;
                case CombatState.ProcessingResults: SetState("Resolving..."); break;
                case CombatState.CombatEnded: SetState("Combat ended"); break;
                default: SetState(""); break;
            }
        }

        void RefreshGoal()
        {
            if (TurnManager == null) return;
            if (TurnManager.State == CombatState.CombatEnded)
            {
                bool victory = AreAllDefeated(TurnManager.EnemyCharacters);
                SetGoal(victory ? "Victory!" : "Defeat...");
            }
            else
            {
                SetGoal("Defeat all enemies");
            }
        }

        void SetGoal(string s) { if (GoalText != null) GoalText.text = s; }
        void SetState(string s) { if (StateText != null) StateText.text = s; }

        void Update()
        {
            if (TurnManager == null || TurnManager.State != CombatState.RealTimeWindow || RealTimeWindowText == null) return;
            RealTimeWindowText.text = $"React! {TurnManager.RealTimeWindowRemaining:F1}s";
        }
    }
}

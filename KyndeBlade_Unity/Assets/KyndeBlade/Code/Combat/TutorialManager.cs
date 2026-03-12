using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Progressive onboarding (Hodent: Learning - one mechanic at a time).</summary>
    public class TutorialManager : MonoBehaviour
    {
        [Header("References")]
        public TurnManager TurnManager;
        public Text HintText;

        public enum TutorialPhase
        {
            None,
            BasicAttack,
            Dodge,
            Parry,
            Counter,
            Complete
        }

        public TutorialPhase Phase { get; private set; } = TutorialPhase.None;

        bool _phaseActionPerformed;
        bool _canSkip;

        void Start()
        {
            if (TurnManager == null) TurnManager = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            Phase = TutorialPhase.BasicAttack;
            _phaseActionPerformed = false;
            SetHint("Select Strike to attack. Defeat all enemies.");
        }

        void OnEnable()
        {
            if (TurnManager != null)
            {
                TurnManager.OnTurnChanged += OnTurnChanged;
                TurnManager.OnCombatEnded += OnCombatEnded;
                TurnManager.OnActionExecuted += OnActionExecuted;
            }
            SubscribeCharacterEvents();
        }

        void OnDisable()
        {
            if (TurnManager != null)
            {
                TurnManager.OnTurnChanged -= OnTurnChanged;
                TurnManager.OnCombatEnded -= OnCombatEnded;
                TurnManager.OnActionExecuted -= OnActionExecuted;
            }
            UnsubscribeCharacterEvents();
        }

        void SubscribeCharacterEvents()
        {
            if (TurnManager == null) return;
            foreach (var pc in TurnManager.PlayerCharacters)
            {
                pc.OnDodgeAttempted += OnDodgeAttempted;
                pc.OnParryAttempted += OnParryAttempted;
            }
        }

        void UnsubscribeCharacterEvents()
        {
            if (TurnManager == null) return;
            foreach (var pc in TurnManager.PlayerCharacters)
            {
                pc.OnDodgeAttempted -= OnDodgeAttempted;
                pc.OnParryAttempted -= OnParryAttempted;
            }
        }

        void OnDodgeAttempted(MedievalCharacter character, bool success)
        {
            if (Phase == TutorialPhase.Dodge && success)
                _phaseActionPerformed = true;
        }

        void OnParryAttempted(MedievalCharacter character, bool success)
        {
            if (Phase == TutorialPhase.Parry && success)
                _phaseActionPerformed = true;
        }

        void OnActionExecuted(MedievalCharacter executor, MedievalCharacter target, CombatAction action)
        {
            if (Phase != TutorialPhase.Counter) return;
            if (action.ActionData.ActionType == CombatActionType.Counter
                && TurnManager.PlayerCharacters.Contains(executor))
            {
                _phaseActionPerformed = true;
            }
        }

        void OnTurnChanged(MedievalCharacter current)
        {
            if (Phase == TutorialPhase.Complete) return;
            if (!TurnManager.IsPlayerTurn()) return;

            switch (Phase)
            {
                case TutorialPhase.BasicAttack:
                    SetHint("Your turn. Attack an enemy with Strike.");
                    break;
                case TutorialPhase.Dodge:
                    SetHint("When enemies attack, try Dodge (Escapade) to avoid damage.");
                    break;
                case TutorialPhase.Parry:
                    SetHint("Parry (Ward) reduces damage and gives Kynde. Time it carefully!");
                    break;
                case TutorialPhase.Counter:
                    SetHint("After a successful Parry, you can Counter for bonus damage.");
                    break;
            }
        }

        void OnCombatEnded()
        {
            if (Phase == TutorialPhase.Complete) return;

            if (Phase == TutorialPhase.BasicAttack || _phaseActionPerformed)
            {
                AdvancePhase();
            }
            else
            {
                SetHint(GetRetryPrompt());
            }
        }

        string GetRetryPrompt()
        {
            switch (Phase)
            {
                case TutorialPhase.Dodge:
                    return "You didn't dodge successfully. Try again — use Escapade when enemies attack!";
                case TutorialPhase.Parry:
                    return "You didn't parry successfully. Try again — use Ward to block an attack!";
                case TutorialPhase.Counter:
                    return "You didn't counter successfully. Try again — Counter after a Parry!";
                default:
                    return "Try again!";
            }
        }

        public void AdvancePhase()
        {
            if (Phase == TutorialPhase.Complete) return;
            Phase = (TutorialPhase)((int)Phase + 1);
            _phaseActionPerformed = false;
            if (Phase == TutorialPhase.Complete)
            {
                SetHint("Tutorial complete. Good luck!");
            }
        }

        public void SetPhase(TutorialPhase phase)
        {
            Phase = phase;
            _phaseActionPerformed = false;
        }

        public void SkipTutorial()
        {
            if (!_canSkip) return;
            Phase = TutorialPhase.Complete;
            _phaseActionPerformed = false;
            SetHint("Tutorial skipped.");
        }

        public void SetCanSkip(bool canSkip)
        {
            _canSkip = canSkip;
        }

        void SetHint(string s)
        {
            if (HintText != null) HintText.text = s;
        }
    }
}

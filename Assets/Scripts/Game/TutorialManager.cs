using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KyndeBlade
{
    /// <summary>Progressive onboarding (Hodent: Learning - one mechanic at a time). Wode-Wo guides the dreamer.</summary>
    public class TutorialManager : MonoBehaviour
    {
        [Header("References")]
        public TurnManager TurnManager;
        public Text HintText;
        [Tooltip("If true, show Wode-Wo story beats at each phase (when he is alive).")]
        public bool UseWodeWoVoice = true;

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

        void Start()
        {
            if (TurnManager == null) TurnManager = FindObjectOfType<TurnManager>();
            Phase = TutorialPhase.BasicAttack;
            SetHint("Select Strike to attack. Defeat all enemies.");
        }

        void OnEnable()
        {
            if (TurnManager != null)
            {
                TurnManager.OnTurnChanged += OnTurnChanged;
                TurnManager.OnCombatEnded += OnCombatEnded;
            }
        }

        void OnDisable()
        {
            if (TurnManager != null)
            {
                TurnManager.OnTurnChanged -= OnTurnChanged;
                TurnManager.OnCombatEnded -= OnCombatEnded;
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
            AdvancePhase();
        }

        public void AdvancePhase()
        {
            if (Phase == TutorialPhase.Complete) return;
            Phase = (TutorialPhase)((int)Phase + 1);
            if (Phase == TutorialPhase.Complete)
            {
                SetHint("Tutorial complete. Good luck!");
                if (UseWodeWoVoice)
                {
                    var wodeWo = FindObjectOfType<WodeWoManager>();
                    var beat = wodeWo?.GetTutorialBeat(Phase);
                    if (beat != null)
                    {
                        var nm = FindObjectOfType<NarrativeManager>();
                        if (nm != null)
                            nm.ShowStoryBeat(beat);
                    }
                }
            }
        }

        public void SetPhase(TutorialPhase phase)
        {
            Phase = phase;
        }

        void SetHint(string s)
        {
            if (HintText != null) HintText.text = s;
        }
    }
}

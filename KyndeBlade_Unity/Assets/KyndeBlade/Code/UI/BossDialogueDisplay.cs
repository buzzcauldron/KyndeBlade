using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Bridges BossDialogueManager to the DialogueSystem (manuscript panel). Subscribe to OnBossLineRequested and show the line. Assign DialogueSystem to use the existing panel; or leave unset and handle lines elsewhere (e.g. speech bubble).</summary>
    public class BossDialogueDisplay : MonoBehaviour
    {
        [Header("Display")]
        [Tooltip("If set, boss lines are shown here (manuscript-style). Otherwise only subscribe to manager and handle in your own UI.")]
        public DialogueSystem DialogueSystem;
        [Tooltip("How long to show phase/action lines before auto-closing.")]
        public float LineDisplayDuration = 3.5f;

        void Start()
        {
            if (BossDialogueManager.Instance == null) return;
            BossDialogueManager.Instance.OnBossLineRequested += OnBossLineRequested;
            if (DialogueSystem == null) DialogueSystem = FindObjectOfType<DialogueSystem>();
        }

        void OnDestroy()
        {
            if (BossDialogueManager.Instance != null)
                BossDialogueManager.Instance.OnBossLineRequested -= OnBossLineRequested;
        }

        void OnBossLineRequested(string line, MedievalCharacter speaker)
        {
            if (string.IsNullOrEmpty(line)) return;
            string speakerName = speaker != null ? speaker.CharacterName : "Boss";
            if (DialogueSystem != null)
                DialogueSystem.ShowLine(line, speakerName, LineDisplayDuration);
        }
    }
}

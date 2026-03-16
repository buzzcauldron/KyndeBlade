using System;
using System.Collections;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Manages Wode-Wo: the kind forest monster companion. Tutorial guide. Dies permanently on first mistake or defeat to major boss.</summary>
    public class WodeWoManager : MonoBehaviour
    {
        public static WodeWoManager Instance { get; private set; }

        [Header("Wode-Wo Death Cutscene")]
        public StoryBeat WodeWoDeathBeat;
        [Tooltip("Fallback if no beat assigned: show this text.")]
        public string WodeWoDeathTextFallback = "Pale hands reach from the gloaming. The fae take him—not gently. They pull him apart, root from branch from breath, as if he were nothing. His voice breaks. \"Dreamer... I tried my best... to...\" Cold fingers close. What remains is scattered. The forest weeps. He does not rise again.";
        public string WodeWoDeathSpeaker = "Narrator";

        [Header("Tutorial Beats (Wode-Wo guides)")]
        public StoryBeat WodeWoIntroBeat;
        public StoryBeat WodeWoAttackHintBeat;
        public StoryBeat WodeWoDodgeHintBeat;
        public StoryBeat WodeWoParryHintBeat;
        public StoryBeat WodeWoCompleteBeat;

        NarrativeManager _narrative;
        DialogueSystem _dialogue;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            _narrative = FindObjectOfType<NarrativeManager>();
            _dialogue = FindObjectOfType<DialogueSystem>();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>Wode-Wo is alive and can appear. False if already dead (install-level).</summary>
        public bool IsWodeWoAlive => !InstallState.WodeWoIsDead;

        /// <summary>Is this encounter against a major boss? (Hunger, Green Knight, Pride, sin miniboss)</summary>
        public static bool IsMajorBossEncounter(EncounterConfig encounter, LocationNode location, bool isSinMiniboss)
        {
            if (encounter == null) return false;
            if (isSinMiniboss) return true;
            var bossType = (encounter.BossCharacterType ?? "").ToLowerInvariant();
            if (bossType.Contains("hunger")) return true;
            if (bossType.Contains("green") || bossType.Contains("knight")) return true;
            if (bossType.Contains("pride")) return true;
            if (encounter.BossPrefab != null)
            {
                var name = encounter.BossPrefab.name.ToLowerInvariant();
                if (name.Contains("hunger") || name.Contains("green") || name.Contains("pride")) return true;
            }
            var locId = location?.LocationId?.ToLowerInvariant() ?? "";
            if (locId.Contains("green_chapel")) return true;
            if (locId.Contains("dongeoun") && bossType.Contains("hunger")) return true;
            return false;
        }

        /// <summary>Trigger Wode-Wo death cutscene. Call when player loses to major boss or makes first mistake.</summary>
        public void TriggerWodeWoDeath(Action onCutsceneComplete = null)
        {
            if (!IsWodeWoAlive)
            {
                onCutsceneComplete?.Invoke();
                return;
            }
            InstallState.MarkWodeWoDead();
            StartCoroutine(PlayDeathCutscene(onCutsceneComplete));
        }

        IEnumerator PlayDeathCutscene(Action onComplete)
        {
            var beat = WodeWoDeathBeat ?? Resources.Load<StoryBeat>("Data/Vision1/WodeWoDeath");
            if (beat == null)
            {
                beat = ScriptableObject.CreateInstance<StoryBeat>();
                beat.Text = WodeWoDeathTextFallback;
                beat.SpeakerName = WodeWoDeathSpeaker;
                beat.DisplayDuration = 8f;
                beat.WaitForInput = true;
            }

            if (_narrative != null)
            {
                bool done = false;
                _narrative.ShowStoryBeat(beat, () => done = true);
                while (!done) yield return null;
            }
            else if (_dialogue != null)
            {
                bool done = false;
                _dialogue.ShowStoryBeat(beat, () => done = true);
                while (!done) yield return null;
            }
            else
            {
                yield return new WaitForSeconds(8f);
            }

            onComplete?.Invoke();
        }

        /// <summary>Get the story beat for a tutorial phase. Returns null if Wode-Wo is dead.</summary>
        public StoryBeat GetTutorialBeat(TutorialManager.TutorialPhase phase)
        {
            if (!IsWodeWoAlive) return null;
            switch (phase)
            {
                case TutorialManager.TutorialPhase.BasicAttack: return WodeWoAttackHintBeat ?? CreateDefaultBeat("Strike, dreamer. Let thy blade speak.", "Wode-Wo");
                case TutorialManager.TutorialPhase.Dodge: return WodeWoDodgeHintBeat ?? CreateDefaultBeat("When they strike—dodge! Escapade will carry thee aside.", "Wode-Wo");
                case TutorialManager.TutorialPhase.Parry: return WodeWoParryHintBeat ?? CreateDefaultBeat("Ward, dreamer. Parry at the last moment. It will grant thee Kynde.", "Wode-Wo");
                case TutorialManager.TutorialPhase.Counter: return WodeWoCompleteBeat ?? CreateDefaultBeat("Thou hast learned. I shall stay by thy side.", "Wode-Wo");
                default: return null;
            }
        }

        static StoryBeat CreateDefaultBeat(string text, string speaker)
        {
            var b = ScriptableObject.CreateInstance<StoryBeat>();
            b.Text = text;
            b.SpeakerName = speaker;
            b.DisplayDuration = 4f;
            b.WaitForInput = true;
            return b;
        }
    }
}

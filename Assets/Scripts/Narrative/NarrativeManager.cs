using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Orchestrates narrative: story beats, dialogue triggers. Phase 1: key beats on arrival.</summary>
    public class NarrativeManager : MonoBehaviour
    {
        [Header("References")]
        public DialogueSystem DialogueSystem;

        public event Action<StoryBeat> OnStoryBeatStarted;
        public event Action<StoryBeat> OnStoryBeatCompleted;

        void Awake()
        {
            if (DialogueSystem == null)
                DialogueSystem = FindObjectOfType<DialogueSystem>();
        }

        public void ShowStoryBeat(StoryBeat beat)
        {
            ShowStoryBeat(beat, null);
        }

        public void ShowStoryBeat(StoryBeat beat, System.Action onComplete)
        {
            if (beat == null)
            {
                onComplete?.Invoke();
                return;
            }
            OnStoryBeatStarted?.Invoke(beat);
            if (DialogueSystem != null)
                DialogueSystem.ShowStoryBeat(beat, () =>
                {
                    OnStoryBeatCompleted?.Invoke(beat);
                    onComplete?.Invoke();
                });
            else
                onComplete?.Invoke();
        }

        /// <summary>Show a sequence of story beats in order. onComplete when all done.</summary>
        public void ShowStoryBeatSequence(IList<StoryBeat> beats, System.Action onComplete)
        {
            if (beats == null || beats.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }
            StartCoroutine(PlaySequence(beats, onComplete));
        }

        IEnumerator PlaySequence(IList<StoryBeat> beats, System.Action onComplete)
        {
            for (int i = 0; i < beats.Count; i++)
            {
                var beat = beats[i];
                if (beat == null) continue;
                bool done = false;
                ShowStoryBeat(beat, () => done = true);
                while (!done) yield return null;
            }
            onComplete?.Invoke();
        }

        /// <summary>Show dialogue with choices. onChoiceSelected(index, isCorrectChoice, transitionToLocationId, associatedSin).</summary>
        public void ShowChoiceBeat(DialogueChoiceBeat beat, Action<int, bool, string, SinType> onChoiceSelected)
        {
            if (beat == null)
            {
                onChoiceSelected?.Invoke(-1, false, null, SinType.None);
                return;
            }
            if (DialogueSystem != null)
                DialogueSystem.ShowChoiceBeat(beat, onChoiceSelected);
            else
                onChoiceSelected?.Invoke(-1, false, null, SinType.None);
        }
    }
}

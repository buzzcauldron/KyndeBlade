using System;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Scriptable story beat for narrative display on arrival or key moments.</summary>
    [CreateAssetMenu(fileName = "StoryBeat", menuName = "KyndeBlade/Story Beat")]
    public class StoryBeat : ScriptableObject
    {
        [Header("Content")]
        public string BeatId;
        [TextArea(3, 8)]
        public string Text;
        public string SpeakerName;
        [Tooltip("Optional portrait or icon.")]
        public Sprite Portrait;

        [Header("Timing")]
        public float DisplayDuration = 5f;
        public bool WaitForInput = true;

        [Header("Campaign")]
        public int VisionIndex;
        public int PassusIndex;

        [Header("Presentation (vista lane)")]
        [Tooltip("Optional full-screen image behind the dialogue panel while this beat is shown.")]
        public Texture2D StoryBackdrop;
    }
}

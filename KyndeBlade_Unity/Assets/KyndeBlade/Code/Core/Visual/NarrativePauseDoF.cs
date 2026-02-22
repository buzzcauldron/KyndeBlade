using UnityEngine;
using UnityEngine.Rendering;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Listens to BaseBossAI narrative pause and enables a Depth-of-Field / portrait look (blur except boss). Assign a Volume with DoF override, or use FocusTarget so your post-process can read distance to boss.</summary>
    public class NarrativePauseDoF : MonoBehaviour
    {
        [Tooltip("Optional. When set, this Volume is enabled during narrative pause and disabled after.")]
        public Volume DoFVolume;
        [Tooltip("Set at runtime during pause to the boss transform. Your DoF post-process can use this to compute focus distance (e.g. Camera.main to FocusTarget).")]
        public static Transform FocusTarget { get; private set; }

        bool _volumeWasEnabled;

        void OnEnable()
        {
            BaseBossAI.OnNarrativePauseStarted += OnPauseStarted;
            BaseBossAI.OnNarrativePauseEnded += OnPauseEnded;
        }

        void OnDisable()
        {
            BaseBossAI.OnNarrativePauseStarted -= OnPauseStarted;
            BaseBossAI.OnNarrativePauseEnded -= OnPauseEnded;
            FocusTarget = null;
            if (DoFVolume != null && !_volumeWasEnabled)
                DoFVolume.enabled = false;
        }

        void OnPauseStarted(MedievalCharacter boss, float duration)
        {
            FocusTarget = boss != null ? boss.transform : null;
            if (DoFVolume == null) return;
            _volumeWasEnabled = DoFVolume.enabled;
            DoFVolume.enabled = true;
        }

        void OnPauseEnded()
        {
            FocusTarget = null;
            if (DoFVolume != null && !_volumeWasEnabled)
                DoFVolume.enabled = false;
        }
    }
}

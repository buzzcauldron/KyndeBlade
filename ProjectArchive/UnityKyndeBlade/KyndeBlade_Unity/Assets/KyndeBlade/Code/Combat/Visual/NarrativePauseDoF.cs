using System;

using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Listens to BaseBossAI narrative pause and enables a Depth-of-Field / portrait look (blur except boss). Assign a Volume (URP) or any Behaviour to toggle, or use FocusTarget so your post-process can read distance to boss.</summary>
    public class NarrativePauseDoF : MonoBehaviour
    {
        [Tooltip("Optional. When set, this component is enabled during narrative pause and disabled after. Assign a URP Volume with DoF override, or any Behaviour.")]
        public Behaviour DoFVolume;
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

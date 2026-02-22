using System;
using System.IO;
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
            // #region agent log
            try { var _p = "/Users/halxiii/KyndeBlade/.cursor/debug.log"; var _d = Path.GetDirectoryName(_p); if (!string.IsNullOrEmpty(_d)) Directory.CreateDirectory(_d); File.AppendAllText(_p, "{\"location\":\"NarrativePauseDoF.cs:OnDisable\",\"message\":\"disabled\",\"data\":{\"hadFocusTarget\":" + (FocusTarget != null ? "true" : "false") + "},\"timestamp\":" + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds + ",\"hypothesisId\":\"H1\"}\n"); } catch { }
            // #endregion
            BaseBossAI.OnNarrativePauseStarted -= OnPauseStarted;
            BaseBossAI.OnNarrativePauseEnded -= OnPauseEnded;
            FocusTarget = null;
            if (DoFVolume != null && !_volumeWasEnabled)
                DoFVolume.enabled = false;
        }

        void OnPauseStarted(MedievalCharacter boss, float duration)
        {
            // #region agent log
            try { var _p = "/Users/halxiii/KyndeBlade/.cursor/debug.log"; var _d = Path.GetDirectoryName(_p); if (!string.IsNullOrEmpty(_d)) Directory.CreateDirectory(_d); File.AppendAllText(_p, "{\"location\":\"NarrativePauseDoF.cs:OnPauseStarted\",\"message\":\"pause started\",\"data\":{\"bossName\":" + (boss != null ? "\"" + boss.name + "\"" : "null") + ",\"hasDoF\":" + (DoFVolume != null ? "true" : "false") + "},\"timestamp\":" + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds + ",\"hypothesisId\":\"H1\"}\n"); } catch { }
            // #endregion
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

using System;
using System.Collections;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Pre-Raphaelite "dream-like" moment: slightly slow time on parry success and optionally trigger a golden wash (sepia) filter. Subscribe to parry success; other systems can listen to OnGoldenWashRequested for post-process.</summary>
    public class ManuscriptTimeManager : MonoBehaviour
    {
        [Header("Parry slow-mo")]
        [Tooltip("Time scale during parry success (e.g. 0.4 = dreamy).")]
        [Range(0.2f, 1f)]
        public float ParrySlowMoScale = 0.4f;
        [Tooltip("Duration of slow-mo in real seconds.")]
        [Range(0.3f, 1.5f)]
        public float ParrySlowMoDuration = 0.6f;

        [Header("Golden wash")]
        [Tooltip("If true, fire OnGoldenWashRequested during parry slow-mo so post-process can apply sepia/gold.")]
        public bool RequestGoldenWashOnParry = true;

        /// <summary>Fired when a "manuscript moment" (e.g. parry success) starts. Listen to apply sepia/gold overlay or post-process. Pass duration in real seconds.</summary>
        public event Action<float> OnGoldenWashRequested;

        bool _subscribed;
        float _previousTimeScale = 1f;

        void Start()
        {
            Subscribe();
        }

        void OnDestroy()
        {
            Unsubscribe();
        }

        void Subscribe()
        {
            if (_subscribed) return;
            _subscribed = true;

            var tm = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (tm == null) return;

            foreach (var p in tm.PlayerCharacters)
            {
                if (p == null) continue;
                p.OnParryAttempted += OnParryAttempted;
            }
        }

        void Unsubscribe()
        {
            if (!_subscribed) return;
            _subscribed = false;

            var tm = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (tm == null) return;

            foreach (var p in tm.PlayerCharacters)
            {
                if (p == null) continue;
                p.OnParryAttempted -= OnParryAttempted;
            }
        }

        void OnParryAttempted(MedievalCharacter character, bool success)
        {
            if (!success) return;
            StartCoroutine(ParrySlowMoRoutine());
        }

        IEnumerator ParrySlowMoRoutine()
        {
            _previousTimeScale = Time.timeScale;
            Time.timeScale = ParrySlowMoScale;

            if (RequestGoldenWashOnParry)
                OnGoldenWashRequested?.Invoke(ParrySlowMoDuration);

            yield return new WaitForSecondsRealtime(ParrySlowMoDuration);
            Time.timeScale = _previousTimeScale;
        }
    }
}

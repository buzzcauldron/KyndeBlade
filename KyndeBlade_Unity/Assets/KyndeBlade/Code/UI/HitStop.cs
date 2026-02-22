using System.Collections;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// Hit-stop: briefly freezes (or slows) time on impact so attacks feel heavy.
    /// Call HitStop.Trigger(duration) from damage logic or from an Animation Event at the hit frame.
    /// Uses real time so the freeze actually ends; restores previous Time.timeScale (e.g. pause menu).
    /// Drop this on any GameObject in the scene (e.g. BattleFeedbackManager or a dedicated "GameFeel" object).
    /// </summary>
    public class HitStop : MonoBehaviour
    {
        [Header("Defaults (when using Trigger with one argument)")]
        [Tooltip("Duration of the freeze in real seconds. 0.03–0.08 feels good for most hits.")]
        [Range(0.01f, 0.2f)]
        public float DefaultDuration = 0.05f;
        [Tooltip("Time scale during hit-stop. Near 0 = full freeze; 0.5 = half speed.")]
        [Range(0.0001f, 1f)]
        public float FreezeTimeScale = 0.0001f;

        static HitStop _instance;
        Coroutine _routine;

        void Awake()
        {
            if (_instance == null) _instance = this;
        }

        void OnDestroy()
        {
            if (_instance == this) _instance = null;
            if (_routine != null) StopCoroutine(_routine);
            Time.timeScale = 1f;
        }

        /// <summary>Trigger hit-stop for the default duration. Call from OnDamageDealt, OnAnimationHit, or VFX.</summary>
        public static void Trigger()
        {
            if (_instance == null) _instance = FindObjectOfType<HitStop>();
            if (_instance != null) _instance.TriggerInstance(_instance.DefaultDuration, _instance.FreezeTimeScale);
        }

        /// <summary>Trigger hit-stop for the given duration (real seconds). Use from Animation Events or damage callbacks.</summary>
        public static void Trigger(float durationInRealSeconds)
        {
            if (durationInRealSeconds <= 0f) return;
            if (_instance == null) _instance = FindObjectOfType<HitStop>();
            if (_instance != null) _instance.TriggerInstance(durationInRealSeconds, _instance.FreezeTimeScale);
        }

        /// <summary>Trigger with custom duration and time scale (e.g. 0.05f duration, 0.0001f scale for full freeze).</summary>
        public static void Trigger(float durationInRealSeconds, float timeScaleDuringFreeze)
        {
            if (durationInRealSeconds <= 0f) return;
            if (_instance == null) _instance = FindObjectOfType<HitStop>();
            if (_instance != null) _instance.TriggerInstance(durationInRealSeconds, timeScaleDuringFreeze);
        }

        public void TriggerInstance(float durationInRealSeconds, float timeScaleDuringFreeze)
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(HitStopRoutine(durationInRealSeconds, timeScaleDuringFreeze));
        }

        IEnumerator HitStopRoutine(float durationReal, float freezeScale)
        {
            float previousScale = Time.timeScale;
            Time.timeScale = Mathf.Clamp(freezeScale, 0.0001f, 1f);
            yield return new WaitForSecondsRealtime(durationReal);
            Time.timeScale = previousScale;
            _routine = null;
        }
    }
}

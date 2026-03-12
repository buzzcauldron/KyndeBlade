using System;
using System.Collections;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>
    /// Handles timing between an AI/player decision and damage: Telegraph → Animation → Impact → Complete.
    /// Enables "intention" UI (who is targeted), boss shout/VFX, and animation before math runs.
    /// Assign to TurnManager; when set, ExecuteAction runs through this sequence instead of instant execution.
    /// </summary>
    public class TurnSequenceController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Assign in Inspector. When null, TurnManager uses legacy instant execution.")]
        public TurnManager TurnManager;

        [Header("Timing")]
        [Tooltip("Duration to show telegraph (e.g. target indicator, boss intent) before animation starts.")]
        [Min(0f)]
        public float TelegraphDuration = 0.6f;
        [Tooltip("If true, wait TelegraphDuration and fire OnTelegraphStarted. If false, skip telegraph phase.")]
        public bool UseTelegraph = true;
        [Tooltip("Minimum time after animation starts before sequence is considered complete (lets impact frame play).")]
        [Min(0f)]
        public float MinExecutionTime = 0.3f;
        [Tooltip("Brief pause before attack animation for anticipation feel.")]
        [Min(0f)]
        public float AnticipationPause = 0.15f;
        [Tooltip("Camera zoom factor during critical hits and boss phases.")]
        public float CriticalZoomFactor = 1.15f;
        [Tooltip("Duration of camera zoom effect.")]
        public float ZoomDuration = 0.4f;
        [Tooltip("Screen flash color for status effect application.")]
        public Color StatusFlashColor = new Color(1f, 1f, 1f, 0.3f);
        [Tooltip("Duration of screen flash.")]
        public float FlashDuration = 0.1f;

        /// <summary>Fired at start of telegraph phase. Use for "Boss targets X" UI, boss shout, VFX.</summary>
        public event Action<MedievalCharacter, CombatAction, MedievalCharacter> OnTelegraphStarted;
        /// <summary>Fired when animation is about to play (after telegraph). Use for attack VFX start.</summary>
        public event Action<MedievalCharacter, CombatAction, MedievalCharacter> OnAnimationStarting;
        /// <summary>Fired when the sequence has finished (after execution time). Use for cleanup or transition.</summary>
        public event Action OnSequenceComplete;

        /// <summary>Run the full sequence. TurnManager should call this from ExecuteAction when this controller is assigned.</summary>
        public void RunSequence(MedievalCharacter actor, CombatAction action, MedievalCharacter target)
        {
            if (TurnManager == null) TurnManager = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (TurnManager == null) return;
            StartCoroutine(SequenceRoutine(actor, action, target));
        }

        IEnumerator SequenceRoutine(MedievalCharacter actor, CombatAction action, MedievalCharacter target)
        {
            if (actor == null || action == null) { TurnManager.DoActionExecution(); yield break; }

            float castTime = action.GetCastTime();
            if (castTime > 0f)
                yield return new WaitForSeconds(castTime);

            if (UseTelegraph && TelegraphDuration > 0f)
            {
                OnTelegraphStarted?.Invoke(actor, action, target);
                yield return new WaitForSeconds(TelegraphDuration);
            }

            if (AnticipationPause > 0f)
                yield return new WaitForSeconds(AnticipationPause);

            bool isCritical = action.ActionData != null && action.ActionData.BaseDamage > 15f;
            if (isCritical)
                StartCoroutine(CameraZoomPulse());

            OnAnimationStarting?.Invoke(actor, action, target);
            TurnManager.DoActionCast();

            float execTime = Mathf.Max(MinExecutionTime, action.GetExecutionTime());
            if (execTime > 0f)
                yield return new WaitForSeconds(execTime);

            OnSequenceComplete?.Invoke();
            TurnManager.DoActionExecution();
        }

        IEnumerator CameraZoomPulse()
        {
            var cam = Camera.main;
            if (cam == null || !cam.orthographic) yield break;
            float originalSize = cam.orthographicSize;
            float targetSize = originalSize / CriticalZoomFactor;
            float half = ZoomDuration * 0.5f;

            float t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                cam.orthographicSize = Mathf.Lerp(originalSize, targetSize, t / half);
                yield return null;
            }
            t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                cam.orthographicSize = Mathf.Lerp(targetSize, originalSize, t / half);
                yield return null;
            }
            cam.orthographicSize = originalSize;
        }

        /// <summary>Call from combat feedback to flash the screen when a status effect is applied.</summary>
        public void TriggerStatusFlash()
        {
            StartCoroutine(ScreenFlash());
        }

        IEnumerator ScreenFlash()
        {
            var cam = Camera.main;
            if (cam == null) yield break;
            var bg = cam.backgroundColor;
            cam.backgroundColor = StatusFlashColor;
            yield return new WaitForSeconds(FlashDuration);
            cam.backgroundColor = bg;
        }
    }
}

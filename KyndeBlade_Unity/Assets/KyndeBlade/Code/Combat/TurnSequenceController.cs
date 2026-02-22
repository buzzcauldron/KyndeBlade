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
        public float TelegraphDuration = 0.5f;
        [Tooltip("If true, wait TelegraphDuration and fire OnTelegraphStarted. If false, skip telegraph phase.")]
        public bool UseTelegraph = true;
        [Tooltip("Minimum time after animation starts before sequence is considered complete (lets impact frame play).")]
        [Min(0f)]
        public float MinExecutionTime = 0.2f;

        /// <summary>Fired at start of telegraph phase. Use for "Boss targets X" UI, boss shout, VFX.</summary>
        public event Action<MedievalCharacter, CombatAction, MedievalCharacter> OnTelegraphStarted;
        /// <summary>Fired when animation is about to play (after telegraph). Use for attack VFX start.</summary>
        public event Action<MedievalCharacter, CombatAction, MedievalCharacter> OnAnimationStarting;
        /// <summary>Fired when the sequence has finished (after execution time). Use for cleanup or transition.</summary>
        public event Action OnSequenceComplete;

        /// <summary>Run the full sequence. TurnManager should call this from ExecuteAction when this controller is assigned.</summary>
        public void RunSequence(MedievalCharacter actor, CombatAction action, MedievalCharacter target)
        {
            if (TurnManager == null) TurnManager = FindObjectOfType<TurnManager>();
            if (TurnManager == null) return;
            StartCoroutine(SequenceRoutine(actor, action, target));
        }

        IEnumerator SequenceRoutine(MedievalCharacter actor, CombatAction action, MedievalCharacter target)
        {
            if (actor == null || action == null) { TurnManager.DoActionExecution(); yield break; }

            // Phase 1: Cast time (from action data, e.g. wind-up)
            float castTime = action.GetCastTime();
            if (castTime > 0f)
                yield return new WaitForSeconds(castTime);

            // Phase 2: Telegraph – show intent before animation
            if (UseTelegraph && TelegraphDuration > 0f)
            {
                OnTelegraphStarted?.Invoke(actor, action, target);
                yield return new WaitForSeconds(TelegraphDuration);
            }

            // Phase 3: Animation + apply (damage may be deferred to animation event)
            OnAnimationStarting?.Invoke(actor, action, target);
            TurnManager.DoActionCast();

            // Phase 4: Execution time – wait so impact frame / animation event can fire
            float execTime = Mathf.Max(MinExecutionTime, action.GetExecutionTime());
            if (execTime > 0f)
                yield return new WaitForSeconds(execTime);

            // Phase 5: Complete – run defense window or next turn
            OnSequenceComplete?.Invoke();
            TurnManager.DoActionExecution();
        }
    }
}

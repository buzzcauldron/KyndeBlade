using System;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Polls for dodge/parry input and fires events. Keyboard primary, controller add-in. Escapade = dodge, Ward = parry.
    /// TurnManager (or another listener) should subscribe and call defender.StartDodge/StartParry so input can be ignored when stunned etc.</summary>
    public class ParryDodgeInputHandler : MonoBehaviour
    {
        [Header("References")]
        public TurnManager TurnManager;

        /// <summary>Fired when player presses dodge during a real-time window. Arg = window time remaining. Listener applies to defender.</summary>
        public event Action<float> OnDodgePressed;
        /// <summary>Fired when player presses parry during a real-time window. Arg = window time remaining. Listener applies to defender.</summary>
        public event Action<float> OnParryPressed;

        [Header("Keyboard (Primary)")]
        public KeyCode DodgeKey = KeyCode.Space;
        public KeyCode ParryKey = KeyCode.LeftShift;
        public KeyCode CounterKey = KeyCode.E;

        [Header("Controller (Add-in)")]
        [Tooltip("A / Cross. Set to None to disable controller for this action.")]
        public KeyCode DodgeButton = KeyCode.JoystickButton0;
        [Tooltip("X / Square. Set to None to disable controller for this action.")]
        public KeyCode ParryButton = KeyCode.JoystickButton2;
        [Tooltip("Y / Triangle. Set to None to disable controller for this action.")]
        public KeyCode CounterButton = KeyCode.JoystickButton3;

        bool GetDodgePressed() =>
            Input.GetKeyDown(DodgeKey) || (DodgeButton != KeyCode.None && Input.GetKeyDown(DodgeButton));
        bool GetParryPressed() =>
            Input.GetKeyDown(ParryKey) || (ParryButton != KeyCode.None && Input.GetKeyDown(ParryButton));
        bool GetCounterPressed() =>
            Input.GetKeyDown(CounterKey) || (CounterButton != KeyCode.None && Input.GetKeyDown(CounterButton));

        void Update()
        {
            if (TurnManager == null) return;

            if (TurnManager.IsCounterWindowActive)
            {
                if (GetCounterPressed())
                    TurnManager.ExecuteCounter();
                return;
            }

            if (TurnManager.State != CombatState.RealTimeWindow) return;

            float remaining = TurnManager.RealTimeWindowRemaining;
            var actionType = TurnManager.ActionTypeDuringWindow;

            if (actionType == CombatActionType.Escapade && GetDodgePressed())
                OnDodgePressed?.Invoke(remaining);
            else if (actionType == CombatActionType.Ward && GetParryPressed())
                OnParryPressed?.Invoke(remaining);
        }
    }
}

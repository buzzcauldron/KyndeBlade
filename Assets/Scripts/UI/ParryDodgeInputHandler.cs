using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Polls for dodge/parry input. Keyboard primary, controller add-in. Escapade = dodge, Ward = parry.</summary>
    public class ParryDodgeInputHandler : MonoBehaviour
    {
        [Header("References")]
        public TurnManager TurnManager;

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

            var defender = TurnManager.DefenderDuringWindow;
            if (defender == null) return;

            float remaining = TurnManager.RealTimeWindowRemaining;
            var actionType = TurnManager.ActionTypeDuringWindow;

            if (actionType == CombatActionType.Escapade && GetDodgePressed())
            {
                defender.StartDodge(remaining);
            }
            else if (actionType == CombatActionType.Ward && GetParryPressed())
            {
                defender.StartParry(remaining);
            }
        }
    }
}
